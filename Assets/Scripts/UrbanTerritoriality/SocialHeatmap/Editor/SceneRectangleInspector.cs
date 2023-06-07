using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UrbanTerritoriality.Maps
{
    /** Custom inspector for SceneRectangle */
    [CustomEditor(typeof(SceneRectangle), true)]
    public class SceneRectangleInspector : Editor
    {
        /** The SceneRectangle object that
         * this inspector is for */
        protected SceneRectangle sRect;

        /** Unity OnEnable method */
        void OnEnable()
        {
            sRect = (SceneRectangle)target;
        }

        /** Shows some things and handles user
         * input in the scene view */
        void OnSceneGUI()
        {
            Vector3 center = sRect.transform.position;
            Vector2 size = sRect.size;
            Vector2 halfSize = size * 0.5f;
            Vector3 minX = new Vector3(center.x - halfSize.x, center.y, center.z);
            Vector3 maxX = new Vector3(center.x + halfSize.x, center.y, center.z);
            Vector3 minZ = new Vector3(center.x, center.y, center.z - halfSize.y);
            Vector3 maxZ = new Vector3(center.x, center.y, center.z + halfSize.y);

            float handleSize = 0.02f;
            float handleSelectRadius = 100;
            Color colorSelectable = new Color(1, 1, 0.5f, 1);
            Color colorSelected = new Color(1, 1, 0, 1f);
            Handles.color = sRect.gizmoColor;

            /** Add four position handles, one on each
             * side of the rectangle */
            int handleCount = 4;
            Vector3[] inPos = new Vector3[] { minX, maxX, minZ, maxZ };
            Vector3[] outPos = new Vector3[handleCount];
            for (int i = 0; i < handleCount; i++)
            {
                int posHandleId = GUIUtility.GetControlID(
                    ("sri_poshandle" + i.ToString()).GetHashCode(), FocusType.Passive);
                outPos[i] = EditorTools.HandlesUtil.PositionHandle(
                    posHandleId, inPos[i], Handles.CubeHandleCap, handleSize, handleSelectRadius,
                    Quaternion.identity, colorSelectable, colorSelected);
            }
            minX = outPos[0];
            maxX = outPos[1];
            minZ = outPos[2];
            maxZ = outPos[3];

            float centerX = (maxX.x + minX.x) * 0.5f;
            float centerZ = (maxZ.z + minZ.z) * 0.5f;
            center.x = centerX;
            center.z = centerZ;
            size = new Vector2(maxX.x - minX.x, maxZ.z - minZ.z);
            if (size.x > 0 && size.y > 0)
            {
                Undo.RecordObject(sRect.transform, "Position Change");
                Undo.RecordObject(sRect, "Heatmap Size Change");
                sRect.size = size;
                sRect.transform.position = center;
            }
        }
    }
}
