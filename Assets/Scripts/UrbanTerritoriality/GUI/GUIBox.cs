using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.GUInterface
{
    /** A GUI box to display using the OnGUI method
     * of a MonoBehaviour class. */
    public class GUIBox
    {
        /** The content of the box */
        public GUIContent content;

        /** A Rect defining the position and size of the box */
        public Rect position;

        /** The style of the box */
        public GUIStyle style;

        /** The background color of the box */
        public Color color;

        /** A dictionary of Texture2D objects used as background
         * for different colors */
        protected Dictionary<Color, Texture2D> backgrounds;

        /** Constructor */
        public GUIBox()
        {
            backgrounds = new Dictionary<Color, Texture2D>();
        }

        /** Add a texture to the background dictionary
         * if c is missing as a key in the dictionary
         * @param c The color of the background.
         */
        public void AddColorIfMissing(Color c)
        {
            if (!backgrounds.ContainsKey(color))
            {
                backgrounds[color] =
                    new Texture2D(128, 128);
                int n = backgrounds[color].width * backgrounds[color].height;
                Color[] cols = new Color[n];
                for (int i = 0; i < n; i++)
                {
                    cols[i] = color;
                }
                backgrounds[color].SetPixels(cols);
                backgrounds[color].Apply();
            }
        }

        /** Creates the box
         * This method should be called inside an
         * OnGUI method of a MonoBehaviour class */
        public virtual void OnGUI()
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.box);
            }
            AddColorIfMissing(color);
            style.normal.background = backgrounds[color];
            GUI.Box(position, content, style);
        }
    }
}

