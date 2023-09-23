using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /** A script that can move a GameObject
     * to a specified position and rotation */
    public class SimpleMover : MonoBehaviour
    {

        /** The speed of the movement */
        public float speed;

        /** An event that is fired when the destination
         * is reached */
        public System.Action destinationReached;

        /** True while moving */
        private bool moving = false;

        /** Weather or not to turn off this SimpleMover when
         * it is finished moving */
        private bool turnOffWhenFinished = false;

        /** Make the GameObject start moving to a desired position
         * and rotation in the scene.
         * @param position The position to move to.
         * @param rotation The rotation to rotate the GameObject into.
         * @param turnOffWhenFinished Weather or not to turn off
         * the SimpleMover when it is finished moving to the position
         * and rotation.
         */
        public void MoveTo(Vector3 position,
            Quaternion rotation,
            bool turnOffWhenFinished)
        {
            destPosition = position;
            destRotation = rotation;
            this.turnOffWhenFinished = turnOffWhenFinished;
            moving = true;
        }

        /** The destination position */
        private Vector3 destPosition;

        /** The destination rotation */
        private Quaternion destRotation;

        /** Weather or not this behavior is turned on or not. */
        public bool TurnedOn
        { get { return turnedOn; } }
        private bool turnedOn = false;

        /** Turn on the behavior */
        public void TurnOn()
        {
            turnedOn = true;
        }

        /** Turn off the behavior */
        public void TurnOff()
        {
            turnedOn = false;
            moving = false;
        }

        /** Unity Update method */
        void Update() {
            if (turnedOn && moving)
            {
                float distanceLeft = Vector3.Distance(destPosition, transform.position);
                float timeLeft = distanceLeft / speed;
                float dt = Time.deltaTime;
                if (dt < timeLeft)
                {
                    float frac = dt / timeLeft;
                    transform.position = Vector3.Lerp(transform.position, destPosition, frac);
                    transform.rotation = Quaternion.Slerp(transform.rotation, destRotation, frac);
                }
                else
                {
                    transform.position = destPosition;
                    transform.rotation = destRotation;
                    moving = false;
                    if (turnOffWhenFinished) TurnOff();

                    if (destinationReached != null)
                    {
                        destinationReached();
                    }
                }
            }
        }
    }
}
