using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class BattleEvent
    {
        public BattleConditions conditions = new BattleConditions();
        public ActionList actionList = new ActionList();
    }
}
