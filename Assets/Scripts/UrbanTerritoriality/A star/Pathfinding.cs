using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality
{
	public class Pathfinding : MonoBehaviour {

		public ColliderMap collidermap;
		public TerritorialHeatmap heatmap;
		/* The character game object */
		public GameObject agentGameObject;

		public Vector3 start;
		public Vector3 end;

		public HeatSpace agent = null;

		public float colliderWeight = 3f;
		public float heatmapWeight = 1f;
		public float maxLineRatioWeight = 0.01f;
		public float maxAngleWeight = 0.01f;
		public float agentAngleWeight = 0.02f;

		PathRequestManager requestManager;
		Grid grid;
		
		void Awake() {
			requestManager = GetComponent<PathRequestManager>();
			grid = GetComponent<Grid>();
		}

		public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
			StartCoroutine(FindPath(startPos,targetPos));
		}
		
		IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			
			Vector3[] waypoints = new Vector3[0];
			bool pathSuccess = false;
			
			Node startNode = grid.NodeFromWorldPoint(startPos);
			Node targetNode = grid.NodeFromWorldPoint(targetPos);
			startNode.parent = startNode;
			
			
			if (startNode.walkable && targetNode.walkable) {
				Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
				HashSet<Node> closedSet = new HashSet<Node>();
				openSet.Add(startNode);
				
				while (openSet.Count > 0) {
					Node currentNode = openSet.RemoveFirst();
					closedSet.Add(currentNode);
					
					if (currentNode == targetNode) {
						sw.Stop();
						//print ("Path found: " + sw.ElapsedMilliseconds + " ms");
						pathSuccess = true;
						break;
					}
					
					foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
						if (!neighbour.walkable || closedSet.Contains(neighbour)) {
							continue;
						}

						//Calculate movementpenalty from collitionmap
						float ?CollitionFromCollitionmapToGrid = null;
						if (collidermap.Initialized) {
							//float null1 = collidermap.GetValueAt (new Vector3(2.2f,0.0f,0.6f) );
							//float einn = collidermap.GetValueAt (new Vector3(2.2f,0.0f,1.0f) );

							CollitionFromCollitionmapToGrid = collidermap.GetValueAt (neighbour.worldPosition);
							CollitionFromCollitionmapToGrid = CollitionFromCollitionmapToGrid * 300;
							//UnityEngine.Debug.Log ("CollitionFromCollitionmapToGrid: " + CollitionFromCollitionmapToGrid);
						}

						if (CollitionFromCollitionmapToGrid != null) {
							neighbour.movementPenalty = (int)CollitionFromCollitionmapToGrid;
						}
					
						//Calculate movementpenalty from heatmap
						float ?MovementPenaltyFromHeatmapToGrid = null;
						if (heatmap.Initialized) {
							MovementPenaltyFromHeatmapToGrid = heatmap.GetValueAt (neighbour.worldPosition, agent);
							//UnityEngine.Debug.Log ("fyrir MovementPenaltyFromHeatmapToGrid: " + MovementPenaltyFromHeatmapToGrid);
							MovementPenaltyFromHeatmapToGrid += MovementPenaltyFromHeatmapToGrid * 100;
							//UnityEngine.Debug.Log ("eftir MovementPenaltyFromHeatmapToGrid: " + MovementPenaltyFromHeatmapToGrid);
						}

						if (MovementPenaltyFromHeatmapToGrid != null) {
							neighbour.movementPenalty += (int)MovementPenaltyFromHeatmapToGrid;
						}

						int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour) + neighbour.movementPenalty;
						if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
							neighbour.gCost = newMovementCostToNeighbour;
							neighbour.hCost = GetDistance(neighbour, targetNode);
							neighbour.parent = currentNode;
							
							if (!openSet.Contains(neighbour))
								openSet.Add(neighbour);
							else 
								openSet.UpdateItem(neighbour);
						}
					}
				}
			}
			yield return null;
			if (pathSuccess) {
				waypoints = RetracePath(startNode,targetNode);
			}
			requestManager.FinishedProcessingPath(waypoints,pathSuccess);
			
		}

		public void FindPathWithoutCoroutine(Vector3 startPos, Vector3 targetPos) {
			Stopwatch sw = new Stopwatch();
			sw.Start();

			Vector3[] waypoints = new Vector3[0];
			bool pathSuccess = false;

			Node startNode = grid.NodeFromWorldPoint(startPos);
			Node targetNode = grid.NodeFromWorldPoint(targetPos);
			startNode.parent = startNode;


			if (startNode.walkable && targetNode.walkable) {
				Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
				HashSet<Node> closedSet = new HashSet<Node>();
				openSet.Add(startNode);

				while (openSet.Count > 0) {
					Node currentNode = openSet.RemoveFirst();
					closedSet.Add(currentNode);

					if (currentNode == targetNode) {
						sw.Stop();
						//print ("Path found: " + sw.ElapsedMilliseconds + " ms");
						pathSuccess = true;
						break;
					}

					foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
						if (!neighbour.walkable || closedSet.Contains(neighbour)) {
							continue;
						}

						//Calculate movementpenalty from collitionmap
						float ?CollitionFromCollitionmapToGrid = null;
						if (collidermap.Initialized) {
							//float null1 = collidermap.GetValueAt (new Vector3(2.2f,0.0f,0.6f) );
							//float einn = collidermap.GetValueAt (new Vector3(2.2f,0.0f,1.0f) );

							CollitionFromCollitionmapToGrid = collidermap.GetValueAt (neighbour.worldPosition);
							CollitionFromCollitionmapToGrid = CollitionFromCollitionmapToGrid * 300;
							//UnityEngine.Debug.Log ("CollitionFromCollitionmapToGrid: " + CollitionFromCollitionmapToGrid);
						}

						if (CollitionFromCollitionmapToGrid != null) {
							neighbour.movementPenalty = (int)CollitionFromCollitionmapToGrid;
						}

						//Calculate movementpenalty from heatmap
						float ?MovementPenaltyFromHeatmapToGrid = null;
						if (heatmap.Initialized) {
							MovementPenaltyFromHeatmapToGrid = heatmap.GetValueAt (neighbour.worldPosition, agent);
							//UnityEngine.Debug.Log ("fyrir MovementPenaltyFromHeatmapToGrid: " + MovementPenaltyFromHeatmapToGrid);
							MovementPenaltyFromHeatmapToGrid += MovementPenaltyFromHeatmapToGrid * 100;
							//UnityEngine.Debug.Log ("eftir MovementPenaltyFromHeatmapToGrid: " + MovementPenaltyFromHeatmapToGrid);
						}

						if (MovementPenaltyFromHeatmapToGrid != null) {
							neighbour.movementPenalty += (int)MovementPenaltyFromHeatmapToGrid;
						}

						int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour) + neighbour.movementPenalty;
						if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
							neighbour.gCost = newMovementCostToNeighbour;
							neighbour.hCost = GetDistance(neighbour, targetNode);
							neighbour.parent = currentNode;

							if (!openSet.Contains(neighbour))
								openSet.Add(neighbour);
							else 
								openSet.UpdateItem(neighbour);
						}
					}
				}
			}
			if (pathSuccess) {
				waypoints = RetracePath(startNode,targetNode);
			}
			requestManager.FinishedProcessingPath(waypoints,pathSuccess);

		}



		Vector3[] RetracePath(Node startNode, Node endNode) {
			List<Node> path = new List<Node>();
			Node currentNode = endNode;
			
			while (currentNode != startNode) {
				path.Add(currentNode);
				currentNode = currentNode.parent;
			}
			Vector3[] waypoints = SimplifyPath(path);
			Array.Reverse(waypoints);
			return waypoints;
			
		}
		
		Vector3[] SimplifyPath(List<Node> path) {
			List<Vector3> waypoints = new List<Vector3>();
			Vector2 directionOld = Vector2.zero;
			
			for (int i = 1; i < path.Count; i ++) {
				Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
				if (directionNew != directionOld) {
					waypoints.Add(path[i].worldPosition);
				}
				directionOld = directionNew;
			}
			return waypoints.ToArray();
		}
		
		int GetDistance(Node nodeA, Node nodeB) {
			int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
			int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
			
			if (dstX > dstY)
				return 14*dstY + 10* (dstX-dstY);
			return 14*dstX + 10 * (dstY-dstX);
		}
		
		
	}
}
