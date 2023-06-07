using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    /**
     * A class with a list of GameObjects intented as waypoints.
     * Can be used as a list of waypoints for
     * a character to walk between.
     */
    public class WaypointList : MonoBehaviour
    {
        /** The waypoints */
        public GameObject[] waypoints;

        /** Get a random waypoint in the list.
         * @return A random GameObject in the waypoints list.
         */
        public GameObject GetRandomWaypoint()
        {
            int randIndex = Random.Range(0, waypoints.Length);
            return waypoints[randIndex];
        }
    }
}
