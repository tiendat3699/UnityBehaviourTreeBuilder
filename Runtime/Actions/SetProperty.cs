using System;

namespace BehaviourTreeBuilder
{
    [Serializable]
    [AddNodeMenu("Actions")]
    public class SetProperty : ActionNode
    {
        public BlackboardKeyValuePair pair;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            pair.WriteValue();

            return State.Success;
        }

        public override string OnShowDescription()
        {
            return pair.key != null ? $"{pair.key.name}{pair.value}" : "";
        }
    }
}