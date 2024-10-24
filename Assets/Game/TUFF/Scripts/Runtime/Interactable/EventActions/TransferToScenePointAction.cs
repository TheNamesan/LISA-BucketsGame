using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class TransferToScenePointAction : EventAction
    {
        [Tooltip("Scene to change to.")]
        public string sceneToLoad = "";
        [Tooltip("Player's world position on the Scene.")]
        public Vector2 position = Vector2.zero;
        [Tooltip("If true, the player's face direction won't change.")]
        public bool retainFacing = false;
        [Tooltip("Player's facing on the Scene.")]
        public FaceDirections faceDirection;
        [Tooltip("If true, will hide the loading icon while loading the Scene.")]
        public bool hideLoadingIcon = false;
        public TransferToScenePointAction()
        {
            eventName = "Transfer to Scene Point";
            eventColor = new Color(0.9f, 1f, 0.5f, 1f);
        }
        public override void Invoke()
        {
            var faceDir = faceDirection;
            if (retainFacing) faceDir = FollowerInstance.player.controller.faceDirection;
            SceneLoaderManager.instance.LoadScene(sceneToLoad, position, faceDir, hideLoadingIcon, false, false, () => isFinished = true);
        }
        public override void OnStart()
        {
            SceneLoaderManager.instance.LoadNeighbourScene(sceneToLoad);
        }
    }
}

