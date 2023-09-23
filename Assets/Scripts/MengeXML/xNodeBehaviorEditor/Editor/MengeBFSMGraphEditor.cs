using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;
using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


[CustomNodeGraphEditor(typeof(MengeBFSMGraph))]
public class MengeBFSMGraphEditor : NodeGraphEditor
{

    private MengeBFSMGraph m_MengeBFSMGraph;

    public override string GetNodeMenuName(System.Type type)
    {
        if (type.Namespace == "Agora.Menge.Heatmap")
        {
            // return base.GetNodeMenuName(type).Replace("X Node/Examples/Logic Toy/", "");
            return base.GetNodeMenuName(type).Replace("Agora/Menge/Heatmap/", "Heatmaps/");
        }
        else if(type.Namespace == "Agora.Evaluator.Metrics")
        {
            return null;
        }
        else return base.GetNodeMenuName(type);
    }

    public override void OnGUI()
    {
        if (m_MengeBFSMGraph == null) m_MengeBFSMGraph = target as MengeBFSMGraph;

        showXMLSerializationUI();

        base.OnGUI();
    }

    private void showXMLSerializationUI()
    {
        // NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("m_MengeBFSMGraph.XMLFilePath"));

        GUILayout.BeginHorizontal();

        m_MengeBFSMGraph.XMLFilePath = EditorGUILayout.TextField(m_MengeBFSMGraph.XMLFilePath);

        if (GUILayout.Button("Select Menge XML Behavior File"))
        {
            m_MengeBFSMGraph.XMLFilePath = EditorUtility.OpenFilePanel("Menge XML Behavior File", "", "xml");
            EditorUtility.SetDirty(m_MengeBFSMGraph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Menge XML Behavior File"))
        {
            FileStream fileStream = new FileStream(m_MengeBFSMGraph.XMLFilePath, FileMode.Create, System.IO.FileAccess.Write);

            var streamWriter = XmlWriter.Create(fileStream, new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true
            });


            XmlSerializer serializer = new XmlSerializer(typeof(MengeBFSMGraph));
            serializer.Serialize(streamWriter, m_MengeBFSMGraph);

            fileStream.Close();
        }
        if (GUILayout.Button("Load Menge XML Behavior File"))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MengeBFSMGraph));
            FileStream file = new FileStream(m_MengeBFSMGraph.XMLFilePath, FileMode.Open);
            m_MengeBFSMGraph = (MengeBFSMGraph)serializer.Deserialize(file);
        }
        GUILayout.EndHorizontal();
    }
}
