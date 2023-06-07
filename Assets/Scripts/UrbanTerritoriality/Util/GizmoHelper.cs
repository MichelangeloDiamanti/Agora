using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Utilities
{
	/** A class with some functions for drawing gizmos */
	public static class GizmoHelper
	{
		/** Draw a plane in the editor
         * This method should only be called in a OnDrawGizmos method of
         * a MonoBehaviour class.
         * @param center The center of the plane.
         * @param size The size of the plane.
         * @param color The color of the plane.
         */
		public static void DrawPlane(Vector3 center, Vector2 size, Color color)
		{
			Color tempCol = color;
			tempCol.a = tempCol.a / 2f;
			Gizmos.color = tempCol;
			Gizmos.DrawCube(center, new Vector3(size.x, 0.01f, size.y));
			float rX = size.x / 2f;
			float rY = size.y / 2f;
			Vector3 southWestCorner = new Vector3(center.x - rX, center.y, center.z - rY);
			Vector3 northWestCorner = new Vector3(center.x - rX, center.y, center.z + rY);
			Vector3 northEastCorner = new Vector3(center.x + rX, center.y, center.z + rY);
			Vector3 southEastCorner = new Vector3(center.x + rX, center.y, center.z - rY);
			Gizmos.color = color;
			Gizmos.DrawLine(southWestCorner, northWestCorner);
			Gizmos.DrawLine(northWestCorner, northEastCorner);
			Gizmos.DrawLine(northEastCorner, southEastCorner);
			Gizmos.DrawLine(southEastCorner, southWestCorner);
		}

		public static void DrawGrid(Vector3 center, Vector2 size, Color color, int cellSize)
		{
			Color tempCol = color;
			tempCol.a = tempCol.a / 2f;
			Gizmos.color = tempCol;
			Gizmos.DrawCube(center, new Vector3(size.x, 0.01f, size.y));
			float rX = size.x / 2f;
			float rY = size.y / 2f;
			Vector3 southWestCorner = new Vector3(center.x - rX, center.y, center.z - rY);
			Vector3 northWestCorner = new Vector3(center.x - rX, center.y, center.z + rY);
			Vector3 northEastCorner = new Vector3(center.x + rX, center.y, center.z + rY);
			Vector3 southEastCorner = new Vector3(center.x + rX, center.y, center.z - rY);
			Gizmos.color = color;
			Gizmos.DrawLine(southWestCorner, northWestCorner);
			Gizmos.DrawLine(northWestCorner, northEastCorner);
			Gizmos.DrawLine(northEastCorner, southEastCorner);
			Gizmos.DrawLine(southEastCorner, southWestCorner);

			int horizontalLines = (int)(size.y / cellSize) + 1;
			int verticalLines = (int)(size.x / cellSize) + 1;

			// if ((size.y % cellSize) > 0) horizontalLines++;
			// if ((size.x % cellSize) > 0) horizontalLines++;


			int drawnLines = 0;
			while (drawnLines < horizontalLines)
			{
				Vector3 offset = new Vector3(0, 0, drawnLines * cellSize);
				Gizmos.DrawLine(southWestCorner + offset, southEastCorner + offset);
				drawnLines++;
			}
			drawnLines = 0;
			while (drawnLines < verticalLines)
			{
				Vector3 offset = new Vector3(drawnLines * cellSize, 0, 0);
				Gizmos.DrawLine(southWestCorner + offset, northWestCorner + offset);
				drawnLines++;
			}
		}

	}
}
