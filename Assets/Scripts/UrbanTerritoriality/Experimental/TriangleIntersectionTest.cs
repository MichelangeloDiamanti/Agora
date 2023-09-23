using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Geometry;

namespace UrbanTerritoriality.Experimental
{
    public class TriangleIntersectionTest : MonoBehaviour
    {
        public GameObject t1p1;
        public GameObject t1p2;
        public GameObject t1p3;

        public GameObject t2p1;
        public GameObject t2p2;
        public GameObject t2p3;

        public Color col = new Color(0, 0, 1, 1);
        public Color col2 = new Color(0, 1, 0, 1);

        private void OnDrawGizmos()
        {
            foreach (GameObject corn in new GameObject[]
            {
                t1p1,
                t1p2,
                t1p3,
                t2p1,
                t2p2,
                t2p3
            })
            {
                if (corn == null)
                {
                    return;
                }
            }

            Vector3 t1p1_3d = t1p1.transform.position;
            Vector3 t1p2_3d = t1p2.transform.position;
            Vector3 t1p3_3d = t1p3.transform.position;

            Vector3 t2p1_3d = t2p1.transform.position;
            Vector3 t2p2_3d = t2p2.transform.position;
            Vector3 t2p3_3d = t2p3.transform.position;

            Gizmos.color = col;
            Gizmos.DrawLine(t1p1_3d, t1p2_3d);
            Gizmos.DrawLine(t1p2_3d, t1p3_3d);
            Gizmos.DrawLine(t1p3_3d, t1p1_3d);
            Gizmos.DrawLine(t2p1_3d, t2p2_3d);
            Gizmos.DrawLine(t2p2_3d, t2p3_3d);
            Gizmos.DrawLine(t2p3_3d, t2p1_3d);

            Vector2 t1p1_2d = new Vector2(t1p1_3d.x, t1p1_3d.z);
            Vector2 t1p2_2d = new Vector2(t1p2_3d.x, t1p2_3d.z);
            Vector2 t1p3_2d = new Vector2(t1p3_3d.x, t1p3_3d.z);
            Vector2 t2p1_2d = new Vector2(t2p1_3d.x, t2p1_3d.z);
            Vector2 t2p2_2d = new Vector2(t2p2_3d.x, t2p2_3d.z);
            Vector2 t2p3_2d = new Vector2(t2p3_3d.x, t2p3_3d.z);

            Vector2[] points = GeometryUtil.GetTriangleIntersection2D(
                new Triangle2d(t1p1_2d, t1p2_2d, t1p3_2d),
                new Triangle2d(t2p1_2d, t2p2_2d, t2p3_2d));

            Gizmos.color = col2;
            foreach (Vector2 p in points)
            {
                Gizmos.DrawSphere(new Vector3(p.x, 0, p.y), 0.5f);
            }
        }
    }
}
