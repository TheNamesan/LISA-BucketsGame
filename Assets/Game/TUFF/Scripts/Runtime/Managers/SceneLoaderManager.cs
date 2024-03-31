using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace TUFF
{ 
    public class SceneNode
    {
        public Scene currentScene;
        public SceneProperties sceneProperties;
        public List<string> neighbourScenes = new List<string>();
        public List<Scene> cachedNeighbourScenes = new List<Scene>();
        
        public SceneNode() { }
        public SceneNode(Scene scene)
        {
            AssignData(scene);
        }
        public void AssignData(Scene scene)
        {
            currentScene = scene;
            var gameObjects = currentScene.GetRootGameObjects();
            var scenePropGO = System.Array.Find(gameObjects, q => q.gameObject.CompareTag("SceneProperties"));
            sceneProperties = scenePropGO?.GetComponent<SceneProperties>();
            if (sceneProperties != null) Debug.Log("SceneProperties: " + sceneProperties.gameObject);
            else Debug.Log("SceneProperties: null");
            neighbourScenes.Clear();
            cachedNeighbourScenes.Clear();
        }
        public void AddNeighbour(Scene scene)
        {
            if (cachedNeighbourScenes.IndexOf(scene) >= 0) return; //Neighbour already added
            if (scene == currentScene) return;
            cachedNeighbourScenes.Add(scene);
        }
        public bool HasScene(Scene scene)
        {
            if (currentScene == scene) return true;
            return cachedNeighbourScenes.IndexOf(scene) >= 0;
        }
        public void LogNeighbours()
        {
            string text = "Neighbours: \n(";
            for (int i = 0; i < cachedNeighbourScenes.Count; i++)
            {
                var scene = cachedNeighbourScenes[i];
                text += scene.name + " [" + scene.buildIndex + "],";
            }
            text += ")";
            Debug.Log(text);
        }
    }
    public class SceneLoaderManager : MonoBehaviour
    {
        public static SceneNode currentSceneNode = new SceneNode();
        public static SceneNode lastSceneNode = new SceneNode();
        public static Scene currentScene = new Scene();
        public static SceneProperties currentSceneProperties { get => currentSceneNode.sceneProperties; }
        public static List<Scene> preloadedScenes = new List<Scene>();
        public static List<string> loadingNeighbourScenes = new List<string>();
        public static List<string> unloadingScenes = new List<string>();
        public static List<AsyncOperation> neighbourOperations = new List<AsyncOperation>();

        public static UnityEvent onSceneLoadStart = new();
        public static UnityEvent onSceneLoad = new();
        public static UnityEvent onSceneChanged = new();

        public static bool loading { get => m_loading; }
        private static bool m_loading = false;

        #region Singleton
        public static SceneLoaderManager instance { get {
                if (!GameManager.instance) return null;
                return GameManager.instance.sceneLoaderManager;
                //if (m_instance == null)
                //{
                //    GameManager.CheckInstance();
                //    if (GameManager.instance == null) return null;
                //    AssignInstance(GameManager.instance.GetComponentInChildren<SceneLoaderManager>());
                //}
                //return m_instance; 
            } 
        }
        //protected static SceneLoaderManager m_instance;
        
        private void Awake()
        {
            //if (m_instance != null)
            //{
            //    Destroy(gameObject);
            //}
            //else
            //{
            //    AssignInstance(this);
            //}
            UpdateCurrentScene(SceneManager.GetActiveScene());
            AddToLoadedScenes(currentScene);
            UpdateNodes(currentScene);
        }
        //public static void AssignInstance(SceneLoaderManager target)
        //{
        //    if (target == null) return;
        //    //m_instance = target;
        //    //DontDestroyOnLoad(m_instance.gameObject);
        //    //m_instance.UpdateCurrentScene(SceneManager.GetActiveScene());
        //    AddToLoadedScenes(currentScene);
        //    UpdateNodes(currentScene);
        //}
        #endregion
        private void Start()
        {
            onSceneLoad?.Invoke();
        }

        public void LoadScene(string newScene, Vector2 playerPosition = new Vector2(), FaceDirections faceDirection = FaceDirections.East, bool hideLoadingIcon = false, bool disableActionMap = false, bool enablePlayerInput = false, Action onLoad = null)
        {
            GameManager.instance.ChangeTimeScale(0);
            StartCoroutine(AsyncSceneLoad(newScene, playerPosition, faceDirection, hideLoadingIcon, disableActionMap, enablePlayerInput, onLoad));
        }

        public void LoadSceneWithFadeIn(string newScene, float fadeDuration, Vector2 playerPosition = new Vector2(), FaceDirections faceDirection = FaceDirections.East, bool disableActionMap = false, bool enablePlayerInputAction = false, Action onLoad = null)
        {
            StartCoroutine(AsyncSceneLoadFadeIn(newScene, fadeDuration, playerPosition, faceDirection, disableActionMap, enablePlayerInputAction, onLoad));
        }

        IEnumerator AsyncSceneLoad(string newScene, Vector2 playerPosition, FaceDirections faceDirection, bool hideLoadingIcon, bool disableActionMap, bool enablePlayerInput, Action onLoad = null)
        {
            UpdateCurrentScene(SceneManager.GetActiveScene());
            AddToLoadedScenes(currentScene);
            string currentSceneName = currentScene.name;

            //for (int i = 0; i < SceneManager.sceneCount; i++)
            //{
            //    var scene = SceneManager.GetSceneAt(i);
            //    Debug.Log(scene.name + " [" + scene.buildIndex + "]");
            //}
            m_loading = true;
            onSceneLoadStart?.Invoke();
            
            if (currentSceneName != newScene)
            {
                float tim = Time.unscaledTime;
                UIController.instance.TriggerLoadingIcon(!hideLoadingIcon);
                var targetScene = SceneManager.GetSceneByName(newScene);
                int sceneIndex = preloadedScenes.IndexOf(targetScene);

                if (sceneIndex < 0) // Scene not loaded
                {
                    var prevPriority = Application.backgroundLoadingPriority;
                    Application.backgroundLoadingPriority = ThreadPriority.High;
                    //SetAllRootGameObjectsActive(currentScene.GetRootGameObjects(), false);
                    AsyncOperation operation = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
                    operation.allowSceneActivation = false;
                    while (!operation.isDone)
                    {
                        if (operation.progress >= 0.9f)
                        {
                            SetAllRootGameObjectsActive(SceneManager.GetActiveScene().GetRootGameObjects(), false);
                            operation.allowSceneActivation = true;
                        }
                        yield return null;
                    }
                    //
                    //SetAllRootGameObjectsActive(currentScene.GetRootGameObjects(), false);
                    yield return new WaitForEndOfFrame();
                    targetScene = SceneManager.GetSceneByName(newScene);
                    Debug.Log(targetScene.IsValid());
                    SceneManager.SetActiveScene(targetScene);
                    UpdateCurrentScene(SceneManager.GetActiveScene());
                    AddToLoadedScenes(currentScene);
                    UpdateNodes(currentScene);
                    SetAllRootGameObjectsActive(currentScene.GetRootGameObjects(), true);
                    SetPlayerPosition(playerPosition, faceDirection);
                    Application.backgroundLoadingPriority = prevPriority;
                }
                else
                {
                    Debug.Log("Has Scene Index in Loaded Scenes");
                    SetAllRootGameObjectsActive(currentScene.GetRootGameObjects(), false);
                    UpdateCurrentScene(preloadedScenes[sceneIndex]);
                    SceneManager.SetActiveScene(currentScene);
                    UpdateNodes(currentScene);
                    SetAllRootGameObjectsActive(currentScene.GetRootGameObjects(), true);
                    SetPlayerPosition(playerPosition, faceDirection);
                }
                Debug.Log(Time.unscaledTime - tim);
                StartCoroutine(UnloadScenes());
            }
            else SetPlayerPosition(playerPosition, faceDirection);
            m_loading = false;
            UIController.instance.TriggerLoadingIcon(false);
            if (disableActionMap) GameManager.instance.DisableActionMaps(false);
            if (enablePlayerInput) GameManager.instance.DisablePlayerInput(false);
            GameManager.instance.ChangeTimeScale(1);
            onSceneLoad?.Invoke();
            onSceneChanged?.Invoke();
            onLoad?.Invoke();
        }
        public void UpdateCurrentScene(Scene scene)
        {
            currentScene = scene;
            string name = (scene != null ? scene.name : "");
            PlayerData.instance?.UpdateLoadedScene(name);
        }

        private static void UpdateNodes(Scene scene)
        {
            lastSceneNode = currentSceneNode;
            Debug.Log("Last Scene: " + lastSceneNode.currentScene.name);
            lastSceneNode.LogNeighbours();
            currentSceneNode = new SceneNode(scene);
            Debug.Log("'Current Scene: " + currentSceneNode.currentScene.name);
        }
        public IEnumerator UnloadScenes()
        {
            yield return new WaitForEndOfFrame();
            while (neighbourOperations.Count > 0)
            {
                yield return null;
            }
            // Check Missing Intersections
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (currentSceneNode.HasScene(scene)) continue;
                if (lastSceneNode.HasScene(scene)) continue;
                var op = SceneManager.UnloadSceneAsync(scene.name);
                unloadingScenes.Add(scene.name);
                op.completed += (asyncOperation) => { unloadingScenes.Remove(scene.name); };
            }
            Debug.Log("Unloaded Scenes");
        }
        public void LoadNeighbourScene(string sceneName)
        {
            StartCoroutine(AsyncLoadNeighbourScene(sceneName));
        }
        IEnumerator AsyncLoadNeighbourScene(string sceneName)
        {
            if (loadingNeighbourScenes.IndexOf(sceneName) >= 0) yield break;
            var targetScene = SceneManager.GetSceneByName(sceneName);
            int sceneIndex = preloadedScenes.IndexOf(targetScene);
            if (sceneIndex < 0) // Scene not loaded
            {
                loadingNeighbourScenes.Add(sceneName);
                AsyncOperation nextOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                neighbourOperations.Add(nextOperation);
                nextOperation.completed += (asyncOperation) =>
                {
                    var targetNode = currentSceneNode;
                    var neighbour = SceneManager.GetSceneByName(sceneName);
                    SetAllRootGameObjectsActive(neighbour.GetRootGameObjects(), false);
                    AddToLoadedScenes(neighbour);
                    targetNode.AddNeighbour(neighbour);
                    loadingNeighbourScenes.Remove(sceneName);
                    neighbourOperations.Remove(nextOperation); // Remove from loading
                };
            }
            else
            {
                var neighbour = SceneManager.GetSceneByName(sceneName);
                currentSceneNode.AddNeighbour(neighbour);
            }
        }

        private static void AddToLoadedScenes(Scene scene)
        {
            if (!preloadedScenes.Contains(scene))
                preloadedScenes.Add(scene);
        }

        IEnumerator AsyncSceneLoadFadeIn(string newScene, float fadeDuration, Vector2 playerPosition, FaceDirections faceDirection, bool disableActionMap, bool enablePlayerInput, Action onLoad = null)
        {
            if (disableActionMap) GameManager.instance.DisableActionMaps(true);
            UIController.instance.TriggerLoadingIcon(true);
            AsyncOperation operation = SceneManager.LoadSceneAsync(newScene);
            operation.completed += (asyncOperation) => {
                UpdateCurrentScene(SceneManager.GetActiveScene());
                AddToLoadedScenes(currentScene);
                //SetAllRootGameObjectsActive(currentScene.GetRootGameObjects(), true); // Temporarily removed to avoid double SceneProp spawning after Game Over Continue
                UpdateNodes(currentScene);
                SetPlayerPosition(playerPosition, faceDirection); 
            };
            while (!operation.isDone)
            {
                yield return null;
            }
            UIController.instance.TriggerLoadingIcon(false);
            UIController.instance.fadeScreen.TriggerFadeIn(fadeDuration, () =>
            {
                if (disableActionMap) GameManager.instance.DisableActionMaps(false);
                if (enablePlayerInput) GameManager.instance.DisablePlayerInput(false);
                onLoad?.Invoke();
            });
            onSceneLoad?.Invoke();
            onSceneChanged?.Invoke();
        }
        public void SetPlayerPosition(Vector2 position, FaceDirections faceDirection)
        {
            if (FollowerInstance.player == null) return;
            var controller = FollowerInstance.player.controller;
            controller.transform.position = (Vector3)position + Vector3.up * Physics2D.defaultContactOffset * 0.5f;
            controller.ChangeFaceDirection(faceDirection);
            controller.SetSceneChangeFrameConditions();
            controller.fallStart = position;
        }
        
        private void SetAllRootGameObjectsActive(GameObject[] rootGOs, bool active)
        {
            Debug.Log("Setting all root objs active: " + active);
            foreach (GameObject go in rootGOs)
            {
                go.SetActive(active);
            }
        }
    }
}
