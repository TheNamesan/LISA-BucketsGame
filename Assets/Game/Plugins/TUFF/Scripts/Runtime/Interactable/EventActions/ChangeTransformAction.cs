using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ChangeTransformAction : EventAction
    {
        public Transform transform;

        [Header("Position")]
        public bool keepPosition = false;
        public Vector3 position = Vector3.zero;
        public bool worldPosition = false;

        [Header("Rotation")]
        public bool keepRotation = true;
        public Vector3 rotation = Vector3.zero;
        public bool worldRotation = false;

        [Header("Scale")]
        public bool keepScale = true;
        public Vector3 scale = Vector3.zero;
        
        public ChangeTransformAction()
        {
            eventName = "Change Transform";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            if (transform) {
                if (!keepPosition) 
                {
                    if (worldPosition) transform.position = position;
                    else transform.localPosition = position;
                }
                if (!keepRotation)
                {
                    if (worldRotation) transform.rotation = Quaternion.Euler(rotation);
                    else transform.localRotation = Quaternion.Euler(rotation);
                }
                if (!keepScale)
                {
                    transform.localScale = scale;
                }
            }
            isFinished = true;
        }
    }
}
