using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** A map viewer for viewing a TerritorialHeatmap
     * and a ColliderMap on top of each other. */
    public class MultiMapViewer : MonoBehaviour
    {
        /** The TerritorialHeatmap to be shown. */
        public TerritorialHeatmap heatmap;

        /** The ColliderMap to be shown. */
        public ColliderMap colliderMap;

        /** Size of the viewer on screen in pixels. */
        public Vector2Int viewerSize = new Vector2Int(300, 300);

        /** The texture used for displaying the map */
        private Texture2D tex = null;

        /** Updates the map texture. */
        private void UpdateTexture()
        {
            if (tex == null)
            {
                tex = new Texture2D(colliderMap.paintGrid.Width, colliderMap.paintGrid.Height);
            }

            Color[] cols = new Color[tex.width * tex.height];

            int n = heatmap.paintGrid.Width * heatmap.paintGrid.Height;
            for (int i = 0; i < n; i++)
            {
                cols[i] = new Color(heatmap.paintGrid.grid[i], colliderMap.paintGrid.grid[i], 0, 1);
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
            GUI.DrawTexture(new Rect(10, 10, (int)viewerSize.x, (int)viewerSize.y), tex);
        }
    }
}
