using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Maps;
using UrbanTerritoriality.Utilities;

namespace UrbanTerritoriality.Agent
{
    /** Lets a GameObject wander around a scene
     * and the wandering behavior is based uppon a map
     */
    public class PhysicalMapWanderer : MapWanderer
    {
        /** Parameters for A DestinationMoverInner object.
         * Used for moving the GameObject to destinations.
         */
        public DestinationMoverParameters destMoverParameters;

        /**
         * Parameters for a SimpleAgentMoverInnver object
         * Used for doing low level movement.
         */
        public SimpleAgentMoverPatameters moverParamters;

        /** The time when the last destination point was picked */
        protected float lastPickTime = 0f;

        /** An object used for moving the GameObject to a
         * specified destination */
        protected DestinationMover destMover;

        /** Unity Start method
         * Do some initialization
         */
        protected override  void Start()
        {
            SimpleAgentMover mover = new SimpleAgentMover(
                gameObject, GetComponent<CharacterController>());
            mover.parameters = moverParamters;
            destMover = new DestinationMover(mover);
            destMover.parameters = destMoverParameters;
        }

        /** Unity Update method
         * Pick a new destination if it is time 
         * to do so.
         */
        protected override  void Update()
        {
            if (map.Initialized)
            {
                if (Time.time > lastPickTime + MaxTimeBetweenPicks)
                {
                    PickNewDestination();
                    lastPickTime = Time.time;
                }
            }
            destMover.Update();
        }

        /** Pick a new destination to go to */
        protected virtual void PickNewDestination()
        {
            List<Vector3> pickedPoints = AgentUtil.GetListOfRandomPoints(
                VisionAngle,
                PerceptualDistance,
                NrOfPickedPoints,
                transform);
            List<float> mapValues = new List<float>();
            int n = pickedPoints.Count;
            for (int i = 0; i < n; i++)
            {
                float mapVal = map.GetValueAt(pickedPoints[i]);
                mapValues.Add(mapVal);
            }
            int nextIndex = AgentUtil.IndexOfHighestValueFromList(mapValues);
            destMover.parameters.currentDestination = pickedPoints[nextIndex];
        }

        /** Unity OnDrawGizmos method
         * Draws a gizmo for the map wanderer.
         */
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (agentParameters == null) return;
            if (destMover != null)
            {
                Gizmos.DrawCube(destMover.parameters.currentDestination, Vector3.one);
            }
        }
    }
}
