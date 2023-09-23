using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Activities
{
    /** A class that manages access to activities in
     * that agents can partake in, in a scene.
     */
    public class ActivityManager : MonoBehaviour {

        /** An array with all the activities
         * in the scene. */
        private Activity[] allActivities;

        private void Start()
        {
            /* Get all the activities in the scene */
            allActivities = (Activity[])
                GameObject.FindObjectsOfType(
                    typeof(Activity));
        }
       
        public Activity RequestRandomActivity(HumanAgent agent)
        {
            if (allActivities != null)
            {
                int n = allActivities.Length;
                Activity[] available = new Activity[n];
                int availCount = 0;
                for (int i = 0; i < n; i++)
                {
                    if (!allActivities[i].Occupied)
                    {
                        available[availCount] = allActivities[i];
                        availCount++;
                    }
                }

                if (availCount > 0)
                {
                    int randIndex = Random.Range((int)0, (int)availCount);
                    Activity randomAct = available[randIndex];
                    randomAct.RequestActivity(agent);
                    return randomAct;
                }
            }
            return null;
        }
    }
}
