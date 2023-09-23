using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;
using UnityEditorInternal;

[CustomNodeEditor(typeof(HeatmapBaseCombinerNode))]
public class TextureBaseCombinerNodeEditor : NodeEditor
{
    private HeatmapBaseCombinerNode m_TextureBaseCombinerNode;


    public override void OnBodyGUI()
    {
        if (m_TextureBaseCombinerNode == null) m_TextureBaseCombinerNode = target as HeatmapBaseCombinerNode;

        serializedObject.Update();

        showAddCombinerPicker();

        m_TextureBaseCombinerNode.updateOutputHeatmap();

         // Apply property modifications
        serializedObject.ApplyModifiedProperties();

        // Keep repainting the GUI of the active NodeEditorWindow
        NodeEditorWindow.current.Repaint();
    }

    private void showAddCombinerPicker()
    {
        NodeEditorGUILayout.DynamicPortList(
            "inputHeatmaps", // field name
            typeof(Texture), // field type
            serializedObject, // serializable object
            NodePort.IO.Input, // new port i/o
            Node.ConnectionType.Override, // new port connection type
            Node.TypeConstraint.None,
            OnCreateReorderableList); // onCreate override. This is where the magic happens.

        // preview the output heatmap
        if (m_TextureBaseCombinerNode.outputHeatmap != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(m_TextureBaseCombinerNode.outputHeatmap), GUILayout.Width(200), GUILayout.Height(200));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // toggle to normalize the output heatmap
        bool normalizeValue = EditorGUILayout.Toggle("Normalize", m_TextureBaseCombinerNode.normalize);
        if (normalizeValue != m_TextureBaseCombinerNode.normalize)
        {
            m_TextureBaseCombinerNode.normalize = normalizeValue;
            m_TextureBaseCombinerNode.updateOutputHeatmap();
        }

        // nodeport for the output heatmap
        if (!m_TextureBaseCombinerNode.HasPort("outputHeatmap")) m_TextureBaseCombinerNode.AddDynamicOutput(typeof(Texture), Node.ConnectionType.Multiple, Node.TypeConstraint.None, "outputHeatmap");
        NodeEditorGUILayout.PortField(m_TextureBaseCombinerNode.GetOutputPort("outputHeatmap"));
    }

    void OnCreateReorderableList(ReorderableList list)
    {

        // Override drawHeaderCallback to display node's name instead
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Heatmaps");
        };

        list.onChangedCallback += (ReorderableList l) =>
        {
            OnListChanged(l);
        };

    }

    protected virtual void OnListChanged(ReorderableList list)
    {
        // update the output heatmap
        m_TextureBaseCombinerNode.updateOutputHeatmap();
    }

    public override int GetWidth()
    {
        return 300;
    }


}
