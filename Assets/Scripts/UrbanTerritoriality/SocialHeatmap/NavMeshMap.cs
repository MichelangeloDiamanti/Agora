using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Maps
{
	/**
     * A grid based map showing a nav mesh in a unity scene.
     */
	public class NavMeshMap : GeneralHeatmap
	{
		[System.Serializable]
		public class myNavMeshMapEvent : UnityEvent<NavMeshMap> { }
		[HideInInspector]
		public myNavMeshMapEvent OnNavMeshMapBakedEvent;
		/** Weather to invert the map or not.
         * If invert is true then walkable areas will have a value
         * of 0 and unwalkable 1, else walkable areas will have a value
         * of 1 and unwalkable a value of 0. */
		public bool invert = false;

		/** Apply settings from a UTSettings object. */
		protected override void applySettings(UTSettings settings)
		{
			//TODO apply settings from settings
		}

		/** Do some initialization */
		protected override void _initialize()
		{
			int resX = (int)(size.x / cellSize);
			int resY = (int)(size.y / cellSize);
			mPaintGrid = new PaintGrid(resX, resY);
			BakeMap();
			OnNavMeshMapBakedEvent.Invoke(this);
		}

		public override float GetValueAt(Vector2 position, PathFollowingAgent agent)
		{
			Vector2Int gridpos = WorldToGridPos(position);
			if (!mPaintGrid.IsWithinGrid(gridpos.x, gridpos.y))
			{
				return mPaintGrid.defaultValue;
			}
			float val = mPaintGrid.GetValueAt(gridpos.x, gridpos.y);
			return val;
		}

		/** Creates the map */
		private void BakeMap()
		{
			NavMeshTriangulation navmesh = NavMesh.CalculateTriangulation();
			int[] indices = navmesh.indices;
			Vector3[] vertices = navmesh.vertices;
			int n = indices.Length;

			mPaintGrid.Clear(invert ? 1 : 0);

			for (int i = 0; i < n; i += 3)
			{
				Vector3 t1 = vertices[indices[i]];
				Vector3 t2 = vertices[indices[i + 1]];
				Vector3 t3 = vertices[indices[i + 2]];
				Vector2 t1i = WorldToGridPosFloat(new Vector2(t1.x, t1.z));
				Vector2 t2i = WorldToGridPosFloat(new Vector2(t2.x, t2.z));
				Vector2 t3i = WorldToGridPosFloat(new Vector2(t3.x, t3.z));

				mPaintGrid.DrawTriangle(invert ? 0 : 1,
					PaintGrid.PaintMethod.REPLACE,
					t1i, t2i, t3i, 0.001f);
			}
		}
	}
}

