using UnityEngine;
using UnityEditor;
using XNode;
using XNodeEditor;
using Menge.BFSM;
using Menge.Math;

[CustomNodeEditor(typeof(TimerConditionNode))]
public class TimerConditionNodeEditor : NodeEditor
{
    private TimerConditionNode m_TimerConditionNode;

    public override void OnBodyGUI()
    {
        if (m_TimerConditionNode == null) m_TimerConditionNode = target as TimerConditionNode;

        serializedObject.Update();

        showConditionPicker();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    private void showConditionPicker()
    {
        if (!m_TimerConditionNode.HasPort("timerCondition.outCondition"))
        {
            m_TimerConditionNode.AddDynamicOutput(typeof(Condition), Node.ConnectionType.Multiple, Node.TypeConstraint.None, fieldName: "timerCondition.outCondition");
        }

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("timerCondition.distribution"), true);
        NodeEditorGUILayout.PortField(new GUIContent("Output Condition"), m_TimerConditionNode.GetPort("timerCondition.outCondition"));
    }

    public override int GetWidth()
    {
        return 200;
    }

}