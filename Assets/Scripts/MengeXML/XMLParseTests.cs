using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Menge.BFSM;

public class XMLParseTests : OdinEditorWindow
{
    [HorizontalGroup("SceneSelector")]
    [LabelText("XML Scene File Path")]
    public string filePath;

    [HorizontalGroup("SceneSelector")]
    [Button]
    public void SelectSceneXMLFileButton() { SelectSceneFile(); }

    public GoalSet fileData;

    [MenuItem("Window/Menge/XMLParseTests")]
    private static void OpenWindow()
    {
        GetWindow<XMLParseTests>().Show();
    }

    // protected override void OnGUI()
    // {
    //     if (fileData != null)
    //     {
    //         Debug.Log("debugging");
    //     }
    //     base.OnGUI();
    // }

    public void SelectSceneFile()
    {
        filePath = EditorUtility.OpenFilePanel("Scene Menge XML File", "", "xml");
        fileData = ParseSceneDataFromXML(filePath);
        Debug.Log(fileData);
    }

    public GoalSet ParseSceneDataFromXML(string path)
    {
        GoalSet sceneFileData = null;
        if (path.Length != 0)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(GoalSet));
            TextReader reader = new StreamReader(filePath);
            object obj = deserializer.Deserialize(reader);
            sceneFileData = (GoalSet)obj;
            reader.Close();
            Debug.LogFormat("Parsed Scene File {0}", filePath);
        }

        return sceneFileData;
    }
}
