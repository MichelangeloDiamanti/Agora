using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace VirPed.Utilities
{
    public class ReassignMaterials : EditorWindow
    {
        public static Material mat1;
        public static Material mat2;
        public static Material mat3;
        public static Material mat4;
        public static Material mat5;
        public static Material mat6;

        private static int _goCount;
        private static int _componentsCount;
        private static int _missingCount;

        private static bool _bHaveRun;

        // [MenuItem("VirPed/Editor/Utility/ReassignMaterials")]
        [MenuItem("VirPed/ReassignMaterials")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ReassignMaterials));
        }

        public void OnGUI()
        {
            mat1 = (Material)EditorGUILayout.ObjectField("No Name:", mat1, typeof(Material), true);
            mat2 = (Material)EditorGUILayout.ObjectField("Building:", mat2, typeof(Material), true);
            mat3 = (Material)EditorGUILayout.ObjectField("Door:", mat3, typeof(Material), true);
            mat4 = (Material)EditorGUILayout.ObjectField("Window:", mat4, typeof(Material), true);
            mat5 = (Material)EditorGUILayout.ObjectField("Stair Steps:", mat5, typeof(Material), true);
            mat6 = (Material)EditorGUILayout.ObjectField("Stair Railing:", mat6, typeof(Material), true);


            if (GUILayout.Button("ReassignMaterials"))
            {
                Reassign();
            }

            if (!_bHaveRun) return;

            EditorGUILayout.TextField($"{_goCount} GameObjects Selected");
            if (_goCount > 0) EditorGUILayout.TextField($"{_componentsCount} Components");
            if (_goCount > 0) EditorGUILayout.TextField($"{_missingCount} Deleted");
        }

        [MenuItem("GameObject/Reassign Materials")]
        private static void Reassign()
        {

            string searchString = "Donkey_";
            GameObject[] objs = Resources.FindObjectsOfTypeAll<GameObject>();

            int c = 0;
            foreach (GameObject go in objs)
            {

                MeshFilter mf;
                bool found = go.TryGetComponent<MeshFilter>(out mf);

                if (found && mf.sharedMesh.name.Contains(searchString))
                {
                    c++;

                    GameObject instance;
                    if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
                        instance = (GameObject)PrefabUtility.InstantiatePrefab(go);
                    else
                        instance = go;

                    MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();

                    // change the materials for all renderer in the Game Object
                    foreach (MeshRenderer renderer in renderers)
                    {
                        Material[] sharedMaterials = renderer.sharedMaterials;
                        if (sharedMaterials.Length == 3)
                        {
                            for (int j = 0; j < sharedMaterials.Length; j++)
                            {
                                if (j == 0)
                                    sharedMaterials[j] = mat1;
                                else if (j == 1)
                                    sharedMaterials[j] = mat2;
                                else if (j == 2)
                                    sharedMaterials[j] = mat4;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < sharedMaterials.Length; j++)
                            {
                                if (j == 0)
                                    sharedMaterials[j] = mat1;
                                else if (j == 1)
                                    sharedMaterials[j] = mat2;
                                else if (j == 2)
                                    sharedMaterials[j] = mat3;
                                else if (j == 3)
                                    sharedMaterials[j] = mat4;
                                else if (j == 4)
                                    sharedMaterials[j] = mat5;
                                else if (j == 5)
                                    sharedMaterials[j] = mat6;
                            }
                        }
                        renderer.sharedMaterials = sharedMaterials;
                    }

                    // mr.sharedMaterials[i] = mat1 != null ? mat1 : mat;

                    if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
                    {
                        //Save the changes into the prefab
                        PrefabUtility.ReplacePrefab(instance, go, ReplacePrefabOptions.ConnectToPrefab);

                        //destroy the previously created instance
                        DestroyImmediate(instance);
                    }
                }
            }
            AssetDatabase.SaveAssets();
        }
    }
}