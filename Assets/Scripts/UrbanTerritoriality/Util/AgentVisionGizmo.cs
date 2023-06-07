using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Utilities
{
    /** A gizmo showing the vision sector for an agent */
    public class AgentVisionGizmo
    {
        /** The vision angle for the agent */
        public float visionAngle;

        /** The vision distance of the agent */
        public float visionDistance;

        /** The position of the sector */
        public Vector3 position;

        /** The rotation of the sector */
        public Quaternion rotation;

        /** The fill color of the sector */
        public Color fillColor;

        /** The line color of the sector */
        public Color lineColor;

        /** Used for drawing a sector showing the vision cone of the agent */
        protected SectorGizmo sectorGizmo = null;

        /** Draw the gizmo
         * This should only be called inside a OnDrawGizmos method
         * of a MonoBehaviour class
         */
        public virtual void OnDrawGizmos()
        {
            if (sectorGizmo == null)
            {
                sectorGizmo = new SectorGizmo();
            }
            sectorGizmo.color = fillColor;
            sectorGizmo.position = position;
            sectorGizmo.rotation = rotation;
            sectorGizmo.radius = visionDistance;
            sectorGizmo.Configure(visionAngle, 32);
            sectorGizmo.OnDrawGizmos();
            Gizmos.color = lineColor;
            float halfVisionAngle = visionAngle / 2f;
            Vector3 v1 = rotation * Quaternion.Euler(0, halfVisionAngle, 0) * Vector3.forward;
            Vector3 v2 = rotation * Quaternion.Euler(0, -halfVisionAngle, 0) * Vector3.forward;
            Gizmos.DrawLine(position, position + v1.normalized * visionDistance);
            Gizmos.DrawLine(position, position + v2.normalized * visionDistance);
        }
    }
}

