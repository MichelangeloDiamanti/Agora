using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** A script that draws a horizontal
     * rectangle gizmo in the scene */
    public class SceneRectangle : MonoBehaviour
    {
        /** The size of the rectangle */
        public Vector2 size;

        /** The color of the rectangle */
        public Color gizmoColor = new Color(0.5f, 0, 0.5f, 1f);

        /** Unity OnDrawGizmos method */
        protected virtual void OnDrawGizmos()
        {
            Utilities.GizmoHelper.DrawPlane(transform.position, size, gizmoColor);
        }
    }
}
