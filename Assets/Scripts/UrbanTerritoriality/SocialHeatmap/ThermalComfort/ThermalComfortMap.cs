using System;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Maps
{

    public enum ThermalComfortType
    {
        PredictedMeanVote,
        PredictedPercentageDissatisfied
    }

    public class ThermalComfortMap : GeneralHeatmap
    {
        public ThermalComfortType thermalComfortType = ThermalComfortType.PredictedPercentageDissatisfied;
        public GameObject agentsRoot;  // The root GameObject containing all the agents

        public float updateIntervalSeconds = 10f;

        public NormalizeBehaviors normalizeBehavior;
        private float normalizationRangeMin;
        private float normalizationRangeMax;

        // Holds the air temperature for each cell in the grid
        // computed using a simplified heat transfer model of heat generation/sinks
        public AirTemperatureMap airTemperatureMap;

        // Constants for PMV calculation these have been set to the values used in the original paper
        // https://www.sciencedirect.com/science/article/pii/S1876610215009868 (page 7)

        // Metabolic rate in met units (1 met = 58.2 W/m²)
        private const float METABOLIC_RATE = 2.4f;

        // Clothing insulation in clo units (1 clo = 0.155 m²·K/W)
        private const float CLOTHING_INSULATION = 1.2f;

        // Air velocity in meters per second (m/s)
        private const float AIR_VELOCITY = 0.2f;

        // Relative humidity in percentage (%)
        private const float RELATIVE_HUMIDITY = 0.3f;

        // Indoor environmental mean radiant temperature (°C)
        // This value is assumed to be equal to the air temperature for indoor scenarios,
        // as it does not account for solar radiation or the heat from surrounding surfaces.
        private const float INDOOR_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE = 20.0f;

        // Outdoor urban environmental mean radiant temperature (°C)
        // This value is slightly higher than the air temperature for outdoor urban scenes,
        // accounting for solar radiation and heat from surrounding surfaces (such as buildings, roads, etc.).
        // You may need to adjust this value based on the specific outdoor conditions in your simulation.
        private const float OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE = 23.0f;

        private const float OFFSET_PARAMETER = 0.1f;  // The offset parameter as mentioned in the paper
        private const float WEIGHT_PARAMETER = 0.5f;  // The weight parameter as mentioned in the paper
        private const float INTIMATE_DISTANCE = 0.45f;  // The intimate distance (in meters) as mentioned in the paper

        float meanRadiantTemperature = OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE;

        private PaintGrid workGrid;

        private float updateTimer = 0;



        // Intermediate variables
        double M = METABOLIC_RATE;      // Metabolic rate (W/m2)
        double W = 0;                   // External work (W/m2), usually assumed to be 0
        double fcl;                     // Clothing surface area factor
        double Icl = CLOTHING_INSULATION * 0.155;  // Thermal insulation of clothing (m2·°C/W)
        double hc; // coefficient of heat transmission by convection For still air (natural convection), hc is usually between 2 to 5 W/(m²·K).
        double tcl; // clothing surface temperature (°C)
        double waterVaporPressure; // water vapor pressure expressed in Pascals (Pa)

        bool debuggedat20 = false;
        bool debuggedat20more = false;


        protected override void applySettings(UTSettings settings)
        {
            throw new System.NotImplementedException();
        }

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

        protected override void Update()
        {
            base.Update();

            updateTimer += Time.deltaTime;
            if (updateTimer > updateIntervalSeconds)
            {
                updateTimer = 0;

                switch (thermalComfortType)
                {
                    case ThermalComfortType.PredictedMeanVote:
                        normalizationRangeMin = -3.0f;
                        normalizationRangeMax = 3.0f;
                        CalculatePMV();
                        break;
                    case ThermalComfortType.PredictedPercentageDissatisfied:
                        normalizationRangeMin = 0.0f;
                        normalizationRangeMax = 100.0f;
                        CalculatePPD();
                        break;
                    default:
                        break;
                }

                switch (normalizeBehavior)
                {
                    case NormalizeBehaviors.COPY_CONTENT:
                        CopyWorkGridToPaintGrid();
                        break;
                    case NormalizeBehaviors.RANGE_NORMALIZE:
                        meanChange = Utilities.Util.RangeNormalization(workGrid, mPaintGrid, normalizationRangeMin, normalizationRangeMax);
                        break;
                    case NormalizeBehaviors.MIN_MAX_NORMALIZE:
                        meanChange = Utilities.Util.MinMaxNormalization(workGrid, mPaintGrid);
                        break;
                    default:
                        break;
                }
            }

        }

        private void CalculatePPD()
        {
            float minPMV = Mathf.Infinity;
            float maxPMV = Mathf.NegativeInfinity;
            // meanRadiantTemperature = OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE;
            meanRadiantTemperature = ComputeMeanRadiantTemperature(OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE);

            for (int x = 0; x < workGrid.Width; x++)
            {
                for (int y = 0; y < workGrid.Height; y++)
                {

                    float airTemperature = airTemperatureMap.paintGrid.GetValueAt(x, y);

                    float pmv = (float)CalculatePMVValue(airTemperature);

                    // save min and max PMV values
                    if (pmv < minPMV)
                    {
                        minPMV = pmv;
                    }
                    if (pmv > maxPMV)
                    {
                        maxPMV = pmv;
                    }

                    float clampedPmv = Mathf.Clamp(pmv, -3f, 3f);

                    float ppd = 100.0f - 95.0f * (float)Math.Exp(-0.03353 * Math.Pow(clampedPmv, 4) - 0.2179 * Math.Pow(clampedPmv, 2));

                    workGrid.DrawPixel(ppd, x, y, PaintGrid.PaintMethod.REPLACE);
                }
            }
            Debug.Log("Min (unclamped) PMV: " + minPMV + " Max (unclamped) PMV: " + maxPMV);

        }

        private void CalculatePMVDebug()
        {
            float minPMV = Mathf.Infinity;
            float maxPMV = Mathf.NegativeInfinity;
            float avgPMV = 0;
            float minAirTemp = Mathf.Infinity;
            float maxAirTemp = Mathf.NegativeInfinity;
            float avgAirTemp = 0;
            // meanRadiantTemperature = OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE;
            meanRadiantTemperature = ComputeMeanRadiantTemperature(OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE);

            for (int x = 0; x < workGrid.Width; x++)
            {
                for (int y = 0; y < workGrid.Height; y++)
                {

                    float airTemperature = airTemperatureMap.paintGrid.GetValueAt(x, y);

                    avgAirTemp += airTemperature;

                    // save min and max air temperature values
                    if (airTemperature < minAirTemp)
                    {
                        minAirTemp = airTemperature;
                    }
                    if (airTemperature > maxAirTemp)
                    {
                        maxAirTemp = airTemperature;
                    }

                    float pmv = (float)CalculatePMVValue(airTemperature);
                    avgPMV += pmv;

                    // save min and max PMV values
                    if (pmv < minPMV)
                    {
                        minPMV = pmv;
                    }
                    if (pmv > maxPMV)
                    {
                        maxPMV = pmv;
                    }

                    float clampedPmv = Mathf.Clamp(pmv, -3f, 3f);

                    workGrid.DrawPixel(clampedPmv, x, y, PaintGrid.PaintMethod.REPLACE);
                }
            }
            avgAirTemp /= workGrid.Width * workGrid.Height;
            avgPMV /= workGrid.Width * workGrid.Height;

            // build a string with the min and max values
            String minMaxString = "Min PMV: " + minPMV + " Max PMV: " + maxPMV;
            minMaxString += " Min Air Temp: " + minAirTemp + " Max Air Temp: " + maxAirTemp;
            minMaxString += " Avg Air Temp: " + avgAirTemp + " Avg PMV: " + avgPMV;
            Debug.Log(minMaxString);
        }


        private void CalculatePMV()
        {
            meanRadiantTemperature = ComputeMeanRadiantTemperature(OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE);

            // Calculate clothing surface area factor (fcl)
            if (CLOTHING_INSULATION <= 0.078)
                fcl = 1 + (1.29 * Icl);
            else
                fcl = 1.05 + (0.645 * Icl);

            for (int x = 0; x < workGrid.Width; x++)
            {
                for (int y = 0; y < workGrid.Height; y++)
                {

                    float airTemperature = airTemperatureMap.paintGrid.GetValueAt(x, y);

                    float pmv = (float)CalculatePMVValue(airTemperature);

                    float clampedPmv = Mathf.Clamp(pmv, -3f, 3f);

                    workGrid.DrawPixel(clampedPmv, x, y, PaintGrid.PaintMethod.REPLACE);
                }
            }
        }

        float NormalizePMV(float pmv)
        {
            float minPMV = -3.0f;
            float maxPMV = 3.0f;

            float normalizedPMV = (pmv - minPMV) / (maxPMV - minPMV);
            return normalizedPMV;
        }


        // PMV Calculation based on:
        // https://www.sciencedirect.com/science/article/pii/S1876610215009868?ref=cra_js_challenge&fr=RR-1
        // and
        // https://iopscience.iop.org/article/10.1088/1755-1315/622/1/012019/pdf
        // and 
        // https://www.researchgate.net/publication/344902060_Experimental_Study_and_Analysis_of_Thermal_Comfort_in_a_University_Campus_Building_in_Tropical_Climate
        public double CalculatePMVValue(float airTemperature)
        {
            // Calculate partial water vapor pressure (pa)
            float exp = (float)Math.Exp((16.6536 - 4030.183) / (airTemperature + 235));
            waterVaporPressure = 10 * RELATIVE_HUMIDITY * exp;

            // Calculate clothing temperature (tcl) and convective heat transfer coefficient (hc)
            CalculateTcl(airTemperature);

            // PMV Calculation
            double PMV = (0.303 * Math.Exp(-0.036 * M) + 0.028) * ((M - W) - (3.05 * Math.Pow(10, -3) * (5733 - 6.99 * (M - W) - waterVaporPressure)) - 0.42 * (M - 58.15) - (1.7 * Math.Pow(10, -5) * M * (5867 - waterVaporPressure)) - 0.0014 * M * (34 - airTemperature) - (3.96 * Math.Pow(10, -8) * fcl * (Math.Pow((tcl + 273), 4) - Math.Pow((meanRadiantTemperature + 273), 4))) - (fcl * hc * (tcl - airTemperature)));

            return PMV;
        }

        // solved iteratively until a prescribed degree of convergence is attained or the maximum number of iterations reached
        // as described https://iopscience.iop.org/article/10.1088/1755-1315/622/1/012019/pdf
        private void CalculateTcl(float airTemperature)
        {
            double tcl_guess = airTemperature + 0.5 * (meanRadiantTemperature - airTemperature);
            double tcl_prev = tcl_guess;
            hc = 0.0;

            for (int i = 0; i < 10; ++i) // Limit the loop to 10 iterations
            {
                double hc_candidate = 2.38 * Math.Pow(Math.Abs(tcl_guess - airTemperature), 0.25);
                hc = Math.Max(hc_candidate, 12.1 * Math.Sqrt(AIR_VELOCITY));

                double tcl_new = (35.7 - 0.028 * (M - W) - Icl * (3.96 * Math.Pow(10, -8) * fcl * (Math.Pow((tcl_guess + 273), 4) - Math.Pow((meanRadiantTemperature + 273), 4)) + fcl * hc * (tcl_guess - airTemperature))) / (1 + Icl * hc);

                if (Math.Abs(tcl_new - tcl_prev) < 1e-6)
                {
                    tcl_guess = tcl_new;
                    break;
                }
                else
                {
                    tcl_prev = tcl_guess;
                    tcl_guess = tcl_new;
                }
            }

            tcl = tcl_guess;
        }

        // calculates the mean radiant temperature considering the environmental mean radiant temperature and the contributions from nearby agents, as described in the paper. 
        //The contribution from each agent is computed based on their distance and orientation with respect to other agents within the intimate proxemic distance.
        // https://www.sciencedirect.com/science/article/pii/S1876610215009868 (page 7)
        private float ComputeMeanRadiantTemperature(float environmentalMeanRadiantTemperature)
        {
            float tr = environmentalMeanRadiantTemperature;
            Transform rootTransform = agentsRoot.transform;

            // Loop through all the agents
            for (int i = 0; i < rootTransform.childCount; i++)
            {
                Transform agentTransform = rootTransform.GetChild(i);
                Vector3 agentPosition = agentTransform.position;
                Quaternion agentRotation = agentTransform.rotation;

                // Loop through all the other agents
                for (int j = 0; j < rootTransform.childCount; j++)
                {
                    if (j == i) continue;  // Skip the current agent

                    Transform otherAgentTransform = rootTransform.GetChild(j);
                    Vector3 otherAgentPosition = otherAgentTransform.position;

                    // Calculate the distance between the two agents
                    float distance = Vector3.Distance(agentPosition, otherAgentPosition);

                    // Check if the distance is within the intimate proxemic distance
                    if (distance <= INTIMATE_DISTANCE)
                    {
                        // Calculate the angle between the agents
                        float angle = Vector3.Angle(agentTransform.forward, otherAgentPosition - agentPosition);

                        // Calculate the radiant temperature contribution from the other agent
                        float radiantTemperatureContribution = OFFSET_PARAMETER + WEIGHT_PARAMETER * (1.0f - Mathf.Cos(Mathf.Deg2Rad * angle));

                        // Add the contribution to the mean radiant temperature
                        tr += radiantTemperatureContribution;
                    }
                }
            }

            return tr;
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

    }
}