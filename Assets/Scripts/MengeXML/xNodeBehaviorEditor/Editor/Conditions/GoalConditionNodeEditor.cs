using UnityEngine;
using XNode;
using XNodeEditor;
using Menge.BFSM;

[CustomNodeEditor(typeof(GoalConditionNode))]
public class GooalConditionNodeEditor : NodeEditor
{
    private GoalConditionNode m_GoalConditionNode;

    public override void OnBodyGUI()
    {
        if (m_GoalConditionNode == null) m_GoalConditionNode = target as GoalConditionNode;

        serializedObject.Update();

        showConditionPicker();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    private void showConditionPicker()
    {
        if (!m_GoalConditionNode.HasPort("condition.outCondition"))
        {
            m_GoalConditionNode.AddDynamicOutput(typeof(Condition), Node.ConnectionType.Multiple, Node.TypeConstraint.None, fieldName: "condition.outCondition");
        }

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("goalCondition.distance"), false);
        NodeEditorGUILayout.PortField(new GUIContent("Output Condition"), m_GoalConditionNode.GetPort("condition.outCondition"));
    }

    public override int GetWidth()
    {
        return 200;
    }

}