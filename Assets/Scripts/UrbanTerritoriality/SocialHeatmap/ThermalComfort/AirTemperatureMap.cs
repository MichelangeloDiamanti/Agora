using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Maps
{

    public class AirTemperatureMap : GeneralHeatmap
    {
        public float InitialAirTemperature = 20f; // in celsius

        public float updateIntervalSeconds = 10f;

        public NormalizeBehaviors normalizeBehavior;

        public ThermalConductivityMap thermalConductivityMap;

        public HeatGenerationMap heatGenerationMap;

        private const float VOLUMETRIC_HEAT_CAPACITY = 1206f; // volumetric heat capacity of air in J/m^3/*C (according to google)


        private float updateTimer = 0;

        /** This PaintGrid object is the one that is used
         * when drawing agents position. The cell values
         * in it will increase infinitelly. No normalizing is
         * done on it.
         */
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
            // workGrid.Clear(0.0f);
            workGrid.Clear(InitialAirTemperature);

            convergenceTimer = 0;
            base.currentTime = 0;
        }

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();

        }


        // Update is called once per frame
        protected override void Update()
        {
            updateTimer += Time.deltaTime;
            if (updateTimer > updateIntervalSeconds)
            {
                updateTimer = 0;
                UpdateTemperatureGrid();

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

        private void UpdateTemperatureGrid()
        {
            float minAirTemp = Mathf.Infinity;
            float maxAirTemp = Mathf.NegativeInfinity;
            float avgAirTemp = 0;

            int width = workGrid.Width;
            int height = workGrid.Height;
            PaintGrid newTemperatureGrid = new PaintGrid(width, height);
            newTemperatureGrid.Clear(0.0f);

            float maxK = thermalConductivityMap.paintGrid.GetMaxValue();
            float stabilityCriterion = 0.5f * cellSize * cellSize * VOLUMETRIC_HEAT_CAPACITY / maxK;
            float timeStep = Mathf.Min(updateIntervalSeconds, stabilityCriterion);

            // Apply boundary conditions (Dirichlet)
            float boundaryTemperature = InitialAirTemperature;
            for (int x = 0; x < width; x++)
            {
                newTemperatureGrid.DrawPixel(boundaryTemperature, x, 0, PaintGrid.PaintMethod.REPLACE);
                newTemperatureGrid.DrawPixel(boundaryTemperature, x, 1, PaintGrid.PaintMethod.REPLACE);
                newTemperatureGrid.DrawPixel(boundaryTemperature, x, height - 1, PaintGrid.PaintMethod.REPLACE);
                newTemperatureGrid.DrawPixel(boundaryTemperature, x, height - 2, PaintGrid.PaintMethod.REPLACE);
            }
            for (int y = 0; y < height; y++)
            {
                newTemperatureGrid.DrawPixel(boundaryTemperature, 0, y, PaintGrid.PaintMethod.REPLACE);
                newTemperatureGrid.DrawPixel(boundaryTemperature, 1, y, PaintGrid.PaintMethod.REPLACE);
                newTemperatureGrid.DrawPixel(boundaryTemperature, width - 1, y, PaintGrid.PaintMethod.REPLACE);
                newTemperatureGrid.DrawPixel(boundaryTemperature, width - 2, y, PaintGrid.PaintMethod.REPLACE);
            }

            for (int x = 2; x < width - 2; x++)
            {
                for (int y = 2; y < height - 2; y++)
                {

                    // Central difference approximations of the temperature with respect to x at grid points (x+1, y) and (x-1, y)
                    float tempDiffXRight = (workGrid.GetValueAt(x + 2, y) - workGrid.GetValueAt(x, y)) / (2 * cellSize);
                    float tempDiffXLeft = (workGrid.GetValueAt(x, y) - workGrid.GetValueAt(x - 2, y)) / (2 * cellSize);

                    // k(x, y) * (∂T/∂x) at grid points (x+1, y) and (x-1, y)
                    float kTempDiffXRight = thermalConductivityMap.paintGrid.GetValueAt(x + 1, y) * tempDiffXRight;
                    float kTempDiffXLeft = thermalConductivityMap.paintGrid.GetValueAt(x - 1, y) * tempDiffXLeft;

                    // Term ∂/∂x (k(x, y) * (∂T/∂x))
                    float dkTempDiff_dx = (kTempDiffXRight - kTempDiffXLeft) / (2 * cellSize);

                    // Central difference approximations of the temperature with respect to y at grid points (x, y+1) and (x, y-1)
                    float tempDiffYUp = (workGrid.GetValueAt(x, y + 2) - workGrid.GetValueAt(x, y)) / (2 * cellSize);
                    float tempDiffYDown = (workGrid.GetValueAt(x, y) - workGrid.GetValueAt(x, y - 2)) / (2 * cellSize);

                    // k(x, y) * (∂T/∂y) at grid points (x, y+1) and (x, y-1)
                    float kTempDiffYUp = thermalConductivityMap.paintGrid.GetValueAt(x, y + 1) * tempDiffYUp;
                    float kTempDiffYDown = thermalConductivityMap.paintGrid.GetValueAt(x, y - 1) * tempDiffYDown;

                    // Term ∂/∂y (k(x, y) * (∂T/∂y))
                    float dkTempDiff_dy = (kTempDiffYUp - kTempDiffYDown) / (2 * cellSize);

                    // Retrieve heat generation value at the current cell
                    float heatGeneration = heatGenerationMap.paintGrid.GetValueAt(x, y);

                    // Compute the new temperature using the formula
                    float newTemperature = workGrid.GetValueAt(x, y) + (timeStep) * ((dkTempDiff_dx + dkTempDiff_dy + heatGeneration) / VOLUMETRIC_HEAT_CAPACITY);

                    // Update the temperature in the grid
                    newTemperatureGrid.DrawPixel(newTemperature, x, y, PaintGrid.PaintMethod.REPLACE);

                    if (newTemperature < minAirTemp)
                    {
                        minAirTemp = newTemperature;
                    }
                    if (newTemperature > maxAirTemp)
                    {
                        maxAirTemp = newTemperature;
                    }
                    avgAirTemp += newTemperature;

                    newTemperatureGrid.DrawPixel(newTemperature, x, y, PaintGrid.PaintMethod.REPLACE);
                }
            }

            avgAirTemp /= ((width - 2) * (height - 2));

            string debugString = "Air temperature: min: " + minAirTemp + " max: " + maxAirTemp + " avg: " + avgAirTemp;
            Debug.Log(debugString);

            // Copy the new temperature values to the original grid
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    workGrid.DrawPixel(newTemperatureGrid.GetValueAt(x, y), x, y, PaintGrid.PaintMethod.REPLACE);
                }
            }
        }


    }
}