using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Geometry;

namespace UrbanTerritoriality.Experimental
{
    public class LinePlaneIntersectionTest : MonoBehaviour
    {
        public Transform lineP1;
        public Transform lineP2;

        public Transform planeP1;
        public Transform planeP2;
        public Transform planeP3;

        public Color col1 = new Color(0, 0, 1, 1);
        public Color col2 = new Color(0, 1, 0, 1);
        public Color col3 = new Color(1, 0, 0, 1);

        private void OnDrawGizmos()
        {
            if (lineP1 == null ||
                lineP2 == null ||
                planeP1 == null ||
                planeP2 == null ||
                planeP3 == null) return;

            LinePlaneIntersection3D result =
                GeometryUtil.GetLinePlaneIntersection3D(
                    new Line3d(lineP1.position, lineP2.position),
                    new Triangle3d(planeP1.position, planeP2.position, planeP3.position));
            switch(result.type)
            {
                case LinePlaneIntersectionType3D.NO_INTERSECTION:
                    Gizmos.color = col1;
                    break;
                case LinePlaneIntersectionType3D.PARALLEL_INTERSECTION:
                    Gizmos.color = col2;
                    break;
                case LinePlaneIntersectionType3D.POINT_INTERSECTION:
                    Gizmos.color = col3;
                    Gizmos.DrawSphere((Vector3)result.point, 0.5f);
                    break;
                default:
                    break;
            }
            Gizmos.DrawLine(lineP1.position, lineP2.position);
            Gizmos.DrawLine(planeP1.position, planeP2.position);
            Gizmos.DrawLine(planeP1.position, planeP3.position);
        }
    }
}
