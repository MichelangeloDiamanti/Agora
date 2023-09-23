using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UrbanTerritoriality.EditorTools
{
    /** 
     * This class contains a static method
     * for an editor commend.
     * The editor command is for saving meshes
     */
    public static class MeshSave
    {
        /** Save the selected mesh in the editor */
        [MenuItem("Window/Urban Territoriality/Save Selected Mesh")]
        public static void SaveSelectedMesh()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                EditorUtility.DisplayDialog("Save Mesh",
                    "In order to save a mesh a GameObject must be selected!",
                    "OK");
                return;
            }

            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf == null)
            {
                EditorUtility.DisplayDialog("Save Mesh",
                    "Please choose a GameObject with a MeshFilter on it!",
                    "OK");
                return;
            }

            Mesh mesh = mf.sharedMesh;
            if (mesh == null)
            {
                EditorUtility.DisplayDialog("Save Mesh",
                    "Please choose a GameObject with a MeshFilter that has a mesh assigned to it",
                    "OK");
                return;
            }

            while (true)
            {
                string assetFolder = "Assets";
                string path = EditorUtility.SaveFilePanel("Save mesh as asset", assetFolder, go.name, "asset");
                if (path.Length == 0)
                {
                    break;
                }

                int index = path.IndexOf(assetFolder);
                if (index == -1)
                {
                    EditorUtility.DisplayDialog("Save Mesh",
                        "Please choose a path inside the Unity project!",
                        "OK");
                }
                else
                {
                    path = path.Substring(index);
                    UnityEditor.MeshUtility.Optimize(mesh);
                    UnityEditor.AssetDatabase.CreateAsset(mesh, path);
                    UnityEditor.AssetDatabase.SaveAssets();
                    break;
                }
            }
        }
    }
}
