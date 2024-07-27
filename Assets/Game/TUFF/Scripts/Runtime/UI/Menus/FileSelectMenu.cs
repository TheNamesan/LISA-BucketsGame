using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public enum FileSelectMenuMode { LoadFile = 0, SaveFile = 1 }
    public class FileSelectMenu : MonoBehaviour
    {
        public FileSelectMenuMode mode = FileSelectMenuMode.LoadFile;
        public TMP_Text promptText;
        public UIMenu uiMenu;
        public Transform buttonsParent;
        public SaveFileHUD saveFileHUDPrefab;
        public List<SaveFileHUD> elements = new();
        [System.NonSerialized]
        public EventAction actionCallback = null;

        protected bool initialized = false;
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (initialized) return;
            if (uiMenu == null) uiMenu = GetComponent<UIMenu>();
            InitializeSaveFileHUDs();
            if (uiMenu) uiMenu.onCloseMenu.AddListener(OnCloseMenu);
            initialized = true;
        }
        private void OnEnable()
        {
            UpdatePromptText();
        }

        public void OpenFileSelectMenu(FileSelectMenuMode openMode, EventAction actionCallback = null)
        {
            mode = openMode;
            this.actionCallback = actionCallback;
            Initialize();
            UpdateSaveFileHUDs();
            UpdatePromptText();
            if (uiMenu)
            {
                uiMenu?.OpenMenu();
                int highlight = ConfigData.instance.lastOpenedFile;
                if (highlight >= 0 && highlight < uiMenu.RowCount) 
                    uiMenu?.HighlightAt(uiMenu.highlightX, ConfigData.instance.lastOpenedFile);
            }
        }
        private void InitializeSaveFileHUDs()
        {
            VerifyMenuArrays(0);
            int index = 0;
            foreach (Transform child in buttonsParent) // Add existing GameObjects to list
            {
                if (child.TryGetComponent(out SaveFileHUD existing))
                {
                    existing.Initialize(index);
                    elements.Add(existing);
                    index++;
                }
                
            }
            if (saveFileHUDPrefab)
            {
                for (; index < TUFFSettings.maxSaveFileSlots; index++)
                {
                    var create = Instantiate(saveFileHUDPrefab, buttonsParent);
                    create.Initialize(index);
                    elements.Add(create);
                }
            }
            for (int i = 0; i < elements.Count; i++)
            {
                AddToMenu(i, elements[i].fileSelectButton);
                elements[i].gameObject.SetActive(true);
                if (i >= TUFFSettings.maxSaveFileSlots) elements[i].gameObject.SetActive(false);
            }
            if (uiMenu.UIElements == null) uiMenu.SetupUIElements();
        }
        private void UpdateSaveFileHUDs()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].UpdateHUD();
            }
        }
        private void VerifyMenuArrays(int rows)
        {
            if (uiMenu == null) return;
            if (uiMenu.UIElementContainers == null)
            {
                uiMenu.UIElementContainers = new UIElementContainer[rows];
            }
            if (rows >= uiMenu.UIElementContainers.Length) // If row is out of row count
            {
                var newArray = new UIElementContainer[rows + 1];
                System.Array.Copy(uiMenu.UIElementContainers, newArray, uiMenu.UIElementContainers.Length);
                uiMenu.UIElementContainers = newArray;
            }
            if (uiMenu.UIElementContainers[0] == null) // Check Index 0
                uiMenu.UIElementContainers[0] = new UIElementContainer();
        }
        protected void AddToMenu(int index, UIButton element)
        {
            if (uiMenu == null) return;
            if (index >= uiMenu.UIElementContainers.Length)
            {
                VerifyMenuArrays(index);
            }
            if (uiMenu.UIElementContainers[index] == null)
                uiMenu.UIElementContainers[index] = new UIElementContainer();
            uiMenu.UIElementContainers[index].UIElements.Add(element);

            element.useCustomSelectSFX = true;
            element.customSelectSFX = new SFX();
            element.onSelect.AddListener(() => SelectFile(index));
            //element.onSelect.AddListener(() => { SelectFile(index); });
        }
        public void SelectFile(int file)
        {
            if (mode == FileSelectMenuMode.LoadFile)
            {
                bool loaded = GameManager.instance.LoadSaveData(file);
                if (loaded) { 
                    Debug.Log($"Loaded File {file}"); 
                    GameManager.instance.stopPlaytime = true; 
                    StartCoroutine(Continue()); 
                }
                else Debug.Log($"No file found at {file}");
            }
            else
            {
                GameManager.instance.SavePlayerData(file);
                AudioManager.instance.PlaySFX(TUFFSettings.saveSFX);
                uiMenu?.CloseMenu();
            }
        }
        private IEnumerator Continue()
        {
            //UIController.instance.SetMenu(null);
            AudioManager.instance.PlaySFX(TUFFSettings.loadSFX);
            GameManager.instance.DisableUIInput(true);
            UIController.instance.UIFadeScreen.FadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            yield return new WaitForSeconds(2f);
            UIController.instance.CloseAllMenus();
            GameManager.instance.stopPlaytime = false;
            //GameManager.instance.DisableUIInput(true); // Put this here to stop the menu from
            PlayerData.instance.GetSceneData(out string sceneName, out Vector3 playerPosition, out FaceDirections playerFacing);
            SceneLoaderManager.instance.LoadSceneWithFadeIn(sceneName, 0.5f, playerPosition, playerFacing, true, true,
                () => {
                    GameManager.instance.DisableUIInput(false);
                });
        }
        private void OnCloseMenu()
        {
            if (actionCallback != null) actionCallback.isFinished = true;
        }

        private void UpdatePromptText()
        {
            if (!promptText) return;
            promptText.text = (mode == FileSelectMenuMode.LoadFile ? TUFFSettings.loadFilePromptText : TUFFSettings.saveFilePromptText );
        }
    }
}
