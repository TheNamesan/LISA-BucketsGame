using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;

namespace TUFF
{
    [CreateAssetMenu(fileName = "InteractableList", menuName = "TUFF/Variables/Persistent Interactable List", order = 98)]
    public class PersistentInteractableList : ScriptableObject
    {
        public static readonly string INTERACTABLELIST_FILE = "InteractableList";
        public static readonly string INTERACTABLELIST_PATH = $"Assets/Resources/{ INTERACTABLELIST_FILE }.asset";

        [SerializeReference] private PersistentInteractableData[] persistentIDs = new PersistentInteractableData[0];

        public static void AddPersistentInteractable(InteractableObject interactable)
        {
#if UNITY_EDITOR
            if (Application.isPlaying) { Debug.LogWarning("Cannot add persistent interactables during runtime."); return; }

            var objectID = GlobalObjectId.GetGlobalObjectIdSlow(interactable);
            ulong localID = objectID.targetPrefabId == 0 ? objectID.targetObjectId : objectID.targetPrefabId;
            if (localID == 0) { Debug.LogWarning("Interactable Object has no local identifier. Did you remember to save the scene first?"); return; }

            if (VerifyPersistentIDExists(interactable, true) >= 0)
            {
                Debug.Log($"Interactable {interactable.gameObject.name} already has a persistent ID. Reassigning ID to component.");
                return;
            }
            Undo.RecordObject(instance, "Assign to List");
            Debug.Log(localID + ", " + interactable.gameObject.scene.path);
            int index = AssignAvailableID(new PersistentInteractableData(interactable, localID));
            Undo.RecordObject(interactable, "Assign to Interactable Object");
            interactable.persistentID = index;
            Undo.SetCurrentGroupName("Assigned Persistent ID to Interactable Object");
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
#endif
        }
        public static void RemovePersistentInteractable(InteractableObject interactable)
        {
#if UNITY_EDITOR
            if (Application.isPlaying) { Debug.LogWarning("Cannot remove persistent interactables during runtime."); return; }
            int index = VerifyPersistentIDExists(interactable);
            if (index < 0) { Debug.LogWarning("Could not find existing ID."); return; }
            Undo.RecordObject(interactable, "Unassign ID from Interactable Object");
            interactable.persistentID = -1;
            Undo.RecordObject(instance, "Remove from List");
            instance.persistentIDs[index] = null;
            Undo.SetCurrentGroupName($"Removed Persistent ID from {interactable.gameObject.name}");
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
#endif
        }
        public static int VerifyPersistentIDExists(InteractableObject interactable, bool reassignID = false)
        {
            if (interactable == null) return -1;
#if UNITY_EDITOR
            var objectID = GlobalObjectId.GetGlobalObjectIdSlow(interactable);
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(interactable.gameObject.scene.path);
            ulong localID = objectID.targetPrefabId == 0 ? objectID.targetObjectId : objectID.targetPrefabId;
            for (int i = 0; i < instance.persistentIDs.Length; i++)
            {
                var persistent = instance.persistentIDs[i];
                if (persistent == null) continue;
                if (persistent.scene == scene && persistent.localID == localID)
                {
                    if (persistent.name != interactable.gameObject.name)
                    {
                        persistent.name = interactable.gameObject.name;
                        Debug.Log($"Updated persistent interactable name for {interactable.gameObject.name}");
                    }
                    if (reassignID)
                    {
                        Undo.RecordObject(interactable, "Reassign ID to Interactable Object");
                        interactable.persistentID = i;
                    }
                    return i;
                }

            }
#endif
            return -1;
        }
        private static int AssignAvailableID(PersistentInteractableData persistent)
        {
            for (int i = 0; i < instance.persistentIDs.Length; i++)
            {
                if (instance.persistentIDs[i] == null)
                {
                    instance.persistentIDs[i] = persistent;
                    return i;
                }
            }
            System.Array.Resize(ref instance.persistentIDs, instance.persistentIDs.Length + 1);
            int index = instance.persistentIDs.Length - 1;
            instance.persistentIDs[index] = persistent;
            return instance.persistentIDs.Length - 1;
        }
    
        public static int GetPersistentIDLength()
        {
            return instance.persistentIDs.Length;
        }
        public static PersistentInteractableList instance
        {
            get
            {
                return GetOrCreateInstance();
            }
        }
        internal static PersistentInteractableList GetOrCreateInstance()
        {
            var settings = Resources.Load<PersistentInteractableList>(INTERACTABLELIST_FILE);
#if UNITY_EDITOR
            if (settings == null)
            {
                settings = CreateInstance<PersistentInteractableList>();
                AssetDatabase.CreateAsset(settings, INTERACTABLELIST_PATH);
                AssetDatabase.SaveAssets();
            }
#endif
            return settings;
        }
    }

    [System.Serializable]
    public class PersistentInteractableData
    {
#if UNITY_EDITOR
        public SceneAsset scene;
#endif
        public ulong localID = 0;
        public string name = "";

        public PersistentInteractableData(InteractableObject interactable, ulong localID)
        {
#if UNITY_EDITOR
            scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(interactable.gameObject.scene.path);
#endif
            this.localID = localID;
            name = interactable.gameObject.name;
        }
    }
}

