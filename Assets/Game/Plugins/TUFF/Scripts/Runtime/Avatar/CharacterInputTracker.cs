using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public struct CharacterInputStream
    {
        [Tooltip("Character current horizontal input value.")]
        [SerializeField] public float horizontalInput;
        [SerializeField] public float horizontalInputTap;
        [Tooltip("Character current vertical input value.")]
        public float verticalInput;
        public float verticalInputTap;
        public bool interactionButtonDown;
        public bool interactionButtonHold;
        public bool runButtonDown;
        public bool runButtonHold;
        public bool pauseButtonDown;
        public bool pauseButtonHold;

        /// <summary>
        /// Checks if current input stream has any active button.
        /// </summary>
        /// <returns>Returns true if at least one button or stick direction is being pressed.</returns>
        public bool HasInput()
        {
            var fields = this.GetType().GetFields();

            foreach (var field in fields)
            {
                // Check if the field is of type float
                if (field.FieldType == typeof(float))
                {
                    // Check if the int variable is not equal to 0
                    if ((float)field.GetValue(this) != 0)
                    {
                        return true; // Input is not 0
                    }
                }
                // Check if the field is of type int
                if (field.FieldType == typeof(int))
                {
                    // Check if the int variable is not equal to 0
                    if ((int)field.GetValue(this) != 0)
                    {
                        return true; // Input is not 0
                    }
                }
                // Check if the field is of type bool
                else if (field.FieldType == typeof(bool))
                {
                    // Check if the bool variable is not false
                    if ((bool)field.GetValue(this) == true)
                    {
                        return true; // Input is not false
                    }
                }
            }
            return false;
        }
    }
    public class CharacterInputTracker : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

