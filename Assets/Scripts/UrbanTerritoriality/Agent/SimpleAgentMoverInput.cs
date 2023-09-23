using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /** A script that controls a SimpleAgentMoverBehavior
     * based on input from the user
     */
    public class SimpleAgentMoverInput : MonoBehaviour
    {
        /** The SimpleAgentMoverBehavior that this script manipulates */
        public SimpleAgentMoverBehaviour mover;

        /** Unity Update method */
        void Update()
        {
            /* Set some values on the mover based on user input */
            mover.moveForward = Input.GetKey(KeyCode.W);
            mover.moveBackward = Input.GetKey(KeyCode.S);
            mover.turnRight = Input.GetKey(KeyCode.D);
            mover.turnLeft = Input.GetKey(KeyCode.A);
        }
    }
}

