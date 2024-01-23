using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class SceneProperties : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CameraManager m_camManager;
        public CameraManager camManager {
            get {
                if (m_camManager) return m_camManager;
                return GetObject(ref m_camManager);
            }
            set { m_camManager = value; }
        }
        [SerializeField] private PlayerController m_player;
        public PlayerController player
        {
            get
            {
                if (m_player) return m_player;
                return GetObject(ref m_player);
            }
            set { m_player = value; }
        }

        public static SceneProperties instance;
        public static Camera cam { get {
                if (!instance) return null;
                return instance.camManager.cam;
            }
        }
        public static PlayerController mainPlayer {
            get
            {
                if (!instance) return null;
                return instance.player;
            }
        }
        private void Awake()
        {
            instance = this;
        }
        private void OnEnable()
        {
            //string text = "";
            //var components = GetComponentsInChildren<Component>();
            ////text += JsonUtility.ToJson(components, true);
            //for (int i = 0; i < components.Length; i++)
            //{
            //    if (components[i] is not MonoBehaviour)
            //    {
            //        if (components[i] is Transform trs)
            //        {
            //            text += $"Position: {trs.position}. Rotation: {trs.rotation}\n";
            //        }
            //        continue;
            //    }
            //    text += JsonUtility.ToJson(components[i], true);
            //}
            //Debug.Log(text);
        }
        public void ResetLevel()
        {

        }
        private T GetObject<T>(ref T variable) where T : Object
        {
            var obj = FindObjectOfType<T>();
            if (obj) variable = obj;
            return obj;
        }
    }
}
