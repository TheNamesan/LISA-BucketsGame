using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BucketsGame
{
    public struct TransformValues
    {
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;
        public TransformValues(Vector3 pos, Quaternion rot, Vector3 scl)
        {
            position = pos; rotation = rot; scale = scl;
        }
        public TransformValues(Transform key) : this(key.position, key.rotation, key.localScale) {}
        public void AssignToTransform(Transform target)
        {
            if (!target) return;
            target.position = position; target.rotation = rotation; target.localScale = scale;
        }
    }
    public class EntityResetCaller : MonoBehaviour
    {
        public bool includeChildren = true;
        public List<GameObject> ignores = new();
        public static UnityEvent onResetLevel = new();
        private Dictionary<Component, string> componentData = new();
        private Dictionary<Transform, TransformValues> transformData = new();

        private void Start()
        {
            SaveObjectValues();
            onResetLevel.AddListener(ResetObject);
        }
        private void OnDestroy()
        {
            onResetLevel.RemoveListener(ResetObject);
        }
        private void SaveObjectValues()
        {
            componentData.Clear();
            Component[] components = new Component[0];
            if (includeChildren)
            {
                components = GetComponentsInChildren<Component>();
            }
            else
            {
                components = GetComponents<Component>();
            }
            SaveComponents(components);
        }

        private void SaveComponents(Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == this) continue;
                if (ignores.Contains(components[i].gameObject)) continue;
                if (components[i] is not MonoBehaviour)
                {
                    if (components[i] is Transform trs)
                    {
                        transformData.Add(trs, new TransformValues(trs));
                    }
                    continue;
                }
                componentData.Add(components[i], JsonUtility.ToJson(components[i], true));
            }
        }

        private void ResetObject()
        {
            Component[] components = new Component[0];
            if (includeChildren)
            {
                components = GetComponentsInChildren<Component>();
            }
            else
            {
                components = GetComponents<Component>();
            }
            LoadComponents(components);
        }

        private void LoadComponents(Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] is not MonoBehaviour)
                {
                    if (components[i] is Transform trs)
                    {
                        if (transformData.TryGetValue(trs, out TransformValues values))
                        {
                            values.AssignToTransform(trs);
                        }
                    }
                    continue;
                }
                if (componentData.TryGetValue(components[i], out string value))
                {
                    JsonUtility.FromJsonOverwrite(value, components[i]);
                }
            }
        }
    }
}

