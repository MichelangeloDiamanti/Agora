using System;
using System.Collections;
using System.Collections.Generic;
using UMA;
using UnityEngine;
using UnityEngine.AI;
using fts;
using Menge.BFSM;
using XNode;
using UrbanTerritoriality.Maps;

namespace MengeCS
{

    class MengeConfig
    {
        public string mengeRoot;
        public string mengePlugins;

        public MengeConfig(string mengeRoot, string mengePlugins)
        {
            this.mengeRoot = mengeRoot;
            this.mengePlugins = mengePlugins;
        }
    }

    public enum CoreOrPlugin
    {
        CORE,
        PLUGIN
    }

    public static class CoreOrPluginExtensions
    {
        public static string GetString(this CoreOrPlugin me)
        {
            switch (me)
            {
                case CoreOrPlugin.CORE:
                    return "core";
                case CoreOrPlugin.PLUGIN:
                    return "plugin";
                default:
                    throw new ArgumentOutOfRangeException("me", me, null);
            }
        }
    }

    public class SimController : MonoBehaviour
    {
        public string pedestrianModel = "orca";

        public CoreOrPlugin coreOrPlugin = CoreOrPlugin.CORE;

        public string mengeProjectName = "";

        public MengeBFSMSceneGraph mengeBFSMSceneGraph;

        public UMACustomRandomAvatar UMApedestrianSpawner;

        public GameObject simplePedestrianModel;

        public bool useUMApedestrianModel = false;

        public PositionConversionMap positionConversionMap;

        private NativePluginLoader nativePluginLoader;

        private Simulator _sim;
        private List<GameObject> _objects = new List<GameObject>();
        private bool _sim_is_valid = false;

        // private UnityEngine.Vector2 _offset; // offset between the 2D heatmap and the 3D scene geometry



        // Note that this is really a pointer to a C++ object.  We need to keep a reference to it so that it doesn't get garbage collected.
        MengeWrapper.AgentChangedStateCallback _agentChangedStateCallback = null;

        private GameObject _agentsRoot;

        // private float _externalAgentGenerationSpawnTime = 3.0f;
        // private float _externalAgentGenerationSpawnTimer = 0.0f;

        // Use this for initialization
        void Start()
        {
            // _offset = new UnityEngine.Vector2(233, 445);

            nativePluginLoader = GameObject.Find("NativePluginLoader").GetComponent<NativePluginLoader>();
            _agentsRoot = GameObject.Find("Agents").gameObject;
            try
            {
                Debug.Log("Starting simulation...");

                //Load Menge config JSON file from (Assets/Resources/menge_config.json)
                TextAsset jsonTextFile = Resources.Load<TextAsset>("menge_config");
                MengeConfig mengeConfig = JsonUtility.FromJson<MengeConfig>(jsonTextFile.text);
                Debug.LogFormat("mengeConfig.mengeRoot: {0}", mengeConfig.mengeRoot);
                Debug.LogFormat("mengeConfig.mengePlugins: {0}", mengeConfig.mengePlugins);

                string demo = mengeProjectName;
                string mengeRoot = mengeConfig.mengeRoot;
                String pluginPath = mengeConfig.mengePlugins;
                // String coreOrPlugin =  "core"; //"plugin"; 
                string behavior = String.Format(@"{0}examples\{1}\{2}\{2}B.xml", mengeRoot, coreOrPlugin.GetString(), demo);
                string scene = String.Format(@"{0}examples\{1}\{2}\{2}S.xml", mengeRoot, coreOrPlugin.GetString(), demo);
                Debug.Log("\tInitialzing sim");
                Debug.Log("\t\tBehavior: " + behavior);
                Debug.Log("\t\tScene: " + scene);

                _sim = new Simulator();

                pedestrianModel = (pedestrianModel == "") ? "orca" : pedestrianModel;
                _sim_is_valid = _sim.Initialize(behavior, scene, pedestrianModel, pluginPath);

                if (_sim_is_valid)
                {
                    int COUNT = _sim.AgentCount;
                    Debug.Log(string.Format("Simulator initialized with {0} agents", COUNT));
                    Debug.Log(string.Format("Simulator time step: {0}", _sim.TimeStep));

                    // subscribe to the agent changed state event
                    _agentChangedStateCallback = new MengeWrapper.AgentChangedStateCallback(AgentChangedStateCallback);
                    MengeWrapper.subscribeToAgentChangedStateEvent(_agentChangedStateCallback);

                    for (int i = 0; i < COUNT; ++i)
                    {
                        Agent a = _sim.GetAgent(i);
                        UnityEngine.Vector3 pos = new UnityEngine.Vector3(a.Position.X, 0, a.Position.Z);

                        GameObject o;
                        if (useUMApedestrianModel)
                            o = UMApedestrianSpawner.GenerateRandomCharacter(pos, UnityEngine.Quaternion.identity, String.Format("Agent{0}", i));
                        else
                        {
                            o = Instantiate(simplePedestrianModel, pos, UnityEngine.Quaternion.identity);
                            o.transform.SetParent(_agentsRoot.transform);
                            o.SetActive(true);
                        }
                        _objects.Add(o);
                    }

                    // TODO: remove this, just for testing
                    // bool res = MengeWrapper.addPositionToExternalAgentGenerator("secondGen", 69, -171);
                    // Debug.LogFormat("addPositionToExternalAgentGenerator: {0}", res);
                }
                else
                {
                    Debug.Log("Failed to initialize the simulator...");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        // write delegate function for when agent changes state
        // note that this is called from the native side, and thus every error might lead to a crash
        // for example, if you try to access something the is null, the application will crash rather than
        // just loggin an error
        public void AgentChangedStateCallback(int agentId, string newState)
        {
            if (mengeBFSMSceneGraph == null)
                return;
            // Debug.LogFormat("Agent {0} changed state to {1}", agentId, newState);
            // do stuff only if the new state has an external goal selector (managed by Unity)
            if (MengeWrapper.isStateGoalSelectorExternal(newState))
            {

                // we need the position of the agent to get a goal destination
                Agent a = _sim.GetAgent(agentId);
                UnityEngine.Vector2 pos = new UnityEngine.Vector2(a.Position.X, a.Position.Z);
                UnityEngine.Vector2 orient = new UnityEngine.Vector2(a.Orientation.X, a.Orientation.Y);
                PointGoal pg = new PointGoal(pos);

                foreach (Node n in mengeBFSMSceneGraph.graph.nodes)
                {
                    if (n == null)
                        continue;
                    // check if the node is a state node with the same name passed in the callback
                    if (n.GetType() == typeof(StateNode))
                    {
                        StateNode stateNode = (StateNode)n;
                        if (stateNode.state.name == newState)
                        {
                            // at this point the state node should have an external goal selector goal selector
                            // which holds a texture representing the goal selection strategy. Get the texture
                            // and use it to set the goal for the agent
                            // Debug.LogFormat("Found Node in the BFSM that matches the callback state name: {0}.", stateNode.state.name);

                            HeatmapGoalSelector hgs = stateNode.GetInputValue<HeatmapGoalSelector>("state.goalSelector");

                            // adjust the agent position to match the 2D heatmap

                            // UnityEngine.Vector2 mapPos = new UnityEngine.Vector2(pos.x + _offset.x, pos.y + _offset.y);
                            UnityEngine.Vector2 mapPos = positionConversionMap.WorldToGridPos(new UnityEngine.Vector2(pos.x, pos.y));

                            pg = hgs.getGoal(mapPos, orient);

                            if (pg == null)
                            {
                                Debug.LogWarningFormat("Agent {0} has no goal for state {1}.", agentId, newState);
                                pg = new PointGoal(pos);
                            }
                            else
                            {
                                // UnityEngine.Vector2 goalMapPos = positionConversionMap.WorldToGridPos(new UnityEngine.Vector2(pg.x, pg.y));
                                UnityEngine.Vector2 worldPos = positionConversionMap.GridToWorldPosition((int)pg.x, (int)pg.y);
                                // pg.x = pg.x - worldPos.x;
                                // pg.y = pg.y - worldPos.y;
                                pg.x = worldPos.x;
                                pg.y = worldPos.y;


                                // // adjust the goal position to match the 2D heatmap
                                // pg.x = pg.x - _offset.x;
                                // pg.y = pg.y - _offset.y;

                                // Debug.LogFormat("Setting goal for agent {0} to ({1}, {2}).", agentId, pg.x, pg.y);
                            }

                            break;
                        }
                    }
                }

                // I'm trying to reproject the goal position to the navmesh here, because the point retrieved from the heatmap
                // is not always on the navmesh (imagine there are some contour pixels on the heatmap that are not on the navmesh)
                // and this is an issue for the Menge simulator, which will crash if the point goal is not on the navmesh

                // TODO: this is very inefficient, the max distance should be way smaller. But I don't have the y coordinate, 
                // so I'm forced to use a large value. Need to find a better way to do this
                // UnityEngine.Vector3 closestPoint = FindClosestNavMeshPoint(new UnityEngine.Vector3(pg.x, 0, pg.y), 1000.0f);

                // if (closestPoint != UnityEngine.Vector3.zero)
                // {
                //     // Debug.LogErrorFormat("Agent {0} Changed into a new State \"{1}\" which has an External Goal Selector.", agentId, newState);
                //     // MengeWrapper.setStatePointGoalForAgent(newState, (uint)agentId, 38.0f, -148.0f);
                //     // Debug.Log("navmesh point found");
                //     MengeWrapper.setStatePointGoalForAgent(newState, (uint)agentId, closestPoint.x, closestPoint.z);
                // }
                // else
                // {
                //     Debug.Log("navmesh point not found");
                //     MengeWrapper.setStatePointGoalForAgent(newState, (uint)agentId, pg.x, pg.y);
                // }

                MengeWrapper.setStatePointGoalForAgent(newState, (uint)agentId, pg.x, pg.y);

            }

        }

        private UnityEngine.Vector3 FindClosestNavMeshPoint(UnityEngine.Vector3 position, float maxDistance)
        {
            // Create a new position with the same x and z coordinates, but with a y coordinate of 0
            UnityEngine.Vector3 positionWithZeroY = new UnityEngine.Vector3(position.x, 0, position.z);

            NavMeshHit hit;

            if (NavMesh.SamplePosition(positionWithZeroY, out hit, maxDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return UnityEngine.Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            try
            {
                if (_sim_is_valid)
                {
                    _sim.TimeStep = Time.deltaTime;

                    // _externalAgentGenerationSpawnTimer += Time.deltaTime;
                    // if (_externalAgentGenerationSpawnTimer > _externalAgentGenerationSpawnTime)
                    // {
                    //     _externalAgentGenerationSpawnTimer = 0.0f;
                    //     bool res = MengeWrapper.triggerExternalAgentGeneratorSpawn("secondGen");
                    //     Debug.LogFormat("Triggered external agent generator spawn. Result: {0}", res);
                    // }

                    // this visualizes any new agents that were added to the simulation
                    int updateRes = _sim.UpdateAgentsCollection();


                    if (updateRes != -1)
                    {
                        // Debug.LogFormat("New agents added to the simulation. Current count: {0}.", updateRes);
                        for (int i = _objects.Count; i < updateRes; ++i)
                        {
                            Agent a = _sim.GetAgent(i);
                            UnityEngine.Vector3 pos = new UnityEngine.Vector3(a.Position.X, 0, a.Position.Z);

                            UnityEngine.Vector3 orientation3D = new UnityEngine.Vector3(a.Orientation.X, 0, a.Orientation.Y).normalized;
                            Quaternion lookRotation = UnityEngine.Quaternion.identity;
                            if (orientation3D != UnityEngine.Vector3.zero)
                            {
                                lookRotation = Quaternion.LookRotation(orientation3D, UnityEngine.Vector3.up);
                            }

                            GameObject o;
                            if (useUMApedestrianModel)
                                o = UMApedestrianSpawner.GenerateRandomCharacter(pos, lookRotation, String.Format("Agent{0}", i));
                            else
                            {
                                o = Instantiate(simplePedestrianModel, pos, lookRotation);
                                o.transform.SetParent(_agentsRoot.transform);
                                o.SetActive(true);
                            }
                            _objects.Add(o);
                        }
                    }


                    _sim.DoStep();

                    UnityEngine.Vector3 newPos = new UnityEngine.Vector3();
                    for (int i = 0; i < _sim.AgentCount; ++i)
                    {

                        Agent agent = _sim.GetAgent(i);
                        UnityEngine.Vector3 vel = new UnityEngine.Vector3(agent.Velocity.X, agent.Velocity.Y, agent.Velocity.Z);

                        Vector3 pos3d = _sim.GetAgent(i).Position;


                        newPos.Set(pos3d.X, pos3d.Y, pos3d.Z);
                        _objects[i].transform.position = newPos;
                        Animator anim = _objects[i].GetComponent<Animator>();
                        if (anim) anim.SetFloat("Forward", vel.magnitude / 3.0f);

                        if (vel.magnitude > 0f)
                        {
                            // Quaternion lookRotation = Quaternion.LookRotation(vel.normalized);
                            // _objects[i].transform.rotation = Quaternion.RotateTowards(_objects[i].transform.rotation, lookRotation, 120 * Time.deltaTime);

                            UnityEngine.Vector3 orientation3D = new UnityEngine.Vector3(agent.Orientation.X, 0, agent.Orientation.Y).normalized;

                            if (orientation3D != UnityEngine.Vector3.zero)
                            {
                                Quaternion lookRotation = Quaternion.LookRotation(orientation3D, UnityEngine.Vector3.up);
                                _objects[i].transform.rotation = lookRotation;
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        void OnDestroy()
        {
            MengeWrapper.unloadPlugins();
            nativePluginLoader.UnloadAll();
        }

    }
}