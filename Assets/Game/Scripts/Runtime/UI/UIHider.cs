using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BucketsGame
{
    public class UIHider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public CanvasGroup group;
        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            //Output to console the GameObject's name and the following message
            Debug.Log("Cursor Entering " + name + " GameObject");
            if (group) group.alpha = 0.5f;
        }

        //Detect when Cursor leaves the GameObject
        public void OnPointerExit(PointerEventData pointerEventData)
        {
            //Output the following message with the GameObject's name
            Debug.Log("Cursor Exiting " + name + " GameObject");
            if (group) group.alpha = 1f;
        }
    }
}
