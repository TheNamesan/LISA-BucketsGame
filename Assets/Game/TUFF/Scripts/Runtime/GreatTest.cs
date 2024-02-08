using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class GreatTest : MonoBehaviour
    {
        [SerializeReference]
        public List<EventAction> content = new List<EventAction>();

        [SerializeReference]
        public EventAction evt = new EventAction();
        //[SerializeReference]
        //public WaitSecondsAction act = new WaitSecondsAction();

        [SerializeReference]
        public List<EventAction> op = new List<EventAction>() { new EventAction(), new WaitSecondsAction() };
    }
}

