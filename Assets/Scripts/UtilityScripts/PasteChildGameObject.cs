using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace VirPed.Utilities
{
    public class PasteChildGameObject : EditorWindow
    {
        public string searchString;

        public GameObject pasteObject;

        public bool onlyActive;

        public bool checkDuplicates;



        [MenuItem("VirPed/Paste Child Game Object")]
        public static void ShowWindow()
        {
            GetWindow(typeof(PasteChildGameObject));
        }

        public void OnGUI()
        {

            searchString = EditorGUILayout.TextField(new GUIContent("Search String", "Game Objects whose name exactly match this search string are going to be processed."), searchString);
            pasteObject = EditorGUILayout.ObjectField(new GUIContent("Paste Object", "The object to be pasted as a child of the selected game object."), pasteObject, typeof(GameObject), true) as GameObject;
            onlyActive = EditorGUILayout.Toggle(new GUIContent("Only Active", "Paste only if the GameObject is active in the hierarchy."), onlyActive);
            checkDuplicates = EditorGUILayout.Toggle(new GUIContent("Check Duplicates", "Check if the child object already exists."), checkDuplicates);


            if (GUILayout.Button("Paste Child Game Object"))
            {
                PasteAsChilds();
            }

        }

        [MenuItem("GameObject/Reassign Materials")]
        private void PasteAsChilds()
        {

            GameObject[] objs = Resources.FindObjectsOfTypeAll<GameObject>();

            int c = 0;
            foreach (GameObject go in objs)
            {
                if (onlyActive)
                {
                    if (!go.activeInHierarchy) continue;
                }
                if(checkDuplicates){
                    if(go.transform.Find(pasteObject.name) != null) continue;
                }
                if (go.name == searchString)
                {
                    GameObject newObject = Instantiate(pasteObject, go.transform);
                    newObject.name = pasteObject.name;
                    c++;
                }
            }


            Debug.LogFormat("Found {0} GO that match the search string", c);
        }
    }
}