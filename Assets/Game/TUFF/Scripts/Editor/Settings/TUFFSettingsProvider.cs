using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UIElements;

namespace TUFF.TUFFEditor
{
    public class TUFFSettingsProvider : SettingsProvider
    {
        private static Object settings;

        public static readonly string PROJECTSETTINGS_FILE = "TUFFSettings"; 
        public static readonly string PROJECTSETTINGS_PATH = $"Assets/Resources/{ PROJECTSETTINGS_FILE }.asset";
        public TUFFSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public static Object IsSettingsAvailable()
        {
            if (settings == null)
            {
                settings = GetOrCreateSettings();
                return settings;
            }
            else return settings;
        }
        internal static TUFFSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<TUFFSettings>(PROJECTSETTINGS_PATH);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<TUFFSettings>();
                AssetDatabase.CreateAsset(settings, PROJECTSETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateTUFFReferencesProvider()
        {
            var settings = IsSettingsAvailable(); 
            if (settings != null)
            {
                var provider = AssetSettingsProvider.CreateProviderFromObject("Project/TUFF Settings/References", settings);//new TUFFSettingsEditor("Project/TUFF Settings/Events", SettingsScope.Project);
                provider.keywords = GetSearchKeywordsFromSerializedObject(new SerializedObject(settings));//GetSearchKeywordsFromGUIContentProperties<TUFFSettingsStyles>();
                return provider;
            }
            return null;
        }
        [SettingsProvider]
        public static SettingsProvider CreateTUFFSettingsProvider()
        {
            var settings = IsSettingsAvailable();
            if (settings != null)
            {
                var provider = new SettingsProvider("Project/TUFF Settings", SettingsScope.Project)
                {
                    label = "TUFF Settings",
                    guiHandler = (searchContext) =>
                    {
                        TUFFSettingsEditor.MainSettingsPanel();
                    },
                    keywords = new HashSet<string>(new[] { "Welcome" })
                };
                return provider;
            }
            return null;
        }
        [SettingsProvider]
        public static SettingsProvider CreateTUFFTermsProvider()
        {
            var settings = IsSettingsAvailable();
            if (settings != null)
            {
                var provider = AssetSettingsProvider.CreateProviderFromObject("Project/TUFF Settings/Terms", null);
                provider.keywords = GetSearchKeywordsFromSerializedObject(new SerializedObject(settings));
                provider.guiHandler = (searchContext) =>
                {
                    EditorGUILayout.LabelField("BRUH");
                };
                return provider;
            }
            return null;
        }
    }
}