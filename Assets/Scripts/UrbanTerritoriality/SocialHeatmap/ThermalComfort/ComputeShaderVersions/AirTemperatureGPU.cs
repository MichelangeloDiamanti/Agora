using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

namespace UrbanTerritoriality.Maps
{

    public class AirTemperatureGPU : GPUHeatmap
    {
        // Struct to hold the result of each thread
        struct ThreadResult
        {
            public float minValue; // minimum red value in the portion processed by this thread
            public float maxValue; // maximum red value in the portion processed by this thread
        }


        public float initialAirTemperature = 20f; // in celsius

        public float updateIntervalSeconds = 10f;

        public bool updateOnDemand = false;

        public NormalizeBehaviors normalizeBehavior;

        private float updateTimer = 0;


        public ComputeShader airTemperatureComputeShader;

        private int airTemperatureKernel;
        private int minMaxKernel;
        private int normalizationKernel;
        private int initialTemperatureKernel;


        public Texture2D thermalConductivityMap;

        public HeatGenerationGPU heatGenerationMap;

        private RenderTexture workGrid;


        float convectiveHeatTransferCoefficient = 25.0f; // W/(m²·°C) within the range for natural convection in air

        private const float VOLUMETRIC_HEAT_CAPACITY = 1206f; // volumetric heat capacity of air in J/m^3/*C (according to google)

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
            airTemperatureKernel = airTemperatureComputeShader.FindKernel("UpdateTemperatureGrid");
            minMaxKernel = airTemperatureComputeShader.FindKernel("FindMinMax");
            normalizationKernel = airTemperatureComputeShader.FindKernel("NormalizeTexture");
            initialTemperatureKernel = airTemperatureComputeShader.FindKernel("SetInitialTemperature");

            SetInitialTemperature(workGrid, initialAirTemperature);

            convergenceTimer = 0;
            base.currentTime = 0;
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
                    ComputeAirTemperatureMap();
                    WorkgridToPaintgrid();
                }
            }
        }

        void SetInitialTemperature(RenderTexture texture, float initialTemperature)
        {
            airTemperatureComputeShader.SetTexture(initialTemperatureKernel, "outputTexture", texture);
            airTemperatureComputeShader.SetFloat("initialTemperature", initialTemperature);

            int threadGroupsX = Mathf.CeilToInt(texture.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(texture.height / 16.0f);
            airTemperatureComputeShader.Dispatch(initialTemperatureKernel, threadGroupsX, threadGroupsY, 1);

            // float minValue, maxValue;
            // FindMinMax(texture, out minValue, out maxValue);
            // Debug.Log("Min: " + minValue + " Max: " + maxValue);
        }

        public void UpdateAirTemperatureMap()
        {
            ComputeAirTemperatureMap();
            WorkgridToPaintgrid();
        }

        void ComputeAirTemperatureMap()
        {
            RenderTexture newTemperatureGrid = new RenderTexture(workGrid.width, workGrid.height, workGrid.depth, workGrid.format);
            newTemperatureGrid.enableRandomWrite = true;
            newTemperatureGrid.Create();

            heatGenerationMap.updateIntervalSeconds = this.updateIntervalSeconds;
            heatGenerationMap.UpdateHeatGenerationMap();

            airTemperatureComputeShader.SetTexture(airTemperatureKernel, "inputTemperatureGrid", workGrid);
            airTemperatureComputeShader.SetTexture(airTemperatureKernel, "inputThermalConductivityGrid", thermalConductivityMap);
            airTemperatureComputeShader.SetTexture(airTemperatureKernel, "inputHeatGenerationGrid", heatGenerationMap.paintGrid);
            airTemperatureComputeShader.SetTexture(airTemperatureKernel, "outputTemperatureGrid", newTemperatureGrid);

            airTemperatureComputeShader.SetInt("inputTemperatureGridWidth", workGrid.width);
            airTemperatureComputeShader.SetInt("inputTemperatureGridHeight", workGrid.height);

            airTemperatureComputeShader.SetFloat("cellSize", cellSize);
            airTemperatureComputeShader.SetFloat("updateIntervalSeconds", updateIntervalSeconds);
            airTemperatureComputeShader.SetFloat("volumetricHeatCapacity", VOLUMETRIC_HEAT_CAPACITY);
            airTemperatureComputeShader.SetFloat("initialAirTemperature", initialAirTemperature);

            int threadGroupsX = Mathf.CeilToInt(workGrid.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(workGrid.height / 16.0f);
            airTemperatureComputeShader.Dispatch(airTemperatureKernel, threadGroupsX, threadGroupsY, 1);

            Graphics.CopyTexture(newTemperatureGrid, workGrid);

            // float minValue, maxValue;
            // FindMinMax(newTemperatureGrid, out minValue, out maxValue);
            // Debug.Log("GPU: Min: " + minValue + " Max: " + maxValue);

            // Texture2D newTemperatureGridTex2D = RenderTextureToTexture2D(newTemperatureGrid);
            // FindMinMaxCPU(newTemperatureGridTex2D, out minValue, out maxValue);
            // Debug.Log("CPU Min: " + minValue + " Max: " + maxValue);

            newTemperatureGrid.Release();
        }


        private void WorkgridToPaintgrid()
        {
            switch (normalizeBehavior)
            {
                case NormalizeBehaviors.MIN_MAX_NORMALIZE:
                    MinmaxNormalization();
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

            // Texture2D newTemperatureGridTex2D = RenderTextureToTexture2D(workGrid);
            // FindMinMaxCPU(newTemperatureGridTex2D, out minValue, out maxValue);

            NormalizeTexture(workGrid, mPaintGrid, minValue, maxValue);
            // Debug.Log("Min: " + minValue + " Max: " + maxValue);
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
            int kernelHandle = airTemperatureComputeShader.FindKernel("FindMinMax");
            airTemperatureComputeShader.SetTexture(kernelHandle, "inputTexture", inputTexture);
            airTemperatureComputeShader.SetInt("inputTextureWidth", inputTexture.width);
            airTemperatureComputeShader.SetInt("inputTextureHeight", inputTexture.height);
            airTemperatureComputeShader.SetBuffer(kernelHandle, "resultBuffer", resultBuffer);

            // Dispatch the compute shader
            airTemperatureComputeShader.Dispatch(kernelHandle, numThreadsX, numThreadsY, 1);

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
            airTemperatureComputeShader.SetTexture(normalizationKernel, "inputTexture", input);
            airTemperatureComputeShader.SetInt("inputTextureWidth", workGrid.width);
            airTemperatureComputeShader.SetInt("inputTextureHeight", workGrid.height);
            airTemperatureComputeShader.SetTexture(normalizationKernel, "outputTexture", output);
            airTemperatureComputeShader.SetFloat("minValue", minValue);
            airTemperatureComputeShader.SetFloat("maxValue", maxValue);

            int threadGroupsX = Mathf.CeilToInt(input.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(input.height / 16.0f);
            airTemperatureComputeShader.Dispatch(normalizationKernel, threadGroupsX, threadGroupsY, 1);

            // float min, max;

            // FindMinMax(output, out min, out max);
            // Debug.Log("Min: " + min + " Max: " + max);
        }


    }
}