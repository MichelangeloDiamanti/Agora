using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UrbanTerritoriality.Maps
{
	/** A viewer that can be used to view a map
     * that inherits from GeneralHeatmap.
     *  */
	public class GeneralHeatmapViewer : MonoBehaviour
	{
		const int textureGradientResolution = 4096;
		[HideInInspector]
		public bool useComputeShader = true;
		private Texture2D textureGradient;
		private RenderTexture outputHeatmap;
		private ComputeShader computeShader;

		private bool _usingGUI;
		private GameObject _HeatMapContainer;
		private GameObject _GUIRenderTarget;
		private GameObject _GeneralHeatmapTitle;


		/** Weather or not to use a custom title that
         * will be shown on top of the map.
         */
		public bool useCustomTitle = false;

		/** A custom title to show */
		public string customTitle = "Map Viewer";

		/** Weather the viewer will be at the top of the screen
         * or at the bottom of the screen. If true, it will be
         * at the top of the screen, else it will be at the
         * bottom of the screen.
         */
		public bool ViewerPositionUp = true;

		/**
         * Weather the viewer will be positioned to the left
         * or to the right of the screen.
         * If true, it will be to the left, else it
         * will be to the right.
         */
		public bool ViewerPositionLeft = true;

		/** The space between the viewer and the nearest
         * edge of the screen.
         */
		public Vector2Int margin = new Vector2Int(10, 10);

		/** The map that thiw viewer shows. */
		public GeneralHeatmap map;

		/** Color of the map in the viewer */
		public Color mapColor = new Color(0, 1, 0, 1);

		/** Weather or not to use a gradient instead of
         * a color for the map. */
		public bool useGradient = false;

		/** The gradient to use for displaying the map
         * if a gradient is to be used.
         * */
		public Gradient mapColorGradient;

		private int _oldGradientHashCode;


		/** A size multiplier for calculating
         * the final size of the texture in the
         * map viewer */
		[Range(0, 10)]
		public float sizeMultiplier = 1f;

		/** Weather or not to the map as a texture to disk. */
		[HideInInspector]
		public bool saveTexture = false;

		/** The path for saving the map as a texture */
		[HideInInspector]
		public string savePath = "Assets/texture.png";

		/** The time after starting in play mode
         * when the map will be saved */
		[HideInInspector]
		public float saveTime = 5f;

		/** Weather or not the map has been saved. */
		public bool HasBeenSaved { get { return hasBeenSaved; } }
		protected bool hasBeenSaved = false;

		/** The path were the map was saved if it has been saved.
         * If it has not been saved then this should be null. */
		public string SavedPath { get { return savedPath; } }
		protected string savedPath = null;

		/** The texture the map will be drawn on. */
		private Texture2D tex = null;

		/** Weather or not the map has been initialized */
		public bool Initialized { get { return initialized; } }

		public RenderTexture OutputHeatmap
		{
			get
			{
				return outputHeatmap;
			}

			set
			{
				outputHeatmap = value;
			}
		}

		protected bool initialized = false;

		/** A special box for the label */
		protected GUInterface.GUIBox labelBox;

		/** The color of the label box when it will not be saved. */
		protected Color colorWontBeSaved = new Color(0, 0, 0.3f, 0.7f);

		/** The color of the label box when it is to be saved and has
         * not been saved */
		protected Color colorUnsaved = new Color(0.3f, 0, 0, 0.7f);

		/** The color of the label box after it has been saved */
		protected Color colorSaved = new Color(0, 0.3f, 0, 0.7f);

		/** Perform some initialization tasks */
		public virtual bool Initialize()
		{
			if (map.Initialized)
			{
				labelBox = new GUInterface.GUIBox();
				labelBox.AddColorIfMissing(colorWontBeSaved);
				labelBox.AddColorIfMissing(colorUnsaved);
				labelBox.AddColorIfMissing(colorSaved);

				// useComputeShader = false;

				if (useComputeShader)
				{   // Load the compute shader from resources
					computeShader = Resources.Load<ComputeShader>("Shaders/Compute/GeneralHeatMapVisualizer");

					if (computeShader == null)
						Debug.LogError("computeShader not found in the specified path");
					else
					{
						outputHeatmap = new RenderTexture(map.paintGrid.Width, map.paintGrid.Height, 0);
						outputHeatmap.enableRandomWrite = true;
						outputHeatmap.Create();

						if (mapColorGradient != null)
						{
							// Convert the gradient into a texture that can be used by the compute shader
							textureGradient = gradientToTexture(mapColorGradient, textureGradientResolution);
							_oldGradientHashCode = Utilities.Util.GradientHashCode(mapColorGradient);
						}
						else
							Debug.LogError("Missing Gradient");
					}
				}
				else
					tex = new Texture2D(map.paintGrid.Width, map.paintGrid.Height);

				if (GameObject.Find("GUICanvas") != null)
				{
					_usingGUI = true;
					_HeatMapContainer = GameObject.Find("HeatMapContainer");
					_GUIRenderTarget = GameObject.Find("GeneralHeatmapDisplay");
					_GeneralHeatmapTitle = GameObject.Find("GeneralHeatmapDisplayTitle");
				}
				else
					_usingGUI = false;
				return true;
			}
			return false;
		}

		private Texture2D gradientToTexture(Gradient gradient, int texResolution)
		{
			Texture2D tex = new Texture2D(texResolution, 1);
			for (int i = 0; i < texResolution; i++)
				tex.SetPixel(i, 0, gradient.Evaluate((float)i / texResolution));
			tex.Apply();
			return tex;
		}

		/** Unity Update method */
		void Update()
		{
			if (!initialized)
			{
				initialized = Initialize();
			}
			else
			{
				if (useComputeShader == false || computeShader == null)
				{
					UpdateTexture();
				}
				else
				{
					UpdateTextureWithComputeShader();
				}

				if (saveTexture && !hasBeenSaved && Time.time > saveTime)
				{
					string path =
						EditorTools.FileUtil.ConvertAssetPathToDiskPath(savePath);
					byte[] bytes = tex.EncodeToPNG();
					System.IO.File.WriteAllBytes(path, bytes);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					savedPath = path;
					hasBeenSaved = true;
				}
			}
		}

		/** Updates the map texture. */
		private void UpdateTexture()
		{
			Color[] cols = new Color[tex.width * tex.height];

			int n = map.paintGrid.Width * map.paintGrid.Height;
			for (int i = 0; i < n; i++)
			{
				cols[i] = mapColor * map.paintGrid.grid[i];
				cols[i].a = 1;
				if (useGradient)
				{
					cols[i] = mapColorGradient.Evaluate(map.paintGrid.grid[i]);
				}
			}
			tex.SetPixels(cols);
			tex.Apply();
		}

		/** Updates the map texture. */
		private int UpdateTextureWithComputeShader()
		{
			if (null == computeShader || null == outputHeatmap)
			{
				Debug.Log("Shader or input texture missing.");
				return -1;
			}

			int offset = 63;
			int updateTextureWithMapValuesMain = computeShader.FindKernel("UpdateTextureWithMapValuesGradient");
			ComputeBuffer mapValuesDataBuffer = new ComputeBuffer(map.paintGrid.Width * map.paintGrid.Height, sizeof(float));
			mapValuesDataBuffer.SetData(map.paintGrid.grid);

			if (updateTextureWithMapValuesMain < 0 || null == mapValuesDataBuffer)
			{
				Debug.Log("Initialization failed.");
				return -1;
			}

			computeShader.SetTexture(updateTextureWithMapValuesMain, "InputTexture", textureGradient);
			computeShader.SetInt("InputTextureResolution", textureGradientResolution);

			computeShader.SetTexture(updateTextureWithMapValuesMain, "OutputTexture", outputHeatmap);
			computeShader.SetInt("OutputTextureWidth", map.paintGrid.Width);
			computeShader.SetInt("OutputTextureHeight", map.paintGrid.Height);

			computeShader.SetBuffer(updateTextureWithMapValuesMain, "MapValuesDataBuffer", mapValuesDataBuffer);

			computeShader.Dispatch(updateTextureWithMapValuesMain, (map.paintGrid.Height + offset) / 64, 1, 1);
			// divided by 64 in x because of [numthreads(64,1,1)] in the compute shader code
			// added 63 to make sure that there is a group for all rows

			mapValuesDataBuffer.Dispose();

			return 0;
		}

		/** Unity OnGUI method */
		void OnGUI()
		{
			if (!initialized) return;

			// if the gradient has changed we have to update the texture
			if (Utilities.Util.GradientHashCode(mapColorGradient) != _oldGradientHashCode)
			{
				textureGradient = gradientToTexture(mapColorGradient, textureGradientResolution);
				_oldGradientHashCode = Utilities.Util.GradientHashCode(mapColorGradient);
			}

			Vector2Int currentSize = new Vector2Int(
				(int)(sizeMultiplier * (float)map.size.x / map.cellSize),
				(int)(sizeMultiplier * (float)map.size.y / map.cellSize));

			float titleHeight = 25;

			float xMin = ViewerPositionLeft ? margin.x :
				Screen.width - (int)currentSize.x - margin.x;
			float yMin = ViewerPositionUp ? margin.y + titleHeight :
				Screen.height - (int)currentSize.y - margin.y;
			Rect texRect = new Rect(xMin, yMin, (int)currentSize.x, (int)currentSize.y);

			// If there's no GUI in the scene, draw directly onto the screen
			if (_usingGUI == false)
			{
				if (useComputeShader == false || computeShader == null)
				{
					GUI.DrawTexture(texRect, tex);
				}
				else
				{
					GUI.DrawTexture(texRect, outputHeatmap);
				}

				Rect labelRect = new Rect(texRect.xMin, texRect.yMin - titleHeight, texRect.width, titleHeight);
				GUIContent labelContent =
					new GUIContent(
						useCustomTitle ? customTitle : map.gameObject.name);
				labelBox.color = (map.saveAsset || map.saveTexture) ?
					colorUnsaved : colorWontBeSaved;
				if (map.HasMapBeenSaved || map.HasTextureBeenSaved)
				{
					labelBox.color = colorSaved;
					labelContent.text += " (Saved)";
				}
				labelBox.content = labelContent;
				labelBox.position = labelRect;
				labelBox.OnGUI();
			}
			else
			{
				if (useComputeShader == false || computeShader == null)
				{
					_HeatMapContainer.GetComponent<RectTransform>().sizeDelta = currentSize;
					_GUIRenderTarget.GetComponent<RectTransform>().sizeDelta = currentSize;
					_GUIRenderTarget.GetComponent<RawImage>().texture = tex;
				}
				else
				{
					_HeatMapContainer.GetComponent<RectTransform>().sizeDelta = currentSize;
					_GUIRenderTarget.GetComponent<RectTransform>().sizeDelta = currentSize;
					_GUIRenderTarget.GetComponent<RawImage>().texture = outputHeatmap;
				}

				_GeneralHeatmapTitle.GetComponent<Image>().color = (map.saveAsset || map.saveTexture) ?
					colorUnsaved : colorWontBeSaved;
				RectTransform textBoxSize = _GeneralHeatmapTitle.GetComponentInChildren<Text>().rectTransform;
				_GeneralHeatmapTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(currentSize.x, textBoxSize.sizeDelta.y);
				textBoxSize.sizeDelta = new Vector2(currentSize.x, textBoxSize.sizeDelta.y);
				_GeneralHeatmapTitle.GetComponentInChildren<Text>().text = useCustomTitle ? customTitle : map.gameObject.name;
				_GeneralHeatmapTitle.GetComponentInChildren<Text>().color = Color.white;

			}

		}
	}
}

