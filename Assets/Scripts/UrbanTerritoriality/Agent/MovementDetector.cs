using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /** A script that can be used to detect how much
     * a GameObject has been moving recently.
     */
    public class MovementDetector : MonoBehaviour
    {
        /** A number indicating how much this
         * GameObject has moved in recent time
         */
        private float movementIndicator = 0;

        /** Fill the movement indicator to
         * make it look like the GameObject
         * has been moving for some time.
         */ 
        public void FillMovementIndicator()
        {
            movementIndicator = minSpeed * timeout;
        }

        /** Empty the movement indicator to
         * make it look like the GameObject
         * has not been moving for some time.
         */ 
        public void EmptyMovementIndicator()
        {
            movementIndicator = 0;
        }

        /** Returns true if the GameObject has been
         * moving recently, else false */
        public bool IsMoving
        {
            get
            {
                return (movementIndicator >
                    minSpeed * timeout / 100) ? true : false;
            }
        }

        /** Maximum time in seconds that the
         * GameObject can remain still without
         * the movementIndicator reaching 0.
         */
        public float timeout = 1;

        /** The GameObject will need to maintain this
         * speed or else movementIndicator will reach 0.
         */
        public float minSpeed = 0.3f;

        /** The position of the GameObject from the last frame */
        private Vector3 lastPos;

        /** Weather this component has been initialized */
        private bool initialized = false;

        /** Unity Update method */
        void Update()
        {
            if (!initialized)
            {
                lastPos = transform.position;
                initialized = true;
            }
            else
            {
                float distTraveled =
                    Vector3.Distance(
                        transform.position, lastPos);
                lastPos = transform.position;

                movementIndicator += distTraveled;
                movementIndicator -= minSpeed * Time.deltaTime;
                movementIndicator =
                    Mathf.Clamp(movementIndicator,
                    0, minSpeed * timeout);
            }
        }
    }
}
