using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UrbanTerritoriality.Utilities;

namespace UrbanTerritoriality.EditorTools
{
    /** A static class with some useful methods
     * that can be used when creating custom graphical
     * user interfaces for the Unity editor.
     */
    public static class EditorGUIUtil
    {
        /**
         * Must be called inside the OnInspectorGUI method
         * of an Editor class.
         * Creates an asset path selector panel.
         * @param style The style of the panel.
         * @param path The asset path.
         * @param extension The mandatory extension for files that the asset path points to.
         * @return Returns the new asset path if it was changed by the user.
         */
        public static string AssetPathSelector(GUIStyle style, string path, string extension)
        {
            GUILayout.BeginVertical(style);
            {
                GUIContent buttonContent = new GUIContent("Set Path",
                    "Click here to choose a path.");
                bool clicked = GUILayout.Button(buttonContent);
                string tempPath = EditorGUILayout.TextField(new GUIContent("Asset Path",
                    "Path for file in project."), path);
                if (FileUtil.IsAssetFilePathValid(tempPath, extension))
                {
                    path = tempPath;
                }

                if (clicked)
                {
                    tempPath = FileUtil.GetFilePathInProject("Choose File Path",
                        StringUtil.ExtractFolderNameFromPath(path),
                        StringUtil.ExtractFileNameFromPath(path, true), extension);
                    if (EditorTools.FileUtil.IsAssetFilePathValid(tempPath, extension))
                    {
                        path = tempPath;
                    }
                }
            }
            GUILayout.EndVertical();
            return path;
        }

        /**
         * Must be called inside the OnInspectorGUI method
         * of an Editor child class.
         * Adds a float box for the user to put in time in seconds
         * when a file will be saved to disk.
         * @param showTime Weather the variable currentTime will be shown
         * in a label.
         * @param saveTime The time when a file will be saved. In seconds
         * after a scene starts.
         * @param currentTime Time in seconds after the scene started
         * playing in play mode.
         * @return Returns the save time in seconds.
         */
        public static float SaveTimeGUI(bool showTime, float saveTime, float currentTime)
        {
            if (showTime)
            {
                GUILayout.Label("Current Time: " + currentTime);
            }
            return EditorGUILayout.FloatField(new GUIContent("Save Time in Seconds",
                "The time in play mode when the file will be saved to disk."), saveTime);
        }
    }
}

