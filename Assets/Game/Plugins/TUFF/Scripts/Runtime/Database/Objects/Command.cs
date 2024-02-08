using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Command", menuName = "Database/Command", order = 3)]
    public class Command : DatabaseElement
    {
        [Header("General Settings")]
        [Tooltip("Name localization key from the Command Table Collection.")]
        public string nameKey = "name_key";
        [Tooltip("Command's icon.")]
        public Sprite icon;
        [Tooltip("Command's grouping type.\nSingle: Only the first element on the list is used.\nGroup: Skills are grouped in a separate menu.")]
        public CommandType commandType;
        public List<SkillsLearned> skills = new List<SkillsLearned>();
        public override string GetName()
        {
            return TUFFTextParser.ParseText(nameKey);
        }
    }
}
