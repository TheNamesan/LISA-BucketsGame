using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class FileSelectMenu : MonoBehaviour
    {
        public UIMenu uiMenu;
        private void Awake()
        {
            if (uiMenu == null) uiMenu = GetComponent<UIMenu>();
            if (uiMenu != null)
            {
                if (uiMenu.UIElements == null) uiMenu.SetupUIElements();
                for (int i = 0; i < uiMenu.UIElements.Length; i++)
                {
                    if (uiMenu.UIElements[i].Length > 0)
                    {
                        int index = i;
                        var element = uiMenu.UIElements[i][0];
                        element.useCustomSelectSFX = true;
                        element.customSelectSFX = TUFFSettings.loadSFX;
                        element.text.text = $"File {i + 1}" ; // Change for localization
                        element.onSelect.AddListener(() => { SelectFile(index); });
                    }
                }
            }
            
        }
        public void SelectFile(int file)
        {
            bool loaded = GameManager.instance.LoadSaveData(file);
            if (loaded) { Debug.Log($"Loaded File {file}"); StartCoroutine(Continue()); }
            else Debug.Log($"No file found at {file}");
        }
        private IEnumerator Continue()
        {
            UIController.instance.SetMenu(null);
            UIController.instance.fadeScreen.TriggerFadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            yield return new WaitForSeconds(2f);
            PlayerData.instance.GetSceneData(out string sceneName, out Vector3 playerPosition, out FaceDirections playerFacing);
            SceneLoaderManager.instance.LoadSceneWithFadeIn(sceneName, 0.5f, playerPosition, playerFacing, true, true); //Replace with save data scene name and start position
        }
        private void OnEnable()
        {
            
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
