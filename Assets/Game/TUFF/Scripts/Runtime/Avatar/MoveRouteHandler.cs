using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class MoveRouteHandler : MonoBehaviour
    {
        public OverworldCharacterController controller;
        public bool playing = false;
        public void PlayMoveRoute(MoveRoute route, EventAction actionCallback)
        {
            if (route == null) return;
            if (playing) { if (actionCallback != null) actionCallback.isFinished = true; return; }
            playing = true;
            Debug.Log("Playing Move Route");
            StartCoroutine(PlayMoveRouteCoroutine(route, actionCallback));
        }
        private IEnumerator PlayMoveRouteCoroutine(MoveRoute route, EventAction actionCallback)
        {
            yield return new WaitForEndOfFrame();
            var elements = route.elements;
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (element == null) continue;
                yield return element.PlayElement(controller);
            }
            if (actionCallback != null) actionCallback.isFinished = true;
            playing = false;
            Debug.Log("Stopped Move Route");
            yield break;
        }
    }
}
