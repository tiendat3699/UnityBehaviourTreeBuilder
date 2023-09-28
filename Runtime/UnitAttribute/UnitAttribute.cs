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
}

