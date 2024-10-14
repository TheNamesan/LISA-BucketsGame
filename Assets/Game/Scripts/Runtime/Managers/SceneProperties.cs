using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class SceneProperties : MonoBehaviour
    {
        [Header("References")]
        public bool cutsceneMode;
        public bool disableCameraFollowForThisRoom = false;
        [SerializeField] private CameraManager m_camManager;
        public NextRoomCheck nextRoomCheck;
        public CameraManager camManager {
            get {
                if (m_camManager) return m_camManager;
                return GetObject(ref m_camManager);
            }
            set { m_camManager = value; }
        }
        [SerializeField] private PlayerController m_player;
        public TUFF.SceneProperties TUFFSceneProperties;
        [SerializeField] private PolygonCollider2D worldBoundsCol;
        public bool disableRoomReset = false;
        public bool roomCleared = false;
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
        public bool nextRoomAvailable { get => roomEnemies.FindIndex(e => !e.dead) < 0; }
        [SerializeField] private List<Enemy> roomEnemies = new();
        private void Awake()
        {
            //BucketsGameManager.CheckInstance();
            if (!instance) instance = this;
            if (!nextRoomCheck) nextRoomCheck = GetComponentInChildren<NextRoomCheck>();
            //TUFF.SceneLoaderManager.onSceneLoadStart.AddListener(RemoveInstance);
            TUFF.SceneLoaderManager.onSceneLoad.AddListener(MakeThisInstance);
            if (TUFFSceneProperties)
            {
                Vector2 min = Vector2.Min(TUFFSceneProperties.max, TUFFSceneProperties.min);
                Vector2 max = Vector2.Max(TUFFSceneProperties.max, TUFFSceneProperties.min);
                //Vector2 center = Vector2.Lerp(min, max, 0.5f);
                //Vector2 size = new Vector2(Mathf.Abs(max.x - min.x), Mathf.Abs(max.y - min.y));
                if (worldBoundsCol)
                {
                    Vector2[] points = new Vector2[] { 
                        new Vector2(min.x, max.y), new Vector2(max.x, max.y),
                        new Vector2(max.x, min.y), new Vector2(min.x, min.y)};
                    worldBoundsCol.SetPath(0, points);
                }
                m_camManager?.SetBounds(worldBoundsCol);
            }
        }
        private void OnDestroy()
        {
            //TUFF.SceneLoaderManager.onSceneLoadStart.RemoveListener(RemoveInstance);
            TUFF.SceneLoaderManager.onSceneLoad.RemoveListener(MakeThisInstance);
        }
        private void MakeThisInstance()
        {
            if (gameObject.activeInHierarchy)
            {
                instance = this;
            }
        }
        private void RemoveInstance()
        {
            instance = null;
        }
        public void AddRoomEnemy(Enemy enemy)
        {
            if (roomEnemies.Contains(enemy)) return;
            roomEnemies.Add(enemy);
        }
        
        private void OnEnable()
        {
        }
        public void ResetLevel()
        {

        }
        private T GetObject<T>(ref T variable) where T : Object
        {
            var obj = GetComponentInChildren<T>();
            //var obj = FindObjectOfType<T>();
            if (obj) variable = obj;
            return obj;
        }
    }
}
