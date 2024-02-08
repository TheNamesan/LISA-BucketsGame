using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Settings;
using UnityEditor.Localization.UI;

#if UNITY_EDITOR
namespace TUFF.TUFFEditor
{
    /// <summary>
    /// Help me Lisa?
    /// </summary>
    public static class LISAEditorUtility
    {
        // Sprite Utility
        private static System.Type spriteUtility = null;
        private static MethodInfo renderStaticPreviewMethod_A = null;
        private static MethodInfo renderStaticPreviewMethod_B = null;

        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

        public static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        public static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }

        public static int GetArrayIndexFromPath(string path)
        {
            if (path.Contains("["))
            {
                return int.Parse(path.Substring(path.LastIndexOf('[') + 1, 1));
            }
            return -1;
        }

        public static void DrawSprite(Rect rect, Sprite sprite)
        {
            if (sprite == null) return;
            Rect spriteRect = sprite.rect;
            Texture2D tex = sprite.texture;
            GUI.DrawTextureWithTexCoords(rect, tex, new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height));
        }

        public static Texture2D SpriteRenderStaticPreview(Sprite sprite, Color color, int width, int height, Matrix4x4 transform)
        {
            if (sprite == null) return null;
            if (spriteUtility == null) spriteUtility = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.SpriteUtility");
            if (renderStaticPreviewMethod_A == null)
                renderStaticPreviewMethod_A = spriteUtility.GetMethod("RenderStaticPreview", new System.Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int), typeof(Matrix4x4) });
            Texture2D texture = (Texture2D)renderStaticPreviewMethod_A.Invoke(spriteUtility, new object[] { sprite, color, width, height, transform });
            return texture;
        }
        public static Texture2D SpriteRenderStaticPreview(Sprite sprite, Color color, int width, int height)
        {
            if (sprite == null) return null;
            if (spriteUtility == null) spriteUtility = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.SpriteUtility");
            if (renderStaticPreviewMethod_B == null)
                renderStaticPreviewMethod_B = spriteUtility.GetMethod("RenderStaticPreview", new System.Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
            Texture2D texture = (Texture2D)renderStaticPreviewMethod_B.Invoke(spriteUtility, new object[] { sprite, color, width, height });
            return texture;
        }

        public static PropertyDrawer GetPropertyDrawer(SerializedProperty property)
        {
            var drawerType = GetPropertyDrawerType(property);
            if (drawerType == null) return null;
            var drawer = CreatePropertyDrawerInstance(drawerType);
            return drawer;
        }

        public static System.Type GetPropertyDrawerType(SerializedProperty property)
        {
            if (property == null) return null;
            //Debug.Log("Path: " + property.propertyPath + " [" + property.propertyType + "]");
            // Return Field Info
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
            BindingFlags nonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;

            System.Type scriptAttributeUtility = Assembly.GetAssembly(typeof(PropertyDrawer)).GetType("UnityEditor.ScriptAttributeUtility");

            var handler = GetPropertyHandler(property);
            PropertyInfo propertyDrawerPropertyInfo = handler.GetType().GetProperty("propertyDrawer", nonPublicInstance);
            MethodInfo getPropertyDrawer = propertyDrawerPropertyInfo.GetAccessors(true)[0];
            System.Type type = (getPropertyDrawer.Invoke(handler, new object[0])).GetType();
            //Debug.Log("GOTCHA: " + getPropertyDrawer.Invoke(handler, new object[0]));

            return type;
        }

        public static void DrawThing(SerializedProperty property)
        {
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
            BindingFlags nonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;
            System.Type scriptAttributeUtility = Assembly.GetAssembly(typeof(PropertyDrawer)).GetType("UnityEditor.ScriptAttributeUtility");

            var handler = GetPropertyHandler(property);

            //FieldInfo m_PropertyDrawers = handler.GetType().GetField("m_PropertyDrawers", nonPublicInstance);
            //var listThing = (List<PropertyDrawer>)(m_PropertyDrawers.GetValue(handler));
            //Debug.Log(listThing.Count);
            //FieldInfo tooltip = handler.GetType().GetField("tooltip");
            //PropertyInfo propertyDrawerPropertyInfo = handler.GetType().GetProperty("propertyDrawer", nonPublicInstance);
            //FieldInfo m_NestingLevel = handler.GetType().GetField("m_NestingLevel", nonPublicInstance);
            //Debug.Log("nesting level: " + (int)m_NestingLevel.GetValue(handler));

            //MethodInfo getPropertyDrawer = propertyDrawerPropertyInfo.GetAccessors(true)[0];
            //Debug.Log("GOTCHA: " + getPropertyDrawer.Invoke(handler, new object[0]));

            MethodInfo OnGUILayout = handler.GetType().GetMethod("OnGUILayout", BindingFlags.Public | BindingFlags.Instance);
            OnGUILayout.Invoke(handler, new object[] { property, new GUIContent("LISA"), true, null });
        }

        public static object GetPropertyHandler(SerializedProperty property)
        {
            if (property == null) return null;
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
            System.Type scriptAttributeUtility = Assembly.GetAssembly(typeof(PropertyDrawer)).GetType("UnityEditor.ScriptAttributeUtility");
            MethodInfo GetHandler = scriptAttributeUtility.GetMethod("GetHandler", bindingFlags);

            var handler = GetHandler.Invoke(scriptAttributeUtility, new object[] { property });
            return handler;
        }

        public static PropertyDrawer GetEventActionPDDrawerForClass(System.Type classType)
        {
            Debug.Log(classType);
            var drawerAttribute = GetEventActionPDAttribute(classType);
            if (drawerAttribute != null)
            {
                var propertyDrawerType = drawerAttribute.GetType();
                return CreatePropertyDrawerInstance(propertyDrawerType);
            }

            return null;
        }

        private static EventActionPD GetEventActionPDAttribute(System.Type classType)
        {
            Debug.Log(classType.GetCustomAttributes(typeof(CustomPropertyDrawer), true).Length);
            return null;
            //var attributes = classType.GetCustomAttributes(typeof(CustomPropertyDrawerAttribute), true);
            var attributes = classType.GetCustomAttributes(true);
            Debug.Log(attributes[0].GetType());
            if (attributes != null && attributes.Length > 0)
            {
                Debug.Log(attributes.Length);
                return attributes[0] as EventActionPD;
            }

            return null;
        }

        private static PropertyDrawer CreatePropertyDrawerInstance(System.Type propertyDrawerType)
        {
            var propertyDrawer = System.Activator.CreateInstance(propertyDrawerType) as PropertyDrawer;
            return propertyDrawer;
        }

        public static string GetIndexOfElementLabel(string orgLabel)
        {
            return orgLabel.Split(" ")[1];
        }

        public static void DrawDatabaseParsedTextPreview(string label, string text, bool wrapText = false, string prefix = "")
        {
            if (LocalizationSettings.HasSettings) LocalizationSettings.Instance.GetInitializationOperation();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label, EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
            GUIStyle contentStyle = new GUIStyle(GUI.skin.GetStyle("label"))
            {
                alignment = TextAnchor.UpperLeft,
                wordWrap = wrapText
            };
            EditorGUILayout.LabelField($"{prefix}{TUFFTextParser.ParseText(text)}", contentStyle);
            EditorGUILayout.BeginVertical(GUILayout.Width(120));
            string[] options = new string[LocalizationSettings.AvailableLocales.Locales.Count];
            int[] values = new int[LocalizationSettings.AvailableLocales.Locales.Count];
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = LocalizationSettings.AvailableLocales.Locales[i].LocaleName;
                values[i] = i;
            }
            LISAUtility.SelectLocale(EditorGUILayout.IntPopup(LISAUtility.GetSelectedLocaleIndex(), options, values));
            if (GUILayout.Button("Open Tables", (EditorStyles.miniButton)))
            {
                LocalizationTablesWindow.ShowWindow();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        public static void DrawDatabaseLocalizationPreview(string label, string tableCollectionName, string key, bool wrapText = false, string prefix = "")
        {
            LISAUtility.CheckLocaleIsNotNull();
            if (LISAUtility.GetSelectedLocaleIndex() < 0) return;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label, EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
            GUIStyle contentStyle = new GUIStyle(GUI.skin.GetStyle("label"))
            {
                alignment = TextAnchor.UpperLeft,
                wordWrap = wrapText
            };
            EditorGUILayout.LabelField($"{prefix}{LISAUtility.GetLocalizedText(tableCollectionName, key)}", contentStyle);
            EditorGUILayout.BeginVertical(GUILayout.Width(120));
            string[] options = new string[LocalizationSettings.AvailableLocales.Locales.Count];
            int[] values = new int[LocalizationSettings.AvailableLocales.Locales.Count];
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = LocalizationSettings.AvailableLocales.Locales[i].LocaleName;
                values[i] = i;
            }
            LISAUtility.SelectLocale(EditorGUILayout.IntPopup(LISAUtility.GetSelectedLocaleIndex(), options, values));
            if(GUILayout.Button("Open Tables", (EditorStyles.miniButton)))
            {
                LocalizationTablesWindow.ShowWindow();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        public static void DrawVariableListLayout(SerializedProperty indexProp, GUIContent label)
        {
            var variablesData = GameVariableList.GetList();
            GUIContent[] options = new GUIContent[variablesData.Length];
            int[] values = new int[variablesData.Length];
            for (int i = 0; i < variablesData.Length; i++)
            {
                options[i] = new GUIContent(variablesData[i].name);
                values[i] = i;
            }
            indexProp.intValue = EditorGUILayout.IntPopup(label, indexProp.intValue, options, values);
        }
        public static void DrawVariableList(Rect position, SerializedProperty indexProp, GUIContent label)
        {
            var variablesData = GameVariableList.GetList();
            GUIContent[] options = new GUIContent[variablesData.Length];
            int[] values = new int[variablesData.Length];
            for (int i = 0; i < variablesData.Length; i++)
            {
                options[i] = new GUIContent(variablesData[i].name);
                values[i] = i;
            }
            indexProp.intValue = EditorGUI.IntPopup(position, label, indexProp.intValue, options, values);
        }
    }
}
#endif
