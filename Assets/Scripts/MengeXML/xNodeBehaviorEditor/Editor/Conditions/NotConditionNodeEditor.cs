using UnityEngine;
using XNode;
using XNodeEditor;
using Menge.BFSM;

[CustomNodeEditor(typeof(NotConditionNode))]
public class NotConditionNodeEditor : NodeEditor
{
    private NotConditionNode m_NotConditionNode;

    public override void OnBodyGUI()
    {
        if (m_NotConditionNode == null) m_NotConditionNode = target as NotConditionNode;

        serializedObject.Update();

        showConditionPicker();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    private void showConditionPicker()
    {
        if (!m_NotConditionNode.HasPort("condition.inCondition"))
        {
            m_NotConditionNode.AddDynamicInput(typeof(Condition), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "condition.inCondition");
        }
        if (!m_NotConditionNode.HasPort("condition.outCondition"))
        {
            m_NotConditionNode.AddDynamicOutput(typeof(Condition), Node.ConnectionType.Multiple, Node.TypeConstraint.None, fieldName: "condition.outCondition");
        }
        NodeEditorGUILayout.PortField(new GUIContent("Input Condition"), m_NotConditionNode.GetPort("condition.inCondition"));
        NodeEditorGUILayout.PortField(new GUIContent("Output Condition"), m_NotConditionNode.GetPort("condition.outCondition"));
    }

    public override int GetWidth()
    {
        return 200;
    }

}