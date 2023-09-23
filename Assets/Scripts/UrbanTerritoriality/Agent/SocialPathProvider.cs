using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.Agent
{
    /** A class that defines an
     interface for classes that provide path
     following. */
    public abstract class SocialPathProvider : MonoBehaviour
    {
        /** A heatmap to be used for social
         * path following. */
        public TerritorialHeatmap heatmap;

        /** A collider map to be used for social
         path following. */
        public ColliderMap colliderMap;

        /* Heatmap zones belonging to this agent */
        public HeatSpace[] ownHeatSpaces = null;

        /** The GameObject of the agent that will
         * move along the path */
        public GameObject agentGameObject;

        /** The agent that is using this path provider */
        public PathFollowingAgent pathFollowingAgent;

        /** Some weights used for calculating the cost of a path */
        public float colliderWeight = 3f;

        /** Weight of the heatmap when computing the
         * path cost */
        public float heatmapWeight = 1f;

        /** Weight for maximum line ration when
         * computing the path cost. */
        public float maxLineRatioWeight = 0.01f;

        /** Weight for maximum angle when
         * computing the path cost.
         */
        public float maxAngleWeight = 0.01f;

        /** Weight for agent angle */
        public float agentAngleWeight = 0.02f;

        /** Weather this SocialPathProvider has been
         * initialized or not.
         */
        public bool Initialized
        { get { return initialized; } }
        protected bool initialized = false;

        /**
         * Do necessary initialization
         */
        protected abstract void initialize();

        /** Perform initialization
         * initialize is called inside this method.
         * Child classes that want to do some initialization
         * tasks should implement the initialize method.
         */
        public void Initialize()
        {
            initialize();
            initialized = true;
        }

        /** This should return the best path found by this path provider
         * from start to end.
         * @param start Where the path begins.
         * @param end The destination to find a path towards.
         * @return All the points in the path including the start and
         * end points.
         */
        public abstract Vector2[] GetBestPath(Vector2 start, Vector2 end);
    }
}
