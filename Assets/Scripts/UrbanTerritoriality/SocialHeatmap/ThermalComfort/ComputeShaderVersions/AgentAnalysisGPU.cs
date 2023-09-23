using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

namespace UrbanTerritoriality.Maps
{

    public class AgentAnalysisGPU : GPUHeatmap
    {
        struct ThreadResult
        {
            public float minValue; // minimum red value in the portion processed by this thread
            public float maxValue; // maximum red value in the portion processed by this thread
        }

        public float updateIntervalSeconds = 10f;

        public bool updateOnDemand = false;

        public NormalizeBehaviors normalizeBehavior;

        private float updateTimer = 0;


        public ComputeShader agentAnalysisComputeShader;

        private Texture2D agentData; // contains xy position and heat generation rate/radius of agents
        private int agentAnalysisKernel;
        private int minMaxKernel;
        private int normalizationKernel;


        private RenderTexture workGrid;

        [SerializeField]
        private int subscribedAgentsCount;

        private List<Vector2Int> agentUpdateMapPositions;


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
            agentAnalysisKernel = agentAnalysisComputeShader.FindKernel("UpdateAgentTrajectories");
            minMaxKernel = agentAnalysisComputeShader.FindKernel("FindMinMax");
            normalizationKernel = agentAnalysisComputeShader.FindKernel("NormalizeTexture");

            agentUpdateMapPositions = new List<Vector2Int>();
            // Register to global event on-agent-spawned. Get notified everytime
            // a new agent is created into the simulation.
            SimulationManager.Instance.OnAgentSpawned.AddListener(OnAgentSpawnedHandler);
            SimulationManager.Instance.OnAgentDeSpawned.AddListener(OnAgentDeSpawnedHandler);


            convergenceTimer = 0;
            base.currentTime = 0;
        }


        private void UpdateCellPosition(ChangeInPositionEventDataStructure positionData)
        {
            Vector2Int mapOldPosition = WorldToGridPos(new Vector2(positionData.OldPosition.x, positionData.OldPosition.z));
            Vector2Int mapNewPosition = WorldToGridPos(new Vector2(positionData.NewPosition.x, positionData.NewPosition.z));
            if (mapNewPosition != mapOldPosition)
            {
                agentUpdateMapPositions.Add(mapNewPosition);
            }
        }

        private void OnAgentSpawnedHandler(TrackedAgent agent)
        {
            // Listen to each agent's UpdatePosition Event, when they notify
            // a new buffer of positions we update the map
            // agent.OnNewPositionsBuffer.AddListener(UpdateCellPositionWithLines);
            // Debug.Log("Subscribed to UpdateCellPosition event");
            agent.OnNewPosition.AddListener(UpdateCellPosition);
            subscribedAgentsCount++;
        }

        private void OnAgentDeSpawnedHandler(TrackedAgent agent)
        {
            agent.OnNewPosition.RemoveListener(UpdateCellPosition);
            subscribedAgentsCount--;
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
                    ComputeAgentAnalysisMap();
                    WorkgridToPaintgrid();
                }
            }
        }

        public void UpdateHeatGenerationMap()
        {
            ComputeAgentAnalysisMap();
            WorkgridToPaintgrid();
        }

        private void ComputeAgentAnalysisMap()
        {
            agentData.Reinitialize(agentUpdateMapPositions.Count, 1);

            int i = 0;
            foreach (Vector2Int position in agentUpdateMapPositions)
            {
                agentData.SetPixel(i, 0, new Color(position.x, position.y, 0.1f, 1.0f));
                i++;
            }
            agentData.Apply();



            // Set shader parameters
            agentAnalysisComputeShader.SetTexture(agentAnalysisKernel, "heatGenerationMap", workGrid);
            agentAnalysisComputeShader.SetInt("heatGenerationMapWidth", workGrid.width);
            agentAnalysisComputeShader.SetInt("heatGenerationMapHeight", workGrid.height);
            agentAnalysisComputeShader.SetTexture(agentAnalysisKernel, "agentData", agentData);
            agentAnalysisComputeShader.SetInt("agentCount", agentUpdateMapPositions.Count);

            agentUpdateMapPositions.Clear();

            // Dispatch the shader
            int threadGroupsX = Mathf.CeilToInt(workGrid.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(workGrid.height / 16.0f);
            agentAnalysisComputeShader.Dispatch(agentAnalysisKernel, threadGroupsX, threadGroupsY, 1);
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
            agentAnalysisComputeShader.SetTexture(minMaxKernel, "inputTexture", inputTexture);
            agentAnalysisComputeShader.SetInt("inputTextureWidth", inputTexture.width);
            agentAnalysisComputeShader.SetInt("inputTextureHeight", inputTexture.height);
            agentAnalysisComputeShader.SetBuffer(minMaxKernel, "resultBuffer", resultBuffer);

            // Dispatch the compute shader
            agentAnalysisComputeShader.Dispatch(minMaxKernel, numThreadsX, numThreadsY, 1);

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
            agentAnalysisComputeShader.SetTexture(normalizationKernel, "inputTexture", input);
            agentAnalysisComputeShader.SetInt("inputTextureWidth", workGrid.width);
            agentAnalysisComputeShader.SetInt("inputTextureHeight", workGrid.height);
            agentAnalysisComputeShader.SetTexture(normalizationKernel, "outputTexture", output);
            agentAnalysisComputeShader.SetFloat("minValue", minValue);
            agentAnalysisComputeShader.SetFloat("maxValue", maxValue);

            int threadGroupsX = Mathf.CeilToInt(input.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(input.height / 16.0f);
            agentAnalysisComputeShader.Dispatch(normalizationKernel, threadGroupsX, threadGroupsY, 1);
        }


    }
}