using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace TUFF.TUFFEditor
{
    public class EventCommandSelectionWindow : ScriptableObject
    {
        public static InteractableEventList eventList;
        public static EventListEditors eventListEditors;
        private static EventCommand customCommand;

        public static void ShowPanelContent(InteractableEventList evt, EventListEditors evtEditors, string title)
        {
            eventList = evt;
            if (eventList == null)
            {
                GUILayout.Label("Event List is missing.", EditorStyles.boldLabel);
                return;
            }
            eventListEditors = evtEditors;
            ShowTitle(title);
            EditorGUILayout.BeginHorizontal();
            MessageOptions();
            PartyOptions();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GameProgressionOptions();
            UnitOptions();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            FlowControlOptions();
            MovementOptions();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TimingOptions();
            CharacterOptions();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            SoundOptions();
            ScreenEffectsOptions();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            SceneOptions();
            MapOptions();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            SystemOptions();
            BattleOptions();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            AdvancedOptions();
            OtherOptions();
            EditorGUILayout.EndHorizontal();
        }
        private static void PartyOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Party", EditorStyles.boldLabel);
            /*
            if (GUILayout.Button(new GUIContent("Event Command", "Shows a character's dialogue in a textbox.")))
            {
                EventListWindow.AddEvent(CreateInstance<EventCommand>(), eventList);
            }*/
            if (GUILayout.Button(new GUIContent("Change Inventory", "Changes the amount of Items, Key Items, Weapons or Armors in the player's inventory.")))
            {
                EventListWindow.AddEvent(CreateInstance<ChangeInventoryEvent>(), eventList, eventListEditors);
            }
            if (GUILayout.Button(new GUIContent("Change Party", "Changes the player's party.")))
            {
                EventListWindow.AddEvent(CreateInstance<ChangePartyEvent>(), eventList, eventListEditors);
            }
            EditorGUILayout.EndVertical();
        }

        private static void MessageOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Message", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Show Dialogue", "Shows a character's dialogue in a textbox.")))
            {
                EventListWindow.AddEvent(CreateInstance<ShowDialogueEvent>(), eventList, eventListEditors);
            }
            EditorGUILayout.EndVertical();
        }
        private static void GameProgressionOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Game Progression", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Change Switch", "Changes the switch of an Interactable Object in the scene.")))
            {
                EventListWindow.AddEvent(CreateInstance<ChangeSwitchEvent>(), eventList, eventListEditors);
            }
            EditorGUILayout.EndVertical();
        }
        private static void UnitOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Unit", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
        }
        private static void FlowControlOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Flow Control", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Conditional Branch", "Creates a portion to be run only when specific conditions are met.")))
            {
                EventListWindow.AddEvent(CreateInstance<ConditionalBranchEvent>(), eventList, eventListEditors);
            }
            EditorGUILayout.EndVertical();
        }
        private static void MovementOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Movement", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Transfer to Scene Point", "Transfers the player's position and Scene.")))
            {
                EventListWindow.AddEvent(CreateInstance<TransferToScenePointEvent>(), eventList, eventListEditors);
            }
            if (GUILayout.Button(new GUIContent("Move Camera", "Moves a camera to a desired position without moving the Avatar.")))
            {
                EventListWindow.AddEvent(CreateInstance<MoveCameraEvent>(), eventList, eventListEditors);
            }
            if (GUILayout.Button(new GUIContent("Switch Camera Follow", "Enables/Disables the camera from following the Avatar.")))
            {
                EventListWindow.AddEvent(CreateInstance<SwitchCameraFollowEvent>(), eventList, eventListEditors);
            }
            EditorGUILayout.EndVertical();
        }
        private static void TimingOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Timing", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Wait Seconds", "Pauses the event for a certain amount of time in seconds. Affected by Time Scale.")))
            {
                EventListWindow.AddEvent(CreateInstance<WaitSecondsEvent>(), eventList, eventListEditors);
            }
            EditorGUILayout.EndVertical();
        }
        private static void CharacterOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Character", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Change Sprite", "Changes a SpriteRenderer component's properties. Original properties are restored when reloading the Scene.")))
            {
                EventListWindow.AddEvent(CreateInstance<ChangeSpriteEvent>(), eventList, eventListEditors);
            }
            EditorGUILayout.EndVertical();
        }
        private static void SoundOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Sound", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Play BGM", "Plays a BGM as Music.")))
            {
                EventListWindow.AddEvent(CreateInstance<PlayBGMEvent>(), eventList, eventListEditors);
            }
            if (GUILayout.Button(new GUIContent("Play SFX", "Plays an Audio clip as a SFX.")))
            {
                EventListWindow.AddEvent(CreateInstance<PlaySFXEvent>(), eventList, eventListEditors);
            }
            if (GUILayout.Button(new GUIContent("Stop BGM", "Stop the currently playing BGM.")))
            {
                EventListWindow.AddEvent(CreateInstance<StopBGMEvent>(), eventList, eventListEditors);
            }
            if (GUILayout.Button(new GUIContent("Change Audio Source", "Changes the properties of an Audio Source component in the Scene.")))
            {
                EventListWindow.AddEvent(CreateInstance<ChangeAudioSourceEvent>(), eventList, eventListEditors);
            }
            EditorGUILayout.EndVertical();
        }
        private static void ScreenEffectsOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Screen Effects", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Modify Global Volume", "Changes the global volume override properties.")))
            {
                EventListWindow.AddEvent(CreateInstance<ModifyGlobalVolumeEvent>(), eventList, eventListEditors);
            }
            if (GUILayout.Button(new GUIContent("Shake Camera", "Shakes a camera with the desired properties.")))
            {
                EventListWindow.AddEvent(CreateInstance<ShakeCameraEvent>(), eventList, eventListEditors);
            }
            
            EditorGUILayout.EndVertical();
        }
        private static void SceneOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Scene", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Start Battle", "Starts a battle.")))
            {
                EventListWindow.AddEvent(CreateInstance<StartBattleEvent>(), eventList, eventListEditors);
            }
            if (GUILayout.Button(new GUIContent("Game Over", "Forces a Game Over.")))
            {
                EventListWindow.AddEvent(CreateInstance<GameOverEvent>(), eventList, eventListEditors);
            }
            EditorGUILayout.EndVertical();
        }
        private static void MapOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Map", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
        }
        private static void SystemOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("System", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
        }
        private static void BattleOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Battle", EditorStyles.boldLabel);
            
            EditorGUILayout.EndVertical();
        }
        private static void AdvancedOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Advanced", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Invoke Unity Event", "Invokes a Unity Event.")))
            {
                EventListWindow.AddEvent(CreateInstance<InvokeUnityEventEvent>(), eventList, eventListEditors);
            }
            EditorGUILayout.EndVertical();
        }
        private static void OtherOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Other", EditorStyles.boldLabel);
            customCommand = (EventCommand)EditorGUILayout.ObjectField("Prefab/Custom Command", customCommand, typeof(EventCommand), false);
            var customGUIContent = new GUIContent("Add Prefab/Custom Command", "Add a command from a Scriptable Object of type EventCommand.");
            if (GUILayout.Button(customGUIContent))
            {
                if (customCommand == null)
                {
                    Debug.LogWarning("No prefab has been set as a custom command.");
                }
                else
                {
                    EventListWindow.AddEvent(Instantiate(customCommand), eventList, eventListEditors);
                }

            }
            EditorGUILayout.EndVertical();
        }
        private static void ShowTitle(string title)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Add Event Actions to {title}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
