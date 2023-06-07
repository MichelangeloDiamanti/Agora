using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UrbanTerritoriality.Maps
{
    /** A custom inspector for BakedMap */
    [CustomEditor(typeof(BakedMap), true)]
    public class BakedHeatmapInspector : GeneralHeatmapInspector
    {
        /** Displays the inspector GUI */
        public override void OnInspectorGUI()
        {
            BakedMap bmap = (BakedMap)heatmap;
            if (bmap == null)
            {
                return;
            }
            Undo.RecordObject(bmap, "Baked map change");

            MapDataScriptableObject mapData = bmap.mapData;
            if (mapData != null)
            {
                bmap.SetValues();
                Undo.RecordObject(mapData, "Baked map data change");
            }
            bmap.mapData = (MapDataScriptableObject)EditorGUILayout.ObjectField(
            "Map Data", bmap.mapData, typeof(MapDataScriptableObject), false);
            if (mapData != null)
            {
                EditorGUILayout.LabelField("Position in World: " + mapData.position);
                EditorGUILayout.LabelField("Data Size: " + mapData.gridSize);
                EditorGUILayout.LabelField("Size in World: " + heatmap.size);
                EditorGUILayout.LabelField("Cell Size: " + heatmap.cellSize);
            }
            bmap.gizmoColor = EditorGUILayout.ColorField("GizmoColor", heatmap.gizmoColor);
        }

        /** Overrides the method in the parent class
         * so the functionality in the OnSceneGUI
         * method of the parent class is not inherited */
        public override void OnSceneGUI()
        {

        }
    }
}

