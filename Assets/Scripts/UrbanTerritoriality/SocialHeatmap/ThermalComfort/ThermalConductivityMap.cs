using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

namespace UrbanTerritoriality.Maps
{
    // ThermalConductivityMap is a heatmap representing the thermal conductivity values
    // across a 2D space. It differentiates between air and concrete areas and assigns
    // different thermal conductivity values accordingly.
    public class ThermalConductivityMap : GeneralHeatmap
    {
        public NormalizeBehaviors normalizeBehavior;

        // Constants for thermal conductivity values (W/m°C)
        private const float airThermalConductivity = 0.024f;
        private const float concreteThermalConductivity = 0.001f; // Very low value to simulate insulation

        private PaintGrid workGrid;

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

            GenerateThermalConductivityMap();

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

        }

        private void GenerateThermalConductivityMap()
        {
            for (int x = 0; x < workGrid.Width; x++)
            {
                for (int y = 0; y < workGrid.Height; y++)
                {
                    Vector2 worldPos = GridToWorldPosition(x, y);
                    Vector3 worldPos3d = new Vector3(worldPos.x, 1000.0f, worldPos.y);
                    Vector3 boxSize = new Vector3(cellSize, 1.0f, cellSize);
                    RaycastHit hit;
                    bool isConcrete = Physics.BoxCast(worldPos3d, boxSize / 2, -Vector3.up, out hit, Quaternion.identity, Mathf.Infinity, LayerMask.GetMask("Concrete"));

                    if (isConcrete)
                    {
                        workGrid.DrawPixel(concreteThermalConductivity, x, y, PaintGrid.PaintMethod.REPLACE);
                    }
                    else
                    {
                        workGrid.DrawPixel(airThermalConductivity, x, y, PaintGrid.PaintMethod.REPLACE);
                    }
                }
            }
        }
    }
}
