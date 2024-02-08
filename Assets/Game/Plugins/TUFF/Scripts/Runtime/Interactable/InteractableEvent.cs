using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    public enum TriggerType
    {
        ActionButton = 0,
        PlayerTouch = 1,
        ActionFaceUp = 2,
        ActionFaceDown = 3,
        PlayOnStart = 4,
        PlayOnAwake = 5,
        None = 6,
        Autorun = 7
    }

    [System.Serializable]
    public class InteractableEvent
    {
        [HideInInspector] public InteractableObject interactableObject = null;
        [Tooltip("Condition to run this Interactable's events.\n" + "Action Button: The player presses Z while looking at the object.\n" + "Player Touch: The player touches the object.\n" + "Action Face Up/Down: The player presses Z while looking at the object while facing up/down.\n" + "Play On Start: Autoruns the event when the Scene is loaded.\n" + "None: No condition. Never runs on its own.")]
        public TriggerType triggerType;
        [Tooltip("Condition to check. If all are true")]
        public InteractableEventConditions conditions = new();

        [Tooltip("Reference to the SpriteRenderer component.")]
        public SpriteRenderer spriteRef;
        [Tooltip("Sprite to change to when this event's index is selected.")]
        public Sprite graphic;
        [Tooltip("Rendering order within a sorting layer.")]
        public int orderInLayer = -1;
        [Tooltip("If true, will override the SpriteRenderer's transform.")]
        public bool overrideSpriteTransform = false;
        public Vector3 spriteLocalPosition = Vector3.zero;
        public Vector3 spriteScale = Vector3.one;
        [Tooltip("If true, deactivates the SpriteRenderer component.")]
        public bool disableSprite = false;

        [Tooltip("Reference to the Collider component.")]
        public Collider2D colliderRef;
        [Tooltip("Sets the collider's isTrigger toggle.")]
        public bool setIsTrigger = false;
        [Tooltip("If true, deactivates the Collider component.")]
        public bool disableCollider = false;

        public UnityEvent onSwitchDataLoad = new UnityEvent();

        public InteractableEventList eventList = new InteractableEventList();
        public ActionList actionList = new ActionList();

        
        public void LoadComponentData()
        {
            if (spriteRef != null)
            {
                spriteRef.enabled = !disableSprite;
                spriteRef.sprite = graphic;
                spriteRef.sortingOrder = orderInLayer;
                if (overrideSpriteTransform)
                {
                    spriteRef.transform.localPosition = spriteLocalPosition;
                    spriteRef.transform.localScale = spriteScale;
                }
            }
            if (colliderRef != null)
            {
                colliderRef.enabled = !disableCollider;
                colliderRef.isTrigger = setIsTrigger;
            }
            onSwitchDataLoad?.Invoke();
        }
        public bool ValidateConditions(int interactableSwitch) => conditions.ValidateConditons(interactableSwitch);
    }


    [System.Serializable]
    public class InteractableEventConditions
    {
        public List<InteractableEventConditionElement> elements = new();
        public bool ValidateConditons(int interactableSwitch)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (element == null) continue;
                if (!element.ValidateCondition(interactableSwitch))
                    return false;
            }
            return true;
        }
    }

    public enum InteractableEventConditionType { SelfSwitch = 0, GameVariable = 1, ItemInInventory = 2, UnitInParty = 3 }
    [System.Serializable]
    public class InteractableEventConditionElement
    {
        [Tooltip("If true, this element will be valid when the conditions is not true.")]
        public bool not = false;
        public InteractableEventConditionType conditionType;
        // Switch
        public int targetSwitch = 0;
        // Game Variable
        public GameVariableComparator variableComparator;
        // Item
        public InventoryItem targetItem = null;
        // Party
        public Unit targetUnit;
        public bool ValidateCondition(int interactableSwitch)
        {
            bool valid = true;
            switch (conditionType)
            {
                case InteractableEventConditionType.SelfSwitch:
                    valid = interactableSwitch == targetSwitch; break;
                case InteractableEventConditionType.GameVariable:
                    valid = variableComparator.ValidateGameVariable(); break;
                case InteractableEventConditionType.ItemInInventory:
                    valid = Inventory.instance.HasItem(targetItem); break;
                case InteractableEventConditionType.UnitInParty:
                    valid = PlayerData.instance.IsInParty(targetUnit); break;
            }
            if (not) valid = !valid;
            return valid;
        }
    }
}
