using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    public class InteractableObject : MonoBehaviour
    {
        public int persistentID = -1;
        public int currentSwitch
        {
            get { return m_currentSwitch; }
            set { 
                m_currentSwitch = value;
                UpdatePlayerDataID();
            }
        }
        private int m_currentSwitch = 0;
        private int m_index = 0;
        public InteractableEvent[] triggerEvents = new InteractableEvent[0];

        protected static UnityEvent onUpdateAll = new();

        public void Awake()
        {
            for(int i = 0; i < triggerEvents.Length; i++)
            {
                triggerEvents[i].interactableObject = this;
            }
            LoadIndexData();
            if (!HasValidActions()) return;
            if (triggerEvents[m_index].triggerType == TriggerType.PlayOnAwake)
                StartCoroutine(PlayOnStart());
        }
        public void OnEnable()
        {
            onUpdateAll.AddListener(LoadIndexData);
            for (int i = 0; i < triggerEvents.Length; i++)
            {
                triggerEvents[i].actionList.OnEnable();
            }
        }
        public void OnDisable()
        {
            onUpdateAll.RemoveListener(LoadIndexData);
        }
        private void OnDestroy()
        {
            onUpdateAll.RemoveListener(LoadIndexData);
        }
        public bool HasValidActions()
        {
            if (triggerEvents.Length <= 0)
            {
                return false;
            }
            if (m_index < 0)
            {
                return false;
            }
            if (m_index >= triggerEvents.Length)
            {
                return false;
            }
            return true;
        }
        public void Start()
        {
            for (int i = 0; i < triggerEvents.Length; i++)
            {
                triggerEvents[i].actionList.OnStart();
            }
            LoadIndexData();
            if (!HasValidActions()) return;
            if (triggerEvents[m_index].triggerType == TriggerType.PlayOnStart)
            {
                if (HasValidActions())
                    TriggerInteractable(true);
            }
                //StartCoroutine(PlayOnStart());
        }
        public void Update()
        {
            Autorun();
        }
        private void Autorun()
        {
            if (!HasValidActions()) return;
            if (triggerEvents[m_index].triggerType == TriggerType.Autorun)
                TriggerInteractable();
        }
        public IEnumerator PlayOnStart()
        {
            if (!HasValidActions()) yield break;
            TriggerInteractable(true);
        }
        private void UpdatePlayerDataID()
        {
            GameManager.instance.playerData.AssignSwitchToPersistentID(persistentID, currentSwitch);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>If Interactable was triggered successfully.</returns>
        public bool TriggerInteractable(bool queue = false)
        {
            if (triggerEvents.Length <= 0) {
                Debug.Log("Trigger Events List is empty");
                return false;
            }
            if (m_index < 0)
            {
                Debug.LogWarning("Index is less than zero");
                return false;
            }
            if (m_index >= triggerEvents.Length)
            {
                Debug.LogWarning("Index is out of range");
                return false;
            }
            CommonEventManager.instance.TriggerInteractableEvent(triggerEvents[m_index], queue);
            return true;
        }
        public TriggerType GetCurrentIndexType()
        {
            if (triggerEvents.Length <= 0)
            {
                Debug.Log("(GetSwitch)Trigger Events List is empty");
                return TriggerType.None;
            }
            if (m_index < 0)
            {
                Debug.Log("(GetSwitch)Current Switch index is invalid (less than zero)");
                return TriggerType.None;
            }
            if (m_index >= triggerEvents.Length)
            {
                Debug.Log("(GetSwitch)Current Switch index is invalid (Out of range)");
                return TriggerType.None;
            }
            return triggerEvents[m_index].triggerType;
        }
        public void LoadIndexData()
        {
            if (persistentID >= 0 && GameManager.instance) currentSwitch = GameManager.instance.playerData.GetSwitchFromPersistentID(persistentID);
            m_index = GetIndex();
            if (!HasValidActions()) return;
            triggerEvents[m_index].LoadComponentData();
        }
        public void DebugLog(string text)
        {
            Debug.Log(text);
        }
        public int GetIndex()
        {
            for (int i = triggerEvents.Length - 1; i >= 0; i--)
            {
                if (triggerEvents[i] == null) continue;
                if (triggerEvents[i].ValidateConditions(m_currentSwitch))
                {
                    return i;
                }
            }
            return -1;
        }
        public static void UpdateAll()
        {
            onUpdateAll?.Invoke();
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            CheckForPlayerLayer(collision);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            CheckForPlayerLayer(collision.collider);
        }

        private void CheckForPlayerLayer(Collider2D collision)
        {
            if (1 << collision.gameObject.layer == LayerMask.GetMask("Player"))
            {
                if (!HasValidActions()) return;
                if (triggerEvents[m_index].triggerType == TriggerType.PlayerTouch)
                {
                    TriggerInteractable();
                }
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, TUFFSettings.interactableGizmoFilename, true);
        }
    }
}
