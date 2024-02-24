using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TUFF
{
    public class SceneProperties : MonoBehaviour
    {
        [Header("Scene Size")]
        [Tooltip("Camera")]
        public CameraFollow camFollow;
        [Tooltip("Scene size min point in world space. If the Character's Y position goes below min Y, the game triggers a Game Over.")]
        public Vector2 min;
        [Tooltip("Scene size max point in world space.")]
        public Vector2 max;

        [Header("Scene Audio")]
        [Tooltip("If true, will play the BGM when the Scene loads.")]
        public bool autoPlayBGM = false;
        [Tooltip("BGM to play.")]
        public BGMPlayData sceneBGM = new BGMPlayData();
        [Tooltip("If true, will play the AMBS when the Scene loads.")]
        public bool autoPlayAMBS = false;
        [Tooltip("AMBS to play.")]
        public AMBSPlayData sceneAMBS = new AMBSPlayData();

        private SceneProperties clone = null;
        
        private void Awake()
        {
            if (camFollow != null)
                camFollow.si = this;
            AwakeScene();
        }
        public void AwakeScene()
        {
            //gameObject.SetActive(false);
            //if (gameObject.scene == SceneManager.GetActiveScene())
            //{
                //clone = Instantiate(this).GetComponent<SceneProperties>();
                //gameObject.SetActive(true);
            //}
        }
        private void OnEnable()
        {
            //if (clone == null)
            //{
                //gameObject.SetActive(false);
                //clone = Instantiate(this).GetComponent<SceneProperties>();
                //gameObject.SetActive(true);
            //}
        }
        private void OnDisable()
        {
            DisableScene();
        }
        public void DisableScene()
        {
            Debug.Log("disale");
            if (clone != null)
            {
                clone.gameObject.name = gameObject.name;
                Destroy(gameObject);
            }
        }
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.01f);
            if (!GameManager.gameOver)
            {
                if (autoPlayBGM)
                {
                    AudioManager.instance.PlayMusic(sceneBGM);
                }
                if (autoPlayAMBS)
                {
                    AudioManager.instance.PlayAMBS(sceneAMBS);
                }
            }
            yield break;
        }

        private void LateUpdate()
        {
            GameOverCheck();
        }

        private void GameOverCheck()
        {
            if (FollowerInstance.player != null && !GameManager.gameOver)
            {
                if (FollowerInstance.player.controller.transform.position.y + 1 < min.y)
                {
                    GameManager.instance.GameOver();
                    //gameObject.SetActive(false);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector2(min.x, max.y), new Vector2(max.x, max.y));
            Gizmos.DrawLine(new Vector2(max.x, max.y), new Vector2(max.x, min.y));
            Gizmos.DrawLine(new Vector2(max.x, min.y), new Vector2(min.x, min.y));
            Gizmos.DrawLine(new Vector2(min.x, min.y), new Vector2(min.x, max.y));
        }
    }
}


