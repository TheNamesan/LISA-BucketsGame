using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace TUFF
{
    public enum PersistentType
    {
        None = 0,
        AvatarController = 1
    }
    /// <summary>
    /// Help me Lisa?
    /// </summary>
    public static class LISAUtility
    {
        public static TableReference dialogueTableReference = TUFFSettings.dialogueTable;
        public static TableReference termsTableReference = TUFFSettings.termsTable;
        public static int Sign(float number)
        {
            return number < 0 ? -1 : (number > 0 ? 1 : 0);
        }
        public static int Truncate(float number)
        {
            if (number < 0) return Mathf.CeilToInt(number);
            return Mathf.FloorToInt(number);
        }
        public static int Ceil(float number)
        {
            if (number < 0) return Mathf.FloorToInt(number);
            return Mathf.CeilToInt(number);
        }
        public static bool WithinValueRange(float min, float max, float value)
        {
            float a = Mathf.Min(min, max);
            float b = Mathf.Max(min, max);
            return (value >= a && value <= b);
        }
        public static float ValueToPercentage(float value)
        {
            return value * 100f;
        }
        public static float PercentageToValue(float value)
        {
            return value * 0.01f;
        }
        public static float PercentTodB(float percent)
        {
            return Mathf.Log10(percent < 0.0001f ? 0.0001f : percent) * 20f;
        }
        public static string IntToString(int value, string format = null)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }
        public static string FloatToString(float value, string format = null)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }
        public static void ListMoveItemToFront<T>(List<T> list, int index)
        {
            if (list == null) return;
            if (index < 0 || index >= list.Count) return;
            var tmp = list[index];
            list.RemoveAt(index);
            list.Insert(0, tmp);
        }
        public static void ListItemSwap<T>(List<T> list, int indexA, int indexB)
        {
            if (list == null) return;
            if (indexA < 0 || indexA >= list.Count) return;
            if (indexB < 0 || indexB >= list.Count) return;
            var tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
        public static object Copy(object source)
        {
            if (source == null) return null;
            string json = JsonUtility.ToJson(source, true);
            //Debug.Log(json);
            var destination = JsonUtility.FromJson(json, source.GetType());
            return destination;
        }
        /// <summary>
        /// Set pivot without changing the position of the element
        /// </summary>
        public static void SetPivot(RectTransform rectTransform, Vector2 pivot)
        {
            Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
            deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
            deltaPosition.Scale(rectTransform.localScale);          // apply scaling
            deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

            rectTransform.pivot = pivot;                            // change the pivot
            rectTransform.localPosition -= deltaPosition;           // reverse the position change
        }
        public static object Port(object source, System.Type type)
        {
            if (source == null) return null;
            string json = JsonUtility.ToJson(source, true);
            //Debug.Log(json);
            var destination = JsonUtility.FromJson(json, type);
            if (destination is EventAction)
            {
                var bruh = System.Activator.CreateInstance(type) as EventAction;
                var action = (EventAction)destination;
                action.eventColor = bruh.eventColor;
            }
            if (destination is PlaySFXAction)
            {
                var action = (PlaySFXAction)destination;
                var command = (PlaySFXEvent)source;
                action.sfxs.Add(Copy(command.sfx) as SFX);
                Debug.Log(action.sfxs.Count);
                Debug.Log(action.sfxs[0].audioClip);
            }
            return destination;
        }
        public static System.Type GetPortType(System.Type comType)
        {
            if (comType == typeof(ChangeAudioSourceEvent)) return typeof(ChangeAudioSourceAction);
            if (comType == typeof(ChangeInventoryEvent)) return typeof(ChangeInventoryAction);
            if (comType == typeof(ChangePartyEvent)) return typeof(ChangePartyAction);
            if (comType == typeof(ChangeSpriteEvent)) return typeof(ChangeSpriteAction);
            if (comType == typeof(ChangeSwitchEvent)) return typeof(ChangeSwitchAction);
            if (comType == typeof(ConditionalBranchEvent)) return typeof(ConditionalBranchAction);
            if (comType == typeof(EventCommand)) return typeof(EventAction);
            if (comType == typeof(GameOverEvent)) return typeof(GameOverAction);
            if (comType == typeof(InvokeUnityEventEvent)) return typeof(InvokeUnityEventAction);
            if (comType == typeof(ModifyGlobalVolumeEvent)) return typeof(ModifyGlobalVolumeAction);
            if (comType == typeof(MoveCameraEvent)) return typeof(MoveCameraAction);
            if (comType == typeof(PlayBGMEvent)) return typeof(PlayBGMAction);
            if (comType == typeof(PlaySFXEvent)) return typeof(PlaySFXAction);
            if (comType == typeof(ShakeCameraEvent)) return typeof(ShakeCameraAction);
            if (comType == typeof(ShowDialogueEvent)) return typeof(ShowDialogueAction);
            if (comType == typeof(StartBattleEvent)) return typeof(StartBattleAction);
            if (comType == typeof(StopBGMEvent)) return typeof(StopBGMAction);
            if (comType == typeof(SwitchCameraFollowEvent)) return typeof(SwitchCameraFollowAction);
            if (comType == typeof(TransferToScenePointEvent)) return typeof(TransferToScenePointAction);
            if (comType == typeof(WaitSecondsEvent)) return typeof(WaitSecondsAction);
            return typeof(EventAction);
        }
        public static bool IsPersistentInstance(Transform origin)
        {
            if (origin == null) return false;
            if (origin.GetComponent<FollowerInstance>() != null)
            {
                return true;
            }
            return false;
        }
        public static void SetRandomSeed(int seed)
        {
            Random.InitState(seed);
        }
        public static Canvas GetCanvasRoot(Transform element)
        {
            var parent = element.parent;
            var lastParent = element.parent;
            while (true)
            {
                if (parent.parent == null) break;
                lastParent = parent;
                parent = parent.parent;
            }
            var parentCanvas = lastParent.GetComponent<Canvas>();
            return parentCanvas;
        }

        public static Vector2 GetCanvasOverlayPosition(GraphicHandler graphicHandler)
        {
            if (graphicHandler.canvasRoot.renderMode == RenderMode.ScreenSpaceOverlay)
                return graphicHandler.rect.position;
            return GetCanvasCameraToOverlayPosition(graphicHandler.rect.position);
        }
        public static Vector2 GetCanvasCameraPosition(GraphicHandler graphicHandler)
        {
            if (graphicHandler.canvasRoot.renderMode == RenderMode.ScreenSpaceCamera)
                return graphicHandler.rect.position;
            return GetCanvasOverlayToCameraPosition(graphicHandler.rect.position);
        }
        public static Vector2 GetCanvasOverlayToCameraPosition(Vector2 position)
        {
            var cam = UIController.instance.cameraCanvas.worldCamera;
            var pos = cam.ScreenToWorldPoint(position);
            return pos;
        }
        public static Vector2 GetCanvasCameraToOverlayPosition(Vector2 position)
        {
            var cam = UIController.instance.cameraCanvas.worldCamera;
            var overlay = UIController.instance.overlayCanvas;
            var pos = cam.WorldToScreenPoint(position);
            pos = new Vector2(
                (pos.x / Screen.width) * overlay.pixelRect.width,
                (pos.y / Screen.height) * overlay.pixelRect.height);
            return pos;
        }
        public static AudioClip CutAudioClip(AudioClip inputAudio, int startSample, int endSample, string nameSuffix, AudioClip.PCMReaderCallback OnAudioRead = null, AudioClip.PCMSetPositionCallback OnAudioSetPosition = null)
        {
            if (OnAudioRead == null) OnAudioRead = (float[] data) => { };
            if (OnAudioSetPosition == null) OnAudioSetPosition = (int pos) => { };
            // Copy samples from input audio to an array. AudioClip uses interleaved format so the length in samples is multiplied by channel count
            float[] samplesOriginal = new float[inputAudio.samples * inputAudio.channels];
            inputAudio.GetData(samplesOriginal, 0);
            // Find first and last sample
            int audioStart = startSample * 2;
            int audioEnd = endSample * 2;
            // Copy trimmed audio data into another array
            float[] samplesTrimmed = new float[audioEnd - audioStart];
            System.Array.Copy(samplesOriginal, audioStart, samplesTrimmed, 0, samplesTrimmed.Length);
            // Create new AudioClip for trimmed audio data
            AudioClip trimmedAudio = AudioClip.Create(inputAudio.name + nameSuffix, samplesTrimmed.Length / inputAudio.channels, inputAudio.channels, inputAudio.frequency,
                (inputAudio.loadType != AudioClipLoadType.DecompressOnLoad), OnAudioRead, OnAudioSetPosition);
            trimmedAudio.SetData(samplesTrimmed, 0);
            return trimmedAudio;
        }
        public static PersistentType GetPersistentOriginType(Transform origin)
        {
            if (origin == null) return PersistentType.None;
            if (origin.GetComponent<OverworldCharacterController>() != null)
            {
                return PersistentType.AvatarController;
            }
            return PersistentType.None;
        }
        public static Transform GetPersistentOrigin(PersistentType origin)
        {
            switch (origin)
            {
                case PersistentType.AvatarController:
                    return FollowerInstance.player.controller.transform;

                default:
                    return null;
            }
        }
        public static object GetPersistentInstance(PersistentType origin)
        {
            switch (origin)
            {
                case PersistentType.AvatarController:
                    return FollowerInstance.player.controller;

                default:
                    return null;
            }
        }
        /// <summary>
        /// Assigns the locale at index.
        /// </summary>
        /// <param name="index">Locale's index</param>
        /// <returns>Returns true if locale was successfully assigned.</returns>
        public static bool SelectLocale(int index)
        {
            if (index >= LocalizationSettings.AvailableLocales.Locales.Count || index < 0) return false;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
            return true;
        }
        /// <summary>
        /// Checks if there's a locale assigned and tries to assign one if not;
        /// </summary>
        /// <returns>Returns true if current locale is valid.</returns>
        public static bool CheckLocaleIsNotNull()
        {
            if (GetSelectedLocaleIndex() < 0) return SelectLocale(0);
            return true;
        }
        public static int GetSelectedLocaleIndex()
        {
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                var locale = LocalizationSettings.AvailableLocales.Locales[i];
                if (LocalizationSettings.SelectedLocale == locale)
                    return i;
            }
            return -1;
        }
        public static string GetLocalizedDialogueText(string key)
        {
            if (!CheckLocaleIsNotNull()) return "";
            return LocalizationSettings.StringDatabase.GetLocalizedString(dialogueTableReference, key);
        }
        public static string GetLocalizedTermsText(string key)
        {
            if (!CheckLocaleIsNotNull()) return "";
            return LocalizationSettings.StringDatabase.GetLocalizedString(termsTableReference, key);
        }
        public static string GetLocalizedText(string tableCollectionName, string key)
        {
            if (!CheckLocaleIsNotNull()) return "";
            return LocalizationSettings.StringDatabase.GetLocalizedString(tableCollectionName, key);
        }
    }
    public class ResolutionEqualityComparer : IEqualityComparer<Resolution>
    {
        public bool Equals(Resolution r1, Resolution r2)
        {
            return r1.width.Equals(r2.width) && r1.height.Equals(r2.height);
        }

        public int GetHashCode(Resolution r)
        {
            // Custom hash function for Vector2
            int hashX = r.width.GetHashCode();
            int hashY = r.height.GetHashCode();
            return hashX ^ hashY;
        }
    }
}
