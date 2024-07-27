using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ChangeGameObjectAction : EventAction
    {
        public GameObject gameObject;

        [Header("Active")]
        public bool keepActive = true;
        public bool setActive = false;

        [Header("Name")]
        public bool changeName = false;
        public string newName = "";

        [Header("Tag")]
        public bool keepTag = true;
        public string newTag = "";



        public ChangeGameObjectAction()
        {
            eventName = "Change Game Object";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            if (gameObject)
            {
                if (!keepActive)
                {
                    gameObject.SetActive(setActive);
                }
                if (changeName)
                {
                    gameObject.name = newName;
                }
                if (!keepTag)
                {
                    gameObject.tag = newTag;
                }
            }
            isFinished = true;
        }
    }

}
