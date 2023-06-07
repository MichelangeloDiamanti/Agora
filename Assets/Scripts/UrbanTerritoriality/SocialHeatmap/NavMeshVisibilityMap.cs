using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
	/** A visibility map that uses a NavMeshMap to create
     * the map.
     */
	public class NavMeshVisibilityMap : GeneralVisibilityMap
	{
		/** The NavMeshMap that will be used to compute
         * the visibility map */
		public NavMeshMap navMeshMap;

		/** How much the a cell increases in value when a single ray is drawn
         * over it */
		private readonly float changePerRay = 0.1f;

		/** This PaintGrid object is the one that is used
         * when drawing the rays. The cell values
         * in it will increase infinitelly. No normalizing is
         * done on it.
         */
		private PaintGrid workGrid;

		/** apply settings from an UTSettings object.
         * @param settings An UTSettings object with
         * some values that can potentially be used
         * in this class.
         */
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
		}

		/** Checks if a position in the world is empty space
         * By looking at the NavMeshMap.
         * @param pos The position in the world in.
         * */
		protected bool IsEmptySpace(Vector2 pos)
		{
			float navMeshValue = navMeshMap.GetValueAt(pos, null);
			bool isEmptySpace = navMeshMap.invert ^ navMeshValue > 0.5f;
			return isEmptySpace;
		}

		/** Draws one line in the map where
         * there is empty space. */
		protected override void DoSingleRay()
		{
			_rayStart.y = rayStart.position.y;

			float randomAngle = Random.Range(0, Mathf.PI * 2);
			float dirX = Mathf.Cos(randomAngle);
			float dirZ = Mathf.Sin(randomAngle);
			Vector2 dir2d = new Vector2(dirX, dirZ);
			dir2d.Normalize();

			Vector2? lastEmptyPos = DrawRayUntilObstacle(
				new Vector2(_rayStart.x, _rayStart.z), dir2d);
			if (lastEmptyPos != null)
			{
				Vector2 lep = (Vector2)lastEmptyPos;
				Vector3 lastEmptyPos3d = new Vector3(lep.x, rayStart.position.y, lep.y);
				Vector3 newStart = (_rayStart + lastEmptyPos3d) / 2f;
				Vector2 newStart2d = new Vector2(newStart.x, newStart.z);
				if (IsWithin(newStart2d) && IsEmptySpace(newStart2d))
				{
					_rayStart = newStart;
				}
			}
		}

		/** Draw a point in the map if it is empty space.
         * @param point The world position of the point.
         * @returns Returns true if the position was in empty
         * space defined by the NavMeshMap.
         */
		protected virtual bool DrawPointIfEmptySpace(Vector2 point)
		{
			if (IsEmptySpace(point))
			{
				Vector2Int currentCell = WorldToGridPos(point);
				if (workGrid.IsWithinGrid(currentCell.x, currentCell.y))
				{
					int index =
						currentCell.y * workGrid.Width +
						currentCell.x;
					workGrid.grid[index] += changePerRay;
					return true;
				}
			}
			return false;
		}

		/** Draws a ray in the map until it reaches an obstacle defined by
         * the NavMeshMap.
         * @param start The start of the ray in the world.
         * @param direction The direction of the ray.
         * @returns Retuns the last empty position in the world. That is
         * the last position of the ray before it went into an obstacle.
         */
		protected virtual Vector2? DrawRayUntilObstacle(Vector2 start, Vector2 direction)
		{
			int steps = 0;
			bool empty = true;
			Vector2? lastEmptyPos = null;
			while (empty)
			{
				Vector2 currentPos = start +
					direction.normalized * (float)steps * cellSize * 0.9f;
				empty = DrawPointIfEmptySpace(currentPos);
				if (empty) lastEmptyPos = currentPos;
				steps++;
			}
			return lastEmptyPos;
		}

		/** Unity Start method */
		public override void Start()
		{
			/* Overriding this so that the Initialize method
             * is not called in the Start method */
			//TODO maybe find a better way to do this
			/* Other possible ways 
             * Do initialization in Update instead of Start and
             * let _initialize return a boolean that tells weather
             * the initialization was successfull or not.
             * Then call _initialize until it returns true.
             */
		}

		/** Unity Update method */
		protected override void Update()
		{
			base.Update();
			if (!initialized && navMeshMap.Initialized)
			{
				Initialize();
			}
			else
			{
				DoRays();
				meanChange = Utilities.Util.Normalize(workGrid, mPaintGrid);
			}
		}

		/** On gizmos update, enforce navmeshmap to have same size,
         * position, cell size and gizmo color
         */
		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (navMeshMap != null)
			{
				navMeshMap.size = size;
				navMeshMap.transform.position = transform.position;
				navMeshMap.cellSize = cellSize;
				navMeshMap.gizmoColor = gizmoColor;
			}
		}
	}
}

