using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ActionList
    {
        public int index = 0;
        [SerializeReference]
        public List<EventAction> content = new List<EventAction>();
        public IEnumerator PlayActions(System.Action onActionPlaying = null)
        {
            for (index = 0;
                index < content.Count;
                index++)
            {
                content[index].parent = this;
                content[index].isFinished = false;
                content[index].Invoke();
                while (!content[index].isFinished)
                {
                    onActionPlaying?.Invoke();
                    yield return null;
                }
            }
            index = 0;
        }
        public void OnEnable()
        {
            for (int i = 0; i < content.Count; i++)
            {
                content[i].OnEnable();
            }
        }
        public void OnStart()
        {
            for (int i = 0; i < content.Count; i++)
            {
                content[i].OnStart();
            }
        }
        public int GetActionIndex(EventAction action)
        {
            return content.IndexOf(action);
        }
    }
}

