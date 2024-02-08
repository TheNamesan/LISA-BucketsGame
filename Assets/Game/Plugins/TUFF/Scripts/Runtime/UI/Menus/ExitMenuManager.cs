using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ExitMenuManager : MonoBehaviour
    {
        public void ExitButton()
        {
            Debug.Log("Exiting Game");
            Application.Quit();
        }
    }
}
