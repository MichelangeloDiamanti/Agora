using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Experimental
{
    public class RandomPointPickerTest : MonoBehaviour
    {

        public float degrees = 90;

        public float distance = 10;

        public int numberOfPoints = 10;

        public Color gizmoColor = new Color(1, 0, 0, 0.5f);

        protected List<Vector3> randomPoints;

        public float timeBetween = 1f;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(GatherPoints());
        }

        protected IEnumerator GatherPoints()
        {
            while (true)
            {
                randomPoints = Agent.AgentUtil.GetListOfRandomPoints(
                    degrees, distance, numberOfPoints, transform);
                yield return new WaitForSeconds(timeBetween);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (randomPoints != null)
            {
                Gizmos.color = gizmoColor;
                int n = randomPoints.Count;
                for (int i = 0; i < n; i++)
                {
                    Gizmos.DrawCube(randomPoints[i], Vector3.one * 0.1f);
                }
            }
        }
    }
}

