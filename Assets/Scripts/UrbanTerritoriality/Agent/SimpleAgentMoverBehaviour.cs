using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /** A script for moving a GameObject around
     * that can be attache to a GameObject 
     * as a component.
     * By setting the values of some boolean
     * values the GameObject can move forward,
     * backward, turn left and turn right.
     */
    [RequireComponent(typeof(CharacterController))]
    public class SimpleAgentMoverBehaviour : MonoBehaviour
    {
        /** Input for controlloing the SimpleAgentMover
         * Makes the GameObject move forward
         */
        public bool moveForward = false;

        /** Input for controlloing the SimpleAgentMover
         * Makes the GameObject move backwards
         */
        public bool moveBackward = false;

        /** Input for controlloing the SimpleAgentMover
         * Makes the GameObject turn right
         */
        public bool turnRight = false;

         /** Input for controlloing the SimpleAgentMover
          * Makes the GameObject turn left.
          */
        public bool turnLeft = false;

        /** Maximum speed of the agent in meters per second */
        public float maxSpeed = 4;

        /**
         * Maximum angular speed in degrees per
         * second. 
         */
        public float maxAngularSpeed = 180;

        /** Acceleration in meters per second squared */
        public float acceleration = 5;

        /** Deceleration in meters per second squared */
        public float deceleration = 5;

        /** Angular acceleration in degrees per second squared */
        public float angularAcceleration = 360;

        /** Angular acceleration in degress per second squared */
        public float angularDeceleration = 360;
        
        /** The character controller attached to this
         * GameObject */
        public CharacterController Controller
        { get { return mover.Controller; } }

        /** A SimpleAgentMover for moving the GameObject */
        protected SimpleAgentMover mover;

        /** Unity Start method */
        protected virtual void Start()
        {
            mover = new SimpleAgentMover(gameObject, GetComponent<CharacterController>());
            SetValues();
        }

        /** Sets all the necessary values on the mover object */
        protected virtual void SetValues()
        {
            mover.parameters.moveForward = moveForward;
            mover.parameters.moveBackward = moveBackward;
            mover.parameters.turnRight = turnRight;
            mover.parameters.turnLeft = turnLeft;
            mover.parameters.maxSpeed = maxSpeed;
            mover.parameters.maxAngularSpeed = maxAngularSpeed;
            mover.parameters.acceleration = acceleration;
            mover.parameters.deceleration = deceleration;
            mover.parameters.angularAcceleration = angularAcceleration;
            mover.parameters.angularDeceleration = angularDeceleration;
        }

        /** Unity Update */
        void Update()
        {
            SetValues();
            mover.Update();
        }
    }
}

