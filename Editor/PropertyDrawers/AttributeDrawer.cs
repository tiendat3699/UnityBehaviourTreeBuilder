using UnityEditor;
using UnityEngine;

namespace BehaviourTreeBuilder
{
    [CustomPropertyDrawer(typeof(PropertyShowIfAttribute))]
    public class PropertyShowIfDrawer : PropertyDrawer
    {
        // private bool _isShow = true;
        
        // public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        // {
        //     // accounts for foldouts on serialized classes
        //     return _isShow ? base.GetPropertyHeight(property, label) : 0;
        // }
        
        
        // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        // {
        //     var showIfAttr = attribute as PropertyShowIfAttribute;
        // }
    }
}