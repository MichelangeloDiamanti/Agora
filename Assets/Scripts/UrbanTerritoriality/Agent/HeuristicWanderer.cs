using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UrbanTerritoriality.Utilities;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.Agent
{
    /**
     * A component providing wandering behavior
     * for virtual agents.
     * It uses a NavMeshAgent for navigation for
     * moving the agent around. A map is used
     * for deciding where to move. A typical
     * type of a map to use would be a visibility
     * map.
     */
    [RequireComponent(typeof(NavMeshAgent))]
    public class HeuristicWanderer : MapWanderer
    {
        /** Is the wanderer paused or not */
        public bool paused = false;

        /** Agent state - walking backward to given destination */
        public bool walkBackward = false;

        /** A fixed number of previous positions that the
        * agent went to. */
        public RingBuffer<Vector3> previousPositions;

        /** The current weight for the map in the goodness function */
        protected float currentMapWeight = 1f;

        /** The current weight for the angle in the goodness function */
        protected float currentAngleWeight = 1f;

        /** The current weight for the newness in the goodness function */
        protected float currentNewnessWeight = 1f;

        /** The last time a position was inserted into previousPositions */
        protected float lastPointSaveTime = float.MinValue;

        /** DEBUG ANNOTATIONS **/
        protected List<Vector3> pickedPoints;
        protected List<float> goodnessOfPoints;

        protected Vector3 _lastPosition;

        /* Percetion of externals. Upon invoking, the agent
         * gathers up percepts from the outside world.
         */
        protected UnityEvent _OnPerception = new UnityEvent();

        /** A list of input points that are pushed by external objects in
         * the environment.
         */
        protected List<Vector3> _pushedPoints;

        /*
         * A cache of processed pushed points. Destination points
         * which are pushed by the outside world can be processed
         * and projected whithin perceptual range. The table here
         * works like a cache that temporarily store the processed
         * points and match them with their originals.
         */
        protected Dictionary<Vector3, Vector3> _ppCache;

        /*
         * A nav mesh path used as a working internal variable.       
         */
        protected NavMeshPath _tmpPath;

        /** Proxemics field for crowding estimation */
        protected ProxemicsField _proxemicField;

        /** The vision angle of this agent */
        public override float VisionAngle { get { return 360; } }

        public UnityEvent OnPerception
        {
            get { return _OnPerception; }
        }

        /** Previous positions that the agent has been to 
         * from the start of play mode */
        public Vector3[] GetAllPreviousPositions()
        {
            return allPreviousPositions.ToArray();
        }

        /** Previous positions that the agent has been to 
         * from the start of play mode */
        public Vector3 GetLastPreviousPosition()
        {
            return allPreviousPositions[allPreviousPositions.Count - 1];
        }
        protected List<Vector3> allPreviousPositions;

        // How many positions are tracked before raising the update event
        // The value 10 is a good one for reducing the overhead of the
        // event raising. 
        // Too low --> too many events
        // Too high --> slow update of the map
        private const int trackedPositionsBufferLength = 10;
        private Vector3[] trackedPositionsBuffer;
        private int trackedPositionsCount;

        /** When the agents gets closer to the destination
         * than this, a new destination will be picked. */
        public float DestinationPickDistance
        { get { return 1f; } }

        /** The last time a new destination was picked */
        protected float lastPickTime = float.MinValue;

        /** The last time a new destination was picked
         * because of some crisis such as the agent
         * being faced up to a wall */
        protected float lastCrisisPickTime = float.MinValue;

        /** Hit position when doing a nav mesh raycast
         * in the forward direction of the agent */
        protected Vector3? navMeshHitPosition;

        private bool isFirstBuffer;

        protected override void Awake()
        {
            base.Awake();

            // Init internals.
            _pushedPoints = new List<Vector3>();
            _ppCache = new Dictionary<Vector3, Vector3>();
            _tmpPath = new NavMeshPath();
            _proxemicField = GetComponentInChildren<ProxemicsField>();
        }

        /** Unity Start method */
        protected override void Start()
        {
            base.Start();

            // Uniform all agents parameters against unit size.
            navMeshAgent.height = base.height * agentUnitSize;
            navMeshAgent.speed = base.speed * agentUnitSize;

            isFirstBuffer = true;
            previousPositions = new RingBuffer<Vector3>(nrOfRememberedPoints);
            allPreviousPositions = new List<Vector3>();

            trackedPositionsBuffer = new Vector3[trackedPositionsBufferLength];
            trackedPositionsCount = 0;

            _lastPosition = transform.position;
        }

        /** Unity Update method */
        protected override void Update()
        {
            base.Update();

            Vector2Int currentMapPosition = map.WorldToGridPos(new Vector2(transform.position.x, transform.position.z));

            navMeshAgent.height = base.height * agentUnitSize;
            navMeshAgent.speed = base.speed * agentUnitSize;

            // __ DEBUG __
            if (navMeshAgent.isPathStale)
            {
                Debug.Log("path stale.");
            }
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial || navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                Debug.Log(navMeshAgent.pathStatus);
            }
            if (navMeshAgent.pathPending)
            {
                Debug.Log("path pending.");
            }
            // __ DEBUG __

            float t = Time.time;

            // Agent state - paused. The agent is standing still.
            if (paused)
            {
                navMeshAgent.isStopped = true;
                return;
            }
            else
            {
                navMeshAgent.isStopped = false;
            }

            // The agent is moving and walking around...

            // Save positions at regular intervals.
            if (t > lastPointSaveTime + pointSavingInterval)
            {
                lastPointSaveTime = t;

                previousPositions.Insert(transform.position);
                allPreviousPositions.Add(transform.position);

                //TrackPositionsForSimulationMapping();
            }


            ChangeInPositionEventDataStructure positionData = new ChangeInPositionEventDataStructure();
            positionData.AgentID = _ID;
            positionData.OldPosition = _lastPosition;
            positionData.NewPosition = transform.position;
            base.OnNewPosition.Invoke(positionData);


            // Orientation Event
            FloatEventDataStructure orientationData = new FloatEventDataStructure();
            orientationData.Position = transform.position;
            orientationData.EventValue = GetAngle(Vector2.right, new Vector2(transform.forward.x, transform.forward.z));
            base.OnNewOrientation.Invoke(orientationData);

            // Crowdness Event
            FloatEventDataStructure crowdnessData = new FloatEventDataStructure();
            crowdnessData.AgentID = _ID;
            crowdnessData.Position = transform.position;
            crowdnessData.EventValue = PerceiveCrowdness();
            base.OnNewCrowdness.Invoke(crowdnessData);

            // UPDATE THIS LAST: Some events are invoked when we change position
            _lastPosition = transform.position;



            // State transition. The agent has to walk backwards.
            // While walking backwards the agent keeps the same destination
            // point until it reaches it. Then transitions back into 
            // regular wandering.
            if (walkBackward)
            {
                if (IsAtDestination(_currentDestination, DestinationPickDistance))
                {
                    // Walking backwards is complete. Exit the state and
                    // pick a new destination in the regular way.
                    walkBackward = false;

                    PickNewDestination(
                        DefaultMapWeight,
                        DefaultAngleWeight,
                        DefaultNewnessWeight);
                }
                return;
            }

            if (map.Initialized)
            {
                if (navMeshAgent.remainingDistance < DestinationPickDistance)
                {
                    /* If the destination has been reached then pick a new one */
                    PickNewDestination(DefaultMapWeight,
                        DefaultAngleWeight,
                        DefaultNewnessWeight);
                    return;
                }
                if (t > (lastPickTime + MaxTimeBetweenPicks))
                {
                    /* If time is up then pick a new destination */
                    PickNewDestination(DefaultMapWeight,
                        DefaultAngleWeight,
                        DefaultNewnessWeight);
                }
            }

        }

        private float PerceiveCrowdness()
        {
            float range = 84f * agentUnitSize;
            float crowdingScore = 0.0f;

            // Find all the other agents in a neighborhood
            Collider[] hitColliders = Physics.OverlapSphere(
                transform.position,
                range, 
                Physics.AllLayers, 
                QueryTriggerInteraction.Collide);

            foreach (Collider c in hitColliders)
            {
                if (c.gameObject.GetComponent<HeuristicWanderer>() != null)
                {
                    Vector3 offset = c.gameObject.transform.position - transform.position;
                    offset.y = 0f;
                    if (Vector3.Dot(offset, transform.forward) > 0f)
                    {
                        crowdingScore += Mathf.Pow(range - offset.magnitude, 2f);
                    }
                }
            }
            return crowdingScore;
        }

        protected void TrackPositionsForSimulationMapping()
        {
            trackedPositionsBuffer[trackedPositionsCount] = transform.position;
            trackedPositionsCount++;
            // Invoke the event so that the map updates with the new agent paths
            if (trackedPositionsCount >= trackedPositionsBufferLength)
            {
                base.OnNewPositionsBuffer.Invoke(trackedPositionsBuffer);
                Vector3 lastPoint = trackedPositionsBuffer[trackedPositionsBufferLength - 1];
                trackedPositionsBuffer = new Vector3[trackedPositionsBufferLength];
                trackedPositionsBuffer[0] = lastPoint;
                trackedPositionsCount = 1;
            }

        }

        // Computes the angle in radians between two vectors
        // The returned value is between [0, 2PI)
        // This is currently being employed for the orientation mapping
        // which is created using the direction (forward vector) of the agents
        private static float GetAngle(Vector2 v1, Vector2 v2)
        {
            var sign = Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
            float angle = Vector2.Angle(v1, v2) * sign;
            if (sign < 0 && angle != 0)
                angle = 360 + angle;
            return angle * Mathf.Deg2Rad;
        }

        /** Rates the "goodness" of a particular point.
         * Gives a point a grade on how desirable it
         * is for the agent to go there.
         */
        protected float GoodnessOfPoint(Vector3 point, float mapWeight, float angleWeight, float newnessWeight)
        {
            float goodness = 0f;
            goodness += mapWeight * map.GetValueAt(point);
            goodness += angleWeight * AngleToPoint(new Vector2(point.x, point.z));
            goodness += newnessWeight * NewnessReplacement(point);
            goodness = point.y - transform.position.y > 2f ? goodness * 0.5f : goodness;
            return goodness;
        }

        public void AddToPushedPoints(Vector3 point, float saliency = 0f)
        {
            //_pushedPoints.Add(point);
            _pushedPoints.Insert(Random.Range(0, _pushedPoints.Count), point);
        }

        public void SetDestination(Vector3 position)
        {
            _currentDestination = position;
            NavMeshPath path = new NavMeshPath();
            if (navMeshAgent.CalculatePath(_currentDestination, path) == false)
                Debug.Log("Path not found");
            navMeshAgent.path = path;
        }

        public bool IsAtDestination(Vector3 dest, float range)
        {
            Vector3 offset = dest - transform.position;
            offset.y = 0f;
            return offset.sqrMagnitude < range * range;
        }

        /** The angle to a point from the forward vector
         * of the agent */
        public float AngleToPoint(Vector2 point)
        {
            Vector3 agentForward3d = transform.forward;
            Vector2 agentForward2d = new Vector2(agentForward3d.x, agentForward3d.z);
            Vector3 agentPos3d = transform.position;
            Vector2 agentPos2d = new Vector2(agentPos3d.x, agentPos3d.z);
            Vector2 toPoint = point - agentPos2d;
            float dot = Vector2.Dot(toPoint.normalized, agentForward2d.normalized);
            dot = Mathf.Clamp(dot, -1f, 1f);
            //return 1f - (Mathf.Acos(dot) / (0.5f * Mathf.PI));
            return 1f - (Mathf.Acos(dot) / (Mathf.PI)); // [1, 0]
        }

        public float NewnessReplacement(Vector3 point)
        {
            Vector3 flooredPosition = transform.position;
            flooredPosition.y = 0f;
            point.y = 0f;
            return Mathf.Clamp01((point - transform.position).magnitude / PerceptualDistance);
        }

        /** Rates how new a particular point is to the agent.
         * It is considered new if it is not close to the previous
         * positions that the agent has explored. Here recent
         * positions have more weight.
         * */
        public static float NewnessWithWeights(Vector3 point, ref Vector3[] prevPositions)
        {
            float newness = 0;
            int n = prevPositions.Length;
            for (int i = 0; i < n; i++)
            {
                newness += Vector3.Distance(point, prevPositions[i]) / (i + 1f);
            }
            return newness;
        }

        /*
         * Rates how new a particular point is to the agent.
         * It is considered new if it is not close to the previous
         * positions that the agent has explored.
         */
        public static float Newness(Vector3 point, ref Vector3[] prevPositions)
        {
            float newness = 0;
            int n = prevPositions.Length;
            for (int i = 0; i < n; i++)
            {
                newness += Vector3.Distance(point, prevPositions[i]);
            }
            return newness;
        }

        /** Unity OnDrawGizmos method */
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (agentParameters == null || !ShowGizmo)
            {
                return;
            }

            if (navMeshAgent != null)
            {
                /* Draw current destination */
                Gizmos.DrawCube(_currentDestination, Vector3.one);
            }

            if (pickedPoints != null)
            {
                for (int i = 0; i < pickedPoints.Count; i++)
                {
                    if (pickedPoints[i] == _currentDestination)
                    {
                        Gizmos.color = Color.cyan * 2f;
                    }
                    else
                    {

                        Gizmos.color = Color.white * goodnessOfPoints[i];
                    }
                    Vector3 scaledGizmoSize = new Vector3(1f, 5f * goodnessOfPoints[i], 1f);
                    scaledGizmoSize.Scale(transform.localScale);
                    Gizmos.DrawCube(pickedPoints[i], scaledGizmoSize);
                }
            }

            /* Draw the position of the agent */
            Gizmos.DrawSphere(transform.position, 1);

            /** Draw position of nav mesh edge if the agent
             * is in front of it */
            Color temp = Gizmos.color;

            if (navMeshHitPosition != null)
            {
                Gizmos.DrawSphere((Vector3)navMeshHitPosition, 0.3f);
            }

            if (previousPositions != null)
            {
                Vector3[] prevPos = previousPositions.ToArray();
                int n = prevPos.Length;
                Gizmos.color = Color.blue;
                if (n > 0)
                {
                    Gizmos.DrawLine(transform.position, prevPos[0]);
                }
                for (int i = 0; i < n; i++)
                {
                    Gizmos.DrawCube(prevPos[i], Vector3.one * 0.2f);
                }
                for (int i = 0; i < n - 1; i++)
                {
                    Gizmos.DrawLine(prevPos[i], prevPos[i + 1]);
                }
            }
            if (allPreviousPositions != null)
            {
                int n = allPreviousPositions.Count;
                Gizmos.color = new Color(0, 0, 1, 0.3f);
                if (n > 0)
                {
                    Gizmos.DrawLine(transform.position, allPreviousPositions[n - 1]);
                }
                for (int i = 0; i < n - 1; i++)
                {
                    Gizmos.DrawLine(allPreviousPositions[i], allPreviousPositions[i + 1]);
                }
            }
            Gizmos.color = temp;
        }

        /** Picks a new destination for the agent to go to. */
        protected virtual void PickNewDestination(float mapWeight, float angleWeight, float newnessWeight)
        {
            // Invoke perception step before choosing a new destination.
            OnPerception.Invoke();

            // Pick a bunch of good random points.
            pickedPoints = GetListOfRandomPoints(
                VisionAngle,
                PerceptualDistance / agentUnitSize,
                NrOfPickedPoints,
                transform);

            // Add a point to list
            if (_pushedPoints.Count == 0)
            {
                Vector3 point = transform.position + transform.forward * speed * MaxTimeBetweenPicks;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(point, out hit, 1f, NavMesh.AllAreas))
                {
                    //pickedPoints.Add(hit.position);
                }
            }

            // Evaluate their fitness according to some heuristic aka defintion
            // of good.
            int n = pickedPoints.Count;
            goodnessOfPoints = new List<float>();
            Vector3[] prevPositions = previousPositions.ToArray();
            for (int i = 0; i < n; i++)
            {
                goodnessOfPoints.Add(
                    GoodnessOfPoint(pickedPoints[i], mapWeight, angleWeight, newnessWeight));
            }

            // Choose the best option.
            int nextIndex = AgentUtil.IndexOfHighestValueFromList(goodnessOfPoints);
            if (nextIndex < 0)
            {
                // There isn't any option to choose from. Abort.
                return;
            }
            else
            {
                // Best option becomes the next destination point to go to.
                _currentDestination = pickedPoints[nextIndex];
            }

            // When best destination falls over the agent position (essentially
            // there is no way to move forward) consider the agent stuck in a 
            // dead end and command to walk backward until it's last previous 
            // destination.
            if ((_currentDestination - transform.position).sqrMagnitude < 0.000099f)
            {
                _currentDestination = prevPositions[prevPositions.Length - 1];
                walkBackward = true;
            }

            // Set current next destination point.
            SetDestination(_currentDestination);

            // Update time counter for "last picked time" (whatever that is).
            lastPickTime = Time.time;

            // Flush internal buffers.
            _pushedPoints.Clear();
            _ppCache.Clear();

        }

        protected List<Vector3> GetListOfRandomPoints(float Degrees, float Distance, int numberOfPoints, Transform agent)
        {
            int cycles = -1;
            List<Vector3> returnList = new List<Vector3>();


            for (int i = 0; i < numberOfPoints; i++)
            {
                Vector3 endPoint;

                //One more cycle done.
                cycles++;

                // Safety check. If the loop keeps going for a long
                // time just kill it.
                if (cycles > 10 * numberOfPoints)
                {
                    break;
                }

                if (cycles < _pushedPoints.Count)
                {
                    endPoint = TransformPushPoint(_pushedPoints[i]);
                }
                else
                {
                    // Generate random point.
                    do
                    {
                        endPoint.x = Random.Range(-1f, 1f);
                        endPoint.z = Random.value;
                        endPoint.y = 0f;
                    }
                    while (endPoint.magnitude > 1f);


                    // Scale and project inside perceptual range
                    endPoint *= Distance;
                    endPoint = agent.TransformPoint(endPoint);

                    NavMeshHit hit;
                    if (!NavMesh.SamplePosition(endPoint, out hit, 1f, NavMesh.AllAreas))
                    {
                        // try again...
                        i--;
                        continue;

                        // RECONSIDER - 
                        // The code works in practice but it might loop
                        // under unattentive modification of the surronding code.
                        // Better is to think of either a check of forced ultimation
                        // or find a more accurate way to sample positions onto the
                        // the navmesh.
                    }
                    endPoint = hit.position;
                }

                // Test for line of sight direct visibility. If not, choose 
                // a different point.
                Vector3 origin = transform.position + Vector3.up * height;
                Vector3 direction = (endPoint + Vector3.up * height) - origin;
                float maxDistance = direction.magnitude;
                direction /= maxDistance;
                if (Physics.Raycast(origin, direction, maxDistance, LayerMask.GetMask("World Obstacle")))
                {
                    // try again...
                    i--;
                    continue;
                }

                //The point is valid. Put it in the list.
                returnList.Add(endPoint);
            }

            return returnList;
        }

        protected Vector3 TransformPushPoint(Vector3 point)
        {
            if (_ppCache.ContainsKey(point))
            {
                return _ppCache[point];
            }
            else
            {
                Vector3 offset;

                // Check if point is suitable destination point by checking
                // if whithin perceptual distance.
                offset = point - transform.position;
                offset.y = 0f;
                if (offset.sqrMagnitude <= PerceptualDistance * PerceptualDistance)
                {
                    // Do nothing and keep the point. It's good.
                    return point;
                }
                else
                {
                    // The destination point is far and falls outside perceptual
                    // distance. Here the strategy is to extract the first suitable
                    // destination point along the path which falls whithin
                    // perceptual distance.
                    Vector3 newPoint;
                    int hotIndex;
                    navMeshAgent.CalculatePath(point, _tmpPath);

                    // Before proceeding check if a path is available...
                    if (_tmpPath.corners.Length == 0)
                    {
                        // ... if not then it's an exception.
                        // Exit and keep the point as a fallback.
                        return point;
                    }

                    // Otherwise go ahead and find the best point of the bunch.
                    hotIndex = _tmpPath.corners.Length - 1;
                    for (int i = 0; i < _tmpPath.corners.Length; i++)
                    {
                        offset = _tmpPath.corners[i] - transform.position;
                        offset.y = 0;
                        if (offset.sqrMagnitude > PerceptualDistance * PerceptualDistance)
                        {
                            hotIndex = i - 1;
                            break;
                        }
                    }

                    if (hotIndex == _tmpPath.corners.Length - 1)
                    {
                        newPoint = _tmpPath.corners[hotIndex];
                    }
                    else
                    {
                        // Approximate by linear interpolation.
                        Vector3 dir = (_tmpPath.corners[hotIndex + 1] - _tmpPath.corners[hotIndex]).normalized;
                        float margin = PerceptualDistance - (_tmpPath.corners[hotIndex] - transform.position).magnitude;
                        newPoint = _tmpPath.corners[hotIndex] + dir * margin;
                    }

                    // Lots of computation so far... Cache the result for future
                    // use.
                    _ppCache[point] = newPoint;

                    // Return the new point which is the original pushed point
                    // but "projected" whithin perceptual range.
                    return newPoint;
                }
            }
        }

        /** Draws the vision gizmo */
        protected override void DrawVisionGizmo()
        {
            base.DrawVisionGizmo();

            if (agentParameters != null)
            {
                DrawVisionGizmo(VisionAngle);
            }
        }
    }
}