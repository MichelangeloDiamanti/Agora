using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Utilities;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.GenAlg
{
    /** This class was just created to test the
     * functionality of the GeneticPathGenerator class.
     */
    public class PathUser : MonoBehaviour {

        /** A GeneticPathGenerator for creating a path */
        public GeneticPathGenerator pathGenerator;

        /** A TerritorialHeatmap for pathGenerator to use */
        public TerritorialHeatmap heatmap;

        /** A collider map for pathGenerator to use */
        public ColliderMap colliderMap;

        /** A MultilineDrawer for drawing lines */
        public MultilineDrawer lineDrawer;

        /** The destination to find a path to */
        public Transform destination;

        /** Configure the start and end locations of the path */
        private void SetEndPoints()
        {
            Vector3 start3 = transform.position;
            Vector3 end3 = destination.position;
            pathGenerator.start = new Vector2(start3.x, start3.z);
            pathGenerator.end = new Vector2(end3.x, end3.z);
        }

        /** Unity Start method */
        void Start() {
            pathGenerator = new GeneticPathGenerator(heatmap, gameObject, 10, 20);
            pathGenerator.colliderMap = colliderMap;
            pathGenerator.lineDrawer = lineDrawer;
            SetEndPoints();
            pathGenerator.InitPopulation();
        }

        /** Unity Update method */
        void Update()
        {
            SetEndPoints();
            pathGenerator.PassOneGeneration();
        }
    }
}
