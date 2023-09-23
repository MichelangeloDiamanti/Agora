using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Experimental
{
    public class DestinationMoverTest : MonoBehaviour {

        public Transform destination;

        public DestinationMoverBehaviour destMover;

        // Update is called once per frame
        protected virtual void Update() {
            destMover.parameters.currentDestination = destination.position;
        }
    }
}

