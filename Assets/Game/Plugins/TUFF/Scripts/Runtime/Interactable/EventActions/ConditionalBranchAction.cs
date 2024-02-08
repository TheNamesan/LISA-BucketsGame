using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ConditionalBranchAction : EventAction
    {
        public List<BranchActionContent> branches = new List<BranchActionContent>();
        public ActionList elseActionList = new();
        public bool addBranchWhenNoConditionsApply = false;
        public ConditionalBranchAction()
        {
            eventName = "Conditional Branch";
            branches = new List<BranchActionContent>();
        }
        public override void Invoke()
        {
            if (branches.Count <= 0)
            {
                isFinished = true;
                Debug.Log("Count is 0");
                return;
            }
            int index = -1;
            for (int i = 0; i < branches.Count; i++)
            {
                if (branches[i].ValidateCondition())
                {
                    index = i;
                    break;
                }
            }
            if (index < 0)
            {
                if (addBranchWhenNoConditionsApply)
                {
                    CommonEventManager.instance.TriggerEventActionBranch(this, elseActionList);
                }
                else
                {
                    isFinished = true;
                }
                return;
            }
            CommonEventManager.instance.TriggerEventActionBranch(this, branches[index].actionList);
        }

        
    }
}

