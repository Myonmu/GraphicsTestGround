using System;
using RenderPipeline.PlantInteraction.EntityInfluenceRecorder.Shared;
using UnityEngine;

namespace RenderPipeline.PlantInteraction.EntityInfluenceRecorder.HDRP
{
    [ExecuteAlways]
    public class EntityInfluenceProvider : MonoBehaviour
    {
        private EntityInfluenceInfoTransformBased _info = new();
        public float radius = 5;
        public float height = 1;
        public float vel = 10;
        private Vector3 _lastPos = Vector3.zero;
        private Vector3 _velocity = Vector3.zero;
        private void OnEnable()
        {
            _info.transform = transform;
            _info.Radius = radius;
            _info.Height = height;
            EntityInfluenceRegistry.Instance.Add(_info);
        }

        private void OnDisable()
        {
            EntityInfluenceRegistry.Instance.Remove(_info);
        }

        private void Update()
        {
            _velocity = (transform.position - _lastPos) / Time.deltaTime;
            _lastPos = transform.position;
            transform.position += Vector3.right * (Time.deltaTime * vel);
            if (transform.position.x > 4096)
            {
                var vector3 = transform.position;
                vector3.x = 0;
                transform.position = vector3;
            }
            _info.WorldVelocity = _velocity;
        }
    }
}