using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Geometry;

namespace UrbanTerritoriality.Experimental
{
    public class LineSegmentIntersectionTest : MonoBehaviour
    {
        public GameObject point1;
        public GameObject point2;
        public GameObject point3;
        public GameObject point4;

        public Color col = new Color(0, 0, 1, 1);

        private void OnDrawGizmos()
        {
            Vector3 p1_3d = point1.transform.position;
            Vector3 p2_3d = point2.transform.position;
            Vector3 p3_3d = point3.transform.position;
            Vector3 p4_3d = point4.transform.position;

            Gizmos.color = col;
            Gizmos.DrawLine(p1_3d, p2_3d);
            Gizmos.DrawLine(p3_3d, p4_3d);

            Vector2 p1 = new Vector2(p1_3d.x, p1_3d.z);
            Vector2 p2 = new Vector2(p2_3d.x, p2_3d.z);
            Vector2 p3 = new Vector2(p3_3d.x, p3_3d.z);
            Vector2 p4 = new Vector2(p4_3d.x, p4_3d.z);

            Vector2? inter =
            GeometryUtil.LineSegment2DIntersect(new Line2d(p1, p2),
                new Line2d(p3, p4));

            if (inter != null)
            {
                Vector2 inter2 = (Vector2)inter;
                float y = transform.position.y;
                Gizmos.DrawCube(new Vector3(inter2.x, y, inter2.y), new Vector3(1, 1, 1));
            }
        }
    }
}
