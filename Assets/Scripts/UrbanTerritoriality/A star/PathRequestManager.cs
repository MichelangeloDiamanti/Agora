using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UrbanTerritoriality
{
	public class PathRequestManager : MonoBehaviour {

		//Queues all the paths
		Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
		PathRequest currentPathRequest;

		static PathRequestManager instance;
		Pathfinding pathfinding;

		bool isProcessingPath;


		void Awake() {
			instance = this;
			pathfinding = GetComponent<Pathfinding>();
		}

		/*
		 * 
		 * Puts the new Request in the queue
		 */
		public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
			PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);
			instance.pathRequestQueue.Enqueue(newRequest);
			instance.TryProcessNext();
		}

		/*
		 * If we are not processing path and there is a path in the queue.
		 * 	Take the first path out of the queue and process it
		 */
		void TryProcessNext() {
			if (!isProcessingPath && pathRequestQueue.Count > 0) {
				currentPathRequest = pathRequestQueue.Dequeue();
				isProcessingPath = true;
				pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
			}
		}

		/*
		 * 
		 */
		public void FinishedProcessingPath(Vector3[] path, bool success) {
			currentPathRequest.callback(path,success);
			isProcessingPath = false;
			TryProcessNext();
		}

		/*
		 * Struct that keeps track of all the paths
		 */
		struct PathRequest {
			public Vector3 pathStart;
			public Vector3 pathEnd;
			public Action<Vector3[], bool> callback;

			public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback) {
				pathStart = _start;
				pathEnd = _end;
				callback = _callback;
			}

		}
	}
}