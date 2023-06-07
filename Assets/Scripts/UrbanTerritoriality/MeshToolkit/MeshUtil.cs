using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UrbanTerritoriality.Geometry;

namespace UrbanTerritoriality.MeshToolkit
{
    /** A static class containing some methods
     * related to mesh generation.
     */
    public static class MeshUtil
    {
        /**
         * Create a sector mesh.
         * A sector is a part of a circle/disk.
         * @param radius The radius of the sector
         * @param angle The angle of the sector
         * @param divisions Number of triangles in the mesh.
         * @return Returns the sector mesh.
         */
        public static Mesh CreateSectorMesh(float radius, float angle, int divisions)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[divisions + 2];
            int[] triangles = new int[divisions * 3];

            float angleRad = Utilities.Util.DegToRadian(angle);
            float halfAngle = angleRad / 2f;

            float currentAngle = -halfAngle;
            float angleStepRad = Utilities.Util.DegToRadian(angle / divisions);
            Vector3 centerPoint = new Vector3(0, 0, 0);
            Vector3 currentStartPoint =
                new Vector3(Mathf.Sin(currentAngle),
                0, Mathf.Cos(currentAngle)) * radius;
            vertices[0] = centerPoint;
            vertices[1] = currentStartPoint;
            for (int i = 0; i < divisions; i++)
            {
                currentAngle = currentAngle + angleStepRad;
                Vector3 currentEndPoint =
                    new Vector3(Mathf.Sin(currentAngle),
                    0, Mathf.Cos(currentAngle)) * radius;
                vertices[i + 2] = currentEndPoint;
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
                currentStartPoint = currentEndPoint;
            }
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return mesh;
        }

        /** Get the outer edges of a mesh. That is edges that are only part of one
         * triangle.
         * @param vertices The vertices of the mesh
         * @param triangles Indices of the triangles in the mesh
         * @return A list of EdgeInfo objects, each containing information about
         * a single edge in the  mesh.
         */
        public static List<MeshToolkit.EdgeInfo> GetOuterEdgesOfMesh(Vector3[] vertices, int[] triangles)
        {
            Dictionary<string, MeshToolkit.EdgeInfo> edgeInfo = GetEdgeInfo(vertices, triangles);
            List<EdgeInfo> ei = new List<EdgeInfo>();
            foreach (string key in edgeInfo.Keys)
            {
                ei.Add(edgeInfo[key]);
            }
            return GetOuterEdgesOfMesh(ei);
        }

        public static List<EdgeInfo> GetOuterEdgesOfMesh(List<EdgeInfo> edgeInfo)
        {
            List<MeshToolkit.EdgeInfo> outerEdges = new List<MeshToolkit.EdgeInfo>();

            /* Find the edges that are only part of one triangle */
            foreach (EdgeInfo ei in edgeInfo)
            {
                if (ei.triangleCount == 1)
                {
                    outerEdges.Add(ei);
                }
            }
            return outerEdges;
        }

        /** A version of FilterOuterInnerEdgesOfNavMesh with fewer parameters */
        public static List<EdgeInfo>
            FilterOuterInnerEdgesOfNavMesh(bool returnOuter, List<EdgeInfo> edges)
        {
            return FilterOuterInnerEdgesOfNavMesh(returnOuter, edges, 0.001f);
        }

        /** Filter outer edges of a nav mesh from the inner ones or vice versa.
         * @param returnOuter If true the method will return the outer edges of the mesh
         * if this is false the method will return the inner edges instead.
         * @param edges The input edges.
         * @param distanceThreshold Distance threshold to use when checking how far an edge
         * is from the boundary of the nav mesh. This is used to check weather an edge is
         * an outer edge or an inner edge. The value represents distance in meters in
         * a Unity scene. Set this to some small value such as 0.001.
         */
        public static List<EdgeInfo>
            FilterOuterInnerEdgesOfNavMesh(
            bool returnOuter,
            List<EdgeInfo> edges,
            float distanceThreshold)
        {
            List<EdgeInfo> outputEdges = new List<EdgeInfo>();
            int n = edges.Count;
            for (int i = 0; i < n; i++)
            {
                Vector3 edgeCenter = (edges[i].vertice1 + edges[i].vertice2) / 2f;
                NavMeshHit hit;
                if (NavMesh.FindClosestEdge(edgeCenter, out hit, NavMesh.AllAreas))
                {
                    float dist = hit.distance;
                    if (returnOuter ? dist <= distanceThreshold : dist > distanceThreshold)
                    {
                        outputEdges.Add(edges[i]);
                    }
                }
            }
            return outputEdges;
        }

        /** Merge vertices in a collection of edges. Vertices
         * that are within a certain distance from each other
         * will be merged into one vertice.
         * @param edges A list of EdgeInfo objects containing information
         * about edges in a mesh.
         * @param minDistance The minimum distance that can be between 
         * vertices. If the distance is less they will be merged.
         * @return A list of EdgeInfo objects representing edges.
         * The vertices in these edges will have been merged if
         * they are within a certain distance of each other.
         */
        public static List<EdgeInfo> MergeVertices(List<EdgeInfo> edges, float minDistance)
        {
            int n = edges.Count;
            int n2 = n * 2;
            List<EdgeInfo> outputEdges = new List<EdgeInfo>();
            Vector3Info[] vInfo = new Vector3Info[n2];
            for (int i = 0; i < n; i++)
            {
                int i2 = 2 * i;
                int i2_1 = i2 + 1;
                vInfo[i2] = new Vector3Info(i2, edges[i].vertice1);
                vInfo[i2_1] = new Vector3Info(i2_1, edges[i].vertice2);
            }

            while (true)
            {
                GridRetriever2D<Vector3Info> retriever = GetGridRetriever2DForVector3Info(vInfo);
                bool pointsFixed = false;
                for (int i = 0; i < n2; i++)
                {
                    Vector3 v = vInfo[i].value;
                    Vector2 v2d = new Vector2(vInfo[i].value.x, vInfo[i].value.z);
                    Vector2 bRad = minDistance * 2 * Vector2.one;
                    Rectangle2d bounds = new Rectangle2d(v2d - bRad, v2d + bRad);
                    HashSet<Vector3Info> nearByPoints = retriever.Retrieve(bounds);
                    bool needsFixing = false;
                    Vector3 center = Vector3.zero;
                    int withinDistCount = 0;
                    List<int> pointsToFix = new List<int>();
                    foreach (Vector3Info p in nearByPoints)
                    {
                        if (Vector3.Distance(v, vInfo[p.id].value) < minDistance)
                        {
                            center += vInfo[p.id].value;
                            withinDistCount++;
                            pointsToFix.Add(p.id);
                            if (v != vInfo[p.id].value)
                            {
                                needsFixing = true;
                                pointsFixed = true;
                            }
                        }
                    }
                    center = center / (float)withinDistCount;
                    if (needsFixing)
                    {
                        int m = pointsToFix.Count;
                        for (int j = 0; j < m; j++)
                        {
                            vInfo[pointsToFix[j]].value = center;
                        }
                    }
                }
                if (!pointsFixed) break;
            }

            for (int i = 0; i < n; i++)
            {
                int i2 = 2 * i;
                int i2_1 = i2 + 1;
                EdgeInfo ei = edges[i];
                ei.vertice1 = vInfo[i2].value;
                ei.vertice2 = vInfo[i2_1].value;
                outputEdges.Add(ei);
            }
            return outputEdges;
        }

        /**
         * Create a dictionary of EdgeInfo objects from mesh data.
         * @param vertices The vertices in the meshCreate a dictionary of EdgeInfo objects from mesh data.
         * @param vertices The vertices in the mesh
         * @param triangles The indices of the triangles in the mesh.
         * @return Returns a Dictionary of EdgeInfo objects, each with information of each edge
         * in the mesh.
         */
        public static Dictionary<string, MeshToolkit.EdgeInfo> GetEdgeInfo(Vector3[] vertices, int[] triangles)
        {
            Dictionary<string, MeshToolkit.EdgeInfo> edgeInfo = new Dictionary<string, MeshToolkit.EdgeInfo>();

            int n = triangles.Length;
            for (int i = 0; i < n; i += 3)
            {
                int vert1 = triangles[i];
                int vert2 = triangles[i + 1];
                int vert3 = triangles[i + 2];
                MeshToolkit.EdgeInfo[] edges = new MeshToolkit.EdgeInfo[3];
                int useCount = 1;
                edges[0] = new MeshToolkit.EdgeInfo(vert1, vert2,
                    vertices[vert1], vertices[vert2], useCount);
                edges[1] = new MeshToolkit.EdgeInfo(vert2, vert3,
                    vertices[vert2], vertices[vert3], useCount);
                edges[2] = new MeshToolkit.EdgeInfo(vert3, vert1,
                    vertices[vert3], vertices[vert1], useCount);
                for (int j = 0; j < 3; j++)
                {
                    string key = edges[j].key;
                    if (edgeInfo.ContainsKey(key))
                    {
                        MeshToolkit.EdgeInfo ei = edgeInfo[key];
                        ei.triangleCount += 1;
                        edgeInfo[key] = ei;
                    }
                    else
                    {
                        edgeInfo[key] = edges[j];
                    }
                }
            }

            return edgeInfo;
        }

        /** Create a mesh by vertically extruding a list of edges.
         * @param edgeInfo A list of EdgeInfo objects, each
         * containing information about an edge. These edges
         * will become the starting edges for the extruded mesh.
         * @param outVertices The vertices of the new mesh will
         * be put into this list.
         * @param outTriangles The indices of the triangles of the
         * new mesh will be put into this list.
         * @param extrusionY The height (y value) of the top of the
         * extruded mesh.
         */
        public static void ExtrudeEdges(
            List<MeshToolkit.EdgeInfo> edgeInfo,
            ref List<Vector3> outVertices,
            ref List<int> outTriangles,
            float extrusionY)
        {
            Dictionary<Vector3, int> vertIndices = new Dictionary<Vector3, int>();
            int currentIndex = 0;
            foreach (MeshToolkit.EdgeInfo eInfo in edgeInfo)
            {
                Vector3 v1 = eInfo.vertice1;
                Vector3 v2 = eInfo.vertice2;
                Vector3[] vert = new Vector3[]
                {
                    v1,
                    new Vector3(v1.x, extrusionY, v1.z),
                    v2,
                    v2,
                    new Vector3(v1.x, extrusionY, v1.z),
                    new Vector3(v2.x, extrusionY, v2.z)
                };

                int n = vert.Length;
                for (int i = 0; i < n; i++)
                {
                    int index = 0;
                    if (vertIndices.ContainsKey(vert[i]))
                    {
                        index = vertIndices[vert[i]];
                    }
                    else
                    {
                        outVertices.Add(vert[i]);
                        index = currentIndex;
                        vertIndices[vert[i]] = index;
                        currentIndex++;
                    }
                    outTriangles.Add(index);
                }
            }
        }

        /**
         * Get the 2D bounding rectangle of a collection of points
         * in 3D space. The 2D rectangle is in the xz plane of
         * the 3D space, perpendicular to the y axis.
         * @param points An array of points.
         * @return A Rectangle2d object representing the bounding
         * rectangle of the points.
         */
        public static Rectangle2d GetPointBoundingRectangle(Vector3[] points)
        {
            float fmax = float.MaxValue;
            float fmin = float.MinValue;
            float maxX = fmin;
            float minX = fmax;
            float maxY = fmin;
            float minY = fmax;
            int n = points.Length;
            for (int i = 0; i < n; i++)
            {
                Vector3 v = points[i];
                if (v.x > maxX) maxX = v.x;
                if (v.x < minX) minX = v.x;
                if (v.z > maxY) maxY = v.z;
                if (v.z < minY) minY = v.z;
            }
            return new Rectangle2d(new Vector2(minX, minY), new Vector2(maxX, maxY));
        }

        /** Get a 2D bounding rectangle of a 3D mesh
         * The rectangle is in the horizontal plane (perpendicular to the y axis
         * in a unity scene).
         * @param vertices The vertices of the mesh.
         * @param triangles The indices of the triangles of the mesh.
         * @return Returns a 2D rectangle that defines the boundaries of the mesh
         * in the horizontal plane.
         */
        public static Rectangle2d GetMeshBoundingRectangle(Vector3[] vertices, int[] triangles)
        {
            float fmax = float.MaxValue;
            float fmin = float.MinValue;
            float maxX = fmin;
            float minX = fmax;
            float maxY = fmin;
            float minY = fmax;
            int n = triangles.Length;
            for (int i = 0; i < n; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3 v = vertices[triangles[i + j]];
                    if (v.x > maxX) maxX = v.x;
                    if (v.x < minX) minX = v.x;
                    if (v.z > maxY) maxY = v.z;
                    if (v.z < minY) minY = v.z;
                }
            }
            return new Rectangle2d(new Vector2(minX, minY), new Vector2(maxX, maxY));
        }

        /** Get a 3D bounding box of a 3D mesh
         * @param vertices The vertices of the mesh.
         * @param triangles The indices of the triangles of the mesh.
         * @return Returns the bounding box of the mesh.
         */
        public static Box3d GetMeshBoundingBox(Vector3[] vertices, int[] triangles)
        {
            float fmax = float.MaxValue;
            float fmin = float.MinValue;
            float maxX = fmin;
            float minX = fmax;
            float maxY = fmin;
            float minY = fmax;
            float maxZ = fmin;
            float minZ = fmax;
            int n = triangles.Length;
            for (int i = 0; i < n; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3 v = vertices[triangles[i + j]];
                    if (v.x > maxX) maxX = v.x;
                    if (v.x < minX) minX = v.x;
                    if (v.y > maxY) maxY = v.y;
                    if (v.y < minY) minY = v.y;
                    if (v.z > maxZ) maxZ = v.z;
                    if (v.z < minZ) minZ = v.z;
                }
            }
            return new Box3d(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        }

        /**
         * Get a 2D grid retriever that can be used
         * to retrieve triangles of a 3D mesh.
         * @param vertices The vertices of the mesh.
         * @param triangles The indices of the triangles of the mesh.
         * @return Returns an object of type FlatFastRetriever<Triangle3d>
         * that can be used to retrieve triangles given a 2 dimensional
         * boundary in the horizontal plane.
         */
        public static GridRetriever2D<Triangle3d>
            GetGridRetriever2DForTriangles(
            Vector3[] vertices, int[] triangles)
        {
            Rectangle2d meshBounds = GetMeshBoundingRectangle(vertices, triangles);
            float cellSize =
                GridRetriever2D<Triangle3d>.CalculateCellSize(
                    meshBounds.maxCorner.x - meshBounds.minCorner.x,
                    meshBounds.maxCorner.y - meshBounds.minCorner.y, 1, (float)triangles.Length / 3f);
            GridRetriever2D<Triangle3d> retriever = new GridRetriever2D<Triangle3d>(cellSize);

            int n = triangles.Length;
            for (int i = 0; i < n; i += 3)
            {
                Triangle3d t = new Triangle3d(
                    vertices[triangles[i]],
                    vertices[triangles[i + 1]],
                    vertices[triangles[i + 2]]);

                Rectangle2d triangleBounds = GeometryUtil.GetBoundingRectangle(t);
                Vector2 increase = new Vector2(1, 1) * 0.01f;
                triangleBounds.maxCorner += increase;
                triangleBounds.minCorner -= increase;

                retriever.Add(t, triangleBounds);
            }
            return retriever;
        }

        /** Get a 2D grid retriever that can
         * be used to retrieve 3D points with ids
         * in a 3D world.
         * @param vInfo An array of objects
         * containing a 3D vector along with
         * an id.
         * @returns A GridRetriever2D object
         * that can be used to retrieve
         * Vector3Info objects from a 3D world
         */
        public static GridRetriever2D<Vector3Info>
            GetGridRetriever2DForVector3Info(
            Vector3Info[] vInfo)
        {
            int n = vInfo.Length;
            Vector3[] vArr = new Vector3[n];
            for (int i = 0; i < n; i++)
            {
                vArr[i] = vInfo[i].value;
            }
            Rectangle2d bounds = GetPointBoundingRectangle(vArr);
            float cellSize = GridRetriever2D<Vector3Info>.CalculateCellSize(
                bounds.maxCorner.x - bounds.minCorner.x,
                bounds.maxCorner.y - bounds.minCorner.y, 1, (float)n);
            GridRetriever2D<Vector3Info> retriever = new GridRetriever2D<Vector3Info>(cellSize);
            for (int i = 0; i < n; i++)
            {
                Vector2 boundsRadius = 0.01f * Vector3.one;
                Vector2 v = new Vector2(vArr[i].x, vArr[i].z);
                Rectangle2d vBounds = new Rectangle2d(v - boundsRadius, v + boundsRadius);
                retriever.Add(vInfo[i], vBounds);
            }
            return retriever;
        }

        /**
         * Gather the upper triangles in a mesh.
         * Gathers triangles that are not beneeth any other triangle
         * in a mesh.
         * @param inVertices The vertices of the mesh.
         * @param inTriangles The indices of the vertices of the triangles of the mesh.
         * @return Returns all triangles that are not beneath some other triangle
         * in the mesh.
         */
        public static List<Triangle3d> GatherUpperTriangles(
            Vector3[] inVertices,
            int[] inTriangles)
        {

            GridRetriever2D<Triangle3d> retriever =
                GetGridRetriever2DForTriangles(inVertices, inTriangles);
            List<Triangle3d> trianglesToKeep = new List<Triangle3d>();

            int n = inTriangles.Length;
            for (int i = 0; i < n; i += 3)
            {
                bool keepTriangle = true;
                Triangle3d mainTri = new Triangle3d(
                    inVertices[inTriangles[i]],
                    inVertices[inTriangles[i + 1]],
                    inVertices[inTriangles[i + 2]]);

                Rectangle2d triBounds = GeometryUtil.GetBoundingRectangle(mainTri);
                Vector2 increase = new Vector2(1, 1) * 0.1f;
                triBounds.maxCorner += increase;
                triBounds.minCorner -= increase;
                HashSet<Triangle3d> triangles = retriever.Retrieve(triBounds);
                foreach (Triangle3d otherTri in triangles)
                {
                    if (mainTri.p1 == otherTri.p1 &&
                        mainTri.p2 == otherTri.p2 &&
                        mainTri.p3 == otherTri.p3) continue;
                     if (!GeometryUtil.AreSharingVertices(mainTri, otherTri) &&
                        !GeometryUtil.IsTriangleAboveAtEveryVerticalIntersection(
                        mainTri, otherTri))
                    {
                        keepTriangle = false;
                        break;
                    }
                }
                if (keepTriangle)
                {
                    trianglesToKeep.Add(mainTri);
                }
            }

            return trianglesToKeep;
        }

        /**
         * Remove all triangles that are below some other triangle
         * in a mesh.
         * @param inVertices The vertices of the input mesh.
         * @param inTriangles The indices of the triangles of the input mesh.
         * @param outVertices The vertices of the new mesh will be put into this list.
         * @param outTriangles The indices of the triangles in the new mesh will
         * be put into this list.
         */
        public static void RemoveLowerTriangles(
            Vector3[] inVertices,
            int[] inTriangles,
            out List<Vector3> outVertices,
            out List<int> outTriangles)
        {
            List<Triangle3d> trianglesToKeep = GatherUpperTriangles(inVertices, inTriangles);

            outVertices = new List<Vector3>();
            int outVerticeCount = 0;
            outTriangles = new List<int>();
            Dictionary<Vector3, int> indexDict = new Dictionary<Vector3, int>();
            int n = trianglesToKeep.Count;
            for (int i = 0; i < n; i++)
            {
                Vector3[] vertices = new Vector3[]
                {
                    trianglesToKeep[i].p1,
                    trianglesToKeep[i].p2,
                    trianglesToKeep[i].p3
                };
                for (int j = 0; j < 3; j++)
                {
                    if (!indexDict.ContainsKey(vertices[j]))
                    {
                        outVertices.Add(vertices[j]);
                        indexDict[vertices[j]] = outVerticeCount;
                        outVerticeCount++;
                    }
                    outTriangles.Add(indexDict[vertices[j]]);
                }
            }
        }

        /** Creates a vertically extruded mesh based on the nav mesh
         * in a Unity scene
         * @param inVertices The vertices of the nav mesh
         * @param inTriangles The indices of the triangles of the nav mesh
         * @param outVertices The vertices in the generated mesh
         * @param outTriangles The triangles in the generated mesh
         * @param minExtrusionHeight The minimum extrusion height.
         * This is the height of the extrusion above the highest point in the nav mes.
         * @param excludeLowerTriangles Weather or not triangles that are beneath some
         * other triangle will be excluded in the output mesh.
         * @param useTriangleCount Use triangle count method to determine
         * what edges of the nav mesh are outer edges. When using this
         * method an edge is considered an outer edge if it is only
         * part of one triangle in the mesh.
         * @param useDistanceToNavMeshEdge Use a method from the Unity
         * NavMesh class that gives the distance to the nav mesh edge
         * to determine what vertices are on the outer edge of the nav mesh.
         * @param navMeshEdgeThreshold The distance threshold (for determining
         * weather a vertice is on the edge of the nav mesh) to use
         * if useDistanceToNavMeshEdge is set to true.
         * @param mergeVertices If this is set to true, then vertices
         * that are within a certain distance of each other will be merged
         * into one vertice.
         * @param verticeMergeThreshold The distance threshold for merging
         * vertices if mergeVertices is set to true.
         */
        public static void CreateVerticallyExtrudedNavMesh(
            Vector3[] inVertices,
            int[] inTriangles,
            out Vector3[] outVertices,
            out int[] outTriangles,
            float minExtrusionHeight,
            bool excludeLowerTriangles,
            bool useTriangleCount,
            bool useDistanceToNavMeshEdge,
            float navMeshEdgeThreshold,
            bool mergeVertices,
            float verticeMergeThreshold)
        {
            Geometry.Box3d bounds = MeshToolkit.MeshUtil.GetMeshBoundingBox(inVertices, inTriangles);
            float extrusionY = bounds.maxCorner.y + minExtrusionHeight;

            Vector3[] verticesToUse;
            int[] trianglesToUse;
            if (excludeLowerTriangles)
            {
                List<Vector3> reducedVertices;
                List<int> reducedTriangles;
                MeshToolkit.MeshUtil.RemoveLowerTriangles(
                    inVertices, inTriangles, out reducedVertices, out reducedTriangles);
                verticesToUse = reducedVertices.ToArray();
                trianglesToUse = reducedTriangles.ToArray();
            }
            else
            {
                verticesToUse = inVertices;
                trianglesToUse = inTriangles;
            }

            Dictionary<string, EdgeInfo> edgeInfoDict = GetEdgeInfo(verticesToUse, trianglesToUse);
            List<EdgeInfo> edgeInfo = new List<EdgeInfo>();
            foreach(string key in edgeInfoDict.Keys)
            {
                edgeInfo.Add(edgeInfoDict[key]);
            }

            if (useTriangleCount)
            {
                edgeInfo = GetOuterEdgesOfMesh(edgeInfo);
            }

            if (useDistanceToNavMeshEdge)
            {
                edgeInfo = FilterOuterInnerEdgesOfNavMesh(true, edgeInfo, navMeshEdgeThreshold);
            }

            if (mergeVertices)
            {
                edgeInfo = MergeVertices(edgeInfo, verticeMergeThreshold);
            }

            //TODO remove edges of length zero

            List<Vector3> newVertices = new List<Vector3>();
            List<int> newTriangles = new List<int>();
            MeshToolkit.MeshUtil.ExtrudeEdges(edgeInfo, ref newVertices, ref newTriangles, extrusionY);
            outVertices = newVertices.ToArray();
            outTriangles = newTriangles.ToArray();
        }
    }
}
