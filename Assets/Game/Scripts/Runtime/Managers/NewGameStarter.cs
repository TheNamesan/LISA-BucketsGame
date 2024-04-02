using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class NewGameStarter : MonoBehaviour
    {
        public void MarkNewGame()
        {
            BucketsGameManager.instance.SetNewGame(true);
        }
    }
}
