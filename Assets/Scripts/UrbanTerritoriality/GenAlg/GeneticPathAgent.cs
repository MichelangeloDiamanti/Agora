using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UrbanTerritoriality.Utilities;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.GenAlg
{
    /**
     * An agent that uses a GeneticPathGenerator
     * to find the best path to travel from one
     * poin in a world to another.
     */
    [RequireComponent(typeof(MovableAgent))]
    public class GeneticPathAgent : MonoBehaviour
    {
        /**
         * A list of waypoints to travel between
         * randomly.
         */
        public WaypointList waypointList;

        /** path generator for creating a path that the agent will try to follow */
        private GeneticPathGenerator pathGenerator;

        /** The heatmap zones belonging to this agent */
        public HeatSpace[] ownHeatSpaces = null;

        /** Territorial heatmap used for creating the path */
        public TerritorialHeatmap heatmap;

        /** Reference to a GeneticPathApplication to be used
         * in this object. */
        public GeneticPathApplication gpApp;

        /** A collider map used for creating the path*/
        public ColliderMap colliderMap;

        /** A component used for drawing lines */
        public MultilineDrawer lineDrawer;

        /** Movable agent used for controlling the motion of the agent */
        public MovableAgent moveAgent;

        /** Weather the agent has been initialized */
        private bool initialized = false;

        /** Distance tolerance for moving towards a location.
         * The agent will move towards a position until
         * the distance to it is less than this. */
        private float distanceTolerance = 1f;

        /** Unity Start method */
        void Start()
        {
            /** Get MovableAgent used for moving the GameObject */
            moveAgent = GetComponent<MovableAgent>();
            moveAgent.maxRotSpeed = 180;
            moveAgent.maxSpeed = 2;
            moveAgent.deceleration = 0;
            moveAgent.speed = moveAgent.maxSpeed;

            if (gpApp == null)
            {
                Debug.Log("Warning: gpApp is null");
            }
        }

        /** Unity Update method */
        void Update()
        {
            if (heatmap.Initialized &&
                colliderMap.Initialized &&
                !initialized)
            {
                /* Create new GeneticPathGenerator to use for pathfinding */
                pathGenerator = new GeneticPathGenerator(heatmap, gameObject, 10, 30);
                pathGenerator.colliderMap = colliderMap;
                pathGenerator.lineDrawer = lineDrawer;
                pathGenerator.ownHeatSpaces = ownHeatSpaces;
                pathGenerator.InitPopulation();
                Vector3 waypoint = waypointList.GetRandomWaypoint().transform.position;
                pathGenerator.end = new Vector2(waypoint.x, waypoint.z);
                initialized = true;
            }

            if (gpApp != null)
            {
                /* Update some values */
                pathGenerator.colliderWeight = gpApp.colliderWeight;
                pathGenerator.heatmapWeight = gpApp.heatmapWeight;
                pathGenerator.maxLineRatioWeight = gpApp.maxLineRatioWeight;
                pathGenerator.maxAngleWeight = gpApp.maxAngleWeight;
                pathGenerator.agentAngleWeight = gpApp.agentAngleWeight;
            }

            Vector3 start3 = transform.position;
            pathGenerator.start = new Vector2(start3.x, start3.z);
            pathGenerator.PassOneGeneration();

            /* Raycast to see how much space is in front of the agent */
            float acceleration = 3;

            float maxSpeed = moveAgent.maxSpeed;
            float desiredSpeed = maxSpeed;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward,
                out hit, 1000))
            {
                Color lineCol = new Color(1, 0.5f, 0, 1);
                float minDist = 4;
                float hitDist = hit.distance;
                if (hitDist < minDist)
                {
                    float frac = hitDist / minDist;
                    frac = Mathf.Clamp(frac, 0.2f, 1f);
                    desiredSpeed = maxSpeed * frac;
                    lineCol = new Color(0, 0.5f, 1, 1);
                }
                Debug.DrawLine(transform.position,
                    hit.point,
                    lineCol);
            }
            if (moveAgent.speed > desiredSpeed)
            {
                acceleration = -acceleration;
            }

            /* Update movement */
            PathDNA path = pathGenerator.GetBestPath();
            Vector2[] totalPath = pathGenerator.GetTotalPath(path);
            Vector2 agentPos = new Vector2(transform.position.x, transform.position.z);
            Vector2 agentForward = new Vector2(transform.forward.x, transform.forward.z);
            int n = totalPath.Length;
            for (int i = 1; i < n; i++)
            {
                Vector2 toPoint = totalPath[i] - agentPos;
                float angle = Vector2.SignedAngle(agentForward, toPoint);
                float sign = angle >= 0 ? -1 : 1;
                moveAgent.maxRotSpeed = Mathf.Abs(angle * 4f);
                moveAgent.angularAcceleration = sign * 1000f;

                float dist = Vector2.Distance(agentPos, totalPath[i]);
                if (dist > distanceTolerance)
                {
                    moveAgent.acceleration = acceleration;
                    break;
                }
                else
                {
                    moveAgent.acceleration = 0;
                    if (i == n - 1)
                    {
                        Vector3 waypoint = waypointList.GetRandomWaypoint().transform.position;
                        pathGenerator.end = new Vector2(waypoint.x, waypoint.z);
                    }
                }
            }
        }
    }
}
