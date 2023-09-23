using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNode;
using XNodeEditor;
using Menge.BFSM;
using System;

[CustomNodeEditor(typeof(TransitionNode))]
public class TransitionNodeEditor : NodeEditor
{
    private TransitionNode m_TransitionNode;

    public override void OnBodyGUI()
    {
        if (m_TransitionNode == null) m_TransitionNode = target as TransitionNode;

        serializedObject.Update();

        if (!m_TransitionNode.HasPort("transition.from")) m_TransitionNode.AddDynamicInput(typeof(State), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "transition.from");
        if (!m_TransitionNode.HasPort("transition.condition")) m_TransitionNode.AddDynamicInput(typeof(Condition), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "transition.condition");
        if (!m_TransitionNode.HasPort("transition.to")) m_TransitionNode.AddDynamicOutput(typeof(Transition), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "transition.to");

        NodePort fromPort = m_TransitionNode.GetPort("transition.from");
        NodeEditorGUILayout.PortField(new GUIContent("From"), fromPort);

        NodePort conditionPort = m_TransitionNode.GetPort("transition.condition");
        NodeEditorGUILayout.PortField(new GUIContent("Condition"), conditionPort);

        NodePort toPort = m_TransitionNode.GetPort("transition.to");
        NodeEditorGUILayout.PortField(new GUIContent("To"), toPort);

        m_TransitionNode.transition.from = m_TransitionNode.GetInputValue<State>("transition.from");
        m_TransitionNode.transition.condition = m_TransitionNode.GetInputValue<Condition>("transition.condition");
        m_TransitionNode.transition.to = toPort.ConnectionCount > 0 ? (toPort.GetConnection(0).node as StateNode).state : null;

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    // public override int GetWidth()
    // {
    //     return 400;
    // }

}