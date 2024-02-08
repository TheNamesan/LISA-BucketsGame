using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "KeyItem", menuName = "Database/KeyItem", order = 5)]
    public class KeyItem : InventoryItem
    {
        [Header("General Settings")]
        [Tooltip("Name localization key from the KeyItem Table Collection.")]
        public string nameKey = "name_key";
        [Tooltip("Description localization key from the KeyItem Table Collection."), TextArea(2, 2)]
        public string descriptionKey = "description_key";

        public override string GetName()
        {
            return TUFFTextParser.ParseText(nameKey);
        }
        public override string GetDescription()
        {
            return TUFFTextParser.ParseText(descriptionKey);
        }
    }
}
