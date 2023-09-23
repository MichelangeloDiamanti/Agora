using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.GenAlg;
using UrbanTerritoriality.Utilities;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.Agent
{
    /** An enum for different path following methods */
    public enum PathFollowingMethod { AstarGrid, GeneticPath }

    /** A script for an agent that can follow a
     * path created by a special path providing component.
     */
    [RequireComponent(typeof(MovableAgent))]
    public class PathFollowingAgent : MonoBehaviour
    {
        /** Gets called when the agent reashes the destination */
        public event System.Action destinationReached;

        /** Path finding algorithm to use */
        public PathFollowingMethod algorithm;

        /** The current destination that the agent is trying to go to */
        private Vector2 destination;

        /** Set the destination to move the agent to. */
        public void SetDestination(Vector2 dest)
        {
            destination = dest;
        }

        /** path generator for creating a path that the agent will try to follow */
        public SocialPathProvider PathProvider
        { get { return pathProvider; } }
        private SocialPathProvider pathProvider;

        /** Territorial heatmap used for creating the path */
        public TerritorialHeatmap heatmap;

        /** A collider map used for creating the path*/
        public ColliderMap colliderMap;

        /** HeatSpaces belonging to this agent */
        public HeatSpace[] ownHeatSpaces = null;

        /** A component used for drawing lines in the scene */
        public MultilineDrawer lineDrawer;

        /** Movable agent used for controlling the motion of the agent */
        public MovableAgent moveAgent;

        /** Weather or not this PathFollowingAgent has been
         * initialized */
        public bool Initialized
        { get { return initialized; } }
        private bool initialized = false;

        /** Distance tolerance when traveling to a destination.
         * The agent will stop moving when the distance to
         * the destination is less than this.
         * */
        public float distanceTolerance = 1f;

        /** A layer mask to use when doing raycasting
         * on colliders in the environment.
         * The agents performs raycasts in order to sense how
         * much space is in front of him.
         * */
        public LayerMask raycastLayerMask;

        /** Weather or not movement is turned on or not */
        public bool TurnedOn
        {
            get { return turnedOn; }
        }
        private bool turnedOn = false;

        /** Turn on movement of the agent */
        public void TurnOn()
        {
            turnedOn = true;
            moveAgent.running = true;
            moveAgent.deceleration = 0f;
            moveAgent.angularDeceleration = 0f;
        }

        /** Turn off movement of the agent */
        public void TurnOff()
        {
            turnedOn = false;
            moveAgent.deceleration = 7f;
            moveAgent.angularDeceleration = 50f;
            moveAgent.acceleration = 0;
            moveAgent.angularAcceleration = 0;
        }

        /**
         * Draw a path in the Unity scene in play mode
         * in the current frame.
         * @param path The path to draw.
         * @param color The color of the drawn path.
         */
        void DrawPathGL(Vector2[] points, Color color)
        {
            if (lineDrawer == null)
            {
                Debug.LogError("Warning MultilineDrawer missing in GeneticPathGenerator!");
            }
            Vector3[] points3d = Util.ConvertToPath3d(points, transform.position.y);
            lineDrawer.DrawMultiline(points3d, color);
        }

        /** Unity Start */
        void Start()
        {
            moveAgent = GetComponent<MovableAgent>();
            moveAgent.maxRotSpeed = 180;
            moveAgent.maxSpeed = 2;
            moveAgent.deceleration = 0;
            moveAgent.speed = moveAgent.maxSpeed;
        }

        /** 
         * Gets a path and moves the agent
         * using moveAgent in the direction of the path.
         */
        private void UpdateMovement()
        {
            Vector3 start3d = transform.position;
            Vector2 start2d = new Vector2(start3d.x, start3d.z);

            /* Raycast to see how much space is in front of the agent */
            float acceleration = 3;

            float maxSpeed = moveAgent.maxSpeed;
            float desiredSpeed = maxSpeed;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward,
                out hit, 1000, raycastLayerMask))
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
            Vector2[] totalPath = pathProvider.GetBestPath(start2d, destination);
            DrawPathGL(totalPath, new Color(0, 1, 0, 1));
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
                        if (destinationReached != null)
                        {
                            destinationReached();
                        }
                    }
                }
            }
        }

        /**
         * Do some initialization
         */
        public void Initialize()
        {
            if (algorithm == PathFollowingMethod.GeneticPath)
            {
                pathProvider =
                    gameObject.AddComponent<GeneticPathProvider>()
                    as GeneticPathProvider;
            }
            else if (algorithm == PathFollowingMethod.AstarGrid)
            {
                /* Add A* path finding provider here */
                // pathProvider = astar provider
            }
            pathProvider.colliderMap = colliderMap;
            pathProvider.heatmap = heatmap;
            pathProvider.ownHeatSpaces = ownHeatSpaces;
            pathProvider.agentGameObject = gameObject;
            pathProvider.pathFollowingAgent = this;
            pathProvider.Initialize();
            initialized = true;
        }

        /** Unity update */
        void Update()
        {
            if (heatmap.Initialized &&
                colliderMap.Initialized &&
                !initialized)
            {
                Initialize();
            }

            if (turnedOn) UpdateMovement();
        }
    }
}
