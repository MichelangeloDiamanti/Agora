using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

namespace UrbanTerritoriality.Maps
{


    public class DensityComfortMap : GeneralHeatmap
    {
        public GameObject agentsRoot;
        public float updateIntervalSeconds = 10f;

        public NormalizeBehaviors normalizeBehavior;

        private PaintGrid workGrid;

        private float updateTimer = 0;

        // Parameters
        public float Beta = 0.5f; // decay factor for personal space (how less w.r.t. intimate space)

        // According to Scheflen, the radius of intimate space is 0.5m and the radius of personal space is 1.5m
        public float intimateSpaceRadius = 0.5f; 
        public float personalSpaceRadius = 1.5f;

        // There were arbitrarily set
        private const float Mi = 4; // Set the maximum number of agents for intimate space
        private const float Mp = 12; // Set the maximum number of agents for personal space

        private const float DiscomfortRadius = 3.0f; // radius of discomfort circle (meters) painted on the map by the agent

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
                UpdateDensityComfortMap();

                switch (normalizeBehavior)
                {
                    case NormalizeBehaviors.COPY_CONTENT:
                        CopyWorkGridToPaintGrid();
                        break;
                    case NormalizeBehaviors.RANGE_NORMALIZE:
                        meanChange = Utilities.Util.RangeNormalization(workGrid, mPaintGrid, 0f, 100f);
                        break;
                    case NormalizeBehaviors.MIN_MAX_NORMALIZE:
                        meanChange = Utilities.Util.MinMaxNormalization(workGrid, mPaintGrid);
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateDensityComfortMap()
        {
            // Clear workGrid
            // workGrid.Clear(0.0f);

            // Get all agent objects
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
                workGrid.DrawEllipse(PPDd, PaintGrid.PaintMethod.ADD, gridPosition.x, gridPosition.y, DiscomfortRadius, DiscomfortRadius, 0);
            }


        }

        public float CalculateDensityDiscomfort(int ni, int np)
        {
            float PPDd = 100 * ((float)ni + Beta * (float)np) / ((float)Mi + Beta * (float)Mp);
            return PPDd;
        }
    }
}
