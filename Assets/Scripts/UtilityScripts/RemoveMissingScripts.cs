using UnityEditor;
using UnityEngine;
using System.Linq;

namespace FLGCoreEditor.Utilities
{
        public class RemoveMissingScripts : Editor
        {
            [MenuItem("GameObject/Remove Missing Scripts")]
            public static void Remove()
            {
                var objs = Resources.FindObjectsOfTypeAll<GameObject>();
                int count = objs.Sum(GameObjectUtility.RemoveMonoBehavioursWithMissingScript);
                Debug.Log($"Removed {count} missing scripts");
            }
        }
}