using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /**
     * A script for copying the rotation
     * of a GameObject to the GameObject
     * this script is attached to,
     * with a lag.
     */
    public class CharacterHolder : MonoBehaviour
    {
        /** The lag in the rotation in seconds. */
        public float smoothRotationTime = 1f;

        /** The GameObject whose rotation will
         * be copied.
         */
        public GameObject agentToFollow;

        private Vector3 lastEulerRotation;

        private void Start()
        {
            lastEulerRotation = transform.eulerAngles;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            float frac = dt / smoothRotationTime;
            frac = frac > 1 ? 1 : frac;
            transform.eulerAngles = Vector3.Lerp(lastEulerRotation,
                agentToFollow.transform.eulerAngles, frac);
        }
    }
}
