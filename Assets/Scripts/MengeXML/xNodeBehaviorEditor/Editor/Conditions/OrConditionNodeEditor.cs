using UnityEngine;
using XNode;
using XNodeEditor;
using Menge.BFSM;

[CustomNodeEditor(typeof(OrConditionNode))]
public class OrConditionNodeEditor : NodeEditor
{
    private OrConditionNode m_OrConditionNode;

    public override void OnBodyGUI()
    {
        if (m_OrConditionNode == null) m_OrConditionNode = target as OrConditionNode;

        serializedObject.Update();

        showConditionPicker();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    private void showConditionPicker()
    {
        if (!m_OrConditionNode.HasPort("condition.inConditionA"))
        {
            m_OrConditionNode.AddDynamicInput(typeof(Condition), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "condition.inConditionA");
        }
        if (!m_OrConditionNode.HasPort("condition.inConditionB"))
        {
            m_OrConditionNode.AddDynamicInput(typeof(Condition), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "condition.inConditionB");
        }
        if (!m_OrConditionNode.HasPort("condition.outCondition"))
        {
            m_OrConditionNode.AddDynamicOutput(typeof(Condition), Node.ConnectionType.Multiple, Node.TypeConstraint.None, fieldName: "condition.outCondition");
        }
        NodeEditorGUILayout.PortField(new GUIContent("Input Condition A"), m_OrConditionNode.GetPort("condition.inConditionA"));
        NodeEditorGUILayout.PortField(new GUIContent("Input Condition B"), m_OrConditionNode.GetPort("condition.inConditionB"));
        NodeEditorGUILayout.PortField(new GUIContent("Output Condition"), m_OrConditionNode.GetPort("condition.outCondition"));
    }

    public override int GetWidth()
    {
        return 200;
    }

}