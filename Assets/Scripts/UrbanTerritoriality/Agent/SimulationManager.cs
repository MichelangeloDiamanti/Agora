using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UrbanTerritoriality.Agent
{
	// Global events -- note: do they need to be serializable?
	[System.Serializable] public class AgentSpawnedEvent : UnityEvent<TrackedAgent> { }

	public class SimulationManager : Singleton<SimulationManager>
	{
		[Range(0.01f, 10f)] public float agentUnitSize = 1f;    // tentative - under consideration
        public UnityEvent SimulationEnding;

		private int _spawnedAgents;    // How many agents have been spawned since the start of the simulation

		private int _agentID;

		// Fire this event everytime a new agent gets spawned.
		[HideInInspector] public AgentSpawnedEvent OnAgentSpawned;

		// Fire this event everytime a new agent gets spawned.
		[HideInInspector] public AgentSpawnedEvent OnAgentDeSpawned;

		public int SpawnedAgents
		{
			get
			{
				return _spawnedAgents;
			}
		}

		public int GetNewAgentID()
		{
			_agentID++;
			return _agentID;
		}

        public void StopSimulation()
        {
            SimulationEnding.Invoke();
        }

        protected SimulationManager()
		{ }

		private void Start()
		{
			_agentID = 0;
			OnAgentSpawned.AddListener(increaseSpawnedAgentsCounter);
			OnAgentDeSpawned.AddListener(decreaseSpawnedAgentsCounter);
		}

		private void increaseSpawnedAgentsCounter(TrackedAgent arg0)
		{
			_spawnedAgents++;
		}
		private void decreaseSpawnedAgentsCounter(TrackedAgent arg0)
		{
			_spawnedAgents--;
		}

	}
}