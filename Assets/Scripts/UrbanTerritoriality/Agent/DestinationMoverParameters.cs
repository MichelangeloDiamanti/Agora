using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /** A structure holding paramters for
     * a DestinationMover object */
    [System.Serializable]
    public struct DestinationMoverParameters
    {
        /** The current destination to move to */
        public Vector3 currentDestination;

        /** Angle tolerance (in degrees) when rotating towards the destination
         * Typical value: 1f
         */
        public float angleTolerance;

        /** Position tolerance (in meters) when moving towards the destination
         * Typical value: 0.5f
         */
        public float positionTolerance;

        /** The maximum angular speed (in degrees per second)
         * Typical value: 180f
         */
        public float maxAngularSpeed;
    }
}
