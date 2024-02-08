using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TUFF
{
    public class GameOverEvent : EventCommand
    {
        public override void Invoke()
        {
            GameManager.instance.GameOver();
        }
        public override void OnInstantiate()
        {
            eventName = "Game Over";
            eventColor = new Color(0.5f, 1f, 0.95f, 1f);
        }
    }
}
