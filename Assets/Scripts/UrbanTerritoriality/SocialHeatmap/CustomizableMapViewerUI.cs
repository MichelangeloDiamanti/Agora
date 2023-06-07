using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UrbanTerritoriality.Maps
{
    /** The user interface for a customizable map viewer. */
	public class CustomizableMapViewerUI : MonoBehaviour
    {
        /** The CustomizableMapViewer that is used
         * for generating the map texture.
         */
		public CustomizableMapViewer mapViewer;

        /** A RawImage object to put the map texture on.
         * This is the image that will be shown in the UI.
         */
		public RawImage mapImage;

        /** The GameObject that will contain toggle
         * control for controlling what layers in the
         * map are visible.
         */
        public GameObject layerToggleContainer;

        /** A prefab for a layer toggle control */
        public GameObject layerTogglePrefab;

        /** An array of toggles for the map layers. */
        private Toggle[] layerToggles;

        /** A reference to map layer info. Used for reading
         and manipulating the map layers. */
        private LayerRenderInfo[] layers;

        /** Weather the map viewer UI has been initialized. */
		public bool Initialized
		{ get { return _initialized; }}
		private bool _initialized = false;

        /** Unity Update method */
		void Update ()
        {
			if (mapViewer.Initialized && !_initialized)
			{
                layers = mapViewer.layers;
                int n = layers.Length;
				layerToggles = new Toggle[n];

                GameObject toggleGO;
                Text txt;
                for (int i = 0; i < n; i++)
                {
                    toggleGO = Instantiate(layerTogglePrefab, layerToggleContainer.transform);
                    layerToggles[i] = toggleGO.GetComponent<Toggle>();
                    txt = toggleGO.GetComponentInChildren<Text>();
                    txt.text = layers[i].layerName;

                    /** We need to create this variable for the so the correct
                     * value will be passed to the event handler. */
                    int index = i;
					layerToggles[index].onValueChanged.AddListener(delegate {
						ToggleChanged(index);
					});
                }

                //TODO find a better way to add icons for game objects
                toggleGO = Instantiate(layerTogglePrefab, layerToggleContainer.transform);
                Toggle hscToggle = toggleGO.GetComponent<Toggle>();
                txt = toggleGO.GetComponentInChildren<Text>();
                txt.text = "Heat Space Icons";
                hscToggle.onValueChanged.AddListener(delegate {
                    HeatSpaceCenterToggle();
                });
				_initialized = true;
			}

			if (_initialized)
			{
				mapImage.texture = mapViewer.texture;
			}
		}

        /** Called when the toggle for the heat spaces changes. */
        private void HeatSpaceCenterToggle()
        {
            mapViewer.showHeatSpaceCenters = !mapViewer.showHeatSpaceCenters;
        }

        /** Called when any of the toggles in the layerToggle array changes
         * @param index The index in the array of the toggle that changed.
         */
		private void ToggleChanged(int index)
		{
            layers[index].showMapData = layerToggles[index].isOn;
		}
	}
}
