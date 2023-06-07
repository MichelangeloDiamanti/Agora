using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;

[CustomNodeEditor(typeof(HeatmapInverterNode))]
public class HeatmapInverterNodeEditor : NodeEditor
{
    private HeatmapInverterNode m_TextureInverterNode;

    public override void OnBodyGUI()
    {
        if (m_TextureInverterNode == null) m_TextureInverterNode = target as HeatmapInverterNode;

        serializedObject.Update();

        ShowInputHeatmapField();
        m_TextureInverterNode.UpdateOutputHeatmap();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();

        // Keep repainting the GUI of the active NodeEditorWindow
        NodeEditorWindow.current.Repaint();
    }

    private void ShowInputHeatmapField()
    {
        EditorGUI.BeginChangeCheck();

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("minValue"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("maxValue"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("clamp"));

        // Preview the output heatmap
        if (m_TextureInverterNode.outputHeatmap != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(m_TextureInverterNode.outputHeatmap), GUILayout.Width(200), GUILayout.Height(200));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // NodePort for the input texture
        if (!m_TextureInverterNode.HasPort("inputHeatmap")) m_TextureInverterNode.AddDynamicInput(typeof(Texture), Node.ConnectionType.Override, Node.TypeConstraint.None, "inputHeatmap");
        NodeEditorGUILayout.PortField(m_TextureInverterNode.GetInputPort("inputHeatmap"));

        // NodePort for the output heatmap
        if (!m_TextureInverterNode.HasPort("outputHeatmap")) m_TextureInverterNode.AddDynamicOutput(typeof(Texture), Node.ConnectionType.Override, Node.TypeConstraint.None, "outputHeatmap");
        NodeEditorGUILayout.PortField(m_TextureInverterNode.GetOutputPort("outputHeatmap"));
    }

    public override int GetWidth()
    {
        return 300;
    }
}
