using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public enum ChoicesCancelBehaviour
    {
        Disallow = 0,
        Branch = 1,
        ChooseIndex = 2
    }
    [System.Serializable]
    public class ShowChoicesAction : EventAction
    {
        [Tooltip("Determines which action to trigger when the choices menu is canceled.")]
        public ChoicesCancelBehaviour cancelBehaviour = ChoicesCancelBehaviour.Disallow;
        public int cancelChoiceIndex = 0;
        public List<ShowChoicesBranch> choices = new();
        public ActionList cancelActionList = new();

        public ShowChoicesAction()
        {
            eventName = "Show Choices";
        }
        public override void Invoke()
        {
            if (choices == null || choices.Count <= 0) { 
                Debug.LogWarning("No choices assigned!"); 
                isFinished = true; 
                return; 
            }

            List<string> texts = new();
            for (int i = 0; i < choices.Count; i++)
            {
                texts.Add(choices[i].choice);
            }
            bool closeWithCancel = !(cancelBehaviour == ChoicesCancelBehaviour.Disallow);
            UIController.instance.ShowChoices(this, texts, closeWithCancel, CancelOption);
        }
        public void PickOption(int index)
        {
            Debug.Log("Picked option: " + index);
            if (index >= 0 && index < choices.Count)
            {
                CommonEventManager.instance.TriggerEventActionBranch(this, choices[index].actionList);
            }
            else {
                Debug.LogWarning($"Index {index} is out of bounds!");
                isFinished = true;
            }
        }
        public void CancelOption()
        {
            if (cancelBehaviour == ChoicesCancelBehaviour.ChooseIndex)
            {
                PickOption(cancelChoiceIndex);
            }
            else if (cancelBehaviour == ChoicesCancelBehaviour.Branch)
            {
                CommonEventManager.instance.TriggerEventActionBranch(this, cancelActionList);
            }
        }
        private List<ActionList> GetAllActionLists()
        {
            var allActionLists = new List<ActionList>();
            for (int i = 0; i < choices.Count; i++)
            {
                allActionLists.Add(choices[i].actionList);
            }
            if (cancelBehaviour == ChoicesCancelBehaviour.Branch) allActionLists.Add(cancelActionList);
            return allActionLists;
        }
        public override void OnEnable()
        {
            var allActionLists = GetAllActionLists();
            for (int i = 0; i < allActionLists.Count; i++)
                allActionLists[i].OnEnable();
        }
        public override void OnStart()
        {
            var allActionLists = GetAllActionLists();
            for (int i = 0; i < allActionLists.Count; i++)
                allActionLists[i].OnStart();
        }
    }

    [System.Serializable]
    public class ShowChoicesBranch
    {
        public string choice = "";
        public ActionList actionList = new();
        public string ParsedChoiceText()
        {
            return TUFFTextParser.ParseText(choice);
        }
    }

}
