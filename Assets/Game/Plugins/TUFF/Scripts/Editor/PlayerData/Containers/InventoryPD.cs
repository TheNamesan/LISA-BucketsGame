using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(Inventory))]
    public class InventoryPD : PropertyDrawer
    {
        private static Texture2D[] itemIcons = null;
        private static Texture2D[] keyItemIcons = null;
        private static Texture2D[] weaponIcons = null;
        private static Texture2D[] armorIcons = null;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
            }
            int lines = 4;
            var inventory = LISAEditorUtility.GetTargetObjectOfProperty(property) as Inventory;
            var items = property.FindPropertyRelative("items");
            var keyItems = property.FindPropertyRelative("keyItems");
            var weapons = property.FindPropertyRelative("weapons");
            var armors = property.FindPropertyRelative("armors");
            if (items.isExpanded) lines += items.arraySize;
            if (keyItems.isExpanded) lines += keyItems.arraySize;
            if (weapons.isExpanded) lines += weapons.arraySize;
            if (armors.isExpanded) lines += armors.arraySize;
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                ((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lines) + 
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgX = position.x;

            var inventory = LISAEditorUtility.GetTargetObjectOfProperty(property) as Inventory;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                GetIconTextures(property);

                var items = property.FindPropertyRelative("items");
                var orgColor = GUI.color;
                GUI.color = Color.grey;
                GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, 20f + (items.isExpanded ? 20f * items.arraySize : 0f)), "");
                GUI.color = orgColor;

                items.isExpanded = EditorGUI.Foldout(position, items.isExpanded, items.displayName, true);
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if(items.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    for (int i = 0; i < items.arraySize; i++)
                    {
                        if (DatabaseLoader.instance == null) break;
                        DrawIconPreview(position, itemIcons[i]);
                        EditorGUI.PropertyField(position, items.GetArrayElementAtIndex(i), new GUIContent(DatabaseLoader.instance.items[i].GetName()));
                        position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    }
                    position.x -= 15f;
                    position.width += 15f;
                }

                var keyItems = property.FindPropertyRelative("keyItems");
                GUI.color = Color.grey;
                GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, 20f + (keyItems.isExpanded ? 20f * keyItems.arraySize : 0f)), "");
                GUI.color = orgColor;

                keyItems.isExpanded = EditorGUI.Foldout(position, keyItems.isExpanded, keyItems.displayName, true);
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if (keyItems.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    for (int i = 0; i < keyItems.arraySize; i++)
                    {
                        if (DatabaseLoader.instance == null) break;
                        DrawIconPreview(position, keyItemIcons[i]);
                        EditorGUI.PropertyField(position, keyItems.GetArrayElementAtIndex(i), new GUIContent(DatabaseLoader.instance.keyItems[i].GetName()));
                        position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    }
                    position.x -= 15f;
                    position.width += 15f;
                }

                var weapons = property.FindPropertyRelative("weapons");
                GUI.color = Color.grey;
                GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, 20f + (weapons.isExpanded ? 20f * weapons.arraySize : 0f)), "");
                GUI.color = orgColor;

                weapons.isExpanded = EditorGUI.Foldout(position, weapons.isExpanded, weapons.displayName, true);
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if (weapons.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    for (int i = 0; i < weapons.arraySize; i++)
                    {
                        if (DatabaseLoader.instance == null) break;
                        DrawIconPreview(position, weaponIcons[i]);
                        EditorGUI.PropertyField(position, weapons.GetArrayElementAtIndex(i), new GUIContent(DatabaseLoader.instance.weapons[i].GetName()));
                        position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    }
                    position.x -= 15f;
                    position.width += 15f;
                }

                var armors = property.FindPropertyRelative("armors");
                GUI.color = Color.grey;
                GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, 20f + (armors.isExpanded ? 20f * armors.arraySize : 0f)), "");
                GUI.color = orgColor;

                armors.isExpanded = EditorGUI.Foldout(position, armors.isExpanded, armors.displayName, true);
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if (armors.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    for (int i = 0; i < armors.arraySize; i++)
                    {
                        if (DatabaseLoader.instance == null) break;
                        DrawIconPreview(position, armorIcons[i]);
                        EditorGUI.PropertyField(position, armors.GetArrayElementAtIndex(i), new GUIContent(DatabaseLoader.instance.armors[i].GetName()));
                        position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    }
                    position.x -= 15f;
                    position.width += 15f;
                }
                position.width += 15f;
            }
            position.x = orgX;
            property.serializedObject.ApplyModifiedProperties();
        }

        private static void GetIconTextures(SerializedProperty property)
        {
            if (DatabaseLoader.instance == null) return;
            if (itemIcons != null && keyItemIcons != null && weaponIcons != null && armorIcons != null) return;
            var items = property.FindPropertyRelative("items");
            var keyItems = property.FindPropertyRelative("keyItems");
            var weapons = property.FindPropertyRelative("weapons");
            var armors = property.FindPropertyRelative("armors");
            itemIcons = new Texture2D[items.arraySize];
            keyItemIcons = new Texture2D[keyItems.arraySize];
            weaponIcons = new Texture2D[weapons.arraySize];
            armorIcons = new Texture2D[armors.arraySize];

            for (int i = 0; i < itemIcons.Length; i++)
            {
                itemIcons[i] = AssetPreview.GetAssetPreview(DatabaseLoader.instance.items[i].icon);
            }
            for (int i = 0; i < keyItemIcons.Length; i++)
            {
                keyItemIcons[i] = AssetPreview.GetAssetPreview(DatabaseLoader.instance.keyItems[i].icon);
            }
            for (int i = 0; i < weaponIcons.Length; i++)
            {
                weaponIcons[i] = AssetPreview.GetAssetPreview(DatabaseLoader.instance.weapons[i].icon);
            }
            for (int i = 0; i < armorIcons.Length; i++)
            {
                armorIcons[i] = AssetPreview.GetAssetPreview(DatabaseLoader.instance.armors[i].icon);
            }
        }

        private void DrawIconPreview(Rect position, Texture2D sprite)
        {
            var preview = sprite;
            if (preview != null) GUI.DrawTexture(new Rect(position.x - 2, position.y + 2, 18f, 18f), preview);
        }
    }
}

