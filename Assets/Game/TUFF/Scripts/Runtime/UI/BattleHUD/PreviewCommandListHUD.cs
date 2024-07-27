using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class PreviewCommandListHUD : MonoBehaviour
    {
        public CommandElement commandElementPrefab;
        public Transform elementsParent;
        public UIMenu uiMenu;
        public ScrollRectForUIMenu scrollRect;
        public PreviewCommandSubmenuHUD previewCommandSubmenuHUD;
        public List<CommandElement> elements = new();
        [System.NonSerialized] public PartyMember memberRef;

        protected bool initialized = false;
        protected bool elementAdded = false;

        private void Awake()
        {
            Initialize();
        }
        private void Initialize()
        {
            if (initialized) return;
            if (uiMenu == null) uiMenu = GetComponent<UIMenu>();
            InitializeElements();
            initialized = true;
        }
        private void InitializeElements()
        {
            uiMenu?.ExpandRows(0);
            foreach (Transform child in elementsParent) // Add existing GameObjects to list
            {
                if (child.TryGetComponent(out CommandElement existing))
                {
                    ApplyEventsToElement(existing);
                    elements.Add(existing);
                }
            }
            for (int i = 0; i < elements.Count; i++)
            {
                AddToMenu(i, elements[i].uiButton);
                elements[i].gameObject.SetActive(true);
            }
            if (uiMenu.UIElements == null) SetupElements();
        }
        protected void AddToMenu(int index, UIButton element)
        {
            if (uiMenu == null) return;
            if (index >= uiMenu.UIElementContainers.Length)
            {
                int rowsDelta = (index + 1) - uiMenu.UIElementContainers.Length;
                uiMenu?.ExpandRows(uiMenu.UIElementContainers.Length + rowsDelta);
            }
            if (uiMenu.UIElementContainers[index] == null)
                uiMenu.UIElementContainers[index] = new UIElementContainer();
            uiMenu.UIElementContainers[index].UIElements.Add(element);
        }
        public void UpdateCommands(PartyMember partyMember)
        {
            Initialize();
            memberRef = partyMember;
            if (memberRef == null) { Debug.LogWarning("No Party Member Reference"); return; }
            var commandList = memberRef.GetCommands();
            UpdateContent(commandList);

            if (elementAdded) SetupElements();
            if (scrollRect) scrollRect.UpdateScroll();
        }

        private void SetupElements()
        {
            uiMenu.SetupUIElements();
            if (scrollRect)
            {
                scrollRect.uiMenu = uiMenu;
            }
            elementAdded = false;
        }

        protected void UpdateContent(List<Command> commandList)
        {
            int row = 0;
            if (commandList == null) return;
            uiMenu?.ExpandRows(commandList.Count);

            for (; row < commandList.Count; row++)
            {
                if (row >= elements.Count)
                {
                    InstantiateCommandElement(commandList[row], row);
                }
                else
                {
                    UpdateCommand(elements[row], commandList[row]);
                }
                elements[row].gameObject.SetActive(true);
            }
            for (int i = row; i < elements.Count; i++)
                elements[i].gameObject.SetActive(false);
            //CheckIfCurrentHighlightIsValid();
        }

        private void InstantiateCommandElement(Command command, int row)
        {
            CommandElement create = Instantiate(commandElementPrefab, elementsParent);
            ApplyEventsToElement(create);

            UpdateCommand(create, command);

            elements.Add(create);
            AddToMenu(row, create.uiButton);
            elementAdded = true;
        }

        private static void UpdateCommand(CommandElement create, Command command)
        {
            if (!command) return;
            if (!create) return;
            create.SetCommand(command);
            create.LoadCommandInfo();

            UIButton button = create.uiButton;
            if (button)
            {
                //if (memberRef.HasCommandSeal(command)) button.disabled = true;
                button.highlightDisplayText = command.GetDescription();
                //if (command.IsSubmenuType())
                //    button.disabled = false;
                //else button.disabled = true;
            }
        }
        private void ApplyEventsToElement(CommandElement element)
        {
            if (!element) return;
            if (!element.uiButton) return;
            
            element.uiButton.onHighlight.AddListener(() => OnCommandHighlight(element.GetCommand()));
            element.uiButton.onSelect.AddListener(() => OnCommandSelect(element.GetCommand()));
        }
        private void OnCommandHighlight(Command command)
        {
            if (!command) return;
            if (previewCommandSubmenuHUD)
            {
                if (command.IsSubmenuType())
                {
                    previewCommandSubmenuHUD.UpdateSubmenu(command, memberRef);
                }
                else previewCommandSubmenuHUD.ClearSubmenu();
            }
        }
        private void OnCommandSelect(Command command)
        {
            if (!command) return;
            if (command.IsSubmenuType())
            {
                if (previewCommandSubmenuHUD)
                    previewCommandSubmenuHUD.uiMenu.OpenMenu();
            }
        }
    }

}
