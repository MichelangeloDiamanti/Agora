using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Maps
{
	public class ComputeShaderVisibilityMap : GeneralHeatmap
	{
		private RenderTexture navmeshTexture;
		private PaintGrid navMeshPaintGrid;
		private ComputeShader navMeshComputeShader;
		private ComputeShader visibilityComputeShader;
		private ComputeBuffer navMeshDataBuffer;
		private float[] navmeshValues;
		private ComputeBuffer visibilityValuesDataBuffer;
		private float[] visibilityValuesData;

		private GameObject outlineMapPrefab;
		private GameObject outlineMap;


		protected override void _initialize()
		{

			int resX = (int)(size.x / cellSize);
			int resY = (int)(size.y / cellSize);

			navmeshTexture = new RenderTexture(resX, resY, 1);

			mPaintGrid = new PaintGrid(resX, resY);

			convergenceTimer = 0;
			base.currentTime = 0;

			// Instantiate the cameras used to compute the outline map
			outlineMapPrefab = Resources.Load<GameObject>("Prefabs/OutlineMap");
			outlineMapPrefab.SetActive(false);
			outlineMap = GameObject.Instantiate(outlineMapPrefab);
			outlineMap.transform.parent = gameObject.transform;
			outlineMap.transform.localPosition = Vector3.zero;

			OutlineMap outlineMapScript = outlineMap.GetComponent<OutlineMap>();
			
			// Create and attach the target textures for the cameras
			outlineMapScript.walkable.targetTexture = new RenderTexture(resX, resY, 1);
			outlineMapScript.notWalkable.targetTexture = new RenderTexture(resX, resY, 1);

			// set the size of the camera: since they're orthographic the size
			// is from the center to the top (so half the actual size)
			outlineMapScript.walkable.orthographicSize = resY / 2;
			outlineMapScript.notWalkable.orthographicSize = resY / 2;

			// set the output texture for the outlineMap
			outlineMapScript.result = navmeshTexture;

			outlineMap.SetActive(true);

			navMeshComputeShader = Resources.Load<ComputeShader>("Shaders/Compute/NavMeshComputeShader");
			navmeshValues = new float[resX * resY];
			visibilityValuesData = new float[resX * resY];

			visibilityComputeShader = Resources.Load<ComputeShader>("Shaders/Compute/PixelBasedVisibility");

		}

		protected override void Update()
		{
			base.Update();
			navMeshPaintGrid = NavMeshPaintGridFromTexture(navmeshTexture);
			visibility(navMeshPaintGrid);
		}

		// This method creates a paintGrid from the given texture
		// it assumes that the received texture contains black and white pixels
		// uses a compute shader to go through the texture and set the white pixel as 1.0
		// and the black ones as 0.0 in the paintgrid.
		private PaintGrid NavMeshPaintGridFromTexture(Texture tex)
		{
			int navMeshKernel = navMeshComputeShader.FindKernel("NavMeshComputeShader");

			uint threadGroupSizeX, threadGroupSizeY, threadGroupSizeZ;
			int offsetX, offsetY, offsetZ;
			int groupsX, groupsY, groupsZ;

			navMeshComputeShader.GetKernelThreadGroupSizes(navMeshKernel, out threadGroupSizeX, out threadGroupSizeY, out threadGroupSizeZ);


			if (navMeshKernel < 0)
			{
				Debug.Log("Initialization failed.");
				return null;
			}

			offsetX = (int)threadGroupSizeX - 1;
			offsetY = (int)threadGroupSizeY - 1;
			offsetZ = (int)threadGroupSizeZ - 1;

			groupsX = (tex.width + offsetX) / (int)threadGroupSizeX;
			groupsY = (tex.height + offsetY) / (int)threadGroupSizeY;
			groupsZ = 1;

			navMeshComputeShader.SetTexture(navMeshKernel, "inputHeatmap", tex);
			navMeshDataBuffer = new ComputeBuffer(tex.width * tex.height, sizeof(float));
			navMeshComputeShader.SetBuffer(navMeshKernel, "OutputBuffer", navMeshDataBuffer);

			navMeshComputeShader.Dispatch(navMeshKernel, groupsX, groupsY, groupsZ);

			navMeshDataBuffer.GetData(navmeshValues);

			PaintGrid grid = new PaintGrid(tex.width, tex.height);
			grid.grid = navmeshValues;
			return grid;
		}

		// Employs a compute shader to generate a visibility map from a given paingrid.
		// The input paingrid should represent an OutlineMap which is a black and white
		// representation of the space. Walkable geometry should be white (1,1,1,1) RGBA 
		// while non-walkable should be black (0,0,0,0) RGBA.
		private void visibility(PaintGrid grid)
		{
			int pixelBasedVisibility = visibilityComputeShader.FindKernel("PixelBasedVisibility");

			uint threadGroupSizeX, threadGroupSizeY, threadGroupSizeZ;
			int offsetX, offsetY, offsetZ;
			int groupsX, groupsY, groupsZ;

			visibilityComputeShader.GetKernelThreadGroupSizes(pixelBasedVisibility, out threadGroupSizeX, out threadGroupSizeY, out threadGroupSizeZ);

			if (pixelBasedVisibility < 0)
			{
				Debug.Log("Initialization failed.");
				return;
			}

			offsetX = (int)threadGroupSizeX - 1;
			offsetY = (int)threadGroupSizeY - 1;
			offsetZ = (int)threadGroupSizeZ - 1;

			groupsX = (grid.Width + offsetX) / (int)threadGroupSizeX;
			groupsY = (grid.Height + offsetY) / (int)threadGroupSizeY;
			groupsZ = 1;

			navMeshDataBuffer = new ComputeBuffer(grid.Width * grid.Height, sizeof(float));
			navMeshDataBuffer.SetData(grid.grid);
			visibilityComputeShader.SetBuffer(pixelBasedVisibility, "NavMeshDataBuffer", navMeshDataBuffer);
			visibilityComputeShader.SetInt("NavMeshWidth", grid.Width);
			visibilityComputeShader.SetInt("NavMeshHeight", grid.Height);

			visibilityValuesDataBuffer = new ComputeBuffer(grid.Width * grid.Height, sizeof(float));
			visibilityComputeShader.SetBuffer(pixelBasedVisibility, "VisibilityValuesDataBuffer", visibilityValuesDataBuffer);

			visibilityComputeShader.Dispatch(pixelBasedVisibility, groupsX, groupsY, groupsZ);

			visibilityValuesDataBuffer.GetData(visibilityValuesData);

			this.paintGrid.grid = visibilityValuesData;
			Utilities.Util.Normalize(this.paintGrid, this.paintGrid);

		}

		/** Returns a value of the map at a position */
		public override float GetValueAt(Vector2 position, PathFollowingAgent agent)
		{
			Vector2Int gridpos = WorldToGridPos(position);
			if (!mPaintGrid.IsWithinGrid(gridpos.x, gridpos.y))
			{
				return mPaintGrid.defaultValue;
			}
			float val = mPaintGrid.GetValueAt(gridpos.x, gridpos.y);
			return val;
		}

		protected override void applySettings(UTSettings settings)
		{
			throw new System.NotImplementedException();
		}

		void OnDestroy()
		{
			if (navMeshDataBuffer != null)
				navMeshDataBuffer.Dispose();
			if (visibilityValuesDataBuffer != null)
				visibilityValuesDataBuffer.Dispose();
		}

		/** Only save at runtime using the button */
		protected override void ConfigureSaveBehavior()
		{
			return;
		}
	}
}
