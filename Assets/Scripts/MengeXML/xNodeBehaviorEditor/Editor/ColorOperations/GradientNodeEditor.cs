using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;

[CustomNodeEditor(typeof(GradientNode))]
public class GradientNodeEditor : NodeEditor
{
    private GradientNode m_GradientNode;

    public override void OnBodyGUI()
    {
        if (m_GradientNode == null) m_GradientNode = target as GradientNode;

        serializedObject.Update();

        ShowInputHeatmapField();
        m_GradientNode.UpdateOutputHeatmap();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();

        // Keep repainting the GUI of the active NodeEditorWindow
        NodeEditorWindow.current.Repaint();
    }

    private void ShowInputHeatmapField()
    {
        EditorGUI.BeginChangeCheck();

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("mapColorGradient"));

        // Preview the output heatmap
        if (m_GradientNode.outputHeatmap != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(m_GradientNode.outputHeatmap), GUILayout.Width(200), GUILayout.Height(200));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // NodePort for the input texture
        if (!m_GradientNode.HasPort("inputHeatmap")) m_GradientNode.AddDynamicInput(typeof(Texture), Node.ConnectionType.Override, Node.TypeConstraint.None, "inputHeatmap");
        NodeEditorGUILayout.PortField(m_GradientNode.GetInputPort("inputHeatmap"));

        // NodePort for the output heatmap
        if (!m_GradientNode.HasPort("outputHeatmap")) m_GradientNode.AddDynamicOutput(typeof(Texture), Node.ConnectionType.Override, Node.TypeConstraint.None, "outputHeatmap");
        NodeEditorGUILayout.PortField(m_GradientNode.GetOutputPort("outputHeatmap"));
    }

    public override int GetWidth()
    {
        return 300;
    }
}
