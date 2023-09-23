using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using XNode;
using XNodeEditor;
using Menge.BFSM;
using MengeEditor;
using System.IO;
using System;

[CustomNodeEditor(typeof(ColorConditionNode))]
public class ColorConditionNodeEditor : NodeEditor
{
    private ColorConditionNode m_ColorConditionNode;

    public override void OnBodyGUI()
    {
        if (m_ColorConditionNode == null) m_ColorConditionNode = target as ColorConditionNode;

        serializedObject.Update();

        showConditionPicker();

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    private void showConditionPicker()
    {
        GUILayout.BeginVertical("Box");

        // Header
        EditorGUILayout.LabelField("Heatmap", EditorStyles.boldLabel);
        MengeEditor.Utils.DrawUILine(Color.gray, 1, 3);

        // Texture Picker
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Texture");
        m_ColorConditionNode.conditionsHeatmap = (Texture2D)EditorGUILayout.ObjectField(m_ColorConditionNode.conditionsHeatmap, typeof(Texture2D), false);
        EditorGUILayout.EndHorizontal();

        // Texture Preview
        // TODO: handle for defining the center of the texture (and compute offset)
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField(new GUIContent(m_ColorConditionNode.conditionsHeatmap), GUILayout.Width(200), GUILayout.Height(200));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        // NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("condition.scale"), false);
        // NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("condition.offset"), false);

        // Scale and Offset
        m_ColorConditionNode.conditionsHeatmapScale = EditorGUILayout.FloatField("Scale", m_ColorConditionNode.conditionsHeatmapScale);
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Offset", GUILayout.MaxWidth(GetWidth() / 4));
        EditorGUILayout.LabelField("X", GUILayout.MaxWidth(GetWidth() / 8));
        m_ColorConditionNode.conditionsHeatmapOffset.x = EditorGUILayout.FloatField(m_ColorConditionNode.conditionsHeatmapOffset.x, GUILayout.MaxWidth(GetWidth() / 4));
        EditorGUILayout.LabelField("Y", GUILayout.MaxWidth(GetWidth() / 8));
        m_ColorConditionNode.conditionsHeatmapOffset.y = EditorGUILayout.FloatField(m_ColorConditionNode.conditionsHeatmapOffset.y, GUILayout.MaxWidth(GetWidth() / 4));
        EditorGUILayout.EndHorizontal();

        GUILayout.EndVertical();

        // // NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("condition.colorConditionColors"), true);

        NodeEditorGUILayout.DynamicPortList(
            "conditionsColors", // field name
            typeof(Condition), // field type
            serializedObject, // serializable object
            NodePort.IO.Output, // new port i/o
            Node.ConnectionType.Override, // new port connection type
            Node.TypeConstraint.None,
            OnCreateReorderableList); // onCreate override. This is where the magic happens.

        for (int i = 0; i < m_ColorConditionNode.colorConditions.Count; i++)
        {
            ColorCondition cc = m_ColorConditionNode.colorConditions[i];

            cc.relativeHeatmap = m_ColorConditionNode.conditionsHeatmap;
            cc.scale = m_ColorConditionNode.conditionsHeatmapScale;
            cc.offset = m_ColorConditionNode.conditionsHeatmapOffset;
            cc.conditionColor = m_ColorConditionNode.conditionsColors[i];
        }

    }



    void OnCreateReorderableList(ReorderableList list)
    {

        // Override drawHeaderCallback to display node's name instead
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Color Conditions");
        };

        list.onAddCallback += new ReorderableList.AddCallbackDelegate(newColorCondition);
        list.onRemoveCallback += new ReorderableList.RemoveCallbackDelegate(deleteColorCondition);

    }

    private void deleteColorCondition(ReorderableList list)
    {
        m_ColorConditionNode.colorConditions.RemoveAt(list.index);
    }

    private void newColorCondition(ReorderableList list)
    {
        m_ColorConditionNode.colorConditions.Add(new ColorCondition());
    }

    public override int GetWidth()
    {
        return 300;
    }

}