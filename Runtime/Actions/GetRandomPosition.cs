using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviourTreeBuilder
{
    [Serializable]
    [AddNodeMenu("Actions")]
    public class GetRandomPosition : ActionNode
    {
#if CORE_3D
        private enum InsideUnit
        {
            Sphere,
            Circle
        }
        // [SerializeField] private NodeProperty<Vector3> _position;
        [SerializeField] private InsideUnit _insideUnit;
        [SerializeField] private NodeProperty<Vector3> _position;
#else
        [SerializeField] private NodeProperty<Vector2> _position;
#endif
        [SerializeField] private float _radiusRandomRange = 10f;
        
        // OnStart is called immediately before execution. It is used to setup any variables that need to be reset from the previous run.
        protected override void OnStart() 
        {
        }
    
        // OnStop is called after execution on a success or failure.
        protected override void OnStop() 
        {
        }
    
        // OnUpdate runs the actual task.
        protected override State OnUpdate()
        {
            var center = context.Transform.position;
#if CORE_3D
            _position.Value = _insideUnit switch
            {
                InsideUnit.Sphere => center + Random.insideUnitSphere * _radiusRandomRange,
                InsideUnit.Circle => (Vector2)center + Random.insideUnitCircle * _radiusRandomRange,
                _ => _position.Value
            };
#else
            _position.Value = center + Random.insideUnitSphere * _radiusRandomRange;
#endif
            return State.Success;
        }

        public override string OnShowDescription()
        {
#if CORE_3D
            return $"InsideUnit: {_insideUnit} \nRadius: {_radiusRandomRange}";
#else

            return $"";
#endif
        }

        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_position.Value, 0.2f);
        }
    }
}
