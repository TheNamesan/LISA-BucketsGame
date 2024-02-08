using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class BranchContent
    {
        public bool condition = false;
        public InteractableEventList content = new InteractableEventList();

        public bool VerifyCondition()
        {
            return condition;
        }
    }
}
