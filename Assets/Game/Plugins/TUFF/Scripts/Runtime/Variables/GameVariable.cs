using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public enum GameVariableType { PerSave = 0, Global = 1 }
    public enum GameVariableValueType { BoolValue = 0, NumberValue = 1, StringValue = 2, VectorValue = 3 }
    [System.Serializable]
    public struct GameVariable
    {
        [SerializeField] private string m_name;
        [SerializeField] private GameVariableType m_variableType;
        // TODO: Make Global Game Variables be saved in a separate file. 
        // When saving Global Variables, the value must be recorded both in the current save file and the global save file.
        // When loading any save file, the global save file values must be assigned to the local ones.
        [SerializeField] private bool m_boolValue;
        [SerializeField] private float m_numberValue;
        [SerializeField] private string m_stringValue;
        [SerializeField] private Vector2 m_vectorValue;
       
        public string name { get => m_name; set => m_name = value; }
        public GameVariableType variableType { get => m_variableType; set => m_variableType = value; }
        public bool boolValue { get => m_boolValue; set => m_boolValue = value; }
        public float numberValue { get => m_numberValue; set => m_numberValue = value; }
        public string stringValue { get => m_stringValue; set => m_stringValue = value; }
        public Vector2 vectorValue { get => m_vectorValue; set => m_vectorValue = value; }

        public GameVariable(string name, GameVariableType variableType)
        {
            m_name = name;
            m_variableType = variableType;
            m_boolValue = false;
            m_numberValue = 0;
            m_stringValue = "";
            m_vectorValue = new Vector2();
        }
        public void AssignValue(object value)
        {
            if (value is bool bol)
                m_boolValue = bol;
            if (value is float flt)
                m_numberValue = flt;
            if (value is int i)
                m_numberValue = i;
            if (value is string str)
                m_stringValue = str;
            if (value is Vector2 vec)
                m_vectorValue = vec;
        }
        public bool EqualsValue(object value)
        {
            if (value is bool bol)
                return m_boolValue == bol;
            if (value is float flt)
               return m_numberValue == flt;
            if (value is int i)
                return m_numberValue == i;
            if (value is string str)
                return m_stringValue == str;
            if (value is Vector2 vec)
                return m_vectorValue == vec;
            return false;
        }
    }
}

