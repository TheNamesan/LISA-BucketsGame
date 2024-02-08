using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(CameraMove))]
    public class CameraMovePD : PropertyDrawer
    {
        int lines = 0;
        float additionalHeight = 0;
        bool showContent = true;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.singleLineHeight * lines) +
                (EditorGUIUtility.standardVerticalSpacing) + additionalHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var target = LISAEditorUtility.GetTargetObjectOfProperty(prop) as CameraMove;
            lines = 0;
            additionalHeight = 0;
            label = EditorGUI.BeginProperty(position, label, prop);
            position.height = 20f;
            showContent = EditorGUI.BeginFoldoutHeaderGroup(position, showContent, label);
            EditorGUI.EndFoldoutHeaderGroup();
            if(showContent)
            {
                EditorGUI.indentLevel++;
                Rect rect = EditorGUI.IndentedRect(position);
                AddLine(ref rect);
                EditorGUI.PropertyField(rect, prop.FindPropertyRelative("easeType"));
                AddLine(ref rect);
                EditorGUI.PropertyField(rect, prop.FindPropertyRelative("timeDuration"));
                AddLine(ref rect);
                var moveCameraType = prop.FindPropertyRelative("moveCameraType");
                EditorGUI.PropertyField(rect, moveCameraType);
                if((MoveCameraType)moveCameraType.enumValueIndex == MoveCameraType.MoveDelta)
                {
                    AddLine(ref rect);
                    EditorGUI.PropertyField(rect, prop.FindPropertyRelative("moveDelta"));
                    AddLine(ref rect);
                }
                else if ((MoveCameraType)moveCameraType.enumValueIndex == MoveCameraType.MoveToWorldPosition)
                {
                    AddLine(ref rect);
                    EditorGUI.PropertyField(rect, prop.FindPropertyRelative("targetWorldPosition"));
                    AddLine(ref rect);
                }
                else if ((MoveCameraType)moveCameraType.enumValueIndex == MoveCameraType.MoveToTransformPosition)
                {
                    AddLine(ref rect);
                    var targetTransform = prop.FindPropertyRelative("targetTransform");
                    EditorGUI.PropertyField(rect, targetTransform);
                    if (LISAUtility.IsPersistentInstance(target.targetTransform))
                    {
                        if(LISAUtility.GetPersistentOriginType(target.targetTransform) == PersistentType.AvatarController)
                        {
                            target.moveCameraType = MoveCameraType.ReturnToPlayer;
                        }
                        else
                        {
                            Debug.LogWarning("Target Transform is a Persistent Instance. Use a scene GameObject instead.");
                        }
                        target.targetTransform = null;
                    }
                }
                AddLine(ref rect);
                var onMovementEnd = prop.FindPropertyRelative("onMovementEnd");
                EditorGUI.PropertyField(rect, onMovementEnd);
                float endHeight = EditorGUI.GetPropertyHeight(onMovementEnd, true);
                additionalHeight += endHeight + EditorGUIUtility.standardVerticalSpacing;
                rect.y += endHeight;
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
            prop.serializedObject.ApplyModifiedProperties();
        }
        private void AddLine(ref Rect position, float spaceMult = 1)
        {
            lines++;
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * spaceMult);
        }
    }
}
