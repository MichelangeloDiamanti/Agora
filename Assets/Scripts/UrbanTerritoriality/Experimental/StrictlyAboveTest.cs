using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Geometry;

namespace UrbanTerritoriality.Experimental
{
    public class StrictlyAboveTest : MonoBehaviour
    {
        public Transform t1p1;
        public Transform t1p2;
        public Transform t1p3;

        public Transform t2p1;
        public Transform t2p2;
        public Transform t2p3;

        public Color col1 = new Color(0, 0, 1, 1);
        public Color col2 = new Color(0, 1, 0, 1);

        protected Mesh tMesh1 = null;
        protected Mesh tMesh2 = null;

        public static void CreateTriangle(ref Mesh m, Triangle3d triangle)
        {
            Vector3[] verts = new Vector3[]
            {
                triangle.p1,
                triangle.p2,
                triangle.p3
            };
            int[] indices = new int[]
            {
                0, 1, 2, 0, 2, 1
            };

            m.vertices = verts;
            m.triangles = indices;
            m.RecalculateBounds();
            m.RecalculateNormals();
            m.RecalculateTangents();
        }

        private void OnDrawGizmos()
        {
            if (t1p1 == null ||
                t1p2 == null ||
                t1p3 == null ||
                t2p1 == null ||
                t2p2 == null ||
                t2p3 == null) return;

            if (tMesh1 == null || tMesh2 == null)
            {
                tMesh1 = new Mesh();
                tMesh2 = new Mesh();
            }

            Triangle3d t1 = new Triangle3d(
                t1p1.position, t1p2.position, t1p3.position);
            Triangle3d t2 = new Triangle3d(
                t2p1.position, t2p2.position, t2p3.position);

            Color c = col1;
            if (!GeometryUtil.IsTriangleAboveAtEveryVerticalIntersection(t1, t2))
            {
                c = col2;
            }
            DrawTriangle(ref tMesh1, t1, c);
            c = col1;
            if (!GeometryUtil.IsTriangleAboveAtEveryVerticalIntersection(t2, t1))
            {
                c = col2;
            }
            DrawTriangle(ref tMesh2, t2, c);
        }

        protected virtual void DrawTriangle(ref Mesh m, Triangle3d t, Color c)
        {
            CreateTriangle(ref m, t);
            Color temp = c;
            temp.a = temp.a * 0.5f;
            Gizmos.color = temp;
            Gizmos.DrawMesh(m);
            Gizmos.color = c;
            Gizmos.DrawLine(t.p1, t.p2);
            Gizmos.DrawLine(t.p2, t.p3);
            Gizmos.DrawLine(t.p3, t.p1);
        }
    }
}
