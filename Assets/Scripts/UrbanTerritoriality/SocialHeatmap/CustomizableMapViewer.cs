using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** A map viewer for viewing multiple maps that can
     * be customized in various ways.
     */
    public class CustomizableMapViewer : MonoBehaviour
    {
        /** The texture used for displaying the map */
        public Texture2D texture
        { get { return tex; } }
        private Texture2D tex = null;

        /** Weather or not the viewer has been
         * initialized or not. */
        public bool Initialized
        { get { return _initialized; } }
        private bool _initialized = false;

        /** Background color for the map image */
        public Color backgroundColor = new Color(0, 0, 0, 1);

        /** Information about map layers to render */
        public LayerRenderInfo[] layers;

        /** Weather or not to show centers of heat spaces */
        public bool showHeatSpaceCenters = true;

        /** The size of the viewer in screen pixels. */
        public Vector2Int viewerSize = new Vector2Int(320, 320);

        /** An icon to show for heat spaces. */
        public Texture2D heatSpaceIcon;

        //TODO remove this at some point, this is only for the heat space icons
        /** A territorial heatmap, for getting the position of the heat spaces. */
        public TerritorialHeatmap heatmap;

        /** Update the texture */
        private void UpdateTexture()
        {
            int texWidth = tex.width;
            int texHeight = tex.height;
            int n = texWidth * texHeight;
            Color[] cols = new Color[n];

            int layerCount = layers.Length;

            /* Clear the texture */
            Color emptyColor = new Color(0, 0, 0, 0);
            MapRenderer.AddColor(ref cols, texWidth, texHeight,
                emptyColor, MapRenderer.ReplaceColor);

            /* Add map layers */
            for (int i = 0; i < layerCount; i++)
            {
                if (layers[i].showMapData)
                {
                    MapRenderer.RenderMapLayer(ref cols,
                        texWidth, texHeight,
                        layers[i].map.paintGrid,
                        layers[i].mainColor, MapRenderer.AddColor);
                }
            }

            /* Add grids for each layer */
            for (int i = 0; i < layerCount; i++)
            {
                if (layers[i].showGrid)
                {
                    MapRenderer.AddGrid(ref cols,
                        texWidth, texHeight,
                        layers[i].map.paintGrid, layers[i].gridColor, MapRenderer.MixColor);
                }
            }

            //TODO add method for drawing icons/points in MapRenderer and use it here
            if (showHeatSpaceCenters)
            {
                int heatmapWidth = heatmap.paintGrid.Width;
                int heatmapHeight = heatmap.paintGrid.Height;
                int iconWidth = heatSpaceIcon.width;
                int iconHeight = heatSpaceIcon.height;
                List<HeatSpace> hsp = heatmap.GetHeatSpaces();
                Color[] icon = heatSpaceIcon.GetPixels();
                foreach (HeatSpace hs in hsp)
                {
                    Vector3 pos3 = hs.transform.position;
                    Vector2 pos2 = new Vector2(pos3.x, pos3.z);
                    Vector2Int pos = heatmap.WorldToGridPos(pos2);
                    int posX = pos.x * texWidth / heatmapWidth;
                    int posY = pos.y * texHeight / heatmapHeight;

                    int iconStartX = posX - iconWidth / 2;
                    int iconStartY = posY - iconHeight / 2;
                    int iconEndX = iconStartX + iconWidth - 1;
                    int iconEndY = iconStartY + iconHeight - 1;
                    int iconX = 0;
                    for (int x = iconStartX; x <= iconEndX; x++)
                    {
                        int iconY = 0;
                        for (int y = iconStartY; y <= iconEndY; y++)
                        {
                            if (x < 0 || x >= texWidth || y < 0 || y >= texHeight) continue;
                            int texIndex = y * texWidth + x;
                            int iconIndex = iconY * iconWidth + iconX;
                            Color iconPixel = icon[iconIndex];
                            Color mapPixel = cols[texIndex];
                            float a = iconPixel.a;
                            float one_a = 1 - a;
                            float r = iconPixel.r * a + mapPixel.r * one_a;
                            float g = iconPixel.g * a + mapPixel.g * one_a;
                            float b = iconPixel.b * a + mapPixel.b * one_a;
                            cols[texIndex] = new Color(r, g, b, 1);
                            iconY++;
                        }
                        iconX++;
                    }
                }
            }

            /* Add the background color */
            MapRenderer.AddColor(ref cols,
            texWidth, texHeight,
            backgroundColor, MapRenderer.MixColorReverse);

            tex.SetPixels(cols);
            tex.Apply();
        }


        /** Initialize the viewer */
        private void Init()
        {
            tex = new Texture2D(viewerSize.x, viewerSize.y);
        }

        /** Unity Update method */
        private void Update()
        {
            if (!_initialized)
            {
                Init();
                _initialized = true;
            }
            UpdateTexture();
        }
    }
}
