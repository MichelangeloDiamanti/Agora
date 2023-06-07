using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /**
     * A component for controlling a
     * MovableAgent with user input.
     */
    public class MovableAgentController : MonoBehaviour
    {
        /**
         * Magnitute of angular acceleration used
         * for rotating the MovableAgent.
         */
        public float angularAcceleration = 500;

        /**
         * Magnitue of the speed used
         * for pushing the MovableAgent
         * forward or backward.
         */ 
        public float speed = 3;

        /**
         * The MovableAgent that this
         * object controls.
         */
        public MovableAgent moveAgent;

        /** Unity Update method */
        void Update()
        {
            moveAgent.deceleration = 0;
            moveAgent.acceleration = 0;
            moveAgent.angularAcceleration = 0;
            if (Input.GetKey(KeyCode.W))
            {
                moveAgent.acceleration = 10f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveAgent.acceleration = -10f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveAgent.angularAcceleration += angularAcceleration;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveAgent.angularAcceleration -= angularAcceleration;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                moveAgent.deceleration = 10f;
            }
        }
    }
}
