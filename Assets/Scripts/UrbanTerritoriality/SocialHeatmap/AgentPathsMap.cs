using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UrbanTerritoriality.Agent;
using UrbanTerritoriality.Utilities;
using UrbanTerritoriality.Enum;

namespace UrbanTerritoriality.Maps
{
	public class AgentPathsMap : GeneralHeatmap
	{
		[System.Serializable]
		public class myAgentAnalysisEvent : UnityEvent<GameObject> { }

		[HideInInspector]
		public myAgentAnalysisEvent OnAgentAnalysisSpawned;

		public GameObject agentPathsMapSimulationPrefab;
		private GameObject agentPathsMapSimulation;

		private AgentSpawner spawner;

		public int agentsToSpawn;

		public float timeScale;

		private int subscribedAgentsCount;

		public int SubscribedAgentsCount { get { return subscribedAgentsCount; } }

		private int events;

		private bool firstNormalizationCompleted;

		public AgentSpawner Spawner { get { return spawner; } }

		public GameObject AgentPathsMapSimulation { get { return agentPathsMapSimulation; } }
		public int Events { get { return events; } }
		/** 
		 * How much the a cell increases in value when 
         * a single agent goes over it 
		*/
		private const float changePerNewPosition = 0.1f;

		/** This PaintGrid object is the one that is used
         * when drawing agents position. The cell values
         * in it will increase infinitelly. No normalizing is
         * done on it.
         */
		private PaintGrid workGrid;

		private Vector3 spawnerPosition;

		public MapDataScriptableObject bakedMapForAgentsNavigation;
		private const int eventThresholdForUpdate = 1000;

		/** Returns a value of the map at a position */
		public override float GetValueAt(Vector2 position, PathFollowingAgent agent)
		{
			Vector2Int gridpos = WorldToGridPos(position);
			if (!mPaintGrid.IsWithinGrid(gridpos.x, gridpos.y))
			{
				return mPaintGrid.defaultValue;
			}
			float val = mPaintGrid.GetValueAt(gridpos.x, gridpos.y);
			return val;
		}

		protected override void applySettings(UTSettings settings)
		{
			throw new System.NotImplementedException();
		}

		/** Do some initialization */
		protected override void _initialize()
		{
			int resX = (int)(size.x / cellSize);
			int resY = (int)(size.y / cellSize);
			mPaintGrid = new PaintGrid(resX, resY);

			workGrid = new PaintGrid(resX, resY);
			workGrid.Clear(0.0f);

			events = 0;
			subscribedAgentsCount = 0;
			convergenceTimer = 0;
			base.currentTime = 0;
			firstNormalizationCompleted = false;

			spawnerPosition = transform.Find("SpawnerPosition").position;

			agentPathsMapSimulation = Instantiate(agentPathsMapSimulationPrefab, Vector3.zero, Quaternion.identity);
			// agentPathsMapSimulation.transform.parent = this.transform;

			spawner = agentPathsMapSimulation.GetComponentInChildren<AgentSpawner>();
			spawner.maximumAgentSpawned = agentsToSpawn;
			spawner.transform.position = spawnerPosition;
			spawner.snapToGroundNow();

			agentPathsMapSimulation.GetComponentInChildren<TimeScaler>().timeScale = timeScale;
			agentPathsMapSimulation.transform.Find("BakedMap").GetComponent<BakedMap>().mapData = bakedMapForAgentsNavigation;

			agentPathsMapSimulation.SetActive(true);

			OnAgentAnalysisSpawned.Invoke(agentPathsMapSimulation);
			spawner.OnSpawnedAgent.AddListener(SubscribeToAgentPositionUpdate);
		}

		void SubscribeToAgentPositionUpdate(MapWanderer agent)
		{
			// Listen to each agent's UpdatePosition Event, when they notify
			// a new buffer of positions we update the map
			agent.OnNewPositionsBuffer.AddListener(UpdateCellPositionWithLines);
			subscribedAgentsCount++;
		}

		// we want to update the "current time" of the map only after
		// all the agents have been subscribed, that's why we override with an
		// empty method
		protected override void Update()
		{
			if (firstNormalizationCompleted)
				base.currentTime += Time.unscaledDeltaTime;
		}

		private void UpdateCellPositionWithLines(Vector3[] positions)
		{
			for (int i = 0; i < positions.Length - 1; i++)
			{
				Vector3 p1 = positions[i];
				Vector3 p2 = positions[i + 1];

				Vector2Int p1_2d = WorldToGridPos(new Vector2(p1.x, p1.z));
				Vector2Int p2_2d = WorldToGridPos(new Vector2(p2.x, p2.z));

				// the points are the same, do nothing and go next
				if (p1_2d.x == p2_2d.x && p1_2d.y == p2_2d.y)
					continue;

				else if (p1_2d.x == p2_2d.x && p1_2d.y != p2_2d.y)
				{
					// Vertical line, equation --> x = x1
					// in this case we need to color all the pixels between the two "y"
					// that have the same "x" coordinate 
					for (int y = Mathf.Min(p1_2d.y, p2_2d.y); y < Mathf.Max(p1_2d.y, p2_2d.y); y++)
					{
						int x = p1_2d.x;

						if (workGrid.IsWithinGrid(x, y))
						{
							int index = y * workGrid.Width + x;
							workGrid.grid[index] += changePerNewPosition;
						}

					}

				}

				else
				{
					// normal equation --> y = mx + q
					float m = (float)(p2_2d.y - p1_2d.y) / (p2_2d.x - p1_2d.x);
					float q = (float)((p2_2d.x * p1_2d.y) - (p1_2d.x * p2_2d.y)) / (p2_2d.x - p1_2d.x);

					float differenceBetweenXs = Mathf.Abs(p1_2d.x - p2_2d.x);
					float differenceBetweenYs = Mathf.Abs(p1_2d.y - p2_2d.y);

					// Solve line equation (segment) by fixing the "x" coordinate and getting "y" values
					if (differenceBetweenXs >= differenceBetweenYs)
					{
						int y = 0;

						// get all the points in the segment defined by the line between x1 and x2
						// and change the corresponding pixels in the paintgrid, as if the agents
						// walked a straight line between the two points
						for (int x = Mathf.Min(p1_2d.x, p2_2d.x); x < Mathf.Max(p1_2d.x, p2_2d.x); x++)
						{
							y = Mathf.RoundToInt((m * x) + q);

							if (workGrid.IsWithinGrid(x, y))
							{
								int index = y * workGrid.Width + x;
								workGrid.grid[index] += changePerNewPosition;
							}
						}
					}
					// Solve line equation (segment) by fixing the "y" coordinate and getting "x" values
					else
					{
						int x = 0;

						for (int y = Mathf.Min(p1_2d.y, p2_2d.y); y < Mathf.Max(p1_2d.y, p2_2d.y); y++)
						{
							x = Mathf.RoundToInt((y - q) / m);

							if (workGrid.IsWithinGrid(x, y))
							{
								int index = y * workGrid.Width + x;
								workGrid.grid[index] += changePerNewPosition;
							}
						}
					}
				}
				events++;

				// if (events % (agentsToSpawn * 10) == 0)
				if(events % eventThresholdForUpdate == 0)
				{
					// Debug.Log("Normalizing...");
					meanChange = Utilities.Util.Normalize(workGrid, mPaintGrid);

					// start the coroutine that handle the saving of the map
					// after the first normalization
					if (firstNormalizationCompleted == false)
					{
						_configureSaveBehavior();
						firstNormalizationCompleted = true;
					}
				}
			}

		}

		// we want to start the timer for saving the map after
		// the the first normalization, so the default function
		// should be empty (because it is called in the base class)
		protected override void ConfigureSaveBehavior()
		{

		}

		private void _configureSaveBehavior()
		{
			if ((saveAsset || saveTexture) == false)
				return;
			switch (saveMethod)
			{
				case SaveMethod.QUALITY:
					{
						StartCoroutine(SaveMapOnThreshold());
					}
					break;
				case SaveMethod.TIME:
					{
						StartCoroutine(SaveMapAfterTime());
					}
					break;
				default:
					break;
			}
		}
	}
}