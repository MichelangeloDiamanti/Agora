using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.EditorTools
{
	/** A tool for creating a mesh by extruding the edges
     * of a navmesh in a scene */
	public class NavMeshExtrusionTool : EditorWindow
	{
		/**
         * Opens the editor window for this tool if it is not
         * already open.
         */
		[MenuItem("Window/Urban Territoriality/NavMesh Extrusion Tool")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow<NavMeshExtrusionTool>("Extrusion");
		}

		/** The color of the mesh in the scene */
		private Color meshColor = new Color(0.5f, 0.5f, 0.5f, 1f);

		/** The minimum extrusion height.
         * This is the height of the extrusion
         * above the highest point in the nav mes. */
		private float minExtrusionHeight = 1;

		/** The distance threshold (for determining
         * weather a vertice is on the edge of the nav mesh) to use
         * if useDistanceToNavMeshEdge is set to true. */
		private float navMeshEdgeThreshold = 0.001f;

		/** The distance threshold for merging
         * vertices if mergeVertices is set to true.
         */
		private float verticeMergeThreshold = 0.3f;

		/** Weather or not triangles that are beneath some
         * other triangle will be excluded in the output mesh.
         */
		private bool excludeLowerTriangles = false;

		/** If true then a triangle count method will be used to determine
         * what edges of the nav mesh are outer edges. When using this
         * method an edge is considered an outer edge if it is only
         * part of one triangle in the mesh. */
		private bool useTriangleCount = true;

		/** Use a method from the Unity
         * NavMesh class that gives the distance to the nav mesh edge
         * to determine what vertices are on the outer edge of the nav mesh.
         */
		private bool useDistanceToNavMeshEdge = true;

		/** If this is set to true, then vertices
         * that are within a certain distance of each other will be merged
         * into one vertice.
         */
		private bool mergeVertices = true;

		/** Weather or not to add a collider for the nav mesh. */
		private bool addCollider = true;

		/**
         * A map to base the UV coordinates of the materialized
         * nav mesh on.
         */
		private MapDataScriptableObject heatmap;

		/** Position of scroll view containing the Nav Mesh Extrusion user interface. */
		protected Vector2 scrollPosition;

		/** OnGUI method for the editor window.
         * For drawing and reading from the interface */
		public void OnGUI()
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUIStyle style = new GUIStyle(GUI.skin.box)
			{
				margin = new RectOffset(5, 5, 5, 5),
				padding = new RectOffset(5, 5, 5, 5)
			};
			GUILayout.BeginVertical(style);
			{
				GUILayout.Label("Mesh Settings", EditorStyles.boldLabel);
				excludeLowerTriangles = GUILayout.Toggle(excludeLowerTriangles,
					new GUIContent("Exclude Lower Triangles",
					"Weather or not triangles in the nav mesh that are beneeth some other " +
					"triangles will be excluded."));
				addCollider = GUILayout.Toggle(addCollider, new GUIContent("Add Collider",
					"Add collider to the generated mesh."));
				meshColor = EditorGUILayout.ColorField(new GUIContent("Mesh Color",
					"The color of the mesh to be created."), meshColor);
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical(style);
			{
				GUILayout.Label("Nav Mesh Extrusion Tool", EditorStyles.boldLabel);
				minExtrusionHeight = EditorGUILayout.FloatField(
					new GUIContent("Minimum Extrusion Height (meters)",
					"The extrusion height in meters above the highest point in the nav mesh."), minExtrusionHeight);
				useTriangleCount = GUILayout.Toggle(useTriangleCount, new GUIContent("Use Triangle Count",
					"Use an outer edge finding method that uses the number of triangles each edge in the mesh" +
					"is part of."));
				useDistanceToNavMeshEdge = GUILayout.Toggle(
					useDistanceToNavMeshEdge, new GUIContent("Use Distance To Nav Mesh Edge",
					"Use the distance to nav mesh edge to filter outer edges of the mesh from the inner edges."));
				if (useDistanceToNavMeshEdge)
				{
					navMeshEdgeThreshold = EditorGUILayout.FloatField(
						new GUIContent("Nav Mesh Edge Threshold (meters)",
						"Distance threshold to nav mesh edge in meters when deciding if an edge is an outer" +
						" edge or not."), navMeshEdgeThreshold);
				}
				mergeVertices = GUILayout.Toggle(mergeVertices, new GUIContent("Merge vertices",
					"Merge vertices in the mesh that are within a certain distance from each other."));
				if (mergeVertices)
				{
					verticeMergeThreshold = EditorGUILayout.FloatField(
						new GUIContent("Vertice Merge Threshold (meters)",
						"Distance threshold for merging vertices in meters."), verticeMergeThreshold);
				}
				bool clicked = GUILayout.Button(new GUIContent(
					"Create Extruded Nav Mesh",
					"Creates an extruded nav mesh in the scene."));
				if (clicked)
				{
					Material mat = new Material(Shader.Find("UrbanTerritoriality/DoubleSidedShader"));
					mat.color = meshColor;
					CreateExtrudedNavMesh(minExtrusionHeight, excludeLowerTriangles, useTriangleCount,
						useDistanceToNavMeshEdge, navMeshEdgeThreshold, mergeVertices, verticeMergeThreshold,
						addCollider, mat);
				}
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical(style);
			{
				GUILayout.Label("Materialized Nav Mesh", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox(
					"Create a mesh in the scene based on the nav mesh in the scene. " +
					"If you choose a map, the UV coordinates of the mesh will be based on the size and location " +
					"of the map in the scene. This is useful if the mesh is rendered with a texture. ",
					MessageType.Info);
				heatmap = (MapDataScriptableObject)EditorGUILayout.ObjectField(
					"Map (For UVs)",
					heatmap, typeof(MapDataScriptableObject), true);
				bool clicked = GUILayout.Button(new GUIContent(
					"Create Materialized Nav Mesh",
					"Creates an actual mesh in the scene based on the nav mesh. If " +
					"Exclude Lower Triangles is ticked then lower triangles will be removed, " +
					"else the mesh will be a copy of the actual nav mesh."));
				if (clicked)
				{
					Material mat = new Material(Shader.Find("UrbanTerritoriality/SimpleUnlit"));
					mat.color = meshColor;

					PaintGrid pGrid = new PaintGrid(heatmap.gridSize.x, heatmap.gridSize.y);
					pGrid.grid = heatmap.data;
					Texture2D tex = pGrid.GetAsTexture(heatmap.GetGradient());
					mat.mainTexture = tex;
					mat.color = new Color(1, 1, 1, 0.25f);

					CreateNavMeshMesh(
						excludeLowerTriangles, addCollider, mat, heatmap);
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}

		public static void CreateMeshInScene(
			Vector3[] vertices, int[] indices,
			string name, Material mat,
			bool addCollider,
			bool turnOffLightSettings)
		{
			CreateMeshInScene(
				vertices, indices, null, name, mat,
				addCollider, turnOffLightSettings);
		}

		/** Creates a mesh in the scene.
         * @param vertices The vertices of the mesh
         * @param indices The indices of the triangles of the mesh.
         * @param uv Uv coordinates for all the vertices
         * @param name The name of the GameObject that the mesh
         * will be on.
         * @param mat The material that will be on the mesh
         * in the scene.
         * @param addCollider If true then a mesh collider will
         * be added to the GameObject for the mesh.
         * @param turnOffLightSettings If true then some light
         * settings for the MeshRenderer will be turned off.
         * */
		public static void CreateMeshInScene(
			Vector3[] vertices, int[] indices, Vector2[] uv,
			string name, Material mat,
			bool addCollider,
			bool turnOffLightSettings)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = indices;
			if (uv != null) mesh.uv = uv;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			GameObject meshGO = new GameObject(name);
			MeshFilter mf = meshGO.AddComponent<MeshFilter>();
			mf.sharedMesh = mesh;
			MeshRenderer renderer = meshGO.AddComponent<MeshRenderer>();
			renderer.material = mat;

			if (addCollider)
			{
				meshGO.AddComponent<MeshCollider>();
			}

			if (turnOffLightSettings)
			{
				renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
				renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				renderer.receiveShadows = false;
			}
		}

		/** Creates a new mesh in the scene
         * that is a copy
         * of the mesh for the nav mesh.
         * in the scene.
         * @param excludeLowerTriangles If set to
         * true then triangles that are below some other triangle
         * in the mesh will be removed from the resulting mesh.
         * @param addCollider If set to true then a mesh collider
         * will be added to the created GameObject.
         * @param mat A material to put on the mesh.
         * @param map A map to use for creating UV coordinates
         * for the mesh.
         */
		public static void CreateNavMeshMesh(
			bool excludeLowerTriangles,
			bool addCollider, Material mat,
			MapDataScriptableObject map)
		{
			NavMeshTriangulation navmesh = NavMesh.CalculateTriangulation();
			Vector3[] vertices = navmesh.vertices;
			int[] indices = navmesh.indices;
			Vector3[] newVertices;
			int[] newTriangles;

			if (excludeLowerTriangles)
			{
				List<Vector3> reducedVertices;
				List<int> reducedTriangles;
				MeshToolkit.MeshUtil.RemoveLowerTriangles(
					vertices, indices, out reducedVertices, out reducedTriangles);
				newVertices = reducedVertices.ToArray();
				newTriangles = reducedTriangles.ToArray();
			}
			else
			{
				newVertices = vertices;
				newTriangles = indices;
			}

			Vector2[] uv = null;

			if (map != null)
			{
				Vector2 size = map.gridSize;
				Vector2 center = new Vector2(map.position.x, map.position.z);
				/* Set UVs */
				int n = newVertices.Length;
				uv = new Vector2[n];
				for (int i = 0; i < n; i++)
				{
					float x = 0.5f + newVertices[i].x / size.x - center.x / size.x;
					float y = 0.5f + newVertices[i].z / size.y - center.y / size.y;
					uv[i] = new Vector2(x, y);
				}
			}

			CreateMeshInScene(newVertices, newTriangles, uv,
				"MappingMesh", mat, addCollider, true);
		}

		/** Creates a mesh in the scene
         * by extruding the sides of the nav mesh in 
         * the scene
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
         * @param addCollider If set to true then a mesh collider will be added
         * to the mesh.
         * @param mat The material to use for the mesh.
         */
		public static void CreateExtrudedNavMesh(
			float minExtrusionHeight,
			bool excludeLowerTriangles,
			bool useTriangleCount,
			bool useDistanceToNavMeshEdge,
			float navMeshEdgeThreshold,
			bool mergeVertices,
			float verticeMergeThreshold,
			bool addCollider,
			Material mat)
		{
			NavMeshTriangulation navmesh = NavMesh.CalculateTriangulation();
			Vector3[] vertices = navmesh.vertices;
			int[] indices = navmesh.indices;
			Vector3[] newVertices;
			int[] newTriangles;

			MeshToolkit.MeshUtil.CreateVerticallyExtrudedNavMesh(vertices, indices,
				out newVertices, out newTriangles,
				minExtrusionHeight, excludeLowerTriangles, useTriangleCount,
				useDistanceToNavMeshEdge, navMeshEdgeThreshold,
				mergeVertices, verticeMergeThreshold);

			CreateMeshInScene(newVertices, newTriangles, "ExtrudedNavMesh",
				mat, addCollider, true);
		}
	}
}

