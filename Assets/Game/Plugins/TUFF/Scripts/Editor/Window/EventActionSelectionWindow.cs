using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    public static class EventActionSelectionWindow
    {
        
        public static List<EventAction> eventList;
        public static List<EventActionPD> eventListPDs;

        public static void ShowPanelContent(List<EventAction> evt, List<EventActionPD> evtPDs, string title)
        {
            eventList = evt;
            if (eventList == null)
            {
                GUILayout.Label("Event List is missing.", EditorStyles.boldLabel);
                return;
            }
            eventListPDs = evtPDs;
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
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = EventGUIColors.party;
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Party", EditorStyles.boldLabel);
            //if (GUILayout.Button(new GUIContent("Event Command", "Shows a character's dialogue in a textbox.")))
            //{
            //    AddEvent(new EventAction(), eventList, eventListPDs);
            //}
            if (GUILayout.Button(new GUIContent("Change Magazines", "Changes the amount of magazines the player's carrying.")))
            {
                AddEvent(new ChangeMagazinesAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Inventory", "Changes the amount of Items, Key Items, Weapons or Armors in the player's inventory.")))
            {
                AddEvent(new ChangeInventoryAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Party", "Changes the player's party.")))
            {
                AddEvent(new ChangePartyAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = oldColor;
        }

        private static void MessageOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Message", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Show Dialogue", "Shows a character's dialogue in a textbox.")))
            {
                AddEvent(new ShowDialogueAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Show Choices", "Displays a list of text choices.")))
            {
                AddEvent(new ShowChoicesAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
        }
        private static void GameProgressionOptions()
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = EventGUIColors.gameProgression;
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Game Progression", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Change Game Variable", "Changes the stored value for a Game Variable.")))
            {
                AddEvent(new ChangeGameVariableAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Switch", "Changes the switch of an Interactable Object in the scene.")))
            {
                AddEvent(new ChangeSwitchAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = oldColor;
        }
        private static void UnitOptions()
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = EventGUIColors.unit;
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Unit", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Change HP", "Changes a Unit's HP value.")))
            {
                AddEvent(new ChangeHPAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change SP", "Changes a Unit's SP value.")))
            {
                AddEvent(new ChangeSPAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change TP", "Changes a Unit's TP value.")))
            {
                AddEvent(new ChangeTPAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change State", "Applies or removes a state from a party member.")))
            {
                AddEvent(new ChangeStateAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Recover From KO", "Removes KO from a party member.")))
            {
                AddEvent(new RecoverFromKOAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Recover All", "Clears the user's states, removes KO and recovers all HP, SP and TP.")))
            {
                AddEvent(new RecoverAllAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change EXP", "Changes a Unit's EXP value.")))
            {
                AddEvent(new ChangeEXPAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Level", "Changes a Unit's Level value.")))
            {
                AddEvent(new ChangeLevelAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Skills", "Changes the Unit's learned skills.")))
            {
                AddEvent(new ChangeSkillsAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = oldColor;
        }
        private static void FlowControlOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Flow Control", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Conditional Branch", "Creates a portion to be run only when specific conditions are met.")))
            {
                AddEvent(new ConditionalBranchAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
        }
        private static void MovementOptions()
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = EventGUIColors.movement;
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Movement", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Transfer to Scene Point", "Transfers the player's position and Scene.")))
            {
                AddEvent(new TransferToScenePointAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Move Camera", "Moves a camera to a desired position without moving the Avatar.")))
            {
                AddEvent(new MoveCameraAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Switch Camera Follow", "Enables/Disables the camera from following the Avatar.")))
            {
                AddEvent(new SwitchCameraFollowAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Set Move Route", "Forces the player or an interactable to move.")))
            {
                AddEvent(new SetMoveRouteAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = oldColor;
        }
        private static void TimingOptions()
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = EventGUIColors.timing;
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Timing", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Wait Seconds", "Pauses the event for a certain amount of time in seconds. Affected by Time Scale.")))
            {
                AddEvent(new WaitSecondsAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = oldColor;
        }
        private static void CharacterOptions()
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = EventGUIColors.character;
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Character", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Change Transform", "Changes a scene object's transform properties. Properties are reset when reloading the Scene.")))
            {
                AddEvent(new ChangeTransformAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Sprite", "Changes a SpriteRenderer component's properties. Properties are reset when reloading the Scene.")))
            {
                AddEvent(new ChangeSpriteAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Animation Pack", "Swaps the animation pack for a Scene Character or a Follower Instance.")))
            {
                AddEvent(new ChangeAnimationPackAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Play Character Animation", "Play an animation for an idle Scene Character or a Follower Instance.")))
            {
                AddEvent(new PlayCharacterAnimationAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Character Sprite", "Change the sprite for an idle Scene Character or a Follower Instance.")))
            {
                AddEvent(new ChangeCharacterSpriteAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Restore Character State", "Stops a Scene Character or a Follower Instance from their animation or sprite change states.")))
            {
                AddEvent(new RestoreCharacterStateAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Toggle Character Light Source", "Enables or disables the light source GameObject for a Scene Character or a Follower Instance.")))
            {
                AddEvent(new ToggleCharacterLightSourceAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Character Run", "Enables or disables a character's ability to run in the overworld.")))
            {
                AddEvent(new ChangeCharacterRunAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Character Rope Jump", "Enables or disables a character's ability to jump from ropes.")))
            {
                AddEvent(new ChangeCharacterRopeJumpAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = oldColor;
        }
        private static void SoundOptions()
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = EventGUIColors.sound;
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Sound", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Play BGM", "Plays a BGM as Music.")))
            {
                AddEvent(new PlayBGMAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Play SFX", "Plays an Audio clip as a SFX.")))
            {
                AddEvent(new PlaySFXAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Stop BGM", "Stop the currently playing BGM.")))
            {
                AddEvent(new StopBGMAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Audio Source", "Changes the properties of an Audio Source component in the Scene.")))
            {
                AddEvent(new ChangeAudioSourceAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = oldColor;
        }
        private static void ScreenEffectsOptions()
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = EventGUIColors.screenEffects;
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Screen Effects", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Modify Global Volume", "Changes the global volume override properties.")))
            {
                AddEvent(new ModifyGlobalVolumeAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Flash Screen", "Flashes the screen with a designated color.")))
            {
                AddEvent(new FlashScreenAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Shake Camera", "Shakes a camera with the desired properties.")))
            {
                AddEvent(new ShakeCameraAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = oldColor;
        }
        private static void SceneOptions()
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = EventGUIColors.scene;
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Scene", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Start Battle", "Starts a selected battle.")))
            {
                AddEvent(new StartBattleAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Open Shop", "Opens the shop menu with the selected items.")))
            {
                AddEvent(new OpenShopAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Open Save Menu", "Calls the save menu to save the current data.")))
            {
                AddEvent(new OpenSaveMenuAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Game Over", "Forces a Game Over.")))
            {
                AddEvent(new GameOverAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = oldColor;
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
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = EventGUIColors.battle;
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Battle", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Change Enemy State", "?.")))
            {
                //AddEvent(new StartBattleAction(), eventList, eventListPDs);
            }
            if (GUILayout.Button(new GUIContent("Change Enemy Graphic", "Changes the selected enemy's rendered graphic.")))
            {
                AddEvent(new ChangeEnemyGraphicAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = oldColor;
        }
        private static void AdvancedOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Advanced", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Invoke Unity Event", "Invokes a Unity Event.")))
            {
                AddEvent(new InvokeUnityEventAction(), eventList, eventListPDs);
            }
            EditorGUILayout.EndVertical();
        }
        private static void OtherOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Other", EditorStyles.boldLabel);
            //customCommand = (EventCommand)EditorGUILayout.ObjectField("Prefab/Custom Command", customCommand, typeof(EventCommand), false);
            //var customGUIContent = new GUIContent("Add Prefab/Custom Command", "Add a command from a Scriptable Object of type EventCommand.");
            //if (GUILayout.Button(customGUIContent))
            //{
            //    if (customCommand == null)
            //    {
            //        Debug.LogWarning("No prefab has been set as a custom command.");
            //    }
            //    else
            //    {
            //        EventListWindow.AddEvent(Instantiate(customCommand), eventList, eventListEditors);
            //    }

            //}
            EditorGUILayout.EndVertical();
        }
        public static void AddEvent(EventAction eventAction, List<EventAction> eventList, List<EventActionPD> eventListPDs)
        {
            EventActionListWindow.AddEvent(eventAction, eventList, eventListPDs);
        }
        private static void ShowTitle(string title)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Add Event Commands to {title}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}

