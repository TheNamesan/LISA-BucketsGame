using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class GameOverAction : EventAction
    {
        public GameOverAction()
        {
            eventName = "Game Over";
            eventColor = new Color(0.5f, 1f, 0.95f, 1f);
        }
        public override void Invoke()
        {
            GameManager.instance.GameOver();
        }
    }
}

