using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

namespace UrbanTerritoriality.Maps
{

    public class ShadowsGPU : GPUHeatmap
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


        public ComputeShader shadowsComputeShader;

        private int minMaxKernel;
        private int normalizationKernel;


        public RenderTexture shadowMap;

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

            // shadowMap = new RenderTexture(resX, resY, 0, RenderTextureFormat.RFloat);
            // shadowMap.enableRandomWrite = true;
            // shadowMap.Create();


            minMaxKernel = shadowsComputeShader.FindKernel("FindMinMax");
            normalizationKernel = shadowsComputeShader.FindKernel("NormalizeTexture");

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
                    ComputeShadowMap();
                    WorkgridToPaintgrid();
                }
            }
        }

        public void UpdateHeatGenerationMap()
        {
            ComputeShadowMap();
            WorkgridToPaintgrid();
        }

        private void ComputeShadowMap()
        {

        }

        private void WorkgridToPaintgrid()
        {
            switch (normalizeBehavior)
            {
                case NormalizeBehaviors.MIN_MAX_NORMALIZE:
                    MinmaxNormalization();
                    break;
                default:
                    Graphics.CopyTexture(shadowMap, mPaintGrid);
                    break;
            }
        }

        private void MinmaxNormalization()
        {
            float minValue, maxValue;
            FindMinMax(shadowMap, out minValue, out maxValue);
            NormalizeTexture(shadowMap, mPaintGrid, minValue, maxValue);
            // Debug.Log("Min: " + minValue + " Max: " + maxValue);
        }

        void FindMinMax(Texture inputTexture, out float minValue, out float maxValue)
        {
            // Create the result buffer with the same length as the number of threads
            int numThreadsX = Mathf.CeilToInt((float)inputTexture.width / 16);
            int numThreadsY = Mathf.CeilToInt((float)inputTexture.height / 16);
            ComputeBuffer resultBuffer = new ComputeBuffer(numThreadsX * numThreadsY, sizeof(float) * 2);

            // Set the compute shader parameters
            shadowsComputeShader.SetTexture(minMaxKernel, "inputTexture", inputTexture);
            shadowsComputeShader.SetInt("inputTextureWidth", inputTexture.width);
            shadowsComputeShader.SetInt("inputTextureHeight", inputTexture.height);
            shadowsComputeShader.SetBuffer(minMaxKernel, "resultBuffer", resultBuffer);

            // Dispatch the compute shader
            shadowsComputeShader.Dispatch(minMaxKernel, numThreadsX, numThreadsY, 1);

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
            shadowsComputeShader.SetTexture(normalizationKernel, "inputTexture", input);
            shadowsComputeShader.SetInt("inputTextureWidth", shadowMap.width);
            shadowsComputeShader.SetInt("inputTextureHeight", shadowMap.height);
            shadowsComputeShader.SetTexture(normalizationKernel, "outputTexture", output);
            shadowsComputeShader.SetFloat("minValue", minValue);
            shadowsComputeShader.SetFloat("maxValue", maxValue);

            int threadGroupsX = Mathf.CeilToInt(input.width / 16.0f);
            int threadGroupsY = Mathf.CeilToInt(input.height / 16.0f);
            shadowsComputeShader.Dispatch(normalizationKernel, threadGroupsX, threadGroupsY, 1);
        }


    }
}