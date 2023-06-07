using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

namespace UrbanTerritoriality.Maps
{


    public class HeatGenerationMap : GeneralHeatmap
    {
        public GameObject agentsRoot;
        public GameObject heatGeneratorsRoot;

        public float updateIntervalSeconds = 10f;

        public NormalizeBehaviors normalizeBehavior;

        private PaintGrid workGrid;

        private float updateTimer = 0;

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

        public override float GetValueAt(Vector2 position)
        {
            return GetValueAt(position, null);
        }

        public float GetValueAt(int x, int y)
        {
            return GetValueAt(new Vector2(x, y), null);
        }


        public override void InitializeFromAsset(MapDataScriptableObject data)
        {
            transform.position = data.position;
            cellSize = data.cellSize;
            size = new Vector2(data.gridSize.x * cellSize, data.gridSize.y * cellSize);
            textureGradient = data.gradient;
            mPaintGrid = new PaintGrid(data.gridSize.x, data.gridSize.y);
            mPaintGrid.grid = data.data;
        }

        protected override void applySettings(UTSettings settings)
        {
            throw new System.NotImplementedException();
        }

        protected override void _initialize()
        {
            int resX = (int)(size.x / cellSize);
            int resY = (int)(size.y / cellSize);
            mPaintGrid = new PaintGrid(resX, resY);

            workGrid = new PaintGrid(resX, resY);
            workGrid.Clear(0.0f);

            convergenceTimer = 0;
            base.currentTime = 0;
        }

        public override void Start()
        {
            base.Start();
        }

        private void CopyWorkGridToPaintGrid()
        {
            for (int x = 0; x < workGrid.Width; x++)
            {
                for (int y = 0; y < workGrid.Height; y++)
                {
                    mPaintGrid.DrawPixel(workGrid.GetValueAt(x, y), x, y, PaintGrid.PaintMethod.REPLACE);
                }
            }
        }

        protected override void Update()
        {
            updateTimer += Time.deltaTime;
            if (updateTimer > updateIntervalSeconds)
            {
                updateTimer = 0;
                UpdateThermalGenerationMap();

                switch (normalizeBehavior)
                {
                    case NormalizeBehaviors.COPY_CONTENT:
                        CopyWorkGridToPaintGrid();
                        break;
                    default:
                        meanChange = Utilities.Util.MinMaxNormalization(workGrid, mPaintGrid);
                        break;
                }
            }
        }

        private void UpdateThermalGenerationMap()
        {
            // Clear workGrid
            workGrid.Clear(0.0f);

            // Get HeatSource components from agentsRoot and heatGeneratorsRoot
            List<HeatSource> heatSources = new List<HeatSource>(agentsRoot.GetComponentsInChildren<HeatSource>());
            heatSources.AddRange(heatGeneratorsRoot.GetComponentsInChildren<HeatSource>());

            // Iterate through heat sources and paint the grid with heat generation values
            foreach (HeatSource heatSource in heatSources)
            {
                Vector2Int center = WorldToGridPos(new Vector2(heatSource.transform.position.x, heatSource.transform.position.z));
                float radius = heatSource.heatGenerationRadius / cellSize;
                workGrid.DrawEllipse(heatSource.heatGenerationRate, PaintGrid.PaintMethod.ADD, center.x, center.y, radius, radius, 0);
            }
        }
    }
}
