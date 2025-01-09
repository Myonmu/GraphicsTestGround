using System.Collections.Generic;
using UnityEngine;

namespace RenderPipeline.PlantInteraction.EntityInfluenceRecorder.Shared
{
	public interface IEntityInfluenceRegistry
	{
		public void Add(IEntityInfluenceInfo info);
		public void Remove(IEntityInfluenceInfo info);
	}
	
    public class EntityInfluenceRegistry: IEntityInfluenceRegistry
    {
	    private static EntityInfluenceRegistry _instance;
	    public static EntityInfluenceRegistry Instance
	    {
		    get
		    {
			    if (_instance == null)
			    {
				    _instance = new EntityInfluenceRegistry();
			    }

			    return _instance;
		    }
	    }
	    private int InfluenceInfoSize => sizeof(float) * 8;
	    private GraphicsBuffer _influenceBuffer;
	    private List<IEntityInfluenceInfo> _influenceInfos = new();
	    private EntityInfluenceInfo[] _infos = new EntityInfluenceInfo[32];
	    private int _largeEntityCount = 0;
	    
	    public GraphicsBuffer InfluenceInfo
	    {
		    get
		    {
			    if (_influenceBuffer == null || !_influenceBuffer.IsValid())
			    {
				    _influenceBuffer?.Dispose();
				    _influenceBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
					    GraphicsBuffer.UsageFlags.None, _infos.Length, InfluenceInfoSize);
			    }
			    return _influenceBuffer;
		    }
	    }

	    public int Size => _influenceInfos.Count;

	    public void Add(IEntityInfluenceInfo info)
	    {
		    _influenceInfos.Add(info);
		    if(info.Radius > 10) _largeEntityCount++;
	    }

	    public void Remove(IEntityInfluenceInfo info)
	    {
		    _influenceInfos.Remove(info);
		    if(info.Radius > 10) _largeEntityCount--;
	    }

	    public void Update()
	    {
		    for (int i = 0; i < _influenceInfos.Count; ++i)
		    {
			    Vector4 pos = _influenceInfos[i].WorldPosition;
			    pos.w = _influenceInfos[i].Radius;
			    _infos[i].pos = pos;
			    Vector4 vel = _influenceInfos[i].WorldVelocity;
			    vel.w = _influenceInfos[i].SlopeConstant;
			    _infos[i].vel = vel;
		    }
		    InfluenceInfo.SetData(_infos);
	    }
	    
	    public EIRTranscribeMethod Method => _largeEntityCount > 0 ? EIRTranscribeMethod.Method2 : EIRTranscribeMethod.Method1;
    }
}