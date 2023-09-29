using System;
using UnityEngine;

namespace BehaviourTreeBuilder
{
    [Serializable]
    [AddNodeMenu("Actions")]
    public class UnityMessage : ActionNode
    {
        [SerializeField] private SendMessageOptions _sendMessageOptions = SendMessageOptions.DontRequireReceiver;
        [SerializeField] private NodeProperty<string> _methodName;
        
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
            context.GameObject.SendMessage(_methodName.Value, _sendMessageOptions);
            return State.Success;
        }   

        public override string OnShowDescription()
        {
            return $"Method: {_methodName.Value} \nOptions: {_sendMessageOptions}";
        }
    }
}
