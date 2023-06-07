using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.MeshToolkit
{
    /** A structure to store some information
     * about an edge in a mesh */
    public struct EdgeInfo
    {
        /** A key for the edge */
        public string key;

        /** index of one of the two points defining the edge */
        public int vertIndex1;

        /** index of the other of the points defining the edge */
        public int vertIndex2;

        /** The vertice for vertIndex1 */
        public Vector3 vertice1;

        /** The vertice for vertIndex2 */
        public Vector3 vertice2;

        /** Number of triangle in the mesh that this edge is part of */
        public int triangleCount;

        /**
         * Construct a new EdgeInfo object
         * The key member will be a semicomma separated string of the 
         * two vertices. The order of the two vertices in the string
         * depends on how they are ordered according to the
         * CompareVector3 method of this structure.
         * @param vertIndex1 Index of one of the two vertices.
         * @param vertIndex2 Index of the other of the two vertices.
         * @param vertice1 The vertice for vertIndex1.
         * @param vertice2 The vertice for vertIndex2.
         * @param triangleCount Number of triangles in the mesh that
         * this edge is part of.
         */
        public EdgeInfo(
            int vertIndex1,
            int vertIndex2,
            Vector3 vertice1,
            Vector3 vertice2,
            int triangleCount)
        {
            this.vertIndex1 = vertIndex1;
            this.vertIndex2 = vertIndex2;
            this.vertice1 = vertice1;
            this.vertice2 = vertice2;
            this.triangleCount = triangleCount;

            int compare = CompareVector3(vertice1, vertice2);
            string first = "";
            string second = "";
            if (compare == 1)
            {
                first = vertice1.ToString();
                second = vertice2.ToString();
            }
            else if (compare == -1)
            {
                first = vertice2.ToString();
                second = vertice1.ToString();
            }
            else
            {
                first = vertice1.ToString();
                second = vertice2.ToString();
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(first);
            sb.Append(";");
            sb.Append(second);
            this.key = sb.ToString();
        }

        /**
         * Compare two vectors
         * @param a The first of the two vectors to be compared.
         * @param b The second of the two vectors to be compared.
         * @return Returns 1 if a should be ordered before b,
         * -1 if b should be ordered before a
         * and 0 if the order does not matter */
        public static int CompareVector3(Vector3 a, Vector3 b)
        {
            if (a.x > b.x)
            {
                return 1;
            }
            else if (a.x < b.x)
            {
                return -1;
            }
            else
            {
                if (a.y > b.y)
                {
                    return 1;
                }
                else if (a.y < b.y)
                {
                    return -1;
                }
                else
                {
                    if (a.z > b.z)
                    {
                        return 1;
                    }
                    else if (a.z < b.z)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
    }
}

