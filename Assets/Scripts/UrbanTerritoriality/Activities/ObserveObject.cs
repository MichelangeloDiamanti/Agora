using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Activities
{
    /** An activity where agents can observe something
     */
    public class ObserveObject : Activity
    {
        /** The object that the agents look at while
         * they are engaged in this activity */
        public GameObject objectToObserve;
    }
}
