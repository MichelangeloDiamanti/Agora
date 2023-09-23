using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UrbanTerritoriality.Maps;
using UrbanTerritoriality.Utilities;
using System;

namespace UrbanTerritoriality.Agent
{
	[System.Serializable]
	//[RequireComponent(typeof(NavMeshAgent))]
	public class BufferizedPositionsEvent : UnityEvent<Vector3[]> { }

	[System.Serializable]
	public class PositionEvent : UnityEvent<ChangeInPositionEventDataStructure> { }

	public struct ChangeInPositionEventDataStructure
	{
		int agentID;
		Vector3 oldPosition;
		Vector3 newPosition;

		public Vector3 OldPosition
		{
			get { return oldPosition; }
			set { oldPosition = value; }
		}

		public Vector3 NewPosition
		{
			get { return newPosition; }
			set { newPosition = value; }
		}

		public int AgentID
		{
			get { return agentID; }
			set { agentID = value; }
		}
	}


	[System.Serializable]
	public class FloatEvent : UnityEvent<FloatEventDataStructure> { }

	public struct FloatEventDataStructure
	{
		int agentID;
		Vector3 position;
		float eventValue;

		public int AgentID
		{
			get { return agentID; }
			set { agentID = value; }
		}

		public Vector3 Position
		{
			get { return position; }
			set { position = value; }
		}

		public float EventValue
		{
			get { return eventValue; }
			set { eventValue = value; }
		}
	}

	/** A base class for agents that have access to a map */
	public abstract class TrackedAgent : MonoBehaviour
	{
		protected int _ID;

		/** 
        * Event triggered when the agent moves to a new location. Should be called
        * in the class tha actually implements the movement function
        */
		[HideInInspector]
		public BufferizedPositionsEvent OnNewPositionsBuffer;

		/** 
        * Event triggered when the agent moves to a new location. Should be called
        * in the class tha actually implements the movement function
        */
		[HideInInspector]
		public PositionEvent OnNewPosition;

		/** 
        * Event triggered when the agent changes its orientation. Should be called
        * in the class tha actually implements the movement function
        */
		[HideInInspector]
		public FloatEvent OnNewOrientation;

		/** 
        * Each time the agent perceives a new crowdness value, we raise this event
        */
		[HideInInspector]
		public FloatEvent OnNewCrowdness;


        public void Dismiss()
		{

			// Broadcast to all - the agent has been destroyed.
			SimulationManager.Instance.OnAgentDeSpawned.Invoke(this);

			Destroy(gameObject);
		}

		public void SetIsVisibile(bool b)
		{
			// Make all child rendered geometry either active or inactive
			foreach (Renderer r in GetComponentsInChildren<Renderer>())
			{
				r.gameObject.SetActive(b);
			}
		}

		protected virtual void Awake()
		{
            // Get a new ID for this agent, can be used to identify it when 
            // passing information with events.
            _ID = SimulationManager.Instance.GetNewAgentID();
		}

		protected virtual void Start()
		{
			// Broadcast to all - the agent has been created.
			SimulationManager.Instance.OnAgentSpawned.Invoke(this);
		}

		protected virtual void Update()
		{

		}

	}
}

