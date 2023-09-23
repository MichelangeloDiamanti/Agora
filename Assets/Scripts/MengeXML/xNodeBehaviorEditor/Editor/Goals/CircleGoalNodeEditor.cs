using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;
using Menge.BFSM;
using System;

[CustomNodeEditor(typeof(CircleGoalNode))]
public class CircleGoalNodeEditor : NodeEditor
{
    private CircleGoalNode m_CircleGoalNode;

    public override void OnBodyGUI()
    {
        if (m_CircleGoalNode == null) m_CircleGoalNode = target as CircleGoalNode;

        serializedObject.Update();

        m_CircleGoalNode.updateGoalset();
        goalSetPicker();

        goalBasePropertiesPicker();

        // Point Goal Properties
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("circleGoal.x"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("circleGoal.y"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("circleGoal.radius"), true);

        if (!m_CircleGoalNode.HasPort("circleGoal")) m_CircleGoalNode.AddDynamicOutput(typeof(Goal), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "circleGoal");
        NodePort outputGoal = m_CircleGoalNode.GetPort("circleGoal");
        NodeEditorGUILayout.PortField(new GUIContent("Output Goal"), outputGoal);

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();  
    }

    private void goalBasePropertiesPicker()
    {
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("circleGoal.capacity"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("circleGoal.weight"), true);
    }

    private void goalSetPicker()
    {
        // Goalset Nodeport
        if (!m_CircleGoalNode.HasPort("goalSet")) m_CircleGoalNode.AddDynamicInput(typeof(GoalSet), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "goalSet");
        NodePort goalSet = m_CircleGoalNode.GetPort("goalSet");

        if(!goalSet.IsConnected)
        {
            // warning
            EditorGUILayout.HelpBox("Goal Set is not connected, so this goal will not be saved", MessageType.Warning);
        }

        NodeEditorGUILayout.PortField(new GUIContent("Goal Set"), goalSet);
    }


    public override int GetWidth()
    {
        return 300;
    }
}