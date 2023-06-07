using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UrbanTerritoriality.Maps;
using UrbanTerritoriality.Utilities;

namespace UrbanTerritoriality.Agent
{
    /**
     * A component providing wandering behavior
     * for virtual agents.
     * It uses a NavMeshAgent for navigation for
     * moving the agent around. A map is used
     * for deciding where to move. A typical
     * type of a map to use would be a visibility
     * map.
     */
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshMapWanderer : MapWanderer
    {
        /** When the agents gets closer to the destination
         * than this, a new destination will be picked. */
        public float DestinationPickDistance
        { get { return 1f; } }

        /** The current vision angle of the agent */
        protected float currentVisionAngle = 360;

        /** The last time a new destination was picked */
        protected float lastPickTime = float.MinValue;

        /** The last time a new destination was picked
         * because of some crisis such as the agent
         * being faced up to a wall */
        protected float lastCrisisPickTime = float.MinValue;

        /** The nav mesh agent */
        protected NavMeshAgent navMeshAgent;

        /** The current destination */
        protected Vector3 currentDestination;

        /** Hit position when doing a nav mesh raycast
         * in the forward direction of the agent */
        protected Vector3? navMeshHitPosition;

        /** Unity Start method */
        protected override  void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        /** Unity Update method */
        protected override  void Update()
        {
            float t = Time.time;
            if (map.Initialized)
            {
                if (navMeshAgent.remainingDistance < DestinationPickDistance)
                {
                    /* If the destination has been reached then pick a new one */
                    PickNewDestination();
                    return;
                }
                if (t > (lastPickTime + MaxTimeBetweenPicks))
                {
                    /* If time is up then pick a new destination */
                    PickNewDestination();
                }
                else
                {
                    if (!navMeshAgent.pathPending &&
                        navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
                    {
                        /* If a path cannot be found then pick a new destination */
                        PickNewDestination();
                    }
                }
            }

            currentVisionAngle = VisionAngle;
            navMeshHitPosition = null;
            NavMeshHit nmHit;
            /* Raycast on the nav mesh to see how much navigatable space
             * is directly in front of the agent */
            if (navMeshAgent.Raycast(
                transform.position +
                transform.forward * PerceptualDistance,
                out nmHit))
            {
                navMeshHitPosition = nmHit.position;
            }
        }

        /** Unity OnDrawGizmos method */
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (!ShowGizmo) return;
            if (navMeshAgent != null)
            {
                /* Draw current destination */
                Gizmos.DrawCube(currentDestination, Vector3.one);
            }
            
            /* Draw the position of the agent */
            Gizmos.DrawSphere(transform.position, 1);

            /** Draw position of nav mesh edge if the agent
             * is in front of it */
            Color temp = Gizmos.color;

            if (navMeshHitPosition != null)
            {
                Gizmos.DrawSphere((Vector3)navMeshHitPosition, 0.3f);
            }
            Gizmos.color = temp;
        }

        /** Picks a new destination for the agent to go to. */
        protected virtual void PickNewDestination()
        {
            List<Vector3> pickedPoints = AgentUtil.GetListOfRandomPoints(
                currentVisionAngle,
                PerceptualDistance,
                NrOfPickedPoints, transform);
            List<float> mapValues = new List<float>();
            int n = pickedPoints.Count;
            for (int i = 0; i < n; i++)
            {
                float mapVal = map.GetValueAt(pickedPoints[i]);
                mapValues.Add(mapVal);
            }
            int nextIndex = AgentUtil.IndexOfHighestValueFromList(mapValues);
            currentDestination = pickedPoints[nextIndex];
            navMeshAgent.destination = currentDestination;

            lastPickTime = Time.time;
        }

        /** Draws the vision gizmo */
        protected override void DrawVisionGizmo()
        {
            DrawVisionGizmo(currentVisionAngle);
        }
    }
}

