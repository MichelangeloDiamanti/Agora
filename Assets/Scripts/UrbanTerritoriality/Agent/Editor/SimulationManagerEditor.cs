using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UrbanTerritoriality.Agent;

[CustomEditor(typeof(SimulationManager))]
public class SimulationManagerEditor : Editor
{
	SimulationManager simulationManager;

	public override void OnInspectorGUI()
	{
		simulationManager = target as SimulationManager;
		// If we call base the default inspector will get drawn too.
		// Remove this line if you don't want that to happen.
		base.OnInspectorGUI();
		EditorGUILayout.LabelField("Spawned Agents: " + simulationManager.SpawnedAgents);

		EditorUtility.SetDirty(target);
	}
}
