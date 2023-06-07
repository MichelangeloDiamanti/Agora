using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UrbanTerritoriality.Maps
{
	/** A custom inspector for a visibility map */
	[CustomEditor(typeof(CrowdnessAnalysis), true)]
	public class CrowdnessAnalysisInspector : GeneralHeatmapInspector
	{
		/** The visibility map */
		protected CrowdnessAnalysis vmap;

		/** Get the visibility map
         * @return Returns the visibility map
         */
		protected virtual CrowdnessAnalysis GetVisibilityMap()
		{
			if (vmap == null)
			{
				vmap = (CrowdnessAnalysis)heatmap;
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
			GUILayout.Label("Subscribers: " + vmap.SubscribedAgentsCount);
			GUILayout.Label("Delta Events: " + vmap.DeltaEvents);
			GUILayout.Label("Mean Change: " + vmap.meanChange);
		}
	}
}
