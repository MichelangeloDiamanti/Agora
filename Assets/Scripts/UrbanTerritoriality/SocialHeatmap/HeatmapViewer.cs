using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace UrbanTerritoriality.Maps
{
	/**
     * A widget that displays a territorial heatmap on the screen
	 * when it is added as a component to a GameObject in a scene.
     */
	public class HeatmapViewer : MonoBehaviour
    {
        /**
         * The heatmap this widget display information for.
         */
		public TerritorialHeatmap heatmap;

        /** Color of the heatmap */
        public Color mapColor = new Color(1, 0, 0, 1);

        /**
         * The size of the viewer on the screen in pixels.
         */
        public Vector2Int viewerSize = new Vector2Int(300, 300);

        /**
         * The texture used for painting the heatmap. The
         * Heatmap values will be painted onto this texture.
         */
        private Texture2D tex = null;

        /** Updates the texture with the map */
        private void UpdateHeatmapTexture()
        {
            if (tex == null)
            {
                tex = new Texture2D(heatmap.paintGrid.Width, heatmap.paintGrid.Height);
            }

            Color[] cols = new Color[tex.width * tex.height];

            int n = heatmap.paintGrid.Width * heatmap.paintGrid.Height;
            for (int i = 0; i < n; i++)
            {
                cols[i] = mapColor * heatmap.paintGrid.grid[i] + new Color(0, 0, 0, 1);
            }

            tex.SetPixels(cols);

            List<HeatSpace> hsp = heatmap.GetHeatSpaces();
            foreach (HeatSpace hs in hsp)
            {
                Vector3 pos3 = hs.transform.position;
                Vector2 pos2 = new Vector2(pos3.x, pos3.z);
                Vector2Int pos = heatmap.WorldToGridPos(pos2);
                tex.SetPixel(pos.x, pos.y, Color.blue);
            }
            tex.Apply();
        }

        /** Unity Update method */
        void Update () {
            UpdateHeatmapTexture();
        }

        /** Unity OnGUI method */
		void OnGUI()
		{
			GUI.DrawTexture(new Rect(10, 10, (int)viewerSize.x, (int)viewerSize.y), tex);
		}
    }
}
