using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Utilities
{
    public static class StringUtil
    {
        /** Modify extension of a filename
         * @param path The filename to modify
         * @param newExtension The new extension for the filename
         * @returns The filename where its extension has been replaced with newExtension
         */
        public static string ModifyExtension(string filename, string newExtension)
        {
            int lastPointIndex = filename.LastIndexOf(".");
            return lastPointIndex > filename.LastIndexOf("/") ?
                filename.Substring(0, lastPointIndex) + "." + newExtension : filename;
        }

        /** Get the filename from a path
         * @param path The path to get the filename in.
         * @param discardExtension Wether to remove the extension from the filename
         * @returns Returns the filename.
         */
        public static string ExtractFileNameFromPath(string path, bool discardExtension)
        {
            int slashIndex = path.LastIndexOf("/");
            if (slashIndex != -1)
            {
                int fileStartIndex = slashIndex + 1;
                path = path.Substring(fileStartIndex, path.Length - fileStartIndex);
            }
            if (discardExtension)
            {
                int dotIndex = path.LastIndexOf(".");
                if ( dotIndex != -1)
                {
                    path = path.Substring(0, dotIndex);
                }
            }
            return path;
        }

        /** Get the folder part of a path. That is remove the filename
         * from the back of the string.
         * @param path The path to get the foldername in.
         * @returns Returns The full folder name.
         */
        public static string ExtractFolderNameFromPath(string path)
        {
            int slashIndex = path.LastIndexOf("/");
            if (slashIndex == -1) return "";
            return path.Substring(0, slashIndex);
        }
    }
}

