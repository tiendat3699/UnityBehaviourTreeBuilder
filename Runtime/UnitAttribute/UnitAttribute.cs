using System;
using UnityEngine;

namespace BehaviourTreeBuilder
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AddNodeMenuAttribute : Attribute
    {
        public readonly string MenuName;

        public AddNodeMenuAttribute(string menuName)
        {
            MenuName = menuName;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyShowIfAttribute : PropertyAttribute
    {
        public readonly string PropertyCompareName;
        public readonly bool Value;

        public PropertyShowIfAttribute(string propertyCompareName, bool value)
        {
            PropertyCompareName = propertyCompareName;
            Value = value;
        }
    }
}

