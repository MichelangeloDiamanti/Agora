using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UrbanTerritoriality.Utilities;

namespace UrbanTerritoriality.Experimental
{
    public class HeuristicWandererTest : MonoBehaviour
    {
        public HeuristicWanderer wanderer;

        public Transform[] pastPoints;

        public Transform newPoint;

        protected virtual void Update()
        {
            if (pastPoints != null)
            {
                int n = pastPoints.Length;
                wanderer.previousPositions = new RingBuffer<Vector3>(n);
                for (int i = 0; i < n; i++)
                {
                    wanderer.previousPositions.Insert(pastPoints[i].position);
                }
            }
        }

        protected virtual void OnGUI()
        {
            Vector3[] prevPos = wanderer.previousPositions.ToArray();
            float newness = HeuristicWanderer.Newness(newPoint.position, ref prevPos);
            GUI.Label(new Rect(10, 10, 200, 200), "Newness: " + newness);
            float angle = wanderer.AngleToPoint(
                new Vector2(newPoint.position.x, newPoint.position.z));
            GUI.Label(new Rect(10, 40, 200, 200), "Angle: " + angle);
        }
    }
}

