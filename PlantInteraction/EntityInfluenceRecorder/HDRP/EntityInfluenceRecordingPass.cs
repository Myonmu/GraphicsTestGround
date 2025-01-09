using System;
using RenderPipeline.PlantInteraction.EntityInfluenceRecorder.Shared;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace RenderPipeline.PlantInteraction.EntityInfluenceRecorder.HDRP17
{
    public class EntityInfluenceRecordingPass : CustomPass
    {
        private static readonly int InfluenceTextureA = Shader.PropertyToID("_InfluenceTextureA");
        private static readonly int InfluenceTextureB = Shader.PropertyToID("_InfluenceTextureB");

        private static readonly int InfluenceTextureParams = Shader.PropertyToID("_InfluenceTextureParams");
        private static readonly int InfluenceCtrlParams = Shader.PropertyToID("_InfluenceCtrlParams");
        private static readonly int InfluenceInfo = Shader.PropertyToID("_InfluenceInfo");
        private static readonly int InfluenceInfoCount = Shader.PropertyToID("_InfluenceInfoCount");
        private static readonly int Time1 = Shader.PropertyToID("_TimeParams");

        public ComputeShader scriptoriumShader;
        private RenderTexture _influenceTextureA;
        private RenderTexture _influenceTextureB;
        public float coverageSize;
        public Vector2 offset;
        public int textureSize;
        public float lZero;
        public float lerpFactor;
        public float frictionFactor;
        public float elasticityFactor;
        private Vector4 _influenceTextureParams;
        private Vector4 _controlParams;
        private bool _initSucceeded = false;
        private int _fadeKernel;
        private int _transcribeKernel1;
        private int _transcribeKernel2;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            Assert.IsNotNull(scriptoriumShader, "EIR: ScriptoriumShader is null");
            Assert.IsTrue(coverageSize != 0, "EIR: CoverageSize is 0");
            Assert.IsTrue(textureSize != 0, "EIR: TextureSize is 0");
            base.Setup(renderContext, cmd);
            var descriptor = new RenderTextureDescriptor(textureSize, textureSize)
            {
                graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat,
                depthBufferBits = 0,
                depthStencilFormat = GraphicsFormat.None,
                enableRandomWrite = true,
            };

            _influenceTextureA = new RenderTexture(descriptor);
            _influenceTextureB = new RenderTexture(descriptor);

            _fadeKernel = scriptoriumShader.FindKernel("Fade");
            _transcribeKernel1 = scriptoriumShader.FindKernel("TranscribeMethod1");
            _transcribeKernel2 = scriptoriumShader.FindKernel("TranscribeMethod2");

            // set params only once in build to reduce overhead
#if !UNITY_EDITOR
            SetParams();
#endif
            _initSucceeded = true;
        }

        private void SetParams()
        {
            _influenceTextureParams = new Vector4(coverageSize / textureSize, textureSize, offset.x, offset.y);
            _controlParams = new Vector4(lZero, lerpFactor, frictionFactor, elasticityFactor);
            Shader.SetGlobalTexture(InfluenceTextureA, _influenceTextureA);
            Shader.SetGlobalTexture(InfluenceTextureB, _influenceTextureB);
            scriptoriumShader.SetBuffer(_transcribeKernel1, InfluenceInfo,
                EntityInfluenceRegistry.Instance.InfluenceInfo);
            scriptoriumShader.SetTexture(_transcribeKernel1, InfluenceTextureA, _influenceTextureA);
            scriptoriumShader.SetTexture(_transcribeKernel1, InfluenceTextureB, _influenceTextureB);
            scriptoriumShader.SetBuffer(_transcribeKernel2, InfluenceInfo,
                EntityInfluenceRegistry.Instance.InfluenceInfo);
            scriptoriumShader.SetTexture(_transcribeKernel2, InfluenceTextureA, _influenceTextureA);
            scriptoriumShader.SetTexture(_transcribeKernel2, InfluenceTextureB, _influenceTextureB);
            scriptoriumShader.SetBuffer(_fadeKernel, InfluenceInfo, EntityInfluenceRegistry.Instance.InfluenceInfo);
            scriptoriumShader.SetTexture(_fadeKernel, InfluenceTextureA, _influenceTextureA);
            scriptoriumShader.SetTexture(_fadeKernel, InfluenceTextureB, _influenceTextureB);
            Shader.SetGlobalVector(InfluenceTextureParams, _influenceTextureParams);
            Shader.SetGlobalVector(InfluenceCtrlParams, _controlParams);
        }

        protected override void Execute(CustomPassContext ctx)
        {
            if (!_initSucceeded) return;
            base.Execute(ctx);

            EntityInfluenceRegistry.Instance.Update();
            scriptoriumShader.SetInt(InfluenceInfoCount, EntityInfluenceRegistry.Instance.Size);
            scriptoriumShader.SetVector(Time1, new Vector4(Time.deltaTime, 0, 0, 0));
            // In Editor mode, set params every frame to support param adjustments.
#if UNITY_EDITOR
            SetParams();
#endif
            var cmd = ctx.cmd;
            cmd.BeginSample("EntityInfluence Fade");
            cmd.DispatchCompute(scriptoriumShader, _fadeKernel,
                textureSize / 8, textureSize / 8, 1);
            cmd.EndSample("EntityInfluence Fade");
            cmd.BeginSample("EntityInfluence Transcribe");
            cmd.DispatchCompute(scriptoriumShader,
                EntityInfluenceRegistry.Instance.Method == EIRTranscribeMethod.Method1
                    ? _transcribeKernel1
                    : _transcribeKernel2,
                textureSize / 8, textureSize / 8, 1);
            cmd.EndSample("EntityInfluence Transcribe");
        }

        protected override void Cleanup()
        {
            _influenceTextureA.Release();
            _influenceTextureB.Release();
        }
    }
}