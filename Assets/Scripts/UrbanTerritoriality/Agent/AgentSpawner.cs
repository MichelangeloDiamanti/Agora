using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UrbanTerritoriality.Maps;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

public class AgentSpawner : MonoBehaviour
{
	[System.Serializable]
	public class MyAgentEvent : UnityEvent<MapWanderer>
	{
	}

	public GameObject agentPrefab;  // prefab of the agent to spawn
	public GeneralHeatmap map;  // needed for the agent navigation
	public float spawnTime = 1.0f;
	public int spawnCounter;    // READ ONLY
	public bool snapToGround = true;
	public bool spawnImmediate = false;

	[HideInInspector]
	public bool spawnLimit;
	[HideInInspector]
	public int maximumAgentSpawned = 10;
	[HideInInspector]
	public UnityEvent OnSpawningComplete;
	[HideInInspector]
	public MyAgentEvent OnSpawnedAgent;
	[HideInInspector]
	public bool randomness;
	[HideInInspector]
	public float radius = 1.0f;
	[Range(0f, 360f)]
	public float agentsOrientation = 0f;
	private float spawnTimer;
	private List<MapWanderer> spawnedAgents;
	private GameObject agentsContainer;
	private Vector3 agentsWorldOrientation;
	private Quaternion agentsQuaternionOrientation;

	private const int MAX_SNAP_HEIGHT = 100000;

	public List<MapWanderer> SpawnedAgents { get { return spawnedAgents; } }


	// Use this for initialization
	void Start()
	{
		ComputeWorldDirection();

		spawnTimer = spawnTime;
		spawnCounter = 0;
		agentPrefab.gameObject.SetActive(false);
		MapWanderer wanderer = agentPrefab.GetComponent<MapWanderer>();
		wanderer.map = this.map;
		spawnedAgents = new List<MapWanderer>();

		agentsContainer = new GameObject("Agents");
		agentsContainer.transform.parent = this.transform.parent;

		// Dont set the prefab as active. Prefab is intended only as
		// blueprint (e.g. collection of attributes)
		//
		//if (agentPrefab.activeSelf == false)
		//	agentPrefab.SetActive(true);

		if (spawnImmediate)
		{
			while (spawnCounter < maximumAgentSpawned)
			{
				GameObject agentGameObject = spawn(agentPrefab, transform.position, agentsQuaternionOrientation);
			}

			OnSpawningComplete.Invoke();
		}
	}

	// Update is called once per frame
	void Update()
	{
		// if we are using the spawn limiter and we already spawned enough agents we stop
		if (spawnLimit == true && spawnCounter >= maximumAgentSpawned)
			return;

		// if we can still spawn agents we do so according to the specified frequency
		spawnTimer += Time.deltaTime;
		if (spawnTimer >= spawnTime)
		{
			if (randomness)
			{
				float currentHMValue = 0f;
				int positionCount = 0;
				int maxCheckedPositions = 100;

				Vector3 randomSpawnPosition = Vector3.zero;

				// randomly select a position inside a circle until we get one that
				// is in an empty space or we reach a treshold. If we succeded at chosing
				// a random position spawn the agent, otherwise do nothing.
				while (currentHMValue <= 0f && positionCount <= maxCheckedPositions)
				{
					// get a random position inside a circle according to the specified radius
					Vector2 randomOffset = Random.insideUnitCircle * radius;
					randomSpawnPosition = new Vector3(transform.position.x + randomOffset.x,
						transform.position.y, transform.position.z + randomOffset.y);

					// check if the chosen position is in an empty space using the heatmap
					Vector2 _2DPos = new Vector2(randomSpawnPosition.x, randomSpawnPosition.z);
					Vector2Int currentHMPosition = map.WorldToGridPos(_2DPos);
					currentHMValue = map.GetValueAt(_2DPos);

					positionCount++;
				}
				if (positionCount <= maxCheckedPositions)
				{
					GameObject agentGameObject = spawn(agentPrefab, randomSpawnPosition, agentsQuaternionOrientation);
				}
			}
			else
			{
				GameObject agentGameObject = spawn(agentPrefab, transform.position, agentsQuaternionOrientation);
			}


			if (spawnLimit == true && spawnCounter >= maximumAgentSpawned)
				OnSpawningComplete.Invoke();

			spawnTimer = 0f;
		}
	}

	private GameObject spawn(GameObject agentPrefab, Vector3 position, Quaternion orientation)
	{
		// Debug.Log("Spawning new Agent");
		GameObject agentGameObject = Instantiate(agentPrefab, position, orientation);
		agentGameObject.transform.parent = agentsContainer.transform;

		agentGameObject.SetActive(true);
		MapWanderer agentScript = agentGameObject.GetComponent<MapWanderer>();
		spawnedAgents.Add(agentScript);
		OnSpawnedAgent.Invoke(agentScript);
		spawnCounter++;

		return agentGameObject;
	}

	public void snapToGroundNow()
	{
		Vector3 cSpawnerPos = transform.position;
		Vector3 rcStart = new Vector3(cSpawnerPos.x, MAX_SNAP_HEIGHT, cSpawnerPos.z);
		RaycastHit hit;
		if (Physics.Raycast(rcStart, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, LayerMask.GetMask("World Obstacle")))
		{
			transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
			//Debug.DrawRay(rcStart, spawner.transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
		}
	}

	void ComputeWorldDirection()
	{
		agentsQuaternionOrientation = Quaternion.AngleAxis(agentsOrientation, Vector3.up);
		agentsWorldOrientation = agentsQuaternionOrientation * transform.forward;
	}

	private void OnDrawGizmos()
	{
		if (randomness)
		{
			Handles.color = Color.white * 1.5f;
			Handles.DrawWireDisc(transform.position, transform.up, radius);
		}

		// Extra gizmos for visual readability
		Handles.color = Color.white;
		Handles.DrawWireDisc(transform.position, transform.up, 1f);
		Handles.DrawLine(transform.position, transform.position + (Vector3.up * 10f));

		ComputeWorldDirection();
		Vector3 start = transform.position + Vector3.up * .1f * radius;
		Vector3 end = start + agentsWorldOrientation * 2f * radius;
		Gizmos.DrawLine(start, end);
	}
}