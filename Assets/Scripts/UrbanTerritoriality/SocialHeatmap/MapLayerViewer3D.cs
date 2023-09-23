using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** A script for rendering a map onto
     * texture in a MeshRenderer */
    public class MapLayerViewer3D : MonoBehaviour
    {
        /** The mesh renderer for displaying the map */
        public MeshRenderer mapRenderer;

        /** The background color of the map. */
        public Color backgroundColor = new Color(0, 0, 0, 1);

        /** Weather the background color will be used. If not
         * the background will be transparent. */
        public bool useBackgroundColor = false;

        /** Information on how the map layer is to be rendered. */
        public LayerRenderInfo layerRenderInfo;

        /** Size of the texture in pixels. */
        public Vector2Int viewerSize = new Vector2Int(300, 300);

        /** Texture used for displaying the map. */
        private Texture2D tex = null;

        /** Weather or not the MapLayerViewer3D has been initialized. */
        public bool Initialized
        { get { return _initialized; } }
        private bool _initialized = false;

        /** Updates the map texture. */
        private void UpdateTexture()
        {
            int texWidth = tex.width;
            int texHeight = tex.height;
            int n = texWidth * texHeight;
            Color[] cols = new Color[n];

            MapRenderer.RenderMapLayer(ref cols,
            texWidth, texHeight,
            layerRenderInfo.map.paintGrid, layerRenderInfo.mainColor, MapRenderer.ReplaceColor);

            if (useBackgroundColor)
            {
                MapRenderer.AddColor(ref cols,
                texWidth, texHeight,
                backgroundColor, MapRenderer.MixColorReverse);
            }

            if (layerRenderInfo.showGrid)
            {
                MapRenderer.AddGrid(ref cols,
                texWidth, texHeight,
                layerRenderInfo.map.paintGrid, layerRenderInfo.gridColor, MapRenderer.MixColor);
            }

            tex.SetPixels(cols);
            tex.Apply();
        }

        /** Do some initialization */
        private void Init()
        {
            tex = new Texture2D(viewerSize.x, viewerSize.y);
        }

        /** Unity Update method */
        void Update()
        {
            if (!_initialized)
            {
                Init();
                _initialized = true;
            }
            UpdateTexture();
            mapRenderer.material.mainTexture = tex;
        }
    }
}
