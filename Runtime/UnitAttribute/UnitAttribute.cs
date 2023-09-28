using System;
using UnityEngine;

namespace BehaviourTreeBuilder
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AddNodeMenuAttribute : Attribute
    {
        public readonly string menuName;
        public readonly bool listAable;

        public AddNodeMenuAttribute(string menuName)
        {
            this.menuName = menuName;
        }
    }
}

