using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /**
     * A script that can be attached to a GameObject
     * as a compont that can be used to make
     * the GameObjec move to a specified location
     * in the scene.
     */
    [RequireComponent(typeof(CharacterController))]
    public class DestinationMoverBehaviour : MonoBehaviour
    {
        /** Paramters for the DestinationMover object used for
         * moving the GameObject.
         * */
        public DestinationMoverParameters parameters;

        /** Paramters for the SimpleAgentMover that is
         * used by the DestinationMover for lower level
         * movement.
         */
        public SimpleAgentMoverPatameters moverParameters;

        /** A DestinationMover object used for moving
         * the GameObject to a destination */
        protected DestinationMover destMover;

        /** Unity Start method */
        protected virtual void Start()
        {
            parameters.currentDestination = transform.position;

            SimpleAgentMover mover =
                new SimpleAgentMover(
                    gameObject, GetComponent<CharacterController>());
            destMover =
                new DestinationMover(mover);
            SetValues();
        }

        /** Sets all the necessary values on the destMover object */
        protected virtual void SetValues()
        {
            destMover.parameters = parameters;
            destMover.mover.parameters = moverParameters;
        }

        /** Unity Update */
        protected virtual void Update()
        {
            SetValues();
            destMover.Update();
        }
    }
}

