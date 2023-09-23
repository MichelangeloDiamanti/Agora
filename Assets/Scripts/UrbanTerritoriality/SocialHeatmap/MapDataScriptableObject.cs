using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.ScriObj;

namespace UrbanTerritoriality.Maps
{
	/** A scriptable object containing data for a saved map. */
	public class MapDataScriptableObject : ScriptableObject
	{
		/** The position of the map. */
		public Vector3 position;

		/** Number of cells in the x and z directions */
		public Vector2Int gridSize;

		/** The cell size of the map */
		public float cellSize;

		/** 
            Gradient used to save this particular map as asset
            Useful when we have to retrieve the texture from the data.
        */
		public GradientScriptableObject gradient;

		/** The data in the map */
		[HideInInspector]
		public float[] data;

		public Gradient GetGradient()
		{
			return gradient.gradient;
		}
	}
}
