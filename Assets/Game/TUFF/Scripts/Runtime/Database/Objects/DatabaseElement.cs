using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class DatabaseElement : ScriptableObject
    {
        [System.NonSerialized] public int id = -1;
        [TextArea(3, 8)] public string notes = "";
        public virtual string GetName()
        {
            return "";
        }
        public virtual string GetDescription()
        {
            return "";
        }
    }
}

