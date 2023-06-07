using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;
using Menge.BFSM;
using MengeEditor;
using System;
using UnityEditorInternal;

[CustomNodeEditor(typeof(HeatmapGoalSelectorNode))]
public class HeatmapGoalSelectorNodeEditor : NodeEditor
{
    private HeatmapGoalSelectorNode m_TextureGoalSelectorNode;
    private bool m_IsCollapsed = false;

    public override void OnBodyGUI()
    {
        // Draw the default GUI first, so we don't have to do all of that manually.
        // base.OnBodyGUI();

        if (m_TextureGoalSelectorNode == null) m_TextureGoalSelectorNode = target as HeatmapGoalSelectorNode;

        serializedObject.Update();

        m_IsCollapsed = EditorGUILayout.Toggle("Collapse", m_IsCollapsed);

        m_TextureGoalSelectorNode.LoadMaterialAndShader();
        m_TextureGoalSelectorNode.UpdateHeatmap();

        showTextureGoalSelector();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    private void showTextureGoalSelector()
    {
        // input texture


        // nodeport for the input texture
        if (!m_TextureGoalSelectorNode.HasPort("heatmapGoalSelector.spatialHeatmap")) m_TextureGoalSelectorNode.AddDynamicInput(typeof(Texture), Node.ConnectionType.Override, Node.TypeConstraint.None, "heatmapGoalSelector.spatialHeatmap");
        NodeEditorGUILayout.PortField(new GUIContent("Spatial Heatmap"), m_TextureGoalSelectorNode.GetInputPort("heatmapGoalSelector.spatialHeatmap"));

        // nodeport for the input texture
        if (!m_TextureGoalSelectorNode.HasPort("heatmapGoalSelector.fieldOfPerceptionlHeatmap")) m_TextureGoalSelectorNode.AddDynamicInput(typeof(Texture), Node.ConnectionType.Override, Node.TypeConstraint.None, "heatmapGoalSelector.fieldOfPerceptionlHeatmap");
        NodeEditorGUILayout.PortField(new GUIContent("Field of Perception Heatmap"), m_TextureGoalSelectorNode.GetInputPort("heatmapGoalSelector.fieldOfPerceptionlHeatmap"));

        if (!m_IsCollapsed)
        {
            // vector2 field for the offset
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("heatmapGoalSelector.offset"));
            // float field for the rotation
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("heatmapGoalSelector.rotation"));


            // preview the output heatmap
            if (m_TextureGoalSelectorNode.heatmapGoalSelector.outputHeatmap != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(new GUIContent(m_TextureGoalSelectorNode.heatmapGoalSelector.outputHeatmap), GUILayout.Width(200), GUILayout.Height(200));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
        // nodeport for the output heatmap
        if (!m_TextureGoalSelectorNode.HasPort("heatmapGoalSelector")) m_TextureGoalSelectorNode.AddDynamicOutput(typeof(HeatmapGoalSelector), Node.ConnectionType.Override, Node.TypeConstraint.None, "heatmapGoalSelector");
        NodeEditorGUILayout.PortField(m_TextureGoalSelectorNode.GetOutputPort("heatmapGoalSelector"));
    }

    public override int GetWidth()
    {
        return 300;
    }

}
