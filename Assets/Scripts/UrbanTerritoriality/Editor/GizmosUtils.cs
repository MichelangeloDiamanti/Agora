using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GizmosUtils : MonoBehaviour 
{
    [DrawGizmo(GizmoType.InSelectionHierarchy| GizmoType.NotInSelectionHierarchy)]
    static public void DrawLabel(Vector3 position, string text, Color color, int fontSize=10, TextAnchor textAnchor=TextAnchor.MiddleLeft, float fixedWidth=10f)
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.alignment = textAnchor;
        style.fixedWidth = fixedWidth;
        style.normal.textColor = color;
        Handles.color = color;
        Handles.Label(position, text, style);
    }
}
