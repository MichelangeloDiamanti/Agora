using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** A script for rendering a map to a texture. */
    public class MapLayerRenderer : MonoBehaviour
    {
        /** The map to render */
        public GeneralHeatmap map;

        /** The main color of the map */
        public Color mainColor;

        /** Weather to show a grid on the map or not. */
        public bool showGrid;
        
        /** The color of the grid. */
        public Color gridColor;

        /** Render the map to a texture.
         * @param texture A color array representing a texture to render to.
         * The texture will be written to this array.
         * @param texWidth Widht of the texture.
         * @param texHeight Height of the texture.
         */
        public void RenderToTexture(ref Color[] texture, int texWidth, int texHeight)
        {
            MapRenderer.RenderMapLayer(ref texture,
            texWidth, texHeight,
            map.paintGrid, mainColor, MapRenderer.ReplaceColor);

            if (showGrid)
            {
                MapRenderer.AddGrid(ref texture,
                texWidth, texHeight,
                map.paintGrid, gridColor, MapRenderer.MixColor);
            }
        }
    }
}
