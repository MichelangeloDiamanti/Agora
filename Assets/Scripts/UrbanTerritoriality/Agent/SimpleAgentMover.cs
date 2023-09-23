using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /** A script for controlling simple agent like movement
     * for a GameObject. The object can be moved foward,
     * backward and it can turn left and right.
     * A few paramters can be set that affect the speed
     * and angular speed of the GameObject when it moves.
     */
    public class SimpleAgentMover
    {
        /** An object holding some paramters for the SimpleAgentMover */
        public SimpleAgentMoverPatameters parameters;
        
        /** The character controller attached to this
         * GameObject */
        public CharacterController Controller
        { get { return controller; } }

        /** The GameObject to be moved */
        public GameObject gameObject;

        /** The CharacterControlloer used for moving the GameObject */
        private CharacterController controller;

        /** The forward speed of the GameObject */
        protected float currentSpeed = 0;

        /** Current acceleration of the agent */
        protected float currentAcceleration = 0;

        /**
         * Decreases the magnitude of the speed
         * by meters per second squared.
         */
        protected float currentDeceleration = 10;

        /** The rotation speed of the GameObject
         * measured in degrees per seconds.
         * */
        protected float currentAngularSpeed = 0;

        /** Angular acceleration in degrees
         * per seconds squared */
        protected float currentAngularAcceleration = 0;

        /** Angular deceleration. Decreases the magnitude
         * of the angular speed in degrees per
         * second squared.
         */
        protected float currentAngularDeceleration = 0;

        /** Constructor
         * @param gameObect The GameObject to be moved
         * @param controller The CharacterController that will
         * be used to move the GameObject.
         */
        public SimpleAgentMover(GameObject gameObject, CharacterController controller)
        {
            this.gameObject = gameObject;
            this.controller = controller;
        }

        /** Reads the input variables and
         * performs the necessary actions
         * based on their values
         */
        protected virtual void HandleInput()
        {
            currentAcceleration = 0;
            currentDeceleration = 0;
            currentAngularAcceleration = 0;
            currentAngularDeceleration = 0;

            if (parameters.moveForward && !parameters.moveBackward)
            {
                currentAcceleration = parameters.acceleration;
            }
            if (parameters.moveBackward && !parameters.moveForward)
            {
                currentAcceleration = -parameters.acceleration;
            }
            if (!(parameters.moveForward ^ parameters.moveBackward))
            {
                currentDeceleration = parameters.deceleration;
            }
            if (parameters.turnRight && !parameters.turnLeft)
            {
                currentAngularAcceleration = parameters.angularAcceleration;
            }
            if (parameters.turnLeft && !parameters.turnRight)
            {
                currentAngularAcceleration = -parameters.angularAcceleration;
            }
            if (!(parameters.turnLeft ^ parameters.turnRight))
            {
                currentAngularDeceleration = parameters.angularDeceleration;
            }
        }

        /** Unity Update */
        public virtual void Update()
        {
            HandleInput();

            float dt = Time.deltaTime;
            currentAngularSpeed += currentAngularAcceleration * dt;
            if (Mathf.Abs(currentAngularSpeed) > parameters.maxAngularSpeed)
            {
                float signFactor = currentAngularSpeed >= 0 ? 1 : -1;
                currentAngularSpeed = signFactor * parameters.maxAngularSpeed;
            }
            if (currentAngularSpeed > 0)
            {
                currentAngularSpeed -= currentAngularDeceleration * dt;
                if (currentAngularSpeed < 0) currentAngularSpeed = 0;
            }
            if (currentAngularSpeed < 0)
            {
                currentAngularSpeed += currentAngularDeceleration * dt;
                if (currentAngularSpeed > 0) currentAngularSpeed = 0;
            }
            gameObject.transform.Rotate(0, currentAngularSpeed * dt, 0);
            Vector3 forward = Vector3.Normalize(gameObject.transform.forward);

            currentSpeed += currentAcceleration * dt;
            if (Mathf.Abs(currentSpeed) > parameters.maxSpeed)
            {
                float sign = currentSpeed >= 0 ? 1 : -1;
                currentSpeed = sign * parameters.maxSpeed;
            }

            if (currentSpeed > 0)
            {
                currentSpeed -= currentDeceleration * dt;
                if (currentSpeed < 0) currentSpeed = 0;
            }
            if (currentSpeed < 0)
            {
                currentSpeed += currentDeceleration * dt;
                if (currentSpeed > 0) currentSpeed = 0;
            }
            controller.SimpleMove(forward * currentSpeed);
        }
    }
}
