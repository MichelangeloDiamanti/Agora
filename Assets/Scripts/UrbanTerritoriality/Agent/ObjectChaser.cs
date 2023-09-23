using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /** A component that will make a GameObject
     copy the position and rotation of another GameObject
     with a lag. */
    public class ObjectChaser : MonoBehaviour
    {
        /** The game object that this object chases */
        public GameObject leader;

        /** The time it takes the rotation to change */
        public float rotationChangeTime = 1f;

        /** The time it takes the position to change */
        public float positionChangeTime = 1f;

        /** Angular speed of the GameObject in degrees
         per second. */
        public float AngularSpeed
        { get { return angularSpeed; } }
        private float angularSpeed;

        /** Velocity of the GameObject in meters per second. */
        public Vector3 Velocity
        { get { return velocity; } }
        private Vector3 velocity;

        /** Velocity along forward axis
         * of the objects transform */
        public Vector3 VelocityZ
        { get { return velocityZ; } }
        private Vector3 velocityZ;

        /** Velocity along the right
         * axis of the objects transform */
        public Vector3 VelocityX
        { get { return velocityX; } }
        private Vector3 velocityX;

        /** Speed in the forward direction of the GameObject */
        public float SpeedZ
        { get { return speedZ; } }
        private float speedZ;

        /** Speed in the right direction of the GameObject */
        public float SpeedX
        { get { return speedX; } }
        private float speedX;

        /** Unity Update method */
        void Update()
        {
            float dt = Time.deltaTime;
            float rotFrac = dt / rotationChangeTime;
            rotFrac = rotFrac > 1 ? 1 : rotFrac;
            Quaternion newRot = Quaternion.Lerp(transform.rotation,
                leader.transform.rotation, rotFrac);
            angularSpeed = Quaternion.Angle(transform.rotation, newRot) / dt;
            transform.rotation = newRot;
            float posFrac = dt / positionChangeTime;
            Vector3 newPos = Vector3.Lerp(transform.position,
                leader.transform.position, posFrac);
            velocity = (newPos - transform.position) / dt;
            transform.position = newPos;

            /* Calculate velocity along the forward and
             * right axis */
            velocityZ = Vector3.Project(velocity, transform.forward);
            velocityX = Vector3.Project(velocity, transform.right);
            speedZ = Vector3.Dot(transform.forward, velocity);
            speedX = Vector3.Dot(transform.right, velocity);
        }
    }
}
