using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality
{
    /**
     * Urban Territoriality Settings
     */
    public class UTSettings : MonoBehaviour
    {
        [Tooltip(@"Weather or not the values in the settings will be applied 
to the components in the scene.")]
        public bool applySettings = true;

        [Header("Path Finding Cost Function Weights")]
        [Tooltip("Weight of collider map when computing cost of a path.")]
        public float colliderWeight = 0.5f;
        [Tooltip("Weight of heatmap when computing the cost of a path.")]
        public float heatmapWeight = 1f;
        [Tooltip("Cost of traveling one meter in a clear area.")]
        public float clearCostPerMeter = 0.1f;
        [Tooltip("Cost of traveling one meter over one heatspace.")]
        public float territoryCostPerMeter = 0.3f;
        [Tooltip("Weight for maximum line ratio in a path.")]
        public float maxLineRatioWeight = 0.01f;
        [Tooltip("Weight for maximum angle of a path.")]
        public float maxAngleWeight = 0.01f;
        [Tooltip("Weight for angle of first line in path relative to an agent.")]
        public float agentAngleWeight = 0.02f;

        [Header("Heatmap")]
        [Tooltip("Size of heatmap. This can not be set in real time.")]
        public Vector2 heatmapSize;
        [Tooltip("Cell size of the heatmap. This can not be set in real time.")]
        public float heatmapCellSize;

        [Header("Collider map")]
        [Tooltip("Size of the collider map. This can not be set in real time.")]
        public Vector2 colliderMapSize;
        [Tooltip("Cell size of the collider map. This can not be set in real time.")]
        public float colliderMapCellSize;
        [Tooltip(@"Padding iterations used to add padding around colliders. The larger this
number the thicker the padding.")]
        public int colliderMapPaddingIterations = 0;
        [Tooltip("Weather or not to use layer mask when doing raycasts for the collider map.")]
        public bool colliderMapUseLayerMask = false;
        [Tooltip("Layer mask to use when doing raycasts for the collider map.")]
        public LayerMask colliderMapLayerMask;

        private TerritorialHeatmap heatmap;
        private ColliderMap colliderMap;
        private PathFollowingAgent[] pathAgents;
        private bool agentsReady = false;

        private void Awake()
        {
            pathAgents = (PathFollowingAgent[])
                GameObject.FindObjectsOfType(typeof(PathFollowingAgent));
            heatmap = (TerritorialHeatmap)
                GameObject.FindObjectOfType(typeof(TerritorialHeatmap));
            colliderMap = (ColliderMap)
                GameObject.FindObjectOfType(typeof(ColliderMap));
        }

        private void Update()
        {
            if (applySettings)
            {
                heatmap.ClearCostPerMeter = clearCostPerMeter;
                heatmap.TerritoryCostPerMeter = territoryCostPerMeter;
                colliderMap.paddingIterations = colliderMapPaddingIterations;
                colliderMap.useLayerMask = colliderMapUseLayerMask;
                colliderMap.layerMask = colliderMapLayerMask;

                if (!agentsReady)
                {
                    agentsReady = true;
                    foreach (PathFollowingAgent ag in pathAgents)
                    {
                        agentsReady = agentsReady && ag.Initialized;
                        if (!agentsReady) break;
                    }
                }

                if (agentsReady)
                {
                    foreach (PathFollowingAgent ag in pathAgents)
                    {
                        ag.PathProvider.colliderWeight = colliderWeight;
                        ag.PathProvider.heatmapWeight = heatmapWeight;
                        ag.PathProvider.maxLineRatioWeight = maxLineRatioWeight;
                        ag.PathProvider.maxAngleWeight = maxAngleWeight;
                        ag.PathProvider.agentAngleWeight = agentAngleWeight;
                    }
                }
            }
        }
    }
}