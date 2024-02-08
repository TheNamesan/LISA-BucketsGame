using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ConditionalBranchEvent : EventCommand
    {
        public List<BranchContent> branches = new List<BranchContent>();
        public override void Invoke()
        {
            if(branches.Count <= 0)
            {
                isFinished = true;
                Debug.Log("Count is 0");
                return;
            }
            int index = -1;
            for(int i = 0; i < branches.Count; i++)
            {
                if(branches[i].VerifyCondition())
                {
                    index = i;
                    break;
                }
            }
            if(index < 0)
            {
                isFinished = true;
                Debug.Log("Couldn't find condition as true");
                return;
            }
            GameManager.instance.StartCoroutine(TriggerEvents(index));
        }

        public IEnumerator TriggerEvents(int index)
        {
            InteractableEventList eventList = branches[index].content;
            eventList.currentEventIndex = 0;
            yield return GameManager.instance.StartCoroutine(RunEvents(eventList));
            eventList.currentEventIndex = 0;
            isFinished = true;
        }

        private IEnumerator RunEvents(InteractableEventList eventList)
        {
            for (eventList.currentEventIndex = 0; 
                eventList.currentEventIndex < eventList.content.Count; 
                eventList.currentEventIndex++)
            {
                int i = eventList.currentEventIndex;
                eventList.content[i].isFinished = false;
                eventList.content[i].Invoke();
                while (!eventList.content[i].isFinished)
                {
                    yield return null;
                }
            }
            eventList.currentEventIndex = 0;
        }

        public override void OnInstantiate()
        {
            eventName = "Conditional Branch";
            branches = new List<BranchContent>();
        }
    }
}
