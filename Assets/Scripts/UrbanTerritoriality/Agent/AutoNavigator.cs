using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Utilities;

namespace UrbanTerritoriality.Agent
{
    /**
     * Put this component on a GameObject and it will start
     * to navigate the world and it avoid obstacles.
     * Raycasting is used to sense the shape of the environment.
     * The GameObject will move in the direction where there
     * is more open space.
     */
    public class AutoNavigator : MonoBehaviour {

        /** The height in the world where the rays are cast.
         */
        public float rayHeightOffset = 1f;

        /** Set this to true if the rays are to be shown in the
         editor window. */
        public bool showRaysInEditor = true;

        private float speed = 2;
        private float speedChangeTime = 0.3f;
        private float lastSpeed = 0;
        private float rotSpeed = 50;
        private float maxRotSpeed = 180;
        private float rayDistance = 100f;
        private float forwardRayDistance = 10f;
        private int raycastsPerFrame = 10;
        private float maxAngle = 0.8f;
        private float rotationBias = 1.5f;
        private bool randomRays = false;
        private float lastRotationSpeed = 0;
        private float rotChangeTime = 0.15f;
        private Ray visionRay;
        private RaycastHit[] hits;
        private bool raycastHit = false;

        /* Weather or not the auto navigator is enabled or not*/
        private bool navigatorEnabled = true;

        /** Weather or not the navigation behavior is enabled.
         * @param enabled Weather or not to enable the navigation.
         * If true the GameObject will move around else this component
         * will not make it move.
         */
        public void SetEnabled(bool enabled)
        {
            navigatorEnabled = enabled;
            lastRotationSpeed = 0;
        }

        private void NavigateUpdate()
        {
             //InputMove();
            hits = new RaycastHit[raycastsPerFrame];
            float dt = Time.deltaTime;

            RaycastHit forwardHit;
            float speedMultiplier = 1;
            float rotBias = 0;

            /* Create foraward ray */
            Ray forwardRay = new Ray(GetRayStartPosition(), transform.forward);
            if (Physics.Raycast(forwardRay, out forwardHit, forwardRayDistance))
            {
                speedMultiplier = forwardHit.distance / forwardRayDistance;
                if (forwardHit.collider.tag == "HumanAgent")
                {
                    rotBias = rotationBias;
                }
            }

            float totalRot = 0f;
            if (randomRays)
            {
                totalRot = CreateRandomRays();
            }
            else
            {
                totalRot = CreateMovingRay();
            }
            float finalRotationSpeed = totalRot * rotSpeed / (raycastsPerFrame * speedMultiplier);
            finalRotationSpeed = finalRotationSpeed < 0 ? Mathf.Max(-maxRotSpeed, finalRotationSpeed)
                : Mathf.Min(maxRotSpeed, finalRotationSpeed);
            if (dt < rotChangeTime)
            {
                finalRotationSpeed = Mathf.Lerp(lastRotationSpeed, finalRotationSpeed, dt/ rotChangeTime);
            }
            lastRotationSpeed = finalRotationSpeed;
            float finalRotation = finalRotationSpeed * dt;
            transform.Rotate(0, finalRotation + rotBias, 0);

            float finalSpeed = speed * speedMultiplier;
            if (dt < speedChangeTime)
            {
                finalSpeed = Mathf.Lerp(lastSpeed, finalSpeed, dt / speedChangeTime);
            }
            lastSpeed = finalSpeed;

            transform.position += transform.forward * finalSpeed * dt;
        }

        // Update is called once per frame
        void Update () {
            if (navigatorEnabled)
            {
                NavigateUpdate();   
            }
        }

        private float CreateMovingRay()
        {
            float t = Time.time;
            float totalRot = 0f;
            float interval = 1f / (float)raycastsPerFrame;
            for (int i = 0; i < raycastsPerFrame; i++)
            {
                float angle = Util.TriangleWave(t*100 + (float)i*interval) * maxAngle;

                Vector3 forward = transform.forward;
                Vector3 side = transform.right;

                /* Weather to turn right or left */
                bool turnRight = true;
                /* Generates either 0 or 1 */
                if (angle < 0)
                {
                    /* Turns right to left */
                    side = -side;
                    turnRight = false;
                    angle = -angle;
                }

                Vector3 orig = GetRayStartPosition();
                Vector3 dir = Vector3.Slerp(forward, side, angle);
                visionRay = new Ray(orig, dir);
                RaycastHit hit;
                float rot = 0;
                PerformRaycast(visionRay, out hit, rayDistance, out rot, angle, turnRight);
                hits[i] = hit;
                totalRot += rot;
            }
            return totalRot;
        }

        /* Create random rays */
        private float CreateRandomRays()
        {
            float totalRot = 0f;
            for (int i = 0; i < raycastsPerFrame; i++)
            {
                Vector3 forward = transform.forward;
                Vector3 side = transform.right;

                /* Weather to turn right or left */
                bool turnRight = true;
                /* Generates either 0 or 1 */
                if (Random.Range(0, 2) == 0)
                {
                    /* Turns right to left */
                    side = -side;
                    turnRight = false;
                }
                float angle = Random.Range(0f, maxAngle);
                Vector3 orig = GetRayStartPosition();
                Vector3 dir = Vector3.Slerp(forward, side, angle);
                visionRay = new Ray(orig, dir);
                RaycastHit hit;
                float rot = 0;
                PerformRaycast(visionRay, out hit, rayDistance, out rot, angle, turnRight);
                hits[i] = hit;
                totalRot += rot;
            }
            return totalRot;
        }

        private void PerformRaycast(Ray visionRay, out RaycastHit hit, float rayDistance, out float rotation, float angle, bool turnRight)
        {
            if (Physics.Raycast(visionRay, out hit, rayDistance))
            {
                raycastHit = true;
            }
            else
            {
                raycastHit = false;
            }
            rotation = hit.distance * angle;
            if (!turnRight)
            {
                rotation = -rotation;
            }
        }

        /*
        void OnGUI()
        {
            if (raycastHit)
            {
                GUI.TextArea(new Rect(10, 10, 250, 30), "NUMBER OF RAYS: " + hits.Length);
            }
        }
        */

        /** Get the position in the world where the rays start.
         * @return The position in the Unity scene where the rays start.
         */
        public Vector3 GetRayStartPosition()
        {
            return transform.position + new Vector3(0, rayHeightOffset, 0);
        }
        
        void OnDrawGizmos()
        {
            if (raycastHit && showRaysInEditor)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < hits.Length; i++)
                {
                    Gizmos.DrawLine(GetRayStartPosition(), hits[i].point);
                }
            }
        }
        

        private void InputMove()
        {
            float dt = Time.deltaTime;
            float speedMultiplier = 1;
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {
                speedMultiplier = 3;
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * speed * speedMultiplier * dt;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * speed * speedMultiplier * dt;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, rotSpeed * dt, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, -rotSpeed * dt, 0);
            }
        }
    }
}