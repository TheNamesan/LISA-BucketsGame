using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    /// <summary>
    /// UI Data of a set Command
    /// </summary>
    public class CommandElement : MonoBehaviour
    { 
        public Image icon;
        public TMP_Text text;
        [SerializeField] Command command;

        public void SetCommand(Command setCommand)
        {
            command = setCommand;
        }

        public Command GetCommand()
        {
            return command;
        }

        public void LoadCommandInfo()
        {
            icon.sprite = command.icon;
            text.text = command.GetName();
        }
    }
}
