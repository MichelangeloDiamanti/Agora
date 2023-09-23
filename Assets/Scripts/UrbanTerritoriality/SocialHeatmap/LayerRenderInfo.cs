using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** Information that is needed for rendering
     * a single layer in a map viewer */
    [System.Serializable]
    public struct LayerRenderInfo {

        /** Name of the layer that is shown in the GUI */
        public string layerName;

        /** Weather or not to show the map data or not */
        public bool showMapData;

        /** The map that is to be shown on this layer */
        public GeneralHeatmap map;

        /** The color of the layer */
        public Color mainColor;

        /** Weather or not to show a grid for this layer */
        public bool showGrid;

        /** The color of the grid if it is shown */
        public Color gridColor;
    }
}

