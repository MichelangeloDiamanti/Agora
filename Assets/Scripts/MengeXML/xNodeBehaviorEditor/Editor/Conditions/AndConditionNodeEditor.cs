using UnityEngine;
using XNode;
using XNodeEditor;
using Menge.BFSM;

[CustomNodeEditor(typeof(AndConditionNode))]
public class AndConditionNodeEditor : NodeEditor
{
    private AndConditionNode m_AndConditionNode;

    public override void OnBodyGUI()
    {
        if (m_AndConditionNode == null) m_AndConditionNode = target as AndConditionNode;

        serializedObject.Update();

        showConditionPicker();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    private void showConditionPicker()
    {
        if (!m_AndConditionNode.HasPort("condition.inConditionA"))
        {
            m_AndConditionNode.AddDynamicInput(typeof(Condition), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "condition.inConditionA");
        }
        if (!m_AndConditionNode.HasPort("condition.inConditionB"))
        {
            m_AndConditionNode.AddDynamicInput(typeof(Condition), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "condition.inConditionB");
        }
        if (!m_AndConditionNode.HasPort("condition.outCondition"))
        {
            m_AndConditionNode.AddDynamicOutput(typeof(Condition), Node.ConnectionType.Multiple, Node.TypeConstraint.None, fieldName: "condition.outCondition");
        }
        NodeEditorGUILayout.PortField(new GUIContent("Input Condition A"), m_AndConditionNode.GetPort("condition.inConditionA"));
        NodeEditorGUILayout.PortField(new GUIContent("Input Condition B"), m_AndConditionNode.GetPort("condition.inConditionB"));
        NodeEditorGUILayout.PortField(new GUIContent("Output Condition"), m_AndConditionNode.GetPort("condition.outCondition"));
    }

    public override int GetWidth()
    {
        return 200;
    }

}