using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;
using UnityEditor;
using XNode;
using UrbanTerritoriality.Maps;

namespace Agora.Menge.Heatmap
{

    /// <summary> 
    /// NodeEditor functions similarly to the Editor class, only it is xNode specific.
    /// Custom node editors should have the CustomNodeEditor attribute that defines which node type it is an editor for.
    /// </summary>
    [CustomNodeEditor(typeof(GPUHeatmapNode))]
    public class GPUHeatmapNodeEditor : NodeEditor
    {

        public override void OnBodyGUI()
        {
            // `target` points to the node, but it is of type `Node`, so cast it.
            GPUHeatmapNode gpuHeatmapNode = target as GPUHeatmapNode;

            // Draw the field for the GPUHeatmap reference
            EditorGUI.BeginChangeCheck();
            gpuHeatmapNode.gpuHeatmap = (GPUHeatmap)EditorGUILayout.ObjectField("GPU Heatmap", gpuHeatmapNode.gpuHeatmap, typeof(GPUHeatmap), true);
            if (EditorGUI.EndChangeCheck())
            {
                // Save the changes
                EditorUtility.SetDirty(gpuHeatmapNode);
            }

            // Preview the output texture
            Texture texture = gpuHeatmapNode.gpuHeatmap?.paintGrid;
            if (texture != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(new GUIContent(texture), GUILayout.Width(200), GUILayout.Height(200));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            NodeEditorGUILayout.PortField(new GUIContent("Output Heatmap"), gpuHeatmapNode.GetPort("outputHeatmap"));

            // Keep repainting the GUI of the active NodeEditorWindow
            NodeEditorWindow.current.Repaint();
        }

        public override int GetWidth()
        {
            return 300;
        }
    }
}