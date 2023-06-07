using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Geometry;

namespace UrbanTerritoriality.Experimental
{
    public class TriangleBoundingBoxTest : MonoBehaviour
    {
        public Transform p1;
        public Transform p2;
        public Transform p3;

        public Color col1 = new Color(0, 0, 1, 1);
        public Color col2 = new Color(0, 1, 0, 1);
        public Color col3 = new Color(1, 0, 0, 1);

        protected Mesh triangleMesh = null;

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
            if (p1 == null ||
                p2 == null ||
                p3 == null) return;

            if (triangleMesh == null)
            {
                triangleMesh = new Mesh();
            }

            Triangle3d t = new Triangle3d(
                p1.position, p2.position, p3.position);

            DrawTriangle(ref triangleMesh, t, col1);

            Box3d bounds = GeometryUtil.GetBoundingBox(t);

            Vector3 center = (bounds.minCorner + bounds.maxCorner) / 2f;
            Vector3 size = bounds.maxCorner - bounds.minCorner;
            Color col = col2;
            col.a = 0.5f;
            Gizmos.color = col;
            Gizmos.DrawCube(center, size);

            Rectangle2d bounds2d = GeometryUtil.GetBoundingRectangle(t);
            col = col3;
            col.a = 0.5f;
            Gizmos.color = col;
            Box3d bounds2 = new Box3d(GeometryUtil.ConvertToVector3(bounds2d.minCorner, Axis.Y, 0),
                GeometryUtil.ConvertToVector3(bounds2d.maxCorner, Axis.Y, 0));
            center = (bounds2.minCorner + bounds2.maxCorner) / 2f;
            size = (bounds2.maxCorner - bounds2.minCorner);
            
            Gizmos.DrawCube(center, size);

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
