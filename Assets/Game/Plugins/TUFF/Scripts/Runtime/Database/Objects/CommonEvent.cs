using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF 
{
    [CreateAssetMenu(fileName = "CommonEvent", menuName = "Database/Command Event", order = 12)]
    public class CommonEvent : ScriptableObject
    {
        public ActionList actionList = new ActionList();
    }
}

