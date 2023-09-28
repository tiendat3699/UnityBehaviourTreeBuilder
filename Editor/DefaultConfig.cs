using UnityEngine;
    
namespace BehaviourTreeBuilder
{
    public class DefaultConfig : ScriptableObject
    {
        [Header("No Namespace")]
        public TextAsset scriptTemplateActionNodeNoNamespace;
        public TextAsset scriptTemplateCompositeNodeNoNamespace;
        public TextAsset scriptTemplateDecoratorNodeNoNamespace;
        
        [Header("With Namespace")]
        public TextAsset scriptTemplateActionNodeNamespace;
        public TextAsset scriptTemplateCompositeNodeNamespace;
        public TextAsset scriptTemplateDecoratorNodeNamespace;
    }
}

