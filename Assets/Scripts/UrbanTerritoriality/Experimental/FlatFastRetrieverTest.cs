using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Experimental
{
    public class FlatFastRetrieverTest : MonoBehaviour
    {

        public Mesh mesh;

        public Material mat;

        public int totalNrOfObjects = 1000;

        public Vector3 size;

        List<Vector3> positions;

        public Transform observer;

        public float maxDistance = 5;

        public float objectsPerCell = 100;

        public bool useFlatFastRetriever = false;
        private Geometry.GridRetriever2D<Vector3> retriever;

        private void Start()
        {
            float cellSize = Mathf.Sqrt(objectsPerCell * size.x * size.z / (float)totalNrOfObjects);
            Debug.Log("cellSize: " + cellSize);
            retriever = new Geometry.GridRetriever2D<Vector3>(cellSize);

            positions = new List<Vector3>();

            Debug.Log("Creating positions " + System.DateTime.Now.ToLongTimeString());
            for (int i = 0; i < totalNrOfObjects; i++)
            {
                float x = Random.Range(-size.x, size.x);
                float y = Random.Range(-size.y, size.y);
                float z = Random.Range(-size.z, size.z);
                Vector3 pos = new Vector3(x, y, z);
                positions.Add(pos);
                //Graphics.DrawMeshInstanced(mesh, 0, mat, matrixes);
            }

            Debug.Log("Adding to retriever " + System.DateTime.Now.ToLongTimeString());
            for (int i = 0; i < totalNrOfObjects; i++)
            {
                Vector2 pos2d = new Vector2(positions[i].x, positions[i].z);
                retriever.Add(positions[i], new Geometry.Rectangle2d(
                    pos2d - new Vector2(1, 1), pos2d + new Vector2(1, 1)));
            }
            Debug.Log("Done adding to retriever: " + System.DateTime.Now.ToLongTimeString());
        }

        private void DrawObject(Vector3 position)
        {
            Matrix4x4 m = Matrix4x4.TRS(position,
                Quaternion.identity, new Vector3(1, 1, 1));
            Graphics.DrawMesh(mesh, m, mat, 0);
        }

        private void Update()
        {
            if (useFlatFastRetriever)
            {
                Vector2 obPos2d = new Vector2(observer.position.x, observer.position.z);
                Vector2 change = new Vector2(1, 1) * maxDistance;
                HashSet<Vector3> positions = retriever.Retrieve(new Geometry.Rectangle2d(
                    obPos2d - change, obPos2d + change));

                foreach (Vector3 p in positions)
                {
                    DrawObject(p);
                }
            }
            else
            {
                int n = positions.Count;
                for (int i = 0; i < n; i++)
                {
                    if (Vector3.Distance(positions[i], observer.position) <= maxDistance)
                    {
                        /*
                        Matrix4x4 m = Matrix4x4.TRS(positions[i],
                            Quaternion.identity, new Vector3(1, 1, 1));
                        Graphics.DrawMesh(mesh, m, mat, 0);
                        */
                        DrawObject(positions[i]);
                    }
                    //Graphics.DrawMeshInstanced(mesh, 0, mat, matrixes[i]);
                }
            }
        }
    }
}
