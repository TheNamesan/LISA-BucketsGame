using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(OpenShopAction))]
    public class OpenShopActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("shopData"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as OpenShopAction;
            var shopData = action.shopData;
            if (shopData == null) return "Open Shop";
            string summary = "Open Shop: (";
            for (int i = 0; i < shopData.shopItems.Count; i++)
            {
                if (shopData.shopItems[i] == null) continue;
                var item = shopData.shopItems[i].invItem;
                if (item == null) continue;
                summary += $"{item.GetName()} |";
            }
            summary += ")";
            return summary;
        }
    }
}

