using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;
using Menge.BFSM;
using System;

[CustomNodeEditor(typeof(PointGoalNode))]
public class PointGoalNodeEditor : NodeEditor
{
    private PointGoalNode m_PointGoalNode;

    public override void OnBodyGUI()
    {
        if (m_PointGoalNode == null) m_PointGoalNode = target as PointGoalNode;

        serializedObject.Update();

        m_PointGoalNode.updateGoalset();
        goalSetPicker();

        goalBasePropertiesPicker();

        // Point Goal Properties
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("pointGoal.x"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("pointGoal.y"), true);

        if (!m_PointGoalNode.HasPort("pointGoal")) m_PointGoalNode.AddDynamicOutput(typeof(Goal), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "pointGoal");
        NodePort outputGoal = m_PointGoalNode.GetPort("pointGoal");
        NodeEditorGUILayout.PortField(new GUIContent("Output Goal"), outputGoal);

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();  
    }

    private void goalBasePropertiesPicker()
    {
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("pointGoal.capacity"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("pointGoal.weight"), true);
    }

    private void goalSetPicker()
    {
        // Goalset Nodeport
        if (!m_PointGoalNode.HasPort("goalSet")) m_PointGoalNode.AddDynamicInput(typeof(GoalSet), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "goalSet");
        NodePort goalSet = m_PointGoalNode.GetPort("goalSet");

        if(!goalSet.IsConnected)
        {
            // warning
            EditorGUILayout.HelpBox("Goal Set is not connected, so this goal will not be saved", MessageType.Warning);
        }

        NodeEditorGUILayout.PortField(new GUIContent("Goal Set"), goalSet);
    }


    // public override int GetWidth()
    // {
    //     return 300;
    // }
}