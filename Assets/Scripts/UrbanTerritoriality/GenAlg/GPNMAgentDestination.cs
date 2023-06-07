using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.GenAlg
{
    /** A script for setting the destination of a GPNMAgent */
    public class GPNMAgentDestination : MonoBehaviour
    {
        /** The GPNMAgent */
        public GPNMAgent gpNavAgent;

        /** The destination the agent should go to */
        public Transform destination;

        /** Unity Start method */
        private void Start()
        {
            gpNavAgent.Destination = destination;
        }
    }
}

