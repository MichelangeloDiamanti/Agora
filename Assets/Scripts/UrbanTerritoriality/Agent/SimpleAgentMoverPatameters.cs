using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /** A structure holding some parameters for
     * a SimpleAgentMover object
     */
    [System.Serializable]
    public struct SimpleAgentMoverPatameters {

        /** Input for controlloing the SimpleAgentMoverInner
         * Makes the GameObject move forward
         */
        public bool moveForward;

        /** Input for controlloing the SimpleAgentMover
         * Makes the GameObject move backwards
         */
        public bool moveBackward;

        /** Input for controlloing the SimpleAgentMover
         * Makes the GameObject turn right
         */
        public bool turnRight;

        /** Input for controlloing the SimpleAgentMover
         * Makes the GameObject turn left.
         */
        public bool turnLeft;

        /** Maximum speed of the agent in meters per second */
        public float maxSpeed;

        /**
         * Maximum angular speed in degrees per
         * second. 
         */
        public float maxAngularSpeed;

        /** Acceleration in meters per second squared */
        public float acceleration;

        /** Deceleration in meters per second squared */
        public float deceleration;

        /** Angular acceleration in degrees per second squared */
        public float angularAcceleration;

        /** Angular acceleration in degress per second squared */
        public float angularDeceleration;
    }
}

