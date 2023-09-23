using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Utilities
{
    /** A first person style camera controller
     * Add this component to a camera to allow
     * the user to control the camera in a first
     * person shooter style.
     */
    public class FirstPersonCamera : MonoBehaviour
    {
        /** The movement speed of the camera */
        private float speed = 10f;
        
        /** How fast the camera rotates when the
         * user moves the mouse. */
        private float mouseSensitivity = 1f;

        /** Read input from user and perform
         * changes to camera based on it.
         */
        private void FPSMove()
        {
            float dt = Time.deltaTime;

            /* Move based on keyboard input */
            float inputForward = 0;
            float inputRight = 0;
            float inputUp = 0;
            if (Input.GetKey(KeyCode.W) ||
                Input.GetKey(KeyCode.UpArrow))
            {
                inputForward += 1f;
            }
            if (Input.GetKey(KeyCode.S) ||
                Input.GetKey(KeyCode.DownArrow))
            {
                inputForward -= 1f;
            }
            if (Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.RightArrow))
            {
                inputRight += 1f;
            }
            if (Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.LeftArrow))
            {
                inputRight -= 1f;
            }
            if (Input.GetKey(KeyCode.Space) ||
                Input.GetKey(KeyCode.E))
            {
                inputUp += 1f;
            }
            if (Input.GetKey(KeyCode.RightShift) ||
                Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.Q))
            {
                inputUp -= 1;
            }
            float translationY = inputUp * speed * dt;
            float translationZ = inputForward * speed * dt;
            float translationX = inputRight * speed * dt;

            Vector3 currentPos = transform.position;
            transform.position =
                new Vector3(currentPos.x,
                currentPos.y + translationY,
                currentPos.z);

            transform.Translate(new Vector3(translationX, 0, translationZ));
            
            /* Rotate based on mouse movement */
            Vector3 euler = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(
                euler.x - Input.GetAxis("Mouse Y") * mouseSensitivity,
                euler.y + Input.GetAxis("Mouse X") * mouseSensitivity,
                euler.z);
        }

        /** Unity Update method */
        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                FPSMove();
            }
        }
    }
}