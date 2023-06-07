using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Geometry;

namespace UrbanTerritoriality.Experimental
{
    public class NormalTest : MonoBehaviour
    {
        public GameObject p1;
        public GameObject p2;
        public GameObject p3;

        public Color col1 = new Color(0, 0, 1, 1);
        public Color col2 = new Color(1, 0.5f, 0, 1);

        private void OnDrawGizmos()
        {
            if (p1 == null || p2 == null || p3 == null) return;

            Vector3 p1p = p1.transform.position;
            Vector3 p2p = p2.transform.position;
            Vector3 p3p = p3.transform.position;
            Vector3 v1 = p2p - p1p;
            Vector3 v2 = p3p - p1p;

            Vector3 norm = Vector3.Cross(v1, v2);

            Gizmos.color = col1;
            Gizmos.DrawLine(p1p, p2p);
            Gizmos.DrawLine(p1p, p3p);
            Gizmos.color = col2;
            Gizmos.DrawLine(p1p, p1p + norm);
        }
    }
}
