using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

namespace UrbanTerritoriality.Maps
{

    struct AgentInfo
    {
        public Vector2 gridPosition;
        public float discomfortRadius;
        public float discomfort;
    }

    public class DensityComfortGPU : GPUHeatmap
    {
        public GameObject agentsRoot;

        public float updateIntervalSeconds = 10f;

        public bool updateOnDemand = false;

        public NormalizeBehaviors normalizeBehavior;

        private float normalizationRangeMin;
        private float normalizationRangeMax;

        private float updateTimer = 0;


        public ComputeShader densityComfortComputeShader;

        private Texture2D agentPositions;
        private int densityComfortKernel;
        private int minMaxKernel;
        private int normalizationKernel;

        // Parameters
        public float Beta = 0.5f; // decay factor for personal space (how less w.r.t. intimate space)

        // According to Scheflen, the radius of intimate space is 0.5m and the radius of personal space is 1.5m
        public float intimateSpaceRadius = 1.5f;
        public float personalSpaceRadius = 3f;

        // There were arbitrarily set
        private const float Mi = 4; // Set the maximum number of agents for intimate space
        private const float Mp = 12; // Set the maximum number of agents for personal space

        private const float DiscomfortRadius = 3.0f; // radius of discomfort circle (meters) painted on the map by the agent


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
            agentPositions = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);

            // Get the kernel index for heat generation shader
            densityComfortKernel = densityComfortComputeShader.FindKernel("DrawDiscomfort");
            minMaxKernel = densityComfortComputeShader.FindKernel("FindMinMax");
            normalizationKernel = densityComfortComputeShader.FindKernel("NormalizeTexture");

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
                    normalizationRangeMin = 0.0f;
                    normalizationRangeMax = 100.0f;
                    ComputeDensityComfortMap();
                    WorkgridToPaintgrid();
                }
            }
        }

        public void UpdateHeatGenerationMap()
        {
            ComputeDensityComfortMap();
            WorkgridToPaintgrid();
        }

        private void ComputeDensityComfortMap()
        {
            // Create a list to store agent information
            List<AgentInfo> agentInfoList = new List<AgentInfo>();


            List<GameObject> agentObjects = new List<GameObject>();
            foreach (Transform child in agentsRoot.transform)
            {
                agentObjects.Add(child.gameObject);
            }

            // Iterate over the agents and update the grid with the calculated PPDd
            foreach (GameObject agentObject in agentObjects)
            {
                Vector2 agentPosition = new Vector2(agentObject.transform.position.x, agentObject.transform.position.z);

                // Count agents within intimate and personal spaces
                int ni = 0;
                int np = 0;

                foreach (GameObject otherAgentObject in agentObjects)
                {
                    if (agentObject == otherAgentObject)
                    {
                        continue;
                    }

                    Vector2 otherAgentPosition = new Vector2(otherAgentObject.transform.position.x, otherAgentObject.transform.position.z);
                    float distance = Vector2.Distance(agentPosition, otherAgentPosition);

                    if (distance <= intimateSpaceRadius)
                    {
                        ni++;
                    }
                    else if (distance <= personalSpaceRadius)
                    {
                        np++;
                    }
                }

                // Calculate PPDd for this agent's position using the CalculateDensityDiscomfort() method
                float PPDd = CalculateDensityDiscomfort(ni, np);

                // Update the corresponding grid cell
                Vector2Int gridPosition = WorldToGridPos(agentPosition);
                agentInfoList.Add(new AgentInfo { gridPosition = new Vector2(gridPosition.x, gridPosition.y), discomfortRadius = DiscomfortRadius, discomfort = PPDd });
            }


            // Create the agentInfoBuffer and set data
            ComputeBuffer agentInfoBuffer = new ComputeBuffer(agentInfoList.Count, sizeof(float) * 4);
            agentInfoBuffer.SetData(agentInfoList.ToArray());

            // Set the compute shader parameters
            densityComfortComputeShader.SetInt("discomfortMapWidth", workGrid.width);
            densityComfortComputeShader.SetInt("discomfortMapHeight", workGrid.height);
            densityComfortComputeShader.SetBuffer(densityComfortKernel, "agentInfoBuffer", agentInfoBuffer);
            densityComfortComputeShader.SetTexture(densityComfortKernel, "discomfortMap", workGrid);
            densityComfortComputeShader.SetInt("agentCount", agentObjects.Count);

            // Dispatch the compute shader
            int threadGroupsX = Mathf.CeilToInt(workGrid.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(workGrid.height / 16.0f);
            densityComfortComputeShader.Dispatch(densityComfortKernel, threadGroupsX, threadGroupsY, 1);

            // Release the agentInfoBuffer
            agentInfoBuffer.Release();
        }

        public float CalculateDensityDiscomfort(int ni, int np)
        {
            float PPDd = 100 * ((float)ni + Beta * (float)np) / ((float)Mi + Beta * (float)Mp);
            return PPDd;
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
            // Debug.Log("Min: " + minValue + " Max: " + maxValue);
        }

        private void RangeNormalization()
        {
            NormalizeTexture(workGrid, mPaintGrid, normalizationRangeMin, normalizationRangeMax);
        }

        void FindMinMax(RenderTexture input, out float minValue, out float maxValue)
        {
            int blockSize = 16;
            int threadGroupsX = Mathf.CeilToInt(input.width / (float)blockSize);
            int threadGroupsY = Mathf.CeilToInt(input.height / (float)blockSize);

            ComputeBuffer minValuesBuffer = new ComputeBuffer(blockSize * blockSize, sizeof(float));
            ComputeBuffer maxValuesBuffer = new ComputeBuffer(blockSize * blockSize, sizeof(float));

            densityComfortComputeShader.SetTexture(minMaxKernel, "inputTexture", input);
            densityComfortComputeShader.SetInt("inputTextureWidth", input.width);
            densityComfortComputeShader.SetInt("inputTextureHeight", input.height);
            densityComfortComputeShader.SetBuffer(minMaxKernel, "minValues", minValuesBuffer);
            densityComfortComputeShader.SetBuffer(minMaxKernel, "maxValues", maxValuesBuffer);

            densityComfortComputeShader.Dispatch(minMaxKernel, threadGroupsX, threadGroupsY, 1);

            float[] minValues = new float[blockSize * blockSize];
            float[] maxValues = new float[blockSize * blockSize];

            minValuesBuffer.GetData(minValues);
            maxValuesBuffer.GetData(maxValues);

            minValue = Mathf.Min(minValues);
            maxValue = Mathf.Max(maxValues);

            minValuesBuffer.Release();
            maxValuesBuffer.Release();
        }


        void NormalizeTexture(RenderTexture input, RenderTexture output, float minValue, float maxValue)
        {
            densityComfortComputeShader.SetTexture(normalizationKernel, "inputTexture", input);
            densityComfortComputeShader.SetInt("inputTextureWidth", workGrid.width);
            densityComfortComputeShader.SetInt("inputTextureHeight", workGrid.height);
            densityComfortComputeShader.SetTexture(normalizationKernel, "outputTexture", output);
            densityComfortComputeShader.SetFloat("minValue", minValue);
            densityComfortComputeShader.SetFloat("maxValue", maxValue);

            int threadGroupsX = Mathf.CeilToInt(input.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(input.height / 16.0f);
            densityComfortComputeShader.Dispatch(normalizationKernel, threadGroupsX, threadGroupsY, 1);
        }


    }
}