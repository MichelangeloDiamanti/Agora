using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

namespace UrbanTerritoriality.Maps
{

    public class ThermalComfortGPU : GPUHeatmap
    {
        public ThermalComfortType thermalComfortType = ThermalComfortType.PredictedPercentageDissatisfied;
        // Struct to hold the result of each thread
        struct ThreadResult
        {
            public float minValue; // minimum red value in the portion processed by this thread
            public float maxValue; // maximum red value in the portion processed by this thread
        }

        public GameObject agentsRoot;  // The root GameObject containing all the agents

        public float updateIntervalSeconds = 10f;

        public bool updateOnDemand = false;

        public NormalizeBehaviors normalizeBehavior;

        private float normalizationRangeMin;
        private float normalizationRangeMax;

        private float updateTimer = 0;


        public ComputeShader thermalComfortComputeShader;

        private int PMVKernel;
        private int PPDKernel;
        private int minMaxKernel;
        private int normalizationKernel;
        private int initialTemperatureKernel;


        public AirTemperatureGPU airTemperatureMap;

        private RenderTexture workGrid;

        // Constants for PMV calculation these have been set to the values used in the original paper
        // https://www.sciencedirect.com/science/article/pii/S1876610215009868 (page 7)

        // Metabolic rate in met units (1 met = 58.2 W/m²)
        public float metabolicRate = 1.7f;

        // Clothing insulation in clo units (1 clo = 0.155 m²·K/W)
        public float clothingInsulation = 0.8f;

        // Air velocity in meters per second (m/s)
        public float airVelocity = 0.2f;

        // Relative humidity in percentage (%)
        public float relativeHumidity = 0.3f;

        float W = 0;                   // External work (W/m2), usually assumed to be 0
        float fcl;                     // Clothing surface area factor
        float Icl;  // Thermal insulation of clothing (m2·°C/W)


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


        protected override void applySettings(UTSettings settings)
        {
            throw new System.NotImplementedException();
        }

        protected override void _initialize()
        {
            int resX = (int)(size.x / cellSize);
            int resY = (int)(size.y / cellSize);

            mPaintGrid = new RenderTexture(resX, resY, 0, RenderTextureFormat.RFloat);
            mPaintGrid.enableRandomWrite = true;
            mPaintGrid.Create();

            workGrid = new RenderTexture(resX, resY, 0, RenderTextureFormat.RFloat);
            workGrid.enableRandomWrite = true;
            workGrid.Create();

            // Get the kernel index for heat generation shader
            PMVKernel = thermalComfortComputeShader.FindKernel("CalculatePMV");
            PPDKernel = thermalComfortComputeShader.FindKernel("CalculatePPD");
            minMaxKernel = thermalComfortComputeShader.FindKernel("FindMinMax");
            normalizationKernel = thermalComfortComputeShader.FindKernel("NormalizeTexture");

            convergenceTimer = 0;
            base.currentTime = 0;

            Icl = clothingInsulation * 0.155f;
        }

        public override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            if (!updateOnDemand)
            {
                updateTimer += Time.deltaTime;
                if (updateTimer > updateIntervalSeconds)
                {
                    updateTimer = 0;
                    ComputeThermalComfortMap();
                    WorkgridToPaintgrid();
                }
            }
        }



        void ComputeThermalComfortMap()
        {
            airTemperatureMap.updateIntervalSeconds = this.updateIntervalSeconds;
            airTemperatureMap.UpdateAirTemperatureMap();

            // meanRadiantTemperature = ComputeMeanRadiantTemperature(OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE);
            // Debug.Log("Mean Radiant Temperature: " + meanRadiantTemperature);
            meanRadiantTemperature = OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE;

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

            // float minValue, maxValue;
            // FindMinMax(airTemperatureMap.paintGrid, out minValue, out maxValue);
            // Debug.Log("Min Temperature: " + minValue + " Max Temperature: " + maxValue);
        }

        private void CalculatePMV()
        {
            // Calculate clothing surface area factor (fcl)
            if (clothingInsulation <= 0.078)
                fcl = 1 + (1.29f * Icl);
            else
                fcl = 1.05f + (0.645f * Icl);

            // Set up textures
            thermalComfortComputeShader.SetTexture(PMVKernel, "airTemperatureTexture", airTemperatureMap.paintGrid);
            thermalComfortComputeShader.SetTexture(PMVKernel, "pmvTexture", workGrid);

            // Set up constants
            thermalComfortComputeShader.SetInt("airTemperatureTextureWidth", workGrid.width);
            thermalComfortComputeShader.SetInt("airTemperatureTextureHeight", workGrid.height);
            thermalComfortComputeShader.SetFloat("meanRadiantTemperature", OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE);
            thermalComfortComputeShader.SetFloat("CLOTHING_INSULATION", clothingInsulation);
            thermalComfortComputeShader.SetFloat("RELATIVE_HUMIDITY", relativeHumidity);
            thermalComfortComputeShader.SetFloat("AIR_VELOCITY", airVelocity);
            thermalComfortComputeShader.SetFloat("M", metabolicRate);
            thermalComfortComputeShader.SetFloat("W", W);
            thermalComfortComputeShader.SetFloat("fcl", fcl);
            thermalComfortComputeShader.SetFloat("Icl", Icl);

            // Dispatch the kernel
            int threadGroupsX = Mathf.CeilToInt(workGrid.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(workGrid.height / 16.0f);
            thermalComfortComputeShader.Dispatch(PMVKernel, threadGroupsX, threadGroupsY, 1);

            float minValue, maxValue;
            FindMinMax(workGrid, out minValue, out maxValue);
            Debug.Log("Min PMV: " + minValue + " Max PMV: " + maxValue);

        }


        private void CalculatePPD()
        {
            // Calculate clothing surface area factor (fcl)
            if (clothingInsulation <= 0.078)
                fcl = 1 + (1.29f * Icl);
            else
                fcl = 1.05f + (0.645f * Icl);

            // Set up textures
            thermalComfortComputeShader.SetTexture(PPDKernel, "airTemperatureTexture", airTemperatureMap.paintGrid);
            thermalComfortComputeShader.SetTexture(PPDKernel, "ppdTexture", workGrid);

            // Set up constants
            thermalComfortComputeShader.SetInt("airTemperatureTextureWidth", workGrid.width);
            thermalComfortComputeShader.SetInt("airTemperatureTextureHeight", workGrid.height);
            thermalComfortComputeShader.SetFloat("meanRadiantTemperature", OUTDOOR_URBAN_ENVIRONMENTAL_MEAN_RADIANT_TEMPERATURE);
            thermalComfortComputeShader.SetFloat("CLOTHING_INSULATION", clothingInsulation);
            thermalComfortComputeShader.SetFloat("RELATIVE_HUMIDITY", relativeHumidity);
            thermalComfortComputeShader.SetFloat("AIR_VELOCITY", airVelocity);
            thermalComfortComputeShader.SetFloat("M", metabolicRate);
            thermalComfortComputeShader.SetFloat("W", W);
            thermalComfortComputeShader.SetFloat("fcl", fcl);
            thermalComfortComputeShader.SetFloat("Icl", Icl);

            // Dispatch the kernel
            int threadGroupsX = Mathf.CeilToInt(workGrid.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(workGrid.height / 16.0f);
            thermalComfortComputeShader.Dispatch(PPDKernel, threadGroupsX, threadGroupsY, 1);

            float minValue, maxValue;
            FindMinMax(workGrid, out minValue, out maxValue);
            Debug.Log("Min PPD: " + minValue + " Max PPD: " + maxValue);

        }


        private void WorkgridToPaintgrid()
        {
            switch (normalizeBehavior)
            {
                case NormalizeBehaviors.MIN_MAX_NORMALIZE:
                    MinmaxNormalization();
                    break;
                case NormalizeBehaviors.RANGE_NORMALIZE:
                    RangeNormalization();
                    break;
                default:
                    Graphics.CopyTexture(workGrid, mPaintGrid);
                    break;
            }
        }

        private void MinmaxNormalization()
        {
            float minValue, maxValue;
            FindMinMax(workGrid, out minValue, out maxValue);
            NormalizeTexture(workGrid, mPaintGrid, minValue, maxValue);
        }

        private void RangeNormalization()
        {
            NormalizeTexture(workGrid, mPaintGrid, normalizationRangeMin, normalizationRangeMax);
        }

        Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
        {
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RFloat, false);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = null;

            return texture2D;
        }

        void FindMinMaxCPU(Texture2D input, out float minValue, out float maxValue)
        {
            minValue = float.MaxValue;
            maxValue = float.MinValue;

            for (int y = 0; y < input.height; y++)
            {
                for (int x = 0; x < input.width; x++)
                {
                    float value = input.GetPixel(x, y).r;
                    minValue = Mathf.Min(minValue, value);
                    maxValue = Mathf.Max(maxValue, value);
                }
            }
        }

        void FindMinMax(Texture inputTexture, out float minValue, out float maxValue)
        {
            // Create the result buffer with the same length as the number of threads
            int numThreadsX = Mathf.CeilToInt((float)inputTexture.width / 16);
            int numThreadsY = Mathf.CeilToInt((float)inputTexture.height / 16);
            ComputeBuffer resultBuffer = new ComputeBuffer(numThreadsX * numThreadsY, sizeof(float) * 2);

            // Set the compute shader parameters
            thermalComfortComputeShader.SetTexture(minMaxKernel, "inputTexture", inputTexture);
            thermalComfortComputeShader.SetInt("inputTextureWidth", inputTexture.width);
            thermalComfortComputeShader.SetInt("inputTextureHeight", inputTexture.height);
            thermalComfortComputeShader.SetBuffer(minMaxKernel, "resultBuffer", resultBuffer);

            // Dispatch the compute shader
            thermalComfortComputeShader.Dispatch(minMaxKernel, numThreadsX, numThreadsY, 1);

            // Read the result buffer into an array
            ThreadResult[] results = new ThreadResult[numThreadsX * numThreadsY];
            resultBuffer.GetData(results);

            // Find the pixel with the highest red value among all results
            minValue = Mathf.Infinity;
            maxValue = -Mathf.Infinity;



            foreach (ThreadResult result in results)
            {
                if (result.minValue < minValue)
                {
                    minValue = result.minValue;
                }
                if (result.maxValue > maxValue)
                {
                    maxValue = result.maxValue;
                }
            }


            // Release the result buffer
            resultBuffer.Release();
        }




        void NormalizeTexture(RenderTexture input, RenderTexture output, float minValue, float maxValue)
        {
            thermalComfortComputeShader.SetTexture(normalizationKernel, "inputTexture", input);
            thermalComfortComputeShader.SetInt("inputTextureWidth", workGrid.width);
            thermalComfortComputeShader.SetInt("inputTextureHeight", workGrid.height);
            thermalComfortComputeShader.SetTexture(normalizationKernel, "outputTexture", output);
            thermalComfortComputeShader.SetFloat("minValue", minValue);
            thermalComfortComputeShader.SetFloat("maxValue", maxValue);

            int threadGroupsX = Mathf.CeilToInt(input.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(input.height / 16.0f);
            thermalComfortComputeShader.Dispatch(normalizationKernel, threadGroupsX, threadGroupsY, 1);
        }


        // calculates the mean radiant temperature considering the environmental mean radiant temperature and the contributions from nearby agents, as described in the paper. 
        //The contribution from each agent is computed based on their distance and orientation with respect to other agents within the intimate proxemic distance.
        // https://www.sciencedirect.com/science/article/pii/S1876610215009868 (page 7)
        // TODO: check this because the temperature might be skyrocking when there are many agents, maybe the weight should be reduced
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

    }
}