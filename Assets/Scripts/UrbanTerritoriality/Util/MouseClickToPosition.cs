using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Utilities
{
    /**
     * Add this component to a GameObject and it will
     * always move to the position where the user clicks
     * with the mouse.
     */
    public class MouseClickToPosition : MonoBehaviour
    {
        /** The camer used by the user */
        public Camera clickCamera;

        /** Unity Update method */
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Vector3 mousePos = Input.mousePosition;
                Ray ray = clickCamera.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out hit))
                {
                    transform.position = hit.point;
                }
            }
        }
    }
}

