using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UrbanTerritoriality.MeshToolkit;

namespace UrbanTerritoriality.Experimental
{
    public class InnerEdgeTest : MonoBehaviour
    {
        private List<EdgeInfo> innerEdges;
        private List<EdgeInfo> outerEdges;

        public Color gizmoColor1 = new Color(1, 0, 0, 1);
        public Color gizmoColor2 = new Color(0, 1, 0, 1);
        public float ballRadius = 1;

        protected virtual void Start()
        {
            NavMeshTriangulation navmesh = NavMesh.CalculateTriangulation();
            Vector3[] vertices = navmesh.vertices;
            int[] indices = navmesh.indices;
            Dictionary<string, EdgeInfo> edges = MeshUtil.GetEdgeInfo(vertices, indices);

            List<EdgeInfo> edgeList = new List<EdgeInfo>();
            foreach (string key in edges.Keys)
            {
                edgeList.Add(edges[key]);
            }

            innerEdges = MeshUtil.FilterOuterInnerEdgesOfNavMesh(false, edgeList, 0.001f);
            outerEdges = MeshUtil.FilterOuterInnerEdgesOfNavMesh(true, edgeList, 0.001f);
        }

        protected virtual void OnDrawGizmos()
        {
            if (innerEdges != null || outerEdges != null)
            {
                Gizmos.color = gizmoColor1;
                int n = innerEdges.Count;
                for (int i = 0; i < n; i++)
                {
                    Vector3 edgeCenter = (innerEdges[i].vertice1 + innerEdges[i].vertice2) / 2f;
                    Gizmos.DrawSphere(edgeCenter, ballRadius);
                    Gizmos.DrawLine(innerEdges[i].vertice1, innerEdges[i].vertice2);
                }

                Gizmos.color = gizmoColor2;
                n = outerEdges.Count;
                for (int i = 0; i < n; i++)
                {
                    Vector3 edgeCenter = (outerEdges[i].vertice1 + outerEdges[i].vertice2) / 2f;
                    Gizmos.DrawCube(edgeCenter, Vector3.one * ballRadius);
                }
            }
        }
    }
}

