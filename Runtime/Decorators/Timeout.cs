using System;
using UnityEngine;

namespace BehaviourTreeBuilder
{
    [Serializable]
    [AddNodeMenu("Decorators")]
    public class Timeout : DecoratorNode
    {
        [Tooltip("Returns failure after this amount of time if the subtree is still running.")]
        [SerializeField] private float _duration = 1;
        private float _counter;

        protected override void OnStart()
        {
            _counter = _duration;
        }

        protected override void OnStop()
        {
        }

        protected override void OnFixedUpdate()
        {
            child.FixedUpdate();
        }

        protected override State OnUpdate()
        {
            if (child == null) return State.Failure;
            _counter -= Time.deltaTime;
            if (_counter <= 0)
            {
                _counter = 0;
                return State.Failure;
            }

            return child.Update();
        }

        protected override void OnLateUpdate()
        {
            child.LateUpdate();
        }

        public override string OnShowDescription()
        {
            return state == State.Idle ? $"Time left: {_duration:F2}s" : $"Time left: {_counter:F2}s";
        }
    }
}