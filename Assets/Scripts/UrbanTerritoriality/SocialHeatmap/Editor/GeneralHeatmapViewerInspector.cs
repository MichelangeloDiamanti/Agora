using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UrbanTerritoriality.Maps
{
    /** A custom inspector for GeneralHeatmapViewer. */
    [CustomEditor(typeof(GeneralHeatmapViewer), true)]
    public class GeneralHeatmapViewerInspector : Editor
    {
        /** The GeneralHeatmapViewer object */
        protected GeneralHeatmapViewer viewer;

        /** Unity OnEnable method */
        void OnEnable()
        {
            viewer = (GeneralHeatmapViewer)target;
        }

        /** Adds a GUI for saving the image */
        protected virtual void AddSaveGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                margin = new RectOffset(5, 5, 5, 5),
                padding = new RectOffset(5, 5, 5, 5)
            };

            Undo.RecordObject(viewer, "General Heatmap Viewer Change");
            
            GUILayout.BeginVertical(style);
            {
                EditorGUILayout.HelpBox("" +
                    "Configure these paramaters in order to save the picture as a file on disk.",
                    MessageType.Info);
                viewer.saveTexture = GUILayout.Toggle(viewer.saveTexture, new GUIContent("Save Texture",
                    "Save the image as a png file on disk."));

                viewer.savePath =
                    EditorTools.EditorGUIUtil.AssetPathSelector(
                        style, viewer.savePath, "png");
                viewer.saveTime =
                    EditorTools.EditorGUIUtil.SaveTimeGUI(
                        viewer.Initialized, viewer.saveTime, viewer.map.CurrentTime);
                if (viewer.HasBeenSaved)
                {
                    EditorGUILayout.HelpBox("Image saved in file " + viewer.SavedPath, MessageType.Info);
                }
            }
            GUILayout.EndVertical();

        }

        /** Display the inspector GUI */
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            /* In order to add the save GUI uncomment this line */
            //AddSaveGUI();
        }
    }
}

