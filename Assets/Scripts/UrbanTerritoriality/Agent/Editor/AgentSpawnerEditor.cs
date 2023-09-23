using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UrbanTerritoriality.Agent;

[CustomEditor(typeof(AgentSpawner))]
[CanEditMultipleObjects]
public class AgentSpawnerEditor : Editor
{
	AgentSpawner spawner;
	int lastSpawnerPosHash = 0;
	const int MAX_SNAP_HEIGHT = 100000;
	public override void OnInspectorGUI()
	{
		// If we call base the default inspector will get drawn too.
		// Remove this line if you don't want that to happen.
		base.OnInspectorGUI();

		spawner = target as AgentSpawner;

		spawner.spawnLimit = EditorGUILayout.Toggle("Spawn Limit", spawner.spawnLimit);

		if (spawner.spawnLimit)
		{
			spawner.maximumAgentSpawned = EditorGUILayout.IntField("Maximum Agent Spawned", spawner.maximumAgentSpawned);
		}

		spawner.randomness = EditorGUILayout.Toggle("Use Randomness", spawner.randomness);

		if (spawner.randomness)
		{
			spawner.radius = EditorGUILayout.FloatField("Spawn Radius", spawner.radius);
		}

	}

	void OnSceneGUI()
	{
		if (spawner == null) return;

		if (spawner.snapToGround)
		{
			int currentSpawnerPosHash = spawner.transform.position.GetHashCode();
			if (currentSpawnerPosHash != lastSpawnerPosHash)
			{
				spawner.snapToGroundNow();
				lastSpawnerPosHash = currentSpawnerPosHash;
			}
		}

		if (spawner.randomness)
		{
            Handles.color = Color.white * 0.9f;
            Handles.DrawWireDisc(spawner.transform.position, spawner.transform.up, spawner.radius);
		}

        // Extra gizmos for visual readability
        Handles.color = Color.white;
        Handles.DrawWireDisc(spawner.transform.position, spawner.transform.up, 1f);
        Handles.DrawLine(spawner.transform.position, spawner.transform.position + (Vector3.up * 10f));
    }
}