using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;
using Menge.BFSM;
using System;

[CustomNodeEditor(typeof(AbsoluteHeatmapGoalNode))]
public class AbsoluteHeatmapGoalNodeEditor : NodeEditor
{
    private AbsoluteHeatmapGoalNode m_AbsoluteHeatmapGoalNode;

    public override void OnBodyGUI()
    {
        if (m_AbsoluteHeatmapGoalNode == null) m_AbsoluteHeatmapGoalNode = target as AbsoluteHeatmapGoalNode;

        serializedObject.Update();

        m_AbsoluteHeatmapGoalNode.updateGoalset();
        goalSetPicker();

        goalBasePropertiesPicker();

        m_AbsoluteHeatmapGoalNode.updateHeatmapTexture();
        heatmapGoalPicker();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();  
    }

    private void goalBasePropertiesPicker()
    {
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("absoluteHeatmapGoal.capacity"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("absoluteHeatmapGoal.weight"), true);
    }

    private void goalSetPicker()
    {
        // Goalset Nodeport
        if (!m_AbsoluteHeatmapGoalNode.HasPort("goalSet")) m_AbsoluteHeatmapGoalNode.AddDynamicInput(typeof(GoalSet), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "goalSet");
        NodePort goalSet = m_AbsoluteHeatmapGoalNode.GetPort("goalSet");

        if(!goalSet.IsConnected)
        {
            // warning
            EditorGUILayout.HelpBox("Goal Set is not connected, so this goal will not be saved", MessageType.Warning);
        }

        NodeEditorGUILayout.PortField(new GUIContent("Goal Set"), goalSet);
    }

    private void heatmapGoalPicker()
    {
        AbsoluteHeatmapGoal goal = m_AbsoluteHeatmapGoalNode.absoluteHeatmapGoal;

        GUILayout.BeginVertical("Box");

        // Header
        EditorGUILayout.LabelField("Heatmap", EditorStyles.boldLabel);
        MengeEditor.Utils.DrawUILine(Color.gray, 1, 3);

        // Texture Nodeport
        if (!m_AbsoluteHeatmapGoalNode.HasPort("absoluteHeatmapGoal.heatmap")) m_AbsoluteHeatmapGoalNode.AddDynamicInput(typeof(Texture2D), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "absoluteHeatmapGoal.heatmap");
        
        // Texture Picker
        EditorGUILayout.BeginHorizontal();
        NodeEditorGUILayout.PortField(new GUIContent("Texture"), m_AbsoluteHeatmapGoalNode.GetInputPort("absoluteHeatmapGoal.heatmap"));

        // EditorGUILayout.LabelField("Texture");
        goal.heatmap = (Texture2D)EditorGUILayout.ObjectField(goal.heatmap, typeof(Texture2D), false);
        EditorGUILayout.EndHorizontal();

        // Texture Preview
        // TODO: handle for defining the center of the texture (and compute offset)
        if (goal.heatmap != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(goal.heatmap), GUILayout.Width(200), GUILayout.Height(200));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // Scale and Offset
        goal.scale = EditorGUILayout.FloatField("Scale", goal.scale);

        GUILayout.EndVertical();

        if (!m_AbsoluteHeatmapGoalNode.HasPort("absoluteHeatmapGoal")) m_AbsoluteHeatmapGoalNode.AddDynamicOutput(typeof(Goal), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "absoluteHeatmapGoal");
        NodePort outputGoal = m_AbsoluteHeatmapGoalNode.GetPort("absoluteHeatmapGoal");
        NodeEditorGUILayout.PortField(new GUIContent("Output Goal"), outputGoal);

    }

    public override int GetWidth()
    {
        return 300;
    }
}