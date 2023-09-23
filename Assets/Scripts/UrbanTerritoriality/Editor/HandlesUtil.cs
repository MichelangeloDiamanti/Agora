using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UrbanTerritoriality.EditorTools
{
    /** A static class with methods related
     * to scene handles. That is objects
     * that appear in the scene view in
     * the Unity editor. Typically used
     * for manipulating objects in a scene.
     */ 
    public static class HandlesUtil
    {
        /** Position of the mouse when it was pressed down */
        private static Vector2 posHandleMouseStart;

        /** The current position of the mouse */
        private static Vector2 posHandleMouseCurrent;

        /** The position of the handle in the world when the mouse is
         * pressed down
         */
        private static Vector3 posHandleWorldStart;

        /** Set to true while a handle is being moved by the user. */
        private static bool posHandleMoved;

        /** Checks if a handle is selectable or not
         * @param id The id of the handle control
         * @param position The position of the handle in the world.
         * @radius The radius around the handle where it is selectable
         * by the mouse.
         * @returns True if the handle is selectable
         */
        public static bool IsSelectable(int id, Vector3 position, float radius)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            if (HandleUtility.nearestControl == id)
            {
                Vector2 moPos = Event.current.mousePosition;
                Vector3 objScreenPos = Camera.current.WorldToScreenPoint(position);
                Vector2 objScreenPos2d = new Vector2(
                    objScreenPos.x, Camera.current.pixelHeight - objScreenPos.y);
                float dist = Vector2.Distance(moPos, objScreenPos2d);
                if (dist <= radius)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * This method should only be called inside the
         * OnSceneGUI method of an Editor class.
         * Creates a position handle in the scene view.
         * @param id The id of the handle.
         * @param position The position of the handle in the world.
         * @param capFunc Function for drawing the handle.
         * @param size The size of the handle
         * @param pixelRadius The radius of the area where the handle is selectable
         * my the mouse.
         * @param rotation of the handle
         * @param colorSelectable The color of the handle when it is selectable.
         * @param colorSelected The color of the handle when it is selected.
         * @return Returns the position of the handle in the scene.
         */
        public static Vector3 PositionHandle(int id, Vector3 position,
            Handles.CapFunction capFunc, float size, float pixelRadius, Quaternion rotation,
            Color colorSelectable,
            Color colorSelected)
        {
            Vector3 screenPos =
                Handles.matrix.MultiplyPoint(position);
            Matrix4x4 cachedMatrix = Handles.matrix;
            float handleSize = size * Vector3.Distance(Camera.current.transform.position, position);
            switch (Event.current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (IsSelectable(id, position, pixelRadius) && (Event.current.button == 0
                        || Event.current.button == 1))
                    {
                        GUIUtility.hotControl = id;
                        posHandleMouseCurrent = Event.current.mousePosition;
                        posHandleMouseStart = posHandleMouseCurrent;
                        posHandleWorldStart = position;
                        posHandleMoved = false;

                        Event.current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id &&
                        (Event.current.button == 0 ||
                        Event.current.button == 1))
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        posHandleMouseCurrent += new Vector2(
                            Event.current.delta.x, -Event.current.delta.y);
                        Vector3 position2 =
                            Camera.current.WorldToScreenPoint(
                                Handles.matrix.MultiplyPoint(posHandleWorldStart))
                                + (Vector3)(posHandleMouseCurrent -
                                posHandleMouseStart);
                        position = Handles.matrix.inverse.MultiplyPoint(Camera.current.ScreenToWorldPoint(position2));

                        if (Camera.current.transform.forward ==
                            Vector3.forward ||
                            Camera.current.transform.forward
                            == -Vector3.forward)
                            position.z = posHandleWorldStart.z;
                        if (Camera.current.transform.forward ==
                            Vector3.up ||
                            Camera.current.transform.forward ==
                            -Vector3.up)
                            position.y = posHandleWorldStart.y;
                        if (Camera.current.transform.forward
                            == Vector3.right ||
                            Camera.current.transform.forward ==
                            -Vector3.right)
                            position.x = posHandleWorldStart.x;

                        posHandleMoved = true;

                        GUI.changed = true;
                        Event.current.Use();
                    }
                    break;
                case EventType.Repaint:
                    Color currentColor = Handles.color;
                    if (IsSelectable(id, position, pixelRadius))
                    {
                        Handles.color = colorSelectable;
                    }
                    if (GUIUtility.hotControl == id && posHandleMoved)
                    {
                        Handles.color = colorSelected;
                    }
                    Handles.matrix = Matrix4x4.identity;
                    capFunc(id, screenPos, rotation,
                        handleSize, EventType.Repaint);
                    Handles.matrix = cachedMatrix;
                    Handles.color = currentColor;
                    break;
                case EventType.Layout:
                    Handles.matrix = Matrix4x4.identity;
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(screenPos, handleSize));
                    Handles.matrix = cachedMatrix;
                    break;
            }

            return position;
        }
    }
}
