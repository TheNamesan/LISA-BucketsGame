using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeEnemyGraphicAction : EventAction
    {
        public EnemyIndex enemyIndex;
        public Sprite graphic = null;
        public ChangeEnemyGraphicAction()
        {
            eventName = "Change Enemy Graphic";
            eventColor = EventGUIColors.battle;
        }
        public override void Invoke()
        {
            var enemyInstance = enemyIndex.GetEnemyInstance();
            if (enemyInstance != null)
            {
                enemyInstance.ChangeGraphic(graphic);
            }
            isFinished = true;
        }
    }
}
