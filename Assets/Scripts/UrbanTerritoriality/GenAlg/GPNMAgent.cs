using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.GenAlg
{
    /** GPNMAgent stands for
     * Genetic Path Nav Mesh Agent
     */
    public class GPNMAgent : MonoBehaviour {

        /** A PathFollowingAgent used for path following */
        public PathFollowingAgent pathAgent;

        /** The destination of the agent */
        public Transform Destination
        {
            get { return destination; }
            set {
                destination = value;
                destReached = false;
            }
        }
        private Transform destination;

        /** Weather or not the agent has reached the destination */
        private bool destReached = false;

        /** An event that is invoked when the agent
         * reached the destination */
        public System.Action destinationReached;

        /** A nav mesh path that is created for the agent
         * so it can reach the destination */
        private UnityEngine.AI.NavMeshPath path;

        /** The distance to the destination when
         * the agent will stop moving towards it */
        public float distanceTolerance = 0.5f;

        /** Weather or not the agent has been initialized. */
        public bool Initialized
        { get { return initialized; } }
        private bool initialized = false;

        /** Unity Update method */
        void Update()
        {
            if (pathAgent.Initialized && !initialized)
            {
                /* Initialize nav mesh path and set distance tolerance
                 * of pathAgent */
                path = new UnityEngine.AI.NavMeshPath();
                pathAgent.distanceTolerance = distanceTolerance / 2f;
                initialized = true;
            }

            /* If the agent is ready to travel to a destination */
            if (initialized && destination != null)
            {

                /* Check how far the agent is from the destination */
                Vector2 agentPos2d = new Vector2(transform.position.x,
                    transform.position.z);
                Vector2 dest2d = new Vector2(destination.position.x,
                    destination.position.z);
                float distanceLeft = Vector2.Distance(
                    agentPos2d, dest2d);
                if (distanceLeft > distanceTolerance)
                {
                    /* The agent has not reached the destination */
                    if (!pathAgent.TurnedOn)
                    {
                        /* Make sure pathAgent is turned on. */
                        pathAgent.TurnOn();
                    }

                    /* Get nav mesh path to destination */
                    UnityEngine.AI.NavMesh.CalculatePath(
                        transform.position,
                        destination.position,
                        UnityEngine.AI.NavMesh.AllAreas, path);
                    if (path.corners.Length > 0)
                    {
                        Vector3 nextCorner =
                            path.corners[path.corners.Length > 1 ? 1 : 0];
                        Vector2 nextCorner2d = new Vector2(nextCorner.x, nextCorner.z);

                        /* Set destination of pathAgent to next
                         * point in nav mesh path. */
                        pathAgent.SetDestination(nextCorner2d);
                    }
                    for (int i = 0; i < path.corners.Length - 1; i++)
                    {
                        Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
                    }
                }
                else
                {
                    /* Agent has reached the destination */
                    if (pathAgent.TurnedOn)
                    {
                        /* Make sure pathAgent is turned off. */
                        pathAgent.TurnOff();
                    }
                    if (!destReached)
                    {
                        /* Invoke destinationReached event. */
                        destReached = true;
                        if (destinationReached != null)
                        {
                            destinationReached();
                        }
                    }
                }
            }
        }
    }
}
