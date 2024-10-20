using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class CommonEventManager : MonoBehaviour
    {
        // Interactable Events
        [SerializeField] private List<InteractableEvent> m_queuedInteractableEvents = new();
        [SerializeField] private List<InteractableEvent> m_parallelInteractableEvents = new();

        public static IEnumerator eventsCoroutine;
        public static bool interactableEventPlaying { get => m_interactableEventPlaying; }
        public static bool parallelProcessPlaying { get => instance.m_parallelInteractableEvents.Count > 0; }
        private static bool m_interactableEventPlaying = false;


        // Common Events
        [SerializeField] private List<CommonEvent> queuedEvents = new List<CommonEvent>();
        public bool isRunning { get => m_isRunning; }
        private bool m_isRunning = false;
        #region Singleton
        public static CommonEventManager instance { get
            {
                if (!GameManager.instance) return null;
                return GameManager.instance.commonEventManager;
            }
        }
        private void Awake()
        {

        }
        #endregion

        public void TriggerParallelProcessEvent(InteractableEvent interactableEvent)
        {
            if (m_parallelInteractableEvents.Contains(interactableEvent)) return;
            m_parallelInteractableEvents.Add(interactableEvent);
            instance.StartCoroutine(TriggerParallelProcess(interactableEvent));
        }
        public void TriggerInteractableEvent(InteractableEvent interactableEvent, bool queue = false)
        {
            if (m_interactableEventPlaying)
            {
                if (queue)
                    m_queuedInteractableEvents.Add(interactableEvent);
                //else Debug.LogWarning($"Tried to play: {interactableEvent.interactableObject} with {interactableEvent.eventList.content.Count} Events");
                return;
            }
            if (eventsCoroutine != null) StopEvents();
            eventsCoroutine = TriggerEventsCoroutine(interactableEvent);
            instance.StartCoroutine(eventsCoroutine);
        }
        public static void StopEvents()
        {
            if (eventsCoroutine != null) instance.StopCoroutine(eventsCoroutine);
            m_interactableEventPlaying = false;
            eventsCoroutine = null;
        }
        protected IEnumerator TriggerEventsCoroutine(InteractableEvent interactableEvent)
        {
            ActionList actionList = interactableEvent.actionList;
            m_interactableEventPlaying = true;
            // This is probably an ugly way of forcing input disabling if a menu is closed for example.
            // Find a better alternative
            bool yielded = false;
            yield return actionList.PlayActions(() =>
            {
                if (!GameManager.disablePlayerInput)
                {
                    GameManager.instance.DisablePlayerInput(true);
                    yielded = true;
                    Debug.Log("Stop Control");
                }
            });
            InteractableObject.UpdateAll();
            
            if (yielded) yield return new WaitForSeconds(.025f);
            GameManager.instance.DisablePlayerInput(false);
            Debug.Log("Regain Control");
            m_interactableEventPlaying = false;
            if (m_queuedInteractableEvents.Count > 0)
            {
                // Dequeue
                var evt = m_queuedInteractableEvents[0];
                m_queuedInteractableEvents.RemoveAt(0);
                TriggerInteractableEvent(evt);
            }
            else
            {
                InteractableObject.CheckAutorunTriggers();
            }
        }
        protected IEnumerator TriggerParallelProcess(InteractableEvent interactableEvent)
        {
            ActionList actionList = interactableEvent.actionList;
            yield return actionList.PlayActions();
            m_parallelInteractableEvents.Remove(interactableEvent);
            InteractableObject.CheckParallelProcessTriggers();
        }

        public void QueueCommonEvent(CommonEvent commonEvent)
        {
            if (commonEvent == null) return;
            queuedEvents.Add(commonEvent);
        }
        public void RunEvents()
        {
            if (queuedEvents.Count <= 0) return;
            StartCoroutine(UnqueueEvents());
        }
        public IEnumerator UnqueueEvents()
        {
            if (isRunning) yield break;
            m_isRunning = true;
            while (queuedEvents.Count > 0)
            {
                yield return queuedEvents[0].actionList.PlayActions();
                queuedEvents.RemoveAt(0);
            }
            m_isRunning = false;
        }
        

        public void TriggerEventActionBranch(EventAction parentEventAction, ActionList actionList)
        {
            StartCoroutine(TriggerEventActionBranchCoroutine(parentEventAction, actionList));
        }
        private IEnumerator TriggerEventActionBranchCoroutine(EventAction eventAction, ActionList actionList)
        {
            if (eventAction == null) yield break;
            if (actionList == null) { Debug.LogWarning("ActionList is null!"); eventAction.isFinished = false; yield break; }
            actionList.index = 0;
            yield return GameManager.instance.StartCoroutine(actionList.PlayActions());
            actionList.index = 0;
            eventAction.isFinished = true;
        }
    }
}
