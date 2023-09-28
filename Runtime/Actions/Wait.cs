using System;
using UnityEngine;

namespace BehaviourTreeBuilder {

    [Serializable]
    [AddNodeMenu("Actions")]
    public class Wait : ActionNode {

        [Tooltip("Amount of time to wait before returning success")] 
        [SerializeField] private float _duration = 1;
        private float _counter;

        protected override void OnStart() {
            _counter = _duration;
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate()
        {
            _counter -= Time.deltaTime;
            if (_counter <= 0)
            {
                _counter = 0;
                return State.Success;
            }
            return State.Running;
        }

        public override string OnShowDescription()
        {
            return state == State.Idle ? $"Wait: {_duration:F2}s" : $"Wait: {_counter:F2}s";
        }
    }
}
