using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.GenAlg
{
    /**
     * A class containing some things that are central
     * to an application showing path generation.
     */
    public class GeneticPathApplication : MonoBehaviour
    {
        /** The territorial heatmap used in the scene */
        public TerritorialHeatmap heatmap;

        /** Weight for the ColliderMap in
         * cost function */
        public float colliderWeight = 3f;

        /** Weight for the TerritorialHeatmap
         * the cost function. */
        public float heatmapWeight = 1f;

        /** Weight for maximum line ratio
         * in the cost function.
         * Maximum line ratio here, means the ratio
         * between the longest and shortest
         * line in a path.
         * */
        public float maxLineRatioWeight = 0.01f;

        /** Weight for the maximum angle in
         * the cost function.
         * Maximum angle here, means the maximum
         * angle between two adjacent lines in
         * a path.
         */
        public float maxAngleWeight = 0.01f;

        /** Weight for agent angle in cost function.
         * Here agent angle means the angle between
         * the first line in a path and the forward
         * vector of the agent traveling along the path.
         */
        public float agentAngleWeight = 0.02f;

        /** The cost for an agent to travel one meter
         * Where there are no obstacles or territorial
         * spaces.
         */
        public float clearCostPerMeter = 0.1f;

        /** Cost for the agent to travel one meter
         * over a HeatSpace.
         * */
        public float territoryCostPerMeter = 0.3f;

        /** PathFollowingAgents in the scene.
         * Some members of them are set in a method
         * of this class */
        private PathFollowingAgent[] pathAgents;

        /** Weather all the PathFollowingAgents have been
         * initialized */
        private bool agentsReady = false;

        /** Unity Awake method */
        private void Awake()
        {
            pathAgents = (PathFollowingAgent[])
                GameObject.FindObjectsOfType(typeof(PathFollowingAgent));
        }

        /** Unity Update method */
        private void Update()
        {
            heatmap.ClearCostPerMeter = clearCostPerMeter;
            heatmap.TerritoryCostPerMeter = territoryCostPerMeter;
            
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
