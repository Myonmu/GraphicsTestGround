using UnityEngine;

namespace RenderPipeline.PlantInteraction.EntityInfluenceRecorder.Shared
{
    public interface IEntityInfluenceInfo
    {
        public Vector3 WorldPosition { get; }
        public Vector3 WorldVelocity { get; }
        public float Radius { get; set; }
        
        public float SlopeConstant { get; }
    }

    public struct EntityInfluenceInfo
    {
        public Vector4 pos;
        public Vector4 vel;
    }


    public class EntityInfluenceInfoTransformBased: IEntityInfluenceInfo
    {
        public Transform transform;
        public Vector3 WorldPosition => transform.position;
        public Vector3 WorldVelocity { get; set; }
        public float Radius { get; set; }
        
        public float Height { get; set; }

        public float SlopeConstant => 2 * Height / (Radius * Radius);
    }
}