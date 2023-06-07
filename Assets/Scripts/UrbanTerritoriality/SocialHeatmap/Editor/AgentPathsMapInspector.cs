using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UrbanTerritoriality.Maps
{
	/** A custom inspector for a visibility map */
	[CustomEditor(typeof(AgentPathsMap), true)]
	public class AgentPathsMapInspector : GeneralHeatmapInspector
	{
		/** The visibility map */
		protected AgentPathsMap vmap;

		/** Get the visibility map
         * @return Returns the visibility map
         */
		protected virtual AgentPathsMap GetVisibilityMap()
		{
			if (vmap == null)
			{
				vmap = (AgentPathsMap)heatmap;
			}
			return vmap;
		}

		/** Overrides The AddSaveTimeGUI method to add some features
         * that were not in the parent class.
         * */
		public override void AddSaveGUI()
		{
			base.AddSaveGUI();

			vmap = GetVisibilityMap();
			GUILayout.Label("Events: " + vmap.Events);
			GUILayout.Label("Subscribers: " + vmap.SubscribedAgentsCount);
		}
	}
}
