using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /**
     * Add this component to a GameObject
     * and it will move around an environment
     * based on its speed and rotationSpeed. It
     * will respond to physics to.
     */
    [RequireComponent(typeof(CharacterController))]
    public class MovableAgent : MonoBehaviour {

        /** The forward speed of the GameObject */
        public float speed = 0;

        /** Maximum speed of the agent */
        public float maxSpeed = 3;

        /** Current acceleration of the agent */
        public float acceleration = 0;

        /**
         * Decreases the magnitude of the speed
         * by meters per second squared.
         */
        public float deceleration = 0;

        /** The rotation speed of the GameObject
         * measured in degrees per seconds.
         * */
        public float rotationSpeed = 0;

        /**
         * Maximum angular speed in degrees per
         * second. 
         */ 
        public float maxRotSpeed = 180;

        /** Angular acceleration in degrees
         * per seconds squared */
        public float angularAcceleration = 0;

        /** Angular deceleration. Decreases the magnitude
         * of the angular speed in degrees per
         * second squared.
         */
        public float angularDeceleration = 0;

        /** For turning the movement of on and off.
         * If true, the GameObject will move,
         * else it will come to a halt.
         */
        public bool running = true;

        /** The character controller attached to this
         * GameObject */
        public CharacterController Controller
        { get { return controller; } }
        private CharacterController controller;

        /** Unity Start method */
        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        /** Unity Update method */
        void Update()
        {
            if (running)
            {

                float dt = Time.deltaTime;
                rotationSpeed += angularAcceleration * dt;
                if (Mathf.Abs(rotationSpeed) > maxRotSpeed)
                {
                    float signFactor = rotationSpeed >= 0 ? 1 : -1;
                    rotationSpeed = signFactor * maxRotSpeed;
                }
                if (rotationSpeed > 0)
                {
                    rotationSpeed -= angularDeceleration * dt;
                    if (rotationSpeed < 0) rotationSpeed = 0;
                }
                if (rotationSpeed < 0)
                {
                    rotationSpeed += angularDeceleration * dt;
                    if (rotationSpeed > 0) rotationSpeed = 0;
                }
                transform.Rotate(0, rotationSpeed * dt, 0);
                Vector3 forward = Vector3.Normalize(transform.forward);

                speed += acceleration * dt;
                if (Mathf.Abs(speed) > maxSpeed)
                {
                    float sign = speed >= 0 ? 1 : -1;
                    speed = sign * maxSpeed;
                }

                if (speed > 0)
                {
                    speed -= deceleration * dt;
                    if (speed < 0) speed = 0;
                }
                if (speed < 0)
                {
                    speed += deceleration * dt;
                    if (speed > 0) speed = 0;
                }
                controller.SimpleMove(forward * speed);
            }
        }
    }
}
