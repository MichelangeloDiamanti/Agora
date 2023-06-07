using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /**
     * This is a viewer script specifically for
     * a collider map.
     * Add this component to a GameObject in a scene
     * to display a image on the screen that shows a
     * ColliderMap.
     */ 
    public class ColliderMapViewer : MonoBehaviour
    {
        /** The ColliderMap that this viewer displays. */
        public ColliderMap colliderMap;

        /** Color of the collider map */
        public Color mapColor = new Color(0, 1, 0, 1);
        
        /** Size of the viewer on screen in pixels. */
        public Vector2Int viewerSize = new Vector2Int(300, 300);

        /** The texture used for displaying the ColliderMap */
        private Texture2D tex = null;

        /** Updates the texture */
        private void UpdateTexture()
        {
            if (tex == null)
            {
                tex = new Texture2D(colliderMap.paintGrid.Width, colliderMap.paintGrid.Height);
            }

            Color[] cols = new Color[tex.width * tex.height];

            int n = colliderMap.paintGrid.Width * colliderMap.paintGrid.Height;
            for (int i = 0; i < n; i++)
            {
                cols[i] = mapColor * colliderMap.paintGrid.grid[i] + new Color(0, 0, 0, 1);
            }
            tex.SetPixels(cols);
            tex.Apply();
        }

        /** Unity Update method */
        void Update()
        {
            UpdateTexture();
        }

        /** Unity OnGUI method */
        void OnGUI()
        {
            GUI.DrawTexture(new Rect(Screen.width - viewerSize.x - 10, 10, (int)viewerSize.x, (int)viewerSize.y), tex);
        }
    }
}

