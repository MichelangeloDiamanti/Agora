using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

namespace UrbanTerritoriality.Maps
{

    public class HeatGenerationGPU : GPUHeatmap
    {
        struct ThreadResult
        {
            public float minValue; // minimum red value in the portion processed by this thread
            public float maxValue; // maximum red value in the portion processed by this thread
        }

        public GameObject agentsRoot;
        public GameObject heatGeneratorsRoot;

        public float updateIntervalSeconds = 10f;

        public bool updateOnDemand = false;

        public NormalizeBehaviors normalizeBehavior;

        private float updateTimer = 0;


        public ComputeShader heatGenerationComputeShader;

        private Texture2D agentData; // contains xy position and heat generation rate/radius of agents
        private int heatGenerationKernel;
        private int minMaxKernel;
        private int normalizationKernel;


        private RenderTexture workGrid;

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


            // Initialize the Texture2D for agentPositions
            agentData = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);

            // Get the kernel index for heat generation shader
            heatGenerationKernel = heatGenerationComputeShader.FindKernel("UpdateHeatGeneration");
            minMaxKernel = heatGenerationComputeShader.FindKernel("FindMinMax");
            normalizationKernel = heatGenerationComputeShader.FindKernel("NormalizeTexture");

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
                    ComputeHeatGenerationMap();
                    WorkgridToPaintgrid();
                }
            }
        }

        public void UpdateHeatGenerationMap()
        {
            ComputeHeatGenerationMap();
            WorkgridToPaintgrid();
        }

        private void ComputeHeatGenerationMap()
        {
            // Get HeatSource components from agentsRoot and heatGeneratorsRoot
            List<HeatSource> heatSources = new List<HeatSource>(agentsRoot.GetComponentsInChildren<HeatSource>());
            heatSources.AddRange(heatGeneratorsRoot.GetComponentsInChildren<HeatSource>());

            agentData.Reinitialize(heatSources.Count, 1);

            // Update agentPositions texture with agent positions
            for (int i = 0; i < heatSources.Count; i++)
            {
                HeatSource heatSource = heatSources[i];
                Vector2Int center = WorldToGridPos(new Vector2(heatSources[i].transform.position.x, heatSources[i].transform.position.z));
                agentData.SetPixel(i, 0, new Color(center.x, center.y, heatSource.heatGenerationRate, heatSource.heatGenerationRadius / cellSize));
            }
            agentData.Apply();

            // Set shader parameters
            heatGenerationComputeShader.SetTexture(heatGenerationKernel, "heatGenerationMap", workGrid);
            heatGenerationComputeShader.SetInt("heatGenerationMapWidth", workGrid.width);
            heatGenerationComputeShader.SetInt("heatGenerationMapHeight", workGrid.height);
            heatGenerationComputeShader.SetTexture(heatGenerationKernel, "agentData", agentData);
            // heatGenerationComputeShader.SetTexture(heatGenerationKernel, "agentPositions", agentPositions);
            // heatGenerationComputeShader.SetFloat("heatGenerationRate", heatSources[0].heatGenerationRate);
            // heatGenerationComputeShader.SetFloat("heatGenerationRadius", heatSources[0].heatGenerationRadius / cellSize);
            heatGenerationComputeShader.SetInt("agentCount", heatSources.Count);

            // Dispatch the shader
            int threadGroupsX = Mathf.CeilToInt(workGrid.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(workGrid.height / 16.0f);
            heatGenerationComputeShader.Dispatch(heatGenerationKernel, threadGroupsX, threadGroupsY, 1);
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
            NormalizeTexture(workGrid, mPaintGrid, minValue, maxValue);
            // Debug.Log("Min: " + minValue + " Max: " + maxValue);
        }

        void FindMinMax(Texture inputTexture, out float minValue, out float maxValue)
        {
            // Create the result buffer with the same length as the number of threads
            int numThreadsX = Mathf.CeilToInt((float)inputTexture.width / 16);
            int numThreadsY = Mathf.CeilToInt((float)inputTexture.height / 16);
            ComputeBuffer resultBuffer = new ComputeBuffer(numThreadsX * numThreadsY, sizeof(float) * 2);

            // Set the compute shader parameters
            heatGenerationComputeShader.SetTexture(minMaxKernel, "inputTexture", inputTexture);
            heatGenerationComputeShader.SetInt("inputTextureWidth", inputTexture.width);
            heatGenerationComputeShader.SetInt("inputTextureHeight", inputTexture.height);
            heatGenerationComputeShader.SetBuffer(minMaxKernel, "resultBuffer", resultBuffer);

            // Dispatch the compute shader
            heatGenerationComputeShader.Dispatch(minMaxKernel, numThreadsX, numThreadsY, 1);

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
            heatGenerationComputeShader.SetTexture(normalizationKernel, "inputTexture", input);
            heatGenerationComputeShader.SetInt("inputTextureWidth", workGrid.width);
            heatGenerationComputeShader.SetInt("inputTextureHeight", workGrid.height);
            heatGenerationComputeShader.SetTexture(normalizationKernel, "outputTexture", output);
            heatGenerationComputeShader.SetFloat("minValue", minValue);
            heatGenerationComputeShader.SetFloat("maxValue", maxValue);

            int threadGroupsX = Mathf.CeilToInt(input.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(input.height / 16.0f);
            heatGenerationComputeShader.Dispatch(normalizationKernel, threadGroupsX, threadGroupsY, 1);
        }


    }
}