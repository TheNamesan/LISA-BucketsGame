using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    public class EventListWindow : EditorWindow
    {
        public static EventListWindow instance;
        public static InteractableEventList eventList;
        public static EventListEditors mainEventListEditors;
        public static string contentName = "";

        private static bool eventDeleted = false;
#pragma warning disable IDE0052
        private static bool referencesReset = false;

        public static InteractableEvent[] triggerEvents;
        public static InteractableObject interactableObject;

        private static Vector2 eventScrollPos;
        private static Vector2 downScrollPos;
        private static Vector2 upScrollPos = new Vector2();
        private static float scrollViewHeight;
        private static Rect boxLineArea;
        private static bool resizing = false;
        private static Texture2D lineTexture;

        private static readonly Vector2 windowMinSize = new Vector2(100f, 200f);
        private const float lineHeight = 1f;
        private const float linePositionLimit = 54f;
        private const float triggerButtonsMinHeight = 16f;
        private const float upperPanelMinHeight = 54f;
        private const float lowerPanelMinHeight = 54f;

        private static int triggerIndex = 0;
        private static int eventPanelIndex = -1;
        private static EventCommandEditor eventPanel;
        private static InteractableEventList panelEventListTarget;
        private static EventListEditors panelEventListEditorsTarget;
        private static int replaceTarget = -1;
        private static string addPanelTitle = "";

        private static ReorderableList list;
        private static EventListEditors listEventListEditors;
        private static string listSelectionPanelTitle;
        private static EventListEditors prevListEventListEditors;
        private static string prevListSelectionPanelTitle;

        private static bool assetCreated = false;

        public static void ShowWindow(int index)
        {
            instance = GetWindow<EventListWindow>("Event List");
            ResetReferences();
            triggerIndex = index;
            instance.AdjustSize();
        }
        [MenuItem("TUFF/Event List Window")]
        public static void ShowWindow()
        {
            ShowWindow(0);
        }
        public static void AddEvent(EventCommand eventCommand, InteractableEventList eventList, EventListEditors eventListEditors, int insertAt = -1)
        {
            if (replaceTarget >= 0)
            {
                Undo.RecordObject(interactableObject, "Replaced event in Event List");

                // Remove from prefab
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                {
                    if (prefabStage.IsPartOfPrefabContents(interactableObject.gameObject))
                    {
                        AssetDatabase.RemoveObjectFromAsset(eventList.content[replaceTarget]);
                    }
                }
                eventList.content.RemoveAt(replaceTarget);
                //Put destroy object immediate undo here

                // Add to asset if prefab
                if (prefabStage != null)
                {
                    if (prefabStage.IsPartOfPrefabContents(interactableObject.gameObject))
                    {
                        AssetDatabase.AddObjectToAsset(eventCommand, prefabStage.assetPath);
                    }
                }

                eventList.AddContentEvent(eventCommand, replaceTarget);
                var editor = Editor.CreateEditor(eventCommand) as EventCommandEditor;
                editor.OnEditorInstantiate();
            }
            else
            {
                Undo.RecordObject(interactableObject, "Added event to Event List");

                // Add to asset if prefab
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null) {
                    if (prefabStage.IsPartOfPrefabContents(interactableObject.gameObject))
                    {
                        AssetDatabase.AddObjectToAsset(eventCommand, prefabStage.assetPath);
                    }
                }
                
                eventList.AddContentEvent(eventCommand, insertAt);
                var editor = Editor.CreateEditor(eventCommand) as EventCommandEditor;
                editor.OnEditorInstantiate();
            }
            eventCommand.parent = eventList;
            list = null;
            mainEventListEditors = null;
            GetEditorsFromEventList(eventList, eventListEditors);
            MarkDirty();
        }
        public static void RemoveEvent(int idx, InteractableEventList eventList, EventListEditors eventListEditors)
        {
            Undo.RecordObject(interactableObject, "RecordInteractiveObject");
            var eventScriptableObject = eventList.content[idx];
            Undo.RecordObject(eventScriptableObject, "Removed");
            // Remove from prefab
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                if (prefabStage.IsPartOfPrefabContents(interactableObject.gameObject))
                {
                    AssetDatabase.RemoveObjectFromAsset(eventList.content[idx]);
                }
            }
            eventList.content.RemoveAt(idx);
            Undo.DestroyObjectImmediate(eventScriptableObject);
            Undo.SetCurrentGroupName("Removed event from Event List");
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            
            GetEditorsFromEventList(eventList, eventListEditors);
            MarkDirty();
        }
        public static void RemoveAllEvents(InteractableEventList eventList)
        {
            for(int i = 0; i < eventList.content.Count; i++)
            {
                RemoveEvent(i, eventList, null);
            }
        }
        
        private static void MarkDirty()
        {
            EditorUtility.SetDirty(interactableObject);
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                if (prefabStage.IsPartOfPrefabContents(interactableObject.gameObject))
                    PrefabUtility.RecordPrefabInstancePropertyModifications(interactableObject);
            }
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += () => { GetEditorsFromEventList(eventList, mainEventListEditors); };
            EditorApplication.playModeStateChanged += ModeChange;
            AdjustSize();
        }
        private void ModeChange(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.EnteredEditMode)
                GetEditorsFromEventList(eventList, mainEventListEditors);
        }

        private void AdjustSize()
        {
            scrollViewHeight = (position.height * 0.5f);
            boxLineArea = new Rect(0, scrollViewHeight, position.width, lineHeight);
            lineTexture = EditorGUIUtility.whiteTexture;
            minSize = windowMinSize;
        }
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= () => { GetEditorsFromEventList(eventList, mainEventListEditors); };
            EditorApplication.playModeStateChanged -= ModeChange;
            ResetReferences();
        }
        private void OnGUI()
        {
            /*eventDeleted = false;
            referencesReset = false;
            GetReferences();
            if (eventList == null)
            {
                GUILayout.Label("Event List is missing.", EditorStyles.boldLabel);
                return;
            }
            PortToActions(eventList);
            CheckForContentParent(eventList, interactableObject);
            ShowTriggerButtons();
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Mathf.Clamp(position.width * 0.4f, 140f, Mathf.Infinity);
            UpperPanelGUI(position);
            
            UpdateScrollView(position);

            LowerPanelGUI(position, panelEventListTarget, panelEventListEditorsTarget);
            EditorGUIUtility.labelWidth = labelWidth;*/
        }
        private void OnInspectorUpdate()
        {
            Repaint();
        }
        private static void ShowTriggerButtons()
        {
            eventScrollPos = EditorGUILayout.BeginScrollView(eventScrollPos, GUILayout.Height(triggerButtonsMinHeight + linePositionLimit));
            EditorGUILayout.BeginVertical();
            ShowTriggersLabel();
            EditorGUILayout.BeginHorizontal("box");
            for (int i = 0; i < triggerEvents.Length; i++)
            {
                var idx = new GUIContent($"{i}");

                if (i == triggerIndex) GUI.backgroundColor = Color.gray;
                else GUI.backgroundColor = Color.white;
                if (GUILayout.Button(idx, EditorStyles.miniButton, GUILayout.Width(50f)))
                {
                    ResetReferences();
                    triggerIndex = i;
                    GetReferences();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        private static void UpperPanelGUI(Rect position)
        {
            float scrollHeight = scrollViewHeight - linePositionLimit - triggerButtonsMinHeight;
            upScrollPos = EditorGUILayout.BeginScrollView(upScrollPos, GUILayout.Height(scrollHeight));
            GUILayout.BeginVertical();
            ShowTitle($"{contentName} Event List Content");

            listEventListEditors = null;
            listSelectionPanelTitle = null;
            prevListEventListEditors = null;
            prevListSelectionPanelTitle = null;
            GUILayout.Label($"Event Editor Count: {mainEventListEditors.editors.Count}");
            DisplayEventListContent(position, eventList, mainEventListEditors, contentName);

            if (list == null)
            {
                listEventListEditors = null;
                listSelectionPanelTitle = null;
                prevListEventListEditors = null;
                prevListSelectionPanelTitle = null;
                mainEventListEditors = null;
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public static void DisplayEventListContent(Rect position, InteractableEventList eventList, EventListEditors eventListEditors, string selectionPanelTitle)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label($"Event Count: {eventList.content.Count}");

            prevListEventListEditors = listEventListEditors;
            prevListSelectionPanelTitle = listSelectionPanelTitle;
            listEventListEditors = eventListEditors;
            listSelectionPanelTitle = selectionPanelTitle;

            Undo.RecordObject(interactableObject, "Replaced event in Event List");
            if (list == null) { GetList(); }
            EditorGUI.BeginChangeCheck();
            list.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                list = null;
                MarkDirty();
            }

            listEventListEditors = prevListEventListEditors;
            listSelectionPanelTitle = prevListSelectionPanelTitle;

            //for (int i = 0; i < eventList.content.Count; i++)
            //{
            //    if (eventList.content[i] == null) continue;
            //    EditorGUILayout.BeginVertical("box");
            //    bool deleted = false;
            //    CommandDefaultButtons(eventList, eventListEditors, i, selectionPanelTitle, out deleted);
            //    if (deleted)
            //    {
            //        i--;
            //        EditorGUILayout.EndVertical();
            //        continue;
            //    }
            //    GUILayout.BeginHorizontal();
            //    float space = 18f;
            //    GUILayout.Space(space);
            //    var style = EditorStyles.boldLabel;
            //    var content = new GUIContent($"{eventList.content[i].GetType()}: ");
            //    var size = style.CalcSize(content).x + 12f + space;
            //    GUILayout.Label(content, style, GUILayout.ExpandWidth(false));
            //    position.width -= size;
            //    if (eventListEditors.editors[i] != null) eventListEditors.editors[i].SummaryGUI(position);
            //    position.width += size;
            //    GUILayout.EndHorizontal();
            //    EditorGUILayout.EndVertical();
            //}
            DisplayButtons(position, eventList, eventListEditors, selectionPanelTitle);
            EditorGUILayout.EndVertical();
        }
        private static void GetList()
        {
            list = new ReorderableList(eventList.content, typeof(EventCommand), true, false, false, false);
            list.drawElementCallback = DrawListItems;
            list.elementHeightCallback = GetElementHeight;
        }
        private static float GetElementHeight(int index)
        {
            //var element = list.serializedProperty.GetArrayElementAtIndex(index);
            //var elementHeight = EditorGUI.GetPropertyHeight(element);
            if (eventDeleted) return 0f;
            if (listEventListEditors.editors[index] == null) return 0f;
            return 20f + listEventListEditors.editors[index].GetSummaryHeight();
        }
        private static void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            int i = index;
            if (eventDeleted) return;
            if (eventList.content[i] == null) return;
            var prevGUIColor = GUI.color;
            GUI.color = eventList.content[i].eventColor;
            rect.height = 20f;
            bool deleted = false;
            CommandDefaultButtons(ref rect, eventList, listEventListEditors, i, listSelectionPanelTitle, ref deleted);
            rect.y += 20f;
            if (deleted)
            {
                eventDeleted = true;
                GUI.color = prevGUIColor;
                return;
            }
            //float space = 18f;
            //var style = EditorStyles.boldLabel;
            //var content = new GUIContent($"{eventList.content[i].GetType()}: ");
            //var size = style.CalcSize(content).x + 12f + space;
            //EditorGUI.LabelField(rect, content, style);
            //rect.width -= size;
            //rect.x += size;
            if (listEventListEditors.editors[i] != null) listEventListEditors.editors[i].SummaryGUI(rect);
            GUI.color = prevGUIColor;
            //rect.x -= size;
            //rect.width += size;
        }
        private static void LowerPanelGUI(Rect position, InteractableEventList eventList, EventListEditors eventListEditors)
        {
            downScrollPos = GUILayout.BeginScrollView(downScrollPos, GUILayout.Height(position.height - scrollViewHeight));
            if (eventDeleted) DisplaySelectionPanel(eventList, mainEventListEditors, contentName);
            EditorGUILayout.BeginVertical();
            if (eventPanel != null)
            {
                ShowTitle($"Trigger #{triggerIndex}, {eventPanelIndex}: {eventPanel.GetEventName()}");
                eventPanel.OnInspectorGUI();
            }
            else
            { 
                EventCommandSelectionWindow.ShowPanelContent(eventList, eventListEditors, addPanelTitle); 
            }
            EditorGUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        private static void UpdateScrollView(Rect position)
        {
            Rect mouseValidArea = new Rect(boxLineArea.x, boxLineArea.y - 2, boxLineArea.width, boxLineArea.height + 4);
            EditorGUIUtility.AddCursorRect(mouseValidArea, MouseCursor.ResizeVertical);
            if (mouseValidArea.Contains(Event.current.mousePosition))
            {
                GUI.DrawTexture(boxLineArea, lineTexture, ScaleMode.StretchToFill, true, 0, Color.white, 0, 0);
                if (Event.current.type == EventType.MouseDown) resizing = true;
            }
            else GUI.DrawTexture(boxLineArea, lineTexture, ScaleMode.StretchToFill, true, 0, Color.grey, 0, 0);
            if (resizing)
            {
                scrollViewHeight = Event.current.mousePosition.y;
            }
            scrollViewHeight = Mathf.Clamp(scrollViewHeight, linePositionLimit + triggerButtonsMinHeight + upperPanelMinHeight, position.height - linePositionLimit - lowerPanelMinHeight);
            if (Event.current.type == EventType.MouseUp)
            {
                resizing = false;
            }
            boxLineArea.Set(boxLineArea.x, scrollViewHeight, position.width, boxLineArea.height);
        }

        private static void GetReferences()
        {
            if (Selection.activeTransform == null)
            {
                ResetReferences();
                return;
            }
            if (interactableObject != Selection.activeTransform.GetComponent<InteractableObject>())
            {
                ResetReferences();
            }
            interactableObject = Selection.activeTransform.GetComponent<InteractableObject>();
            if (interactableObject == null)
            {
                ResetReferences();
                return;
            }
            triggerEvents = interactableObject.triggerEvents;
            if (triggerEvents.Length == 0)
            {
                ResetReferences();
                return;
            }
            contentName = $"{Selection.activeTransform.gameObject.name} Trigger #{triggerIndex}";
            eventList = triggerEvents[triggerIndex].eventList;
            if (mainEventListEditors == null)
            {
                mainEventListEditors = new EventListEditors();
                GetEditorsFromEventList(eventList, mainEventListEditors);
            }
            if (panelEventListTarget == null)
            {
                DisplaySelectionPanel(eventList, mainEventListEditors, contentName);
            }
        }

        public static void ResetReferences()
        {
            eventList = null;
            mainEventListEditors = null;
            eventPanelIndex = -1;
            eventPanel = null;
            panelEventListTarget = null;
            panelEventListEditorsTarget = null;
            replaceTarget = -1;
            SetSelectionPanelTitle("");
            triggerIndex = 0;

            list = null;
            listEventListEditors = null;
            listSelectionPanelTitle = null;
            prevListEventListEditors = null;
            prevListSelectionPanelTitle = null;

            upScrollPos = new Vector2();

            assetCreated = false;

            referencesReset = true;
        }   
        public static void SetSelectionPanelTitle(string targetTitle, int replaceIdx = -1)
        {
            addPanelTitle = targetTitle;
            if (replaceIdx >= 0) addPanelTitle = $"{targetTitle} Replace Event {replaceIdx}";
        }
        private static void GetEditorsFromEventList(InteractableEventList eventList, EventListEditors eventListEditors)
        {
            if (eventListEditors != null && eventList != null) eventListEditors.UpdateEditors(eventList);
        }

        private static void PortToActions(InteractableEventList eventList)
        {
            if (eventList != null)
            {
                var actionListContent = interactableObject.triggerEvents[triggerIndex].actionList.content;
                if (actionListContent.Count <= 0 && eventList.content.Count > 0)
                {
                    for (int i = 0; i < eventList.content.Count; i++)
                    {
                        var obj = LISAUtility.Port(eventList.content[i], LISAUtility.GetPortType(eventList.content[i].GetType()));
                        actionListContent.Add(obj as EventAction);
                        //Debug.Log(obj);
                    }
                    Debug.Log("Ported");
                }
                MarkDirty();
            }
        }

        private static void ShowTitle(string title)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(title, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private static void ShowTriggersLabel()
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{Selection.activeTransform.gameObject.name} Events", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void CommandDefaultButtons(InteractableEventList eventList, EventListEditors eventListEditors, int i, string selectionPanelTitle, out bool deleted)
        {
            EditorGUILayout.BeginHorizontal();
            GUIContent btn = new GUIContent($"{i}: {eventList.content[i].eventName}", "Modify event command content."); //Modify Button
            if (GUILayout.Button(btn))
            {
                OpenWindowOfElement(eventList, i);
            }
            GUIContent up = new GUIContent("Move Up", "Add an event command."); //Move Up Button
            if (GUILayout.Button(up, EditorStyles.miniButtonLeft, GUILayout.Width(75f)))
            {
                if (i != 0)
                {
                    Undo.RecordObject(interactableObject, "Moved Event List element up");
                    LISAUtility.ListItemSwap(eventList.content, i, i - 1);
                    GetEditorsFromEventList(eventList, eventListEditors);
                    if (eventPanelIndex >= 0) OpenWindowOfElement(eventList, i - 1);
                    MarkDirty();
                } 
            }
            GUIContent down = new GUIContent("Move Down", "Add an event command."); //Move Down Button
            if (GUILayout.Button(down, EditorStyles.miniButtonMid, GUILayout.Width(90f)))
            {
                if (i != eventList.content.Count - 1)
                {
                    Undo.RecordObject(interactableObject, "Moved Event List element down");
                    LISAUtility.ListItemSwap(eventList.content, i, i + 1);
                    GetEditorsFromEventList(eventList, eventListEditors);
                    if (eventPanelIndex >= 0) OpenWindowOfElement(eventList, i + 1);
                    MarkDirty();
                } 
            }
            GUIContent change = new GUIContent("Change Command", "Add an event command."); //Change Button
            if(GUILayout.Button(change, EditorStyles.miniButtonMid, GUILayout.Width(120f)))
            {
                DisplaySelectionPanel(eventList, eventListEditors, selectionPanelTitle, i);
            }
            GUIContent remove = new GUIContent("Remove", "Add an event command."); //Remove Button
            if (GUILayout.Button(remove, EditorStyles.miniButtonRight, GUILayout.Width(75f)))
            {
                RemoveEvent(i, eventList, eventListEditors);
                deleted = true;
            }
            else deleted = false;
            EditorGUILayout.EndHorizontal();
        }
        private static void CommandDefaultButtons(ref Rect rect, InteractableEventList eventList, EventListEditors eventListEditors, int i, string selectionPanelTitle, ref bool deleted)
        {
            float orgX = rect.x;
            float orgWidth = rect.width;
            float orgHeight = rect.height;
            float width = orgWidth * 0.6f;
            rect.height = 20f;
            rect.width = width;

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.alignment = TextAnchor.MiddleLeft;
            btnStyle.fontStyle = FontStyle.Bold;
            GUIContent btn = new GUIContent($"{i}: {eventList.content[i].eventName} ({ eventList.content[i].GetType() }) ", "Modify event command content."); //Modify Button
            if (GUI.Button(rect, btn, btnStyle))
            {
                OpenWindowOfElement(eventList, i);
            }
            int buttons = 3;
            float buttonWidth = orgWidth * (0.4f / buttons);
            rect.x += width;
            width = buttonWidth;
            rect.width = width;
            GUIContent change = new GUIContent("Swap Command", "Replace this event command."); //Change Button
            if (GUI.Button(rect, change))
            {
                DisplaySelectionPanel(eventList, eventListEditors, selectionPanelTitle, i);
            }
            rect.x += width;
            width = buttonWidth;
            rect.width = width;
            GUIContent duplicate = new GUIContent("Duplicate", "Duplicate this event command."); //Duplicate Button
            if (GUI.Button(rect, duplicate))
            {
                replaceTarget = -1;
                AddEvent(Instantiate(eventList.content[i]), eventList, eventListEditors, i + 1);
                DisplaySelectionPanel(eventList, mainEventListEditors, contentName);
            }
            rect.x += width;
            width = buttonWidth;
            rect.width = width;
            GUIContent remove = new GUIContent("Remove", "Remove this event command."); //Remove Button
            if (GUI.Button(rect, remove))
            {
                RemoveEvent(i, eventList, eventListEditors);
                deleted = true;
            }
            //else deleted = false;
            rect.width = orgWidth;
            rect.height = orgHeight;
            rect.x = orgX;
        }

        private static void DisplayButtons(Rect position, InteractableEventList eventList, EventListEditors eventListEditors, string title)
        {
            GUILayout.Space(18);
            GUILayout.BeginVertical();
            GUIContent add = new GUIContent("+", "Add an event command.");
            if (GUILayout.Button(add, GUILayout.Width(position.width - 6f)))
            {
                DisplaySelectionPanel(eventList, eventListEditors, title);
            }
            GUIContent remove = new GUIContent("-", "Remove an event command.");
            if (GUILayout.Button(remove, GUILayout.Width(position.width - 6f)))
            {
                if (eventList.content.Count > 0)
                {
                    RemoveEvent(eventList.content.Count - 1, eventList, eventListEditors);
                    DisplaySelectionPanel(eventList, eventListEditors, title);
                }
            }
            GUILayout.EndVertical();
        }

        private static void DisplaySelectionPanel(InteractableEventList eventList, EventListEditors eventListEditors, string targetTitle = "", int replaceIdx = -1)
        {
            eventPanelIndex = -1;
            eventPanel = null;
            panelEventListTarget = eventList;
            panelEventListEditorsTarget = eventListEditors;
            replaceTarget = replaceIdx;
            SetSelectionPanelTitle(targetTitle, replaceIdx);
        }

        private static void OpenWindowOfElement(InteractableEventList eventList, int i)
        {
            eventPanelIndex = i;
            eventPanel = Editor.CreateEditor(eventList.content[i]) as EventCommandEditor;
        }
        private static void CheckForContentParent(InteractableEventList eventList, InteractableObject interactableObject)
        {
            // Tmp code
            //var events = eventList.content;
            //if (!assetCreated) 
            //{
            //    CommonEventEditor.CreateCommonEvent(interactableObject.gameObject.scene, $"{interactableObject.name}Trigger{triggerIndex}", events);
            //    assetCreated = true;
            //}
                
            //Debug.Log("Hi :3");

            //for(int i = 0; i < eventList.content.Count; i++)
            //{
            //    if (eventList.content[i].parent == null)
            //    {
            //        eventList.content[i].parent = eventList;
            //        Debug.Log($"Assigned missing parent at index {i}");
            //        MarkDirty();
            //    }
            //    else if(eventList.content[i].parent != eventList)
            //    {
            //        var eventCopy = Instantiate(eventList.content[i]);
            //        eventCopy.parent = eventList;
            //        eventList.content[i] = eventCopy;
            //        Debug.Log($"Reassigning parent at index {i}.");
            //        MarkDirty();
            //    }
            //}
        }
    }
}

