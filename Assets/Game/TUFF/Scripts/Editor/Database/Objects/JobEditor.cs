using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(Job))]
    public class JobEditor : Editor
    {
        int levelValue = 1;
        int lastLevelValue = -1;
        bool queueLabelsUpdate = false;
        string expLabel = "";
        string HPLabel = "";
        string SPLabel = "";
        string TPLabel = "";
        string ATKLabel = "";
        string DEFLabel = "";
        string SATKLabel = "";
        string SDEFLabel = "";
        string AGILabel = "";
        string LUKLabel = "";
        private Job job
        {
            get { return (target as Job); }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

            var nameKey = serializedObject.FindProperty("nameKey");
            EditorGUILayout.PropertyField(nameKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Name", nameKey.stringValue);
            var descriptionKey = serializedObject.FindProperty("descriptionKey");
            EditorGUILayout.PropertyField(descriptionKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Description", descriptionKey.stringValue, true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("usesSP"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("usesTP"));
            var resetTPOnBattleStart = serializedObject.FindProperty("resetTPOnBattleStart");
            EditorGUILayout.PropertyField(resetTPOnBattleStart);
            if (resetTPOnBattleStart.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startTPMin"), new GUIContent("Min%"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startTPMax"), new GUIContent("Max%"));
                EditorGUILayout.EndHorizontal();
            }
            levelValue = EditorGUILayout.IntSlider("Debug Level Value", levelValue, 1, 100);
            if(queueLabelsUpdate)
            {
                UpdateLabels(job);
                queueLabelsUpdate = false;
            }
            if(lastLevelValue != levelValue)
            {
                UpdateLabels(job);
                lastLevelValue = levelValue;
            }
            EditorGUILayout.LabelField(new GUIContent("Stat Curves", "Curve to evaluate stats as the Unit levels up. Minimum level is 1, Maximum is 100."), EditorStyles.boldLabel);
            DrawStat(job, "expCurve", "startEXP", "endEXP", expLabel);
            DrawStat(job, "maxHP", "startMaxHP", "endMaxHP", HPLabel);
            DrawStat(job, "maxSP", "startMaxSP", "endMaxSP", SPLabel);
            DrawStat(job, "maxTP", "startMaxTP", "endMaxTP", TPLabel);
            DrawStat(job, "ATK", "startATK", "endATK", ATKLabel);
            DrawStat(job, "DEF", "startDEF", "endDEF", DEFLabel);
            DrawStat(job, "SATK", "startSATK", "endSATK", SATKLabel);
            DrawStat(job, "SDEF", "startSDEF", "endSDEF", SDEFLabel);
            DrawStat(job, "AGI", "startAGI", "endAGI", AGILabel);
            DrawStat(job, "LUK", "startLUK", "endLUK", LUKLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hitRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("evasionRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("critRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("critEvasionRate"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("features"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("commands"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("skills"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("menuPortrait"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("faceGraphics"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("zoomedFaceGraphics"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));

            serializedObject.ApplyModifiedProperties();
        }
        private string PrintEvaluate(Job job, string label, LevelToStatType stat, bool exp = false)
        {
            return $"{label}{Evaluate(job, levelValue, stat)} {(exp ? "for" : "at")} Level {levelValue}";
        }
        private int Evaluate(Job job, int level, LevelToStatType stat)
        {
            return job.LevelToStat(level, stat);
        }

        private void UpdateLabels(Job job)
        {
            expLabel = PrintEvaluate(job, "EXP Needed: ", LevelToStatType.EXP, true);
            HPLabel = PrintEvaluate(job, "MaxHP: ", LevelToStatType.MaxHP);
            SPLabel = PrintEvaluate(job, "MaxSP: ", LevelToStatType.MaxSP);
            TPLabel = PrintEvaluate(job, "MaxTP: ", LevelToStatType.MaxTP);
            ATKLabel = PrintEvaluate(job, "ATK: ", LevelToStatType.ATK);
            DEFLabel = PrintEvaluate(job, "DEF: ", LevelToStatType.DEF);
            SATKLabel = PrintEvaluate(job, "SATK: ", LevelToStatType.SATK);
            SDEFLabel = PrintEvaluate(job, "SDEF: ", LevelToStatType.SDEF);
            AGILabel = PrintEvaluate(job, "AGI: ", LevelToStatType.AGI);
            LUKLabel = PrintEvaluate(job, "LUK: ", LevelToStatType.LUK);
        }
        private void DrawStat(Job job, string curveName, string startName, string endName, string evaluateLabel)
        {
            var labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(curveName));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(startName));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(endName));
            if (EditorGUI.EndChangeCheck()) queueLabelsUpdate = true;
            EditorGUILayout.LabelField(evaluateLabel, labelStyle);
            EditorGUILayout.EndVertical();
        }
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return LISAEditorUtility.SpriteRenderStaticPreview(job.faceGraphics.defaultFaceGraphic, Color.white, width, height);
        }
    }
}
