using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MengeCS
{
    /// <summary>
    /// Wrapper for Menge::SimulatorBase
    /// </summary>
    public class Simulator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Simulator()
        {
            _agents = new List<Agent>();
        }

        public bool Initialize(String behaveXml, String sceneXml, String model, String pluginPath)
        {
            try
            {
                if (MengeWrapper.initSimulator(behaveXml, sceneXml, model, pluginPath))
                {
                    FindTriggers();
                    UpdateAgentsCollection();
                    return true;
                }
                else
                {
                    System.Console.WriteLine("Failed to initialize simulator.");
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// Updates the agent collection to match the simulator if the number of agents has changed.
        /// returns true if the collection was updated.
        /// </summary>
        public int UpdateAgentsCollection()
        {
            // _agents = new List<Agent>();
            uint simAgentCount = MengeWrapper.agentCount();

            if (simAgentCount > _agents.Count)
            {
                for (int i = _agents.Count; i < simAgentCount; ++i)
                {
                    uint agentId = (uint)i;

                    Agent agt = new Agent();
                    float posX = 0;
                    float posY = 0;
                    float posZ = 0;
                    MengeWrapper.getAgentPosition(agentId, ref posX, ref posY, ref posZ);
                    agt._pos = new Vector3(posX, posY, posZ);

                    float rotX = 0;
                    float rotY = 0;

                    MengeWrapper.getAgentOrient(agentId, ref rotX, ref rotY);
                    Debug.LogFormat("Agent {0} orientation [{1}, {2}]", i, rotX, rotY);

                    agt._orient = new Vector2(rotX, rotY);
                    agt._class = MengeWrapper.getAgentClass(agentId);
                    agt._radius = MengeWrapper.getAgentRadius(agentId);
                    _agents.Add(agt);
                }
                return _agents.Count;
            }
            return -1;
        }

        /// <summary>
        /// The number of agents in the simulation.
        /// </summary>
        public int AgentCount { get { return _agents.Count; } }

        /// <summary>
        /// Returns the ith agent.
        /// </summary>
        /// <param name="i">Index of the agent to retrieve.</param>
        /// <returns>The ith agent.</returns>
        public Agent GetAgent(int i)
        {
            return _agents[i];
        }

        /// <summary>
        /// The simulation time step.
        /// </summary>
        public float TimeStep
        {
            get { return _timeStep; }
            set { _timeStep = value; MengeWrapper.setTimeStep(_timeStep); }
        }
        private float _timeStep = 0.1f;

        /// <summary>
        /// Advances the simulation by the current time step.
        /// </summary>
        /// <returns>True if evaluation is successful and the simulation can proceed.</returns>
        public bool DoStep()
        {
            bool running = MengeWrapper.doStep();
            for (int i = 0; i < _agents.Count; ++i)
            {
                float x = 0, y = 0, z = 0;
                MengeWrapper.getAgentPosition((uint)i, ref x, ref y, ref z);
                _agents[i].Position.Set(x, y, z);
                MengeWrapper.getAgentOrient((uint)i, ref x, ref y);
                _agents[i].Orientation.Set(x, y);
                MengeWrapper.getAgentVelocity((uint)i, ref x, ref y, ref z);
                _agents[i].Velocity.Set(x, y, z);
            }
            return true;
        }

        /// <summary>
        /// Populates the trigger list from the simulator.
        /// </summary>
        private void FindTriggers()
        {
            _triggers = new List<ExternalTrigger>();
            int triggerCount = MengeWrapper.externalTriggerCount();
            for (int i = 0; i < triggerCount; ++i)
            {
                string s = Marshal.PtrToStringAnsi(MengeWrapper.externalTriggerName(i));
                _triggers.Add(new ExternalTrigger(s));
            }
        }

        /// <summary>
        /// The agents in the simulation.
        /// </summary>
        private List<Agent> _agents;

        /// <summary>
        /// Read-only access to the set of triggers.
        /// </summary>
        public List<ExternalTrigger> Triggers { get { return _triggers; } }

        /// <summary>
        /// The external triggers exposed by the simulator.
        /// </summary>
        private List<ExternalTrigger> _triggers;

    }
}
