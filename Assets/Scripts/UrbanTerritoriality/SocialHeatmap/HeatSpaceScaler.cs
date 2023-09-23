using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** A component that scales the front of a heatspace based
     * on the its speed and colliders in the scene. */
    public class HeatSpaceScaler : MonoBehaviour
    {
        /**  The heatspac to be scaled */
        public HeatSpace heatSpace;

        /** Minum size of the territory front */
        public float frontMinSize = 1f;

        /** The scaling factor used when setting the length of the
         * front based on the forward speed */
        public float frontScalingFactor = 1f;

        /** A layer mask to use when doing raycasts */
        public LayerMask raycastLayerMask;

        /** Last position of heatSpace */
        private Vector3 lastPos;

        /** Unity Update method */
        void Update()
        {
            float dt = Time.deltaTime;
            /* Calculate the speed of the heatspace in the forward direction */
            Vector3 forwardNormalized = heatSpace.transform.forward.normalized;
            Vector3 currentPos = heatSpace.transform.position;
            /*
            Debug.DrawLine(currentPos, currentPos + forwardNormalized,
                new Color(0f, 0.5f, 1f, 1));
                */
            Vector3 change = currentPos - lastPos;
            Vector3 changeNormalized = change.normalized;
            float forwardChange = 
                Vector3.Dot(forwardNormalized,
                changeNormalized) * change.magnitude;
            float forwardSpeed = forwardChange / dt;
            lastPos = currentPos;
            if (forwardSpeed < 0) forwardSpeed = 0;

            float front = frontMinSize + frontScalingFactor * forwardSpeed;

            RaycastHit hit;
            if (Physics.Raycast(heatSpace.transform.position, forwardNormalized,
                out hit, float.MaxValue, raycastLayerMask))
            {
                float dist = hit.distance;
                if (front > dist) front = dist;
            }

            heatSpace.territoryFront = front;
        }
    }
}
