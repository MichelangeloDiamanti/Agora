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
    /** A base class for agents that have access to a map */
    public abstract class MapWanderer : TrackedAgent
    {
        /** A map used by the map wanderer */
        //TODO transform into setter
        //[HideInInspector]
        public GeneralHeatmap map;

        /** Paramters for the map wanderer */
        public ScriObj.WanderingAgentParameters agentParameters;

        // Agent unit size is a scaling factor on the agent dimensions. It tells
        // us how many meters corresponds one agent unit. Default is 1 agent
        // unit equal one meter. Increase to model bigger agents.
        [Range(0.01f, 10f)]
        public float agentUnitSize = 1f;

        // Agent approximated radius and height. This value will affect the 
        // radius and height of the navmesh agent component.
        public float radius = 0.45f;
        public float height = 2f;

        // the base speed of the agent, scaled by the agentUnitSize parameter
        public float minSpeed = 1.1f;
        public float maxSpeed = 1.45f;
        [HideInInspector] public float speed = 1.4f; // hidden for now, waiting for refactoring of minimax vars

        /** The current destination */
        protected Vector3 _currentDestination;

        /** Navmesh agent for path-finding **/
        protected NavMeshAgent _navMeshAgent;

        protected HashSet<GameObject> _memory;

        // private SimulationManager _eventManager;

        protected float DefaultMapWeight
        {
            get { return agentParameters.defaultMapWeight; }
        }

        public Vector3 currentDestination
        {
            get { return _currentDestination; }
        }

        public NavMeshAgent navMeshAgent
        {
            get { return _navMeshAgent; }
        }

        public float DefaultAngleWeight
        {
            get { return agentParameters.defaultAngleWeight; }
        }
        public float DefaultNewnessWeight
        {
            get { return agentParameters.defaultNewnessWeight; }
        }
        public float PerceptualDistance
        {
            get { return agentParameters.perceptualDistance * agentUnitSize; }
        }
        public int NrOfPickedPoints
        {
            get { return agentParameters.nrOfPickedPoints; }
        }
        public float MaxTimeBetweenPicks
        {
            get { return agentParameters.maxTimeBetweenPicks; }
        }
        public bool ShowGizmo
        {
            get { return agentParameters.showGizmo; }
        }
        public Color GizmoColor
        {
            get { return agentParameters.gizmoColor; }
        }
        public Vector3 GizmoPositionOffset
        {
            get { return agentParameters.gizmoPositionOffset; }
        }

        /** Number of previous destinations in
		 * the memory of the agent.
		 * E.g. if this is 3, then the agent
		 * will remember the 3 last destination
		 * points he was trying to reach.
		 */
        protected const int nrOfRememberedPoints = 3;

        /** Time interval in seconds for saving previous locations */
        protected const float pointSavingInterval = 5.0f;

        /** Used for drawing a sector showing the vision cone of the agent */
        protected AgentVisionGizmo visionGizmo = null;

        /** The vision angle of the agent */
        public virtual float VisionAngle
        {
            get
            {
                return 120f;
            }
        }

        public void Memorize(GameObject gameObject)
        {
            _memory.Add(gameObject);
        }

        public bool IsRemembering(GameObject gameObject)
        {
            return _memory.Contains(gameObject);
        }

        protected override void Awake()
        {
            base.Start();

            // Init. internal navmesh agent 
            _navMeshAgent = GetComponent<NavMeshAgent>();

            // Init memory
            _memory = new HashSet<GameObject>();
        }

        protected override void Start()
        {
            base.Start();

            // Randomize
            Randomize();

            // Reset to default attributes.
            Reset();
        }

        protected override void Update()
        {
            transform.localScale = new Vector3(agentUnitSize, agentUnitSize, agentUnitSize);
        }

        /** Draw the vision gizmo
         * @param visionAngle The vision angle */
        protected virtual void DrawVisionGizmo(float visionAngle)
        {
            if (visionGizmo == null)
            {
                visionGizmo = new AgentVisionGizmo();
            }
            visionGizmo.position =
                transform.position + GizmoPositionOffset;
            visionGizmo.rotation = transform.rotation;
            visionGizmo.visionAngle = visionAngle;
            visionGizmo.visionDistance = PerceptualDistance;
            visionGizmo.fillColor = new Color(
                GizmoColor.r,
                GizmoColor.g,
                GizmoColor.b,
                GizmoColor.a / 2f);
            visionGizmo.lineColor = GizmoColor;
            visionGizmo.OnDrawGizmos();
        }

        /** Draws the vision gizmo */
        protected virtual void DrawVisionGizmo()
        {
            DrawVisionGizmo(VisionAngle);
        }

        /** Unity OnDrawGizmos method */
        protected virtual void OnDrawGizmosSelected()
        {
            if (agentParameters == null)
            {
                return;
            }

            if (!ShowGizmo)
            {
                return;
            }

            DrawVisionGizmo();

            // Reset agent attributes on gizmo update to make
            // in-editor interactive parameter setting possible.
            Reset();
        }

        protected void Randomize()
        {
            speed *= UnityEngine.Random.Range(minSpeed, maxSpeed);
        }

        protected void Reset()
        {
            transform.localScale = Vector3.one * agentUnitSize;
            ResetNavMeshAgent();
        }

        protected void ResetNavMeshAgent()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = speed * agentUnitSize;
            _navMeshAgent.height = height * agentUnitSize;
            _navMeshAgent.radius = radius * agentUnitSize;
        }
    }
}

