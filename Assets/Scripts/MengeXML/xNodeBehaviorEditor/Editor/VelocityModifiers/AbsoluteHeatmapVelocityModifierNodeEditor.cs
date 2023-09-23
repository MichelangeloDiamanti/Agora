using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;
using Menge.BFSM;

[CustomNodeEditor(typeof(AbsoluteHeatmapVelocityModifierNode))]
public class AbsoluteHeatmapVelocityModifierNodeEditor : NodeEditor
{
    private AbsoluteHeatmapVelocityModifierNode m_AbsoluteHeatmapVelocityModifierNode;

    public override void OnBodyGUI()
    {
        if (m_AbsoluteHeatmapVelocityModifierNode == null) m_AbsoluteHeatmapVelocityModifierNode = target as AbsoluteHeatmapVelocityModifierNode;

        serializedObject.Update();

        m_AbsoluteHeatmapVelocityModifierNode.updateHeatmapTexture();

        showVelModifierPicker();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();  
    }

    private void showVelModifierPicker()
    {
        AbsoluteHeatmapVelocityModifier velocityModifier = m_AbsoluteHeatmapVelocityModifierNode.absoluteHeatmapVelocityModifier;

        GUILayout.BeginVertical("Box");

        // Header
        EditorGUILayout.LabelField("Heatmap", EditorStyles.boldLabel);
        MengeEditor.Utils.DrawUILine(Color.gray, 1, 3);

        // Texture Nodeport
        if (!m_AbsoluteHeatmapVelocityModifierNode.HasPort("absoluteHeatmapVelocityModifier.heatmap")) m_AbsoluteHeatmapVelocityModifierNode.AddDynamicInput(typeof(Texture2D), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "absoluteHeatmapVelocityModifier.heatmap");
        
        // Texture Picker
        EditorGUILayout.BeginHorizontal();
        NodeEditorGUILayout.PortField(new GUIContent("Texture"), m_AbsoluteHeatmapVelocityModifierNode.GetInputPort("absoluteHeatmapVelocityModifier.heatmap"));

        // EditorGUILayout.LabelField("Texture");
        velocityModifier.heatmap = (Texture2D)EditorGUILayout.ObjectField(velocityModifier.heatmap, typeof(Texture2D), false);
        EditorGUILayout.EndHorizontal();

        // Texture Preview
        // TODO: handle for defining the center of the texture (and compute offset)
        if (velocityModifier.heatmap != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(velocityModifier.heatmap), GUILayout.Width(200), GUILayout.Height(200));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // Scale and Offset
        velocityModifier.scale = EditorGUILayout.FloatField("Scale", velocityModifier.scale);

        GUILayout.EndVertical();

        if (!m_AbsoluteHeatmapVelocityModifierNode.HasPort("outputVelocityModifier")) m_AbsoluteHeatmapVelocityModifierNode.AddDynamicOutput(typeof(VelocityModifier), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "outputVelocityModifier");
        NodePort outputVelocityModifier = m_AbsoluteHeatmapVelocityModifierNode.GetPort("outputVelocityModifier");
        NodeEditorGUILayout.PortField(new GUIContent("Output Velocity Modifier"), outputVelocityModifier);

    }

    public override int GetWidth()
    {
        return 300;
    }
}