using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;
using UnityEditorInternal;

[CustomNodeEditor(typeof(HeatmapAverageCombinerNode))]
public class TextureAverageCombinerNodeEditor : TextureBaseCombinerNodeEditor
{
    private HeatmapAverageCombinerNode m_TextureAverageCombinerNode;

    private ReorderableList weightList;

    public override void OnBodyGUI()
    {
        if (m_TextureAverageCombinerNode == null)
        {
            m_TextureAverageCombinerNode = target as HeatmapAverageCombinerNode;
            InitializeWeightList();
        }
        
        // Display the list of weights
        showWeightList();
        
        // Call the base class OnBodyGUI method
        base.OnBodyGUI();

    }

    private void showWeightList()
    {
        weightList.DoLayoutList();
    }

    protected override void OnListChanged(ReorderableList list)
    {
        base.OnListChanged(list);

        // Add or remove weights according to the inputHeatmaps list
        // Make sure the weights list is the same length as the inputHeatmaps list
        while (m_TextureAverageCombinerNode.weights.Count < m_TextureAverageCombinerNode.inputHeatmaps.Count - 1)
        {
            m_TextureAverageCombinerNode.weights.Add(0.5f);
        }

        while (m_TextureAverageCombinerNode.weights.Count > m_TextureAverageCombinerNode.inputHeatmaps.Count - 1)
        {
            m_TextureAverageCombinerNode.weights.RemoveAt(m_TextureAverageCombinerNode.weights.Count - 1);
        }
    }

    private void InitializeWeightList()
    {
        weightList = new ReorderableList(serializedObject, serializedObject.FindProperty("weights"), true, true, false, false);

        weightList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Weights");
        };

        weightList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = weightList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };
    }
}
