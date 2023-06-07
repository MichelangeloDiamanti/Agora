using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Geometry;

namespace UrbanTerritoriality.Experimental
{
    public class MeshBoundingRectTest : MonoBehaviour
    {
        private Mesh mesh;

        public Color col1 = new Color(0, 0, 1, 1);
        public Color col2 = new Color(0, 1, 0, 1);
        public Color col3 = new Color(1, 0, 0, 1);

        public Vector3 RandVert(float variance)
        {
            return new Vector3(
                Random.Range(-variance, variance),
                Random.Range(-variance, variance),
                Random.Range(-variance, variance));
        }

        public Mesh CreateMesh()
        {
            int triCount = 3;
            int iCount = triCount * 3;
            Vector3[] vertices = new Vector3[iCount];
            int[] triangles = new int[iCount * 2];

            for (int i = 0; i < iCount; i += 3)
            {
                Vector3 location = RandVert(2);
                vertices[i] = RandVert(1) + location;
                vertices[i + 1] = RandVert(1) + location;
                vertices[i + 2] = RandVert(1) + location;
                int i2 = i * 2;
                triangles[i2] = i;
                triangles[i2 + 1] = i + 1;
                triangles[i2 + 2] = i + 2;
                triangles[i2 + 3] = i;
                triangles[i2 + 4] = i + 2;
                triangles[i2 + 5] = i + 1;
            }
               
            Mesh m = new Mesh();
            m.vertices = vertices;
            m.triangles = triangles;

            m.RecalculateBounds();
            m.RecalculateNormals();
            m.RecalculateTangents();

            return m;
        }

        private System.DateTime? lastTime = null;

        

        private void OnDrawGizmos()
        {
            bool createMesh = false;
            if (lastTime == null)
            {
                createMesh = true;
            }
            else
            {
                if ((System.DateTime.Now - (System.DateTime)lastTime).TotalSeconds > 1)
                {
                    createMesh = true;
                }
            }
            if (createMesh)
            {
                mesh = CreateMesh();
                lastTime = System.DateTime.Now;
            }

            Rectangle2d rect2d =
                MeshToolkit.MeshUtil.GetMeshBoundingRectangle(
                    mesh.vertices, mesh.GetIndices(0));
            Box3d bounds = new Box3d(GeometryUtil.ConvertToVector3(rect2d.minCorner, Axis.Y, 0),
                GeometryUtil.ConvertToVector3(rect2d.maxCorner, Axis.Y, 0));
            Color col = col1;
            col.a = 0.5f;
            Gizmos.color = col;
            Gizmos.DrawMesh(mesh);
            col = col2;
            col.a = 0.5f;
            Gizmos.color = col;
            Vector3 center = (bounds.minCorner + bounds.maxCorner) / 2f;
            Vector3 size = bounds.maxCorner - bounds.minCorner;
            Gizmos.DrawCube(center, size);

            Box3d bounds3d =
                MeshToolkit.MeshUtil.GetMeshBoundingBox(
                    mesh.vertices, mesh.triangles);
            col = col3;
            col.a = 0.5f;
            Gizmos.color = col;
            center = (bounds3d.minCorner + bounds3d.maxCorner) / 2f;
            size = (bounds3d.maxCorner - bounds3d.minCorner);
            Gizmos.DrawCube(center, size);
        }
    }
}
