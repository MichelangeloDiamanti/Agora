using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.ScriObj;
using UrbanTerritoriality.Agent;
using UrbanTerritoriality.Utilities;
using UrbanTerritoriality.Enum;


namespace UrbanTerritoriality.Maps
{

    /**
     * A parent class for various types of grid based maps.
     */
    public abstract class GPUHeatmap : MonoBehaviour
    {
        protected RenderTexture mPaintGrid;


        /** Current time in seconds since the start of the scene */
        public float CurrentTime { get { return currentTime; } }
        protected float currentTime = 0;

        /** A PaintGrid object for holding the values
         * of the map. */
        public RenderTexture paintGrid { get { return mPaintGrid; } }

        private ComputeShader computeShader;
        int gradientKernel;


        const int textureGradientResolution = 4096;


        /** Size of the map in a scene */
        public Vector2 size;

        /** The size of each cell in the map */
        public float cellSize;

        /** Color of the map gizmo in the scene view */
        public Color gizmoColor = new Color(0.5f, 0, 0.5f, 1f);

        /** Weather or not to save the map to disk as an asset */
        [HideInInspector]
        public bool saveAsset = false;

        /** Weather or not to save the map as a texture */
        [HideInInspector]
        public bool saveTexture = false;

        /** Gradient to use when saving the map as a texture */
        [HideInInspector]
        public GradientScriptableObject textureGradient;

        /** Path were the asset will be saved */
        [HideInInspector]
        public string savePath = "Assets/map.asset";

        /** Time after the scene starts when the map will be saved. */
        [HideInInspector]
        public float saveTime = 10f;

        /** Path were the asset was saved if it has been saved */
        public string SavedMapPath { get { return savedMapPath; } }
        protected string savedMapPath = null;

        protected bool saveMapOnApplicationQuit = false;

        public bool SaveMapOnApplicationQuit
        {
            get
            {
                return saveMapOnApplicationQuit;
            }

            set
            {
                saveMapOnApplicationQuit = value;
            }
        }


        /** Weather or not the map has been saved */
        public bool HasMapBeenSaved { get { return hasMapBeenSaved; } }
        protected bool hasMapBeenSaved = false;

        /** Weather or not the map has converged */
        public bool HasMapConverged { get { return hasMapConverged; } }
        protected bool hasMapConverged = false;

        /** Path were the texture of the map is saved if it has been saved */
        public string SavedTexturePath { get { return savedTexturePath; } }
        protected string savedTexturePath = null;

        /** Weather or not the map has been saved as a texture */
        public bool HasTextureBeenSaved { get { return hasTextureBeenSaved; } }
        protected bool hasTextureBeenSaved = false;

        /** Weather or not the map has been initialized */
        public bool Initialized { get { return initialized; } }
        protected bool initialized = false;


        /** The saving method of the map */
        [HideInInspector]
        public SaveMethod saveMethod = SaveMethod.TIME;

        /** Threshold for mean change, used for
         * knowing when to save the map */
        [HideInInspector]
        public float meanChangeThreshold = 0.01f;
        [HideInInspector]
        public float meanChange;

        protected float convergenceTimer;

        public float ConvergenceTimer { get { return convergenceTimer; } }

        /** This method should do initialization */
        protected abstract void _initialize();

        /** This method should apply settings from an UTSettings object
         * @param settings An object with settings, some of which could
         * be applied to this map.
         */
        protected abstract void applySettings(UTSettings settings);

        /** Check if a world position is within the map
         * @param pos The world position
         * @return Returns true if the world position is
         * within the map boundaries.
         */
        public bool IsWithin(Vector2 pos)
        {
            Vector2 mapPos = new Vector2(transform.position.x, transform.position.z);
            float rX = size.x / 2f;
            float rY = size.y / 2f;
            return pos.x >= mapPos.x - rX &&
                pos.x <= mapPos.x + rX &&
                pos.y >= mapPos.y - rY &&
                pos.y <= mapPos.y + rY;
        }

        /**
         * Check some values to see if they are within
         * their expected limits.
         * @return Returns true if all the values are
         * within their expected limits, else false
         * is returned.
         */
        private bool CheckValues()
        {
            string typename = this.GetType().FullName.ToString();
            if (size.x <= 0 || size.y <= 0)
            {
                Debug.LogError(typename +
                    " The size of this map should be greater than 0!");
                return false;
            }
            if (cellSize <= 0)
            {
                Debug.LogError(typename +
                    " The cell size of this map should be greater than 0!");
                return false;
            }
            return true;
        }

        /** Get full disk path of the map texture file.
         * @return A string containing the full path
         * of the map texture file.
         */
        public virtual string GetTextureFullPath()
        {
            return
                GetTextureFullPath(GetTextureAssetPath());
        }

        /** Get full disk path of the map texture file.
         * @param textureAssetPath The path to the file
         * inside the Unity project.
         * @return A string containing the full disk path
         * of the map texture file.
         */
        public virtual string GetTextureFullPath(string textureAssetPath)
        {
            return
                EditorTools.FileUtil.ConvertAssetPathToDiskPath(
                textureAssetPath);
        }

        /** Get the path inside the Unity project to
         * the texture file for the map.
         * @return The path inside the Unity project to the
         * map texture.
         */
        public virtual string GetTextureAssetPath()
        {
            return
                StringUtil.ModifyExtension(
                    savePath, "png");
        }

        /**
         * Save the map as a texture on disk.
         * If textureGradient is not null, the colors
         * in it will be used for creating the texture
         * from the values in the map.
         * If it is null, then the texture will be
         * black and white.
         */
        public virtual void SaveMapAsTexture()
        {
            string textureAssetPath = GetTextureAssetPath();
            string path = GetTextureFullPath(textureAssetPath);
            Gradient grad = null;
            if (textureGradient != null) grad = textureGradient.gradient;


            // Convert the gradient into a texture that can be used by the compute shader
            Texture2D gradientTex = new Texture2D(textureGradientResolution, 1);
            for (int i = 0; i < textureGradientResolution; i++)
                gradientTex.SetPixel(i, 0, textureGradient.gradient.Evaluate((float)i / textureGradientResolution));
            gradientTex.Apply();

            RenderTexture tmp = RenderTexture.GetTemporary(mPaintGrid.width, mPaintGrid.height, 0, RenderTextureFormat.ARGB32);
            tmp.enableRandomWrite = true;
            ApplyGradient(mPaintGrid, gradientTex, tmp);

            // Save the heatmap
            Texture2D tex = new Texture2D(tmp.width, tmp.height, TextureFormat.RGBA32, false);
            RenderTexture.active = tmp;
            tex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;


            // TextureScaler.scale(tex, tex.width * (int)cellSize, tex.height * (int)cellSize, FilterMode.Point);
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            savedTexturePath = textureAssetPath;
            hasTextureBeenSaved = true;

            RenderTexture.ReleaseTemporary(tmp);
            Destroy(gradientTex);
            Destroy(tex);
        }

        void ApplyGradient(RenderTexture inputHeatmap, Texture2D gradient, RenderTexture outputHeatmap)
        {
            computeShader.SetTexture(gradientKernel, "NormalizedHeatmapTexture", inputHeatmap);
            computeShader.SetTexture(gradientKernel, "GradientTexture", gradient);
            computeShader.SetTexture(gradientKernel, "OutputTexture", outputHeatmap);
            computeShader.SetInt("NormalizedHeatmapTextureWidth", inputHeatmap.width);
            computeShader.SetInt("NormalizedHeatmapTextureHeight", inputHeatmap.height);
            computeShader.SetInt("GradientTextureWidth", textureGradientResolution);

            int threadGroupsX = Mathf.CeilToInt(inputHeatmap.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(inputHeatmap.height / 16.0f);
            computeShader.Dispatch(gradientKernel, threadGroupsX, threadGroupsY, 1);
        }

        // /** Save the map as an asset to disk
        //  * using the save path in the savePath
        //  * variable.
        //  */
        // public virtual void SaveMapAsAsset()
        // {
        //     MapDataScriptableObject data = ScriptableObject.CreateInstance<MapDataScriptableObject>();
        //     data.position = transform.position;
        //     data.cellSize = cellSize;
        //     data.gridSize = new Vector2Int(mPaintGrid.width, mPaintGrid.height);
        //     data.gradient = textureGradient;
        //     data.data = mPaintGrid.grid;
        //     UnityEditor.AssetDatabase.CreateAsset(data, savePath);
        //     savedMapPath = savePath;
        //     hasMapBeenSaved = true;
        // }

        // public virtual void InitializeFromAsset(MapDataScriptableObject data)
        // {
        //     transform.position = data.position;
        //     cellSize = data.cellSize;
        //     size = new Vector2(data.gridSize.x * cellSize, data.gridSize.y * cellSize);
        //     textureGradient = data.gradient;
        //     mPaintGrid = new PaintGrid(data.gridSize.x, data.gridSize.y);
        //     mPaintGrid.grid = data.data;
        // }


        /** Handle the saving of the map.
         * A few options allow the user
         * to control if and how the map
         * is saved.
         * This method takes care of saving
         * the map in the right way according
         * to those options.
         */
        public virtual void HandleSavingOfMap()
        {
            bool somethingSaved = false;
            if (!HasMapBeenSaved && saveAsset)
            {
                // SaveMapAsAsset();
                somethingSaved = true;
            }
            if (!HasTextureBeenSaved && saveTexture)
            {
                SaveMapAsTexture();
                somethingSaved = true;
            }
            if (somethingSaved)
            {
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
        }

        public virtual void SaveMapImmediate()
        {
            // SaveMapAsAsset();
            SaveMapAsTexture();
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        /**
         * Calls HandleSavingOfMap at the right time.
         * That is at the time specified by saveTime.
         * @param startTime This should be the current
         * time (when calling this method) in seconds
         * after the scene started.
         */
        protected virtual IEnumerator SaveMapAfterTime()
        {
            yield return new WaitUntil(() => currentTime > saveTime);
            HandleSavingOfMap();
            yield return null;
        }

        /** Saves map on quality threshold if saveMethod is set to QUALITY */
        protected virtual IEnumerator SaveMapOnThreshold()
        {
            yield return new WaitUntil(() => (meanChange < meanChangeThreshold));
            convergenceTimer = currentTime;
            if (saveMethod == SaveMethod.QUALITY)
            {
                hasMapConverged = true;
                HandleSavingOfMap();
            }
            yield return null;
        }

        /** Starts coroutines that handle the saving of the map. */
        protected virtual void ConfigureSaveBehavior()
        {
            if ((saveAsset || saveTexture) == false)
                return;
            switch (saveMethod)
            {
                case SaveMethod.QUALITY:
                    {
                        StartCoroutine(SaveMapOnThreshold());
                    }
                    break;
                case SaveMethod.TIME:
                    {
                        StartCoroutine(SaveMapAfterTime());
                    }
                    break;
                default:
                    break;
            }
        }

        /** A delegate for methods to call
         * right after the GeneralHeatmap has been
         * initialized.
         */
        public delegate void InitializeAction();

        /** An event that occurs right after a
         * GeneralHeatmap has been initialized. */
        public event InitializeAction OnInitialized;

        /** Do some initialization tasks for the map */
        public void Initialize()
        {
            meanChange = float.NaN;
            UTSettings settings = (UTSettings)GameObject.FindObjectOfType(typeof(UTSettings));
            if (settings != null)
            {
                if (settings.applySettings)
                {
                    applySettings(settings);
                }
            }

            // Get notified when the simulation is ending.
            SimulationManager.Instance.SimulationEnding.AddListener(OnSimulationEnding);

            if (CheckValues())
            {
                _initialize();
                ConfigureSaveBehavior();
                initialized = true;
                if (OnInitialized != null)
                {
                    OnInitialized();
                }
            }

            computeShader = Resources.Load<ComputeShader>("Shaders/Compute/GeneralHeatMapVisualizer");
            gradientKernel = computeShader.FindKernel("ApplyGradient");
        }

        /** Unity Start method */
        public virtual void Start()
        {
            Initialize();
        }

        /** Calculates the distance to the border of the map
         * in the direction of a ray that has a starting point
         * and a direction.
         * @param rayStart The world position of the starting
         * point of the ray.
         * @param rayDirection A 2D vector specifying the
         * direction of the ray.
         * @return Returns the distance in the world from the point
         * rayStart to the border in the direction of rayDirection.
         * If the ray does not intersect with the border then
         * null is returned.
         */
        public virtual float? CalculateDistanceToBorder(Vector2 rayStart, Vector2 rayDirection)
        {
            /* Calculate positions of the corners of the grid in the scene */
            Vector3 pos = transform.position;
            float rX = size.x / 2f;
            float rY = size.y / 2f;
            Vector2 southWestCorner = new Vector2(pos.x - rX, pos.z - rY);
            Vector2 northWestCorner = new Vector2(pos.x - rX, pos.z + rY);
            Vector2 northEastCorner = new Vector2(pos.x + rX, pos.z + rY);
            Vector2 southEastCorner = new Vector2(pos.x + rX, pos.z - rY);

            Vector2[] borders = new Vector2[]
            {
                southWestCorner,
                northWestCorner,
                northWestCorner,
                northEastCorner,
                northEastCorner,
                southEastCorner,
                southEastCorner,
                southWestCorner
            };

            int n = borders.Length;
            float shortestDist = float.MaxValue;
            bool intersects = false;
            for (int i = 0; i < n; i += 2)
            {
                Vector2? inter = Util.RayLineIntersection(rayStart,
                    rayDirection, borders[i], borders[i + 1]);
                if (inter != null)
                {
                    intersects = true;
                    float dist = Vector2.Distance(rayStart, (Vector2)inter);
                    if (dist < shortestDist) shortestDist = dist;
                }
            }
            return intersects ? (float?)shortestDist : null;
        }

        /** Convert a world position to a position in the paint grid.
         * That is find the cell in the grid that represents a particular
         * position in the unity scene.
         * @param pos The world position that is to be converted
         * to a position in the grid.
         * return Returns the position in the PaintGrid.
         */
        public Vector2Int WorldToGridPos(Vector2 pos)
        {
            Vector2Int v = new Vector2Int();
            float transX = pos.x - transform.position.x;
            float transXnorm = transX / size.x;
            v.x = (int)(mPaintGrid.width * 0.5f + transXnorm * mPaintGrid.width);
            float transY = pos.y - transform.position.z;
            float transYnorm = transY / size.y;
            v.y = (int)(mPaintGrid.height * 0.5f + transYnorm * mPaintGrid.height);
            return v;
        }

        /** Convert a world position to a position in the PaintGrid.
         * This method returns a Vector2 containing x and y floating
         * point values that are not rounded to an integer.
         * @param pos A 2D position in the world.
         * @return Returns the position in the PaintGrid of the map.
         */
        public Vector2 WorldToGridPosFloat(Vector2 pos)
        {
            Vector2 v = new Vector2();
            float transX = pos.x - transform.position.x;
            float transXnorm = transX / size.x;
            v.x = (float)mPaintGrid.width / 2 + transXnorm * (float)mPaintGrid.width;
            float transY = pos.y - transform.position.z;
            float transYnorm = transY / size.y;
            v.y = (float)mPaintGrid.height / 2 + transYnorm * (float)mPaintGrid.height;
            return v;
        }

        /** Convert a world distance to a distance in the grid.
         * @param distance A world distance.
         * @return The distance in the grid.
         */
        public float WorldToGridDistance(float distance)
        {
            return (int)(distance / cellSize);
        }

        /** Get world position of a cell in the paint grid.
         * @param x X coordinate of the grid cell.
         * @param y Y coordinate of the grid cell.
         * @return The world position of the cell.
         */
        public Vector2 GridToWorldPosition(int x, int y)
        {
            Vector2 halfSize = size / 2f;
            float xStart = transform.position.x - halfSize.x;
            float yStart = transform.position.z - halfSize.y;
            float halfCellSize = cellSize / 2f;
            float worldPosX = cellSize * (float)x + xStart + halfCellSize;
            float worldPosY = cellSize * (float)y + yStart + halfCellSize;
            return new Vector2(worldPosX, worldPosY);
        }


        /** Unity OnDrawGizmos method */
        protected virtual void OnDrawGizmos()
        {
            // GizmoHelper.DrawPlane(transform.position, size, gizmoColor);
            GizmoHelper.DrawGrid(transform.position, size, gizmoColor, (int)cellSize);
        }

        /** Unity Update method */
        protected virtual void Update()
        {
            currentTime += Time.unscaledDeltaTime;
        }

        void OnSimulationEnding()
        {
            if (saveMapOnApplicationQuit)
                HandleSavingOfMap();
        }
    }
}

