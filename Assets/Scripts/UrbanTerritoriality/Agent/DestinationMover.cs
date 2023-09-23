using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /**
     * A script that lets a GameObject
     * move to a specified location in a scene.
     */
    public class DestinationMover
    {
        /** Some parameters for the destination mover */
        public DestinationMoverParameters parameters;

        /** The SimpleAgentMover object used for moving the gameObject */
        public SimpleAgentMover mover;

        /** Constructor
         * @param mover A SimpleAgentMover that this script
         * will use for moving the GameObject to the
         * destination.
         */
        public DestinationMover(SimpleAgentMover mover)
        {
            parameters.currentDestination =
                mover.gameObject.transform.position;
            this.mover = mover;
        }

        /** Unity update */
        public virtual void Update()
        {
            Transform transform = mover.gameObject.transform;
            Vector2 agentPos2d = new Vector2(transform.position.x, transform.position.z);
            Vector2 dest2d = new Vector2(
                parameters.currentDestination.x,
                parameters.currentDestination.z);

            float angle = 0;
            if (Vector2.Distance(agentPos2d, dest2d)
                > parameters.positionTolerance)
            {
                mover.parameters.moveForward = true;

                Vector2 toPoint2d = dest2d - agentPos2d;
                Vector2 forward2d = new Vector2(transform.forward.x, transform.forward.z);
                angle = Vector2.SignedAngle(forward2d, toPoint2d);
                mover.parameters.turnLeft = angle > parameters.angleTolerance;
                mover.parameters.turnRight = angle < -parameters.angleTolerance;
            }
            else
            {
                mover.parameters.moveForward = false;
                mover.parameters.turnLeft = false;
                mover.parameters.turnRight = false;
            }

            float angleMagn = Mathf.Abs(angle);
            mover.parameters.maxAngularSpeed = parameters.maxAngularSpeed * angleMagn / 180f;
            mover.Update();
        }
    }
}

