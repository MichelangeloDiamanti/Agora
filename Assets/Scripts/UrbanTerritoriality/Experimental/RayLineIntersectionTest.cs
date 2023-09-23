using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Utilities;

namespace UrbanTerritoriality.Experimental
{
    public class RayLineIntersectionTest : MonoBehaviour
    {
        public GameObject rayStart;
        public GameObject rayPoint2;
        public GameObject linePoint1;
        public GameObject linePoint2;

        private void OnDrawGizmos()
        {
            Vector3 rayStart_3d = rayStart.transform.position;
            Vector3 rayPoint2_3d = rayPoint2.transform.position;
            Vector3 linePoint1_3d = linePoint1.transform.position;
            Vector3 linePoint2_3d = linePoint2.transform.position;

            Gizmos.color = new Color(1, 0, 0, 1);
            Gizmos.DrawLine(rayStart_3d, rayPoint2_3d);
            Gizmos.DrawLine(linePoint1_3d, linePoint2_3d);


            Vector2 rayStart_2d = new Vector2(rayStart_3d.x, rayStart_3d.z);
            Vector2 rayPoint2_2d = new Vector2(rayPoint2_3d.x, rayPoint2_3d.z);
            Vector2 linePoint1_2d = new Vector2(linePoint1_3d.x, linePoint1_3d.z);
            Vector2 linePoint2_2d = new Vector2(linePoint2_3d.x, linePoint2_3d.z);
            Vector2 rayDir_2d = rayPoint2_2d - rayStart_2d;

            Vector2? inter =
                Util.RayLineIntersection(
                    rayStart_2d,
                    rayDir_2d,
                    linePoint1_2d,
                    linePoint2_2d);

            if (inter != null)
            {
                Vector2 inter2 = (Vector2)inter;
                float y = transform.position.y;
                Gizmos.DrawCube(new Vector3(inter2.x, y, inter2.y), new Vector3(1, 1, 1));
            }
        }
    }
}
