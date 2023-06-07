using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
	/** A map showing the visibility of at a certain height in a scene.
     * The area that is shown is centered at the position of the GameObect
     * that this component is on.
     */
	public class VisibilityMap2 : GeneralVisibilityMap
	{
		/** How much the a cell increases in value when a single ray is drawn
         * over it */
		public float changePerRaycast = 0.1f;

		/** Weather to use a layer mask when raycasting */
		public bool useLayerMask = false;

		/** The layer mask that is used when performing raycasts
         * if useLayerMask is set to true */
		public LayerMask layerMask;

		/** Weather or not to show raycasting rays in the editor window */
		public bool showRaysInEditor = false;

		/** Color for rays that are drawn in the editor window */
		public Color raysInEditorColor = new Color(0, 1, 0, 1);

		/** This PaintGrid object is the one that is used
         * when drawing the raycasting lines. The cell values
         * in it will increase infinitelly. No normalizing is
         * done on it.
         */
		private PaintGrid workGrid;
		private const int normalizationPeriod = 1000;
		private int normalizationCounter;

		/** apply settings from UTSettings */
		protected override void applySettings(UTSettings settings)
		{
			//TODO apply settings
		}

		/** Do some initialization */
		protected override void _initialize()
		{
			base._initialize();
			int resX = (int)(size.x / cellSize);
			int resY = (int)(size.y / cellSize);
			workGrid = new PaintGrid(resX, resY);
			workGrid.Clear(0.0f);

			/* Start the normalizing coroutine that copies
             * and normalizes data from workGrid into mPaintGrid */
			// StartCoroutine(normalizer.Normalize(workGrid, mPaintGrid));
			normalizationCounter = 0;
		}

		/** Performs a raycast and raws a line in the map based on it */
		protected override void DoSingleRay()
		{
			RaycastHit hit;
			float randomAngle = Random.Range(0, Mathf.PI * 2);
			float dirX = Mathf.Cos(randomAngle);
			float dirZ = Mathf.Sin(randomAngle);
			Vector3 direction = new Vector3(dirX, 0, dirZ);
			direction.Normalize();
			Vector2 dir2d = new Vector2(dirX, dirZ);
			dir2d.Normalize();


			/* Get distance to border */
			Vector2 rayStart = new Vector2(_rayStart.x, _rayStart.z);
			float? dist = CalculateDistanceToBorder(rayStart, dir2d);
			if (dist == null)
			{
				//TODO Investigate if it is should be possible for dist to become null
				Debug.LogWarning("Warning, it was not possible to find the distance from" +
					" raycast start position to the boundaries of the map");
				return;
			}

			//float distance = size.x + size.y;
			float distance = (float)dist;
			float hitDistance = distance;

			if (!IsWithin(new Vector2(_rayStart.x, _rayStart.z)))
			{
				Debug.LogWarning("Warning, the raycast starting point is not within the map." +
					"This may lead to errors!");
				Debug.DrawRay(_rayStart, direction * distance, new Color(1, 0, 0, 1));
			}

			Vector3 hitPoint = _rayStart + direction * distance;

			if (useLayerMask ?
			Physics.Raycast(_rayStart, direction, out hit, distance, layerMask)
			: Physics.Raycast(_rayStart, direction, out hit, distance))
			{
				hitDistance = hit.distance;
				hitPoint = hit.point;
				if (showRaysInEditor)
				{
					Debug.DrawLine(_rayStart, hitPoint, raysInEditorColor);
				}
			}
			Vector2Int s = WorldToGridPos(new Vector2(_rayStart.x, _rayStart.z));
			float hitGridDist = WorldToGridDistance(hitDistance);
			Vector2 sf = new Vector2(s.x, s.y);

			float steps = 0;
			int lastIndex = -1;
			while (steps < hitGridDist)
			{
				Vector2 currentCellF = sf + dir2d * steps;
				steps += 1f;
				Vector2Int currentCell = new Vector2Int((int)currentCellF.x, (int)currentCellF.y);
				if (workGrid.IsWithinGrid(currentCell.x, currentCell.y))
				{
					int index =
						currentCell.y * workGrid.Width +
						currentCell.x;
					if (index != lastIndex)
					{
						workGrid.grid[index] += changePerRaycast;
						lastIndex = index;
					}
				}
			}
			_rayStart = (_rayStart + hitPoint) / 2f;
			FixRayStart();

            normalizationCounter++;
            if(normalizationCounter % normalizationPeriod == 0)
                meanChange = Utilities.Util.Normalize(workGrid, mPaintGrid);
		}
	}
}
