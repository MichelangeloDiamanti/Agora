using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MengeEditor
{
    public static class Utils
    {
        public static int listHashcode<T>(List<T> list)
        {
            int hash = list.Count * 19;
            foreach (var item in list)
            {
                if (item != null)
                {
                    hash += item.GetHashCode();
                }
            }
            return hash;
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

    }
}