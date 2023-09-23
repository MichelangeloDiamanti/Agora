using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Utilities
{
    /**
     * A script for controlling how fast
     * time passes in a scene.
     */
    public class TimeScaler : MonoBehaviour
    {
        /** How fast the time passes.
         * The larger the number the faster
         * it pases. */
        [Range(0.0f, 10)]
        public float timeScale = 1f;

        void Start()
        {
            // Debug.Log("timeScale: " + timeScale);
        }

        /** Unity Update method
         * Set the time scale */
        public virtual void Update()
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }
}
