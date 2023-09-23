using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;
using Menge.BFSM;
using System;

[CustomNodeEditor(typeof(AABBGoalNode))]
public class AABBGoalNodeEditor : NodeEditor
{
    private AABBGoalNode m_AABBGoalNode;

    public override void OnBodyGUI()
    {
        if (m_AABBGoalNode == null) m_AABBGoalNode = target as AABBGoalNode;

        serializedObject.Update();

        m_AABBGoalNode.updateGoalset();
        goalSetPicker();

        goalBasePropertiesPicker();

        // Point Goal Properties
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("aabbGoal.min_x"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("aabbGoal.max_x"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("aabbGoal.min_y"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("aabbGoal.max_y"), true);


        if (!m_AABBGoalNode.HasPort("aabbGoal")) m_AABBGoalNode.AddDynamicOutput(typeof(Goal), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "aabbGoal");
        NodePort outputGoal = m_AABBGoalNode.GetPort("aabbGoal");
        NodeEditorGUILayout.PortField(new GUIContent("Output Goal"), outputGoal);

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();  
    }

    private void goalBasePropertiesPicker()
    {
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("aabbGoal.capacity"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("aabbGoal.weight"), true);
    }

    private void goalSetPicker()
    {
        // Goalset Nodeport
        if (!m_AABBGoalNode.HasPort("goalSet")) m_AABBGoalNode.AddDynamicInput(typeof(GoalSet), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "goalSet");
        NodePort goalSet = m_AABBGoalNode.GetPort("goalSet");

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