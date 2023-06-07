using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using MengeXML;

public class MengeSceneBuilderOdinInspector : OdinEditorWindow
{
    [HorizontalGroup("SceneSelector")]
    [LabelText("XML Scene File Path")]
    public string sceneFilePath;

    [HorizontalGroup("SceneSelector")]
    [Button]
    public void SelectSceneXMLFileButton() { SelectSceneFile(); }

    [LabelText("Show Agents")]
    public bool showAgentHandles;

    public Experiment sceneFileData;


    [HorizontalGroup("ReloadAndSave")]
    [Button("Reload Scene from XML")]
    public void ReloadSceneXMLFileButton() { sceneFileData = ParseSceneDataFromXML(sceneFilePath); }

    [HorizontalGroup("ReloadAndSave")]
    [Button("Save Scene to XML")]
    public void SaveSceneXMLFileButton() { SaveSceneFile(); }

    [MenuItem("Window/Menge/OdinSceneBuilder")]
    private static void OpenWindow()
    {
        GetWindow<MengeSceneBuilderOdinInspector>().Show();
    }


    // Window has been selected
    void OnFocus()
    {
        // Remove delegate listener if it has previously
        // been assigned.
        SceneView.duringSceneGui -= this.OnSceneGUI;
        // Add (or re-add) the delegate.
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    protected override void OnDestroy()
    {
        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.duringSceneGui -= this.OnSceneGUI;
        base.OnDestroy();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (showAgentHandles)
        {
            if (sceneFileData != null)
            {
                foreach (AgentGroup agentGroup in sceneFileData.AgentGroups)
                {
                    foreach (AgentGroupAgent agent in agentGroup.Generator.Agents)
                    {
                        Vector3 oldPosition = agent.GetPosition();
                        // Handles.DrawWireCube(new Vector3(agent.PositionX, 0, agent.PositionY), new Vector3(1, 1, 1));
                        agent.SetPosition(Handles.PositionHandle(new Vector3(agent.PositionX, 0, agent.PositionY), Quaternion.identity));

                        if (oldPosition != agent.GetPosition())
                        {
                            this.Repaint();
                        }
                    }
                }
            }
        }
    }

    public void SelectSceneFile()
    {
        sceneFilePath = EditorUtility.OpenFilePanel("Scene Menge Scene File", "", "xml");
        sceneFileData = ParseSceneDataFromXML(sceneFilePath);
    }

    public Experiment ParseSceneDataFromXML(string path)
    {
        Experiment sceneFileData = null;
        if (path.Length != 0)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Experiment));
            TextReader reader = new StreamReader(sceneFilePath);
            object obj = deserializer.Deserialize(reader);
            sceneFileData = (Experiment)obj;
            reader.Close();
            Debug.LogFormat("Parsed Scene File {0}", sceneFilePath);
        }

        return sceneFileData;
    }

    private void SaveSceneFile()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Experiment));
        using (TextWriter writer = new StreamWriter(sceneFilePath))
        {
            serializer.Serialize(writer, sceneFileData);
        }
    }

}
