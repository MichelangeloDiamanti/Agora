using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UrbanTerritoriality.Maps
{
	/** A custom inspector for a visibility map */
	[CustomEditor(typeof(GeneralVisibilityMap), true)]
	public class GeneralVisibilityMapInspector : GeneralHeatmapInspector
	{
		/** The visibility map */
		protected GeneralVisibilityMap vmap;

		/** Get the visibility map
         * @return Returns the visibility map
         */
		protected virtual GeneralVisibilityMap GetVisibilityMap()
		{
			if (vmap == null)
			{
				vmap = (GeneralVisibilityMap)heatmap;
			}
			return vmap;
		}

		/** Overrides The AddSaveTimeGUI method to add some features
         * that were not in the parent class.
         * */
		public override void AddSaveGUI()
		{
			base.AddSaveGUI();
		}
	}
}
