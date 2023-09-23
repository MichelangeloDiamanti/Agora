using UnityEngine;
using System.Collections;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality
{
	public class Unit : MonoBehaviour {


		public Transform target;
		public WaypointList wayPointList;
		private Vector3 waypoint;
		float speed = 3;
		Vector3[] path;
		int targetIndex;
		public Grid grid;
		Node currentNode;
		//bool found = false;

		void Start() {
			setWaypoint ();
			PathRequestManager.RequestPath(transform.position,currentNode.worldPosition, OnPathFound);
		}

		void Update() {
			/*Debug.Log("transform.position: " + transform.position + " + waypoint: " + waypoint);
			if (transform.position == waypoint) {
				waypoint = wayPointList.GetRandomWaypoint ().transform.position;
			}*/
			//Debug.Log ("transform.position: " + transform.position);
			//Debug.Log ("tempNode.worldPosition: " + tempNode.worldPosition);

			PathRequestManager.RequestPath (transform.position, currentNode.worldPosition, OnPathFound);
		}

		public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
			if (pathSuccessful) {
				path = newPath;
				targetIndex = 0;
				StopCoroutine("FollowPath");
				StartCoroutine("FollowPath");
			}
		}

		void setWaypoint(){
			Vector3 oldWayPoint = waypoint;
			waypoint = wayPointList.GetRandomWaypoint ().transform.position;
			if (waypoint == oldWayPoint) {
				setWaypoint ();
			}
			currentNode = grid.NodeFromWorldPoint (waypoint);
			currentNode.worldPosition.y = 1.5f;
		}

		IEnumerator FollowPath() {
			/*Debug.Log(Vector3.Distance(transform.position, tempNode.worldPosition));

			if(Vector3.Distance(transform.position, tempNode.worldPosition) < 1.5f){
				found = true;
				setWaypoint ();
				PathRequestManager.RequestPath (transform.position, tempNode.worldPosition, OnPathFound);
				yield return null;
			}

			Vector3 currentWaypoint = path[0];

			currentWaypoint.y = 1.5f;
			while (true) {
				//Debug.Log ("transform.position: " + transform.position + "currentWaypoint: " + currentWaypoint);
				if (transform.position == currentWaypoint) {
					//PathRequestManager.TryUpdatePath();
					//Debug.Log("Hingad?");
					//PathRequestManager.RequestPath (transform.position, tempNode.worldPosition, OnPathFound);

					targetIndex++;
					Debug.Log ("targetIndex: " + targetIndex);
					if (targetIndex >= path.Length) {
						/*waypoint = wayPointList.GetRandomWaypoint ().transform.position;
						tempNode = grid.NodeFromWorldPoint (waypoint);
						PathRequestManager.RequestPath (transform.position, tempNode.worldPosition, OnPathFound);
						yield break;
					}
					currentWaypoint = path [targetIndex];
				}

				//Sér um hreyfinguna
				transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
				transform.LookAt (currentWaypoint);
				yield return null;*/
			//Debug.Log(Vector3.Distance(transform.position, currentNode.worldPosition));
			if(Vector3.Distance(transform.position, currentNode.worldPosition) < 2f){
				setWaypoint();
				//PathRequestManager.RequestPath (transform.position, currentNode.worldPosition, OnPathFound);
				yield break;
			}



			//Debug.Log (path.Length);
			Vector3 currentWaypoint = path[0];
			currentWaypoint.y = 1.5f;
			transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);


			transform.LookAt (waypoint);

			yield return null;
		}

		public void OnDrawGizmos() {
			if (path != null) {
				for (int i = targetIndex; i < path.Length; i++) {
					Gizmos.color = Color.black;
					Gizmos.DrawCube(path[i], Vector3.one);

					if (i == targetIndex) {
						Gizmos.DrawLine(transform.position, path[i]);
					}
					else {
						Gizmos.DrawLine(path[i-1],path[i]);
					}
				}
			}
		}
	}
}