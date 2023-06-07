using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UrbanTerritoriality.Utilities;

namespace UrbanTerritoriality.EditorTools
{
    /**
     * A static class with some methods regarding files
     * on disk.
     */
    public static class FileUtil
    {
        /** Convert a Unity asset path to a full path on disk.
         * @param assetPath The asset path to convert.
         * @return The full disk path.
         */
        public static string ConvertAssetPathToDiskPath(string assetPath)
        {
            int firstSlashIndex = assetPath.IndexOf("/");
            return Application.dataPath +
                assetPath.Substring(firstSlashIndex,
                assetPath.Length - firstSlashIndex);
        }

        /**
         * Check if an asset file path is valid.
         * The asset path must start with the string
         * Assets/
         * @param path The path to theck.
         * @param mandatoryExtension If this string
         * is not null then the path must end with
         * this file extension string or else it
         * will not be considered valid.
         * @returns Returns true is the asset path is valid, else false.
         */
        public static bool IsAssetFilePathValid(string path, string mandatoryExtension)
        {
            if (mandatoryExtension != null)
            {
                if (!path.EndsWith(mandatoryExtension)) return false;
            }
            return path.StartsWith("Assets/") && !StringUtil.ExtractFileNameFromPath(path, true).Equals("");
        }

        /** Opens a Save File Panel and returns the chosen path if it is valid.
         * @param dialogTitle Title of the panel
         * @param folder The current folder when the panel is opened.
         * @param filename The current filename when the panel is opened.
         * @param extension The extension that files must have in order to be
         * selectable in the save dialog.
         * @return Returns the path that was chosen in the dialog.
         */
        public static string GetFilePathInProject(string dialogTitle, string folder, string filename, string extension)
        {
            string path = null;
            while (true)
            {
                path = EditorUtility.SaveFilePanel(dialogTitle, folder, filename, extension);
                if (path.Length == 0)
                {
                    break;
                }

                string assetFolder = "Assets/";
                int index = path.IndexOf(assetFolder);
                if (index == -1)
                {
                    EditorUtility.DisplayDialog(dialogTitle,
                        "Please choose a path inside the Unity project!",
                        "OK");
                }
                else
                {
                    path = path.Substring(index);
                    break;
                }
            }
            return path;
        }
    }
}

