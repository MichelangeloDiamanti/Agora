using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Enum;

namespace UrbanTerritoriality.Maps
{
    /** A base class for visibility maps */
    public abstract class GeneralVisibilityMap : RayMap
    {
        /** This variable is a dummy variable
         * that was just created in order to
         * force the custom inspector to
         * be updated every frame.
         */
        [HideInInspector]
        public float dummy;

        /** A MapNormalizer for normalizing
         * the values in the map between 0 and 1. */
        public MapNormalizer normalizer;

        /** Do some initialization */
        protected override void _initialize()
        {
            base._initialize();
            normalizer = new MapNormalizer(maxTime);
        }

        /** Starts coroutines that handle the saving of the map. */
        protected override void ConfigureSaveBehavior()
        {
            StartCoroutine(SaveMapOnThreshold());
            StartCoroutine(SaveMapAfterTime());
        }

        /** Saves map after certain time has passed in play mode if
         * saveMethod is set to TIME */
        protected override IEnumerator SaveMapAfterTime()
        {
            yield return new WaitUntil(() => currentTime  >= saveTime);
            if (saveMethod == SaveMethod.TIME)
            {
                HandleSavingOfMap();
            }
            yield return null;
        }

        /** Unity update method */
        protected override void Update()
        {
            base.Update();
            if (initialized)
            {
                normalizer.maxTime = this.maxTime;
            }
            /** Setting this variable to something
             * in order to force the custom inspector
             * to be updated every frame */
            dummy = Random.Range(0f, 1f);
        }
    }
}

