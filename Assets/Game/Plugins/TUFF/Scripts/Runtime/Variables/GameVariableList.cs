using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TUFF
{
    [CreateAssetMenu(fileName = "GameVariableList", menuName = "TUFF/Variables/Game Variables List", order = 98)]
    public class GameVariableList : ScriptableObject
    {
        public static readonly string GAMEVARIABLELIST_FILE = "GameVariableList";
        public static readonly string GAMEVARIABLELIST_PATH = $"Assets/Resources/{ GAMEVARIABLELIST_FILE }.asset";
        [SerializeField] private GameVariableData[] m_gameVariableData = new GameVariableData[0];

        public static GameVariableList instance
        {
            get => GetOrCreateInstance();
        }
        internal static GameVariableList GetOrCreateInstance()
        {
            var settings = Resources.Load<GameVariableList>(GAMEVARIABLELIST_FILE);
#if UNITY_EDITOR
            if (settings == null)
            {
                settings = CreateInstance<GameVariableList>();
                AssetDatabase.CreateAsset(settings, GAMEVARIABLELIST_PATH);
                AssetDatabase.SaveAssets();
            }
#endif
            return settings;
        }
        public static int GetListLength()
        {
            return instance.m_gameVariableData.Length;
        }
        public static GameVariableData[] GetList()
        {
            return instance.m_gameVariableData;
        }
        public static string GetVariableName(int index)
        {
            string name = "null";
            if (instance != null && index >= 0 && index < GetListLength())
                name = instance.m_gameVariableData[index].name;
            return name;
        }

    }
    [System.Serializable]
    public struct GameVariableData
    {
        [SerializeField] private string m_name;
        [SerializeField] private GameVariableType m_variableType;
        public string name { get => m_name; }
        public GameVariableType variableType { get => m_variableType; }
    }
}
