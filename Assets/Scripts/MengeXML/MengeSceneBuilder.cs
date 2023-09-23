using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MengeXML;
using System.Linq;

public class MengeSceneBuilder : EditorWindow
{
    private string sceneFilePath = "";
    private Experiment sceneFileData;

    private Vector2 scrollPos;
    private List<bool> toggleAgentProfiles;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Menge/SceneBuilder")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MengeSceneBuilder window = (MengeSceneBuilder)EditorWindow.GetWindow(typeof(MengeSceneBuilder));
        window.toggleAgentProfiles = new List<bool>();
        window.Show();
    }

    void OnGUI()
    {
        // Parse and load scene XML contents if something is selected and still not loaded
        if (sceneFilePath != "" && sceneFileData == null)
        {
            sceneFileData = ParseSceneDataFromXML(sceneFilePath);
        }

        // GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        // Selection of the scene XML file
        GUILayout.BeginHorizontal();

        sceneFilePath = EditorGUILayout.TextField("Scene File", sceneFilePath);
        if (GUILayout.Button("Select")) { SelectSceneFile(); }

        GUILayout.EndHorizontal();

        // Show scene parameters
        if (sceneFileData != null)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Experiment version
            if (sceneFileData.Version != null) sceneFileData.Version = EditorGUILayout.TextField("Experiment Version", sceneFileData.Version);

            if (sceneFileData.CommonParameters != null)
            {
                GUILayout.BeginVertical("Common Parameters", "window");
                sceneFileData.CommonParameters.TimeStep = EditorGUILayout.FloatField("Time Step", sceneFileData.CommonParameters.TimeStep);
                GUILayout.EndVertical();
            }

            // Spatial Query
            if (sceneFileData.SpatialQuery != null)
            {
                GUILayout.BeginVertical("Spatial Query", "window");
                sceneFileData.SpatialQuery.Type = EditorGUILayout.TextField("Type", sceneFileData.SpatialQuery.Type);
                sceneFileData.SpatialQuery.TestVisibility = EditorGUILayout.Toggle("Test Visibility", sceneFileData.SpatialQuery.TestVisibility);
                sceneFileData.SpatialQuery.FileName = EditorGUILayout.TextField("File Name", sceneFileData.SpatialQuery.FileName);
                GUILayout.EndVertical();
            }

            // Elevation
            if (sceneFileData.Elevation != null)
            {
                GUILayout.BeginVertical("Elevation", "window");
                sceneFileData.Elevation.Type = EditorGUILayout.TextField("Type", sceneFileData.Elevation.Type);
                sceneFileData.Elevation.FileName = EditorGUILayout.TextField("File Name", sceneFileData.Elevation.FileName);
                GUILayout.EndVertical();
            }

            // Obstacle Set
            if (sceneFileData.ObstacleSet != null)
            {
                GUILayout.BeginVertical("Obstacle Set", "window");
                sceneFileData.ObstacleSet.Type = EditorGUILayout.TextField("Type", sceneFileData.ObstacleSet.Type);
                sceneFileData.ObstacleSet.FileName = EditorGUILayout.TextField("File Name", sceneFileData.ObstacleSet.FileName);
                sceneFileData.ObstacleSet.Class = EditorGUILayout.IntField("Class", sceneFileData.ObstacleSet.Class);
                GUILayout.EndVertical();
            }

            if (sceneFileData.AgentProfiles != null)
            {
                if (toggleAgentProfiles.Count < sceneFileData.AgentProfiles.Count)
                {
                    toggleAgentProfiles.AddRange(Enumerable.Repeat(false, sceneFileData.AgentProfiles.Count - toggleAgentProfiles.Count));
                }

                GUILayout.BeginVertical("Agent Profiles", "window");
                for (int i = 0; i < sceneFileData.AgentProfiles.Count; i++)
                {
                    AgentProfile agentProfile = sceneFileData.AgentProfiles[i];
                    toggleAgentProfiles[i] = EditorGUILayout.Foldout(toggleAgentProfiles[i], agentProfile.Name);

                    if (toggleAgentProfiles[i])
                    {
                        agentProfile.Name = EditorGUILayout.TextField("Name", agentProfile.Name);

                        GUILayout.BeginVertical("Common Parameters", "window");

                        agentProfile.CommonParameters.MaxAngularVelocity = EditorGUILayout.FloatField("Max Angular Velocity", agentProfile.CommonParameters.MaxAngularVelocity);
                        agentProfile.CommonParameters.MaxNeighbors = EditorGUILayout.IntField("Max Neighbors", agentProfile.CommonParameters.MaxNeighbors);
                        agentProfile.CommonParameters.ObstacleSet = EditorGUILayout.IntField("Obstacle Set", agentProfile.CommonParameters.ObstacleSet);
                        agentProfile.CommonParameters.NeighborDist = EditorGUILayout.FloatField("Neighbor Distance", agentProfile.CommonParameters.NeighborDist);
                        agentProfile.CommonParameters.Class = EditorGUILayout.IntField("Class", agentProfile.CommonParameters.Class);
                        agentProfile.CommonParameters.Radius = EditorGUILayout.FloatField("Neighbor Distance", agentProfile.CommonParameters.Radius);
                        agentProfile.CommonParameters.PreferredSpeed = EditorGUILayout.FloatField("Preferred Speed", agentProfile.CommonParameters.PreferredSpeed);
                        agentProfile.CommonParameters.MaxSpeed = EditorGUILayout.FloatField("Max Speed", agentProfile.CommonParameters.MaxSpeed);
                        agentProfile.CommonParameters.MaxAcceleration = EditorGUILayout.FloatField("Max Acceleration", agentProfile.CommonParameters.MaxAcceleration);

                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("ORCA Parameters", "window");

                        agentProfile.ORCAParameters.Tau = EditorGUILayout.FloatField("Tau", agentProfile.ORCAParameters.Tau);
                        agentProfile.ORCAParameters.TauObst = EditorGUILayout.FloatField("Tau Obstacles", agentProfile.ORCAParameters.TauObst);

                        GUILayout.EndVertical();

                    }
                }

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
        }



        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Reload Scene File")) { sceneFileData = ParseSceneDataFromXML(sceneFilePath); }
        if (GUILayout.Button("Save Scene File")) { SaveSceneFile(); }

        GUILayout.EndHorizontal();
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