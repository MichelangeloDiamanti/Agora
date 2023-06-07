using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;
using Menge.BFSM;
using System;

[CustomNodeEditor(typeof(OBBGoalNode))]
public class OBBGoalNodeEditor : NodeEditor
{
    private OBBGoalNode m_OBBGoalNode;

    public override void OnBodyGUI()
    {
        if (m_OBBGoalNode == null) m_OBBGoalNode = target as OBBGoalNode;

        serializedObject.Update();

        m_OBBGoalNode.updateGoalset();
        goalSetPicker();

        goalBasePropertiesPicker();

        // Point Goal Properties
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("obbGoal.x"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("obbGoal.y"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("obbGoal.width"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("obbGoal.height"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("obbGoal.angle"), true);


        if (!m_OBBGoalNode.HasPort("obbGoal")) m_OBBGoalNode.AddDynamicOutput(typeof(Goal), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "obbGoal");
        NodePort outputGoal = m_OBBGoalNode.GetPort("obbGoal");
        NodeEditorGUILayout.PortField(new GUIContent("Output Goal"), outputGoal);

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();  
    }

    private void goalBasePropertiesPicker()
    {
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("obbGoal.capacity"), true);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("obbGoal.weight"), true);
    }

    private void goalSetPicker()
    {
        // Goalset Nodeport
        if (!m_OBBGoalNode.HasPort("goalSet")) m_OBBGoalNode.AddDynamicInput(typeof(GoalSet), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "goalSet");
        NodePort goalSet = m_OBBGoalNode.GetPort("goalSet");

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