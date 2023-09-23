using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using XNode;
using XNodeEditor;
using Menge.BFSM;
using System;

[CustomNodeEditor(typeof(VelocityModifierNode))]
public class VelocityModifierNodeEditor : NodeEditor
{
    private VelocityModifierNode m_VelocityModifierNode;

    public override void OnBodyGUI()
    {
        if (m_VelocityModifierNode == null) m_VelocityModifierNode = target as VelocityModifierNode;

        serializedObject.Update();


        showVelocityModifierPicker();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    private void showVelocityModifierPicker()
    {
        // Allows Adding a new action of the specified type
        m_VelocityModifierNode.selectedVelocityModifierType = (VelocityModifierTypes)EditorGUILayout.EnumPopup(m_VelocityModifierNode.selectedVelocityModifierType);

        switch (m_VelocityModifierNode.selectedVelocityModifierType)
        {
            case VelocityModifierTypes.SCALE:
                {
                    if (m_VelocityModifierNode.velocityModifier.GetType() != typeof(ScaleVelocityModifier))
                    {
                        m_VelocityModifierNode.velocityModifier = new ScaleVelocityModifier();
                        serializedObject.Update();

                        refreshOutputVelocityModifierConnections();
                    }
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("velocityModifier.scale"), true);
                    break;
                }
            case VelocityModifierTypes.FORMATION:
                {
                    if (m_VelocityModifierNode.velocityModifier.GetType() != typeof(FormationVelocityModifier))
                    {
                        m_VelocityModifierNode.velocityModifier = new FormationVelocityModifier();
                        serializedObject.Update();

                        refreshOutputVelocityModifierConnections();

                    }

                    FormationVelocityModifier formationVelocityModifier = m_VelocityModifierNode.velocityModifier as FormationVelocityModifier;

                    GUILayout.BeginHorizontal();
                    //formationVelocityModifier.fileName = EditorGUILayout.TextField(formationVelocityModifier.fileName);
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("velocityModifier.fileName"), true);
                    if (GUILayout.Button("Select"))
                    {
                        formationVelocityModifier.fileName = EditorUtility.OpenFilePanel("Menge XML Behavior File", "", "txt");
                        serializedObject.Update();
                    }

                    GUILayout.EndHorizontal();

                    //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("velocityModifier.fileName"), true);
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("velocityModifier.formation"), true);
                    break;
                }
            default:
                break;
        }

        NodeEditorGUILayout.PortField(new GUIContent("Output Velocity Modifier"), m_VelocityModifierNode.GetPort("velocityModifier"));

    }

    private void refreshOutputVelocityModifierConnections()
    {
        // reconnect each connection to trigger the OnCreateConnection event, thus getting the updated value for the output port
        List<NodePort> connections = m_VelocityModifierNode.GetPort("velocityModifier").GetConnections();
        foreach (NodePort np in connections) m_VelocityModifierNode.GetPort("velocityModifier").Disconnect(np);
        foreach (NodePort np in connections) m_VelocityModifierNode.GetPort("velocityModifier").Connect(np);
    }

    public override int GetWidth()
    {
        return 400;
    }

}