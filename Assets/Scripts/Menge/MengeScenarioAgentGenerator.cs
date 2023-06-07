using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MengeCS
{
    public class MengeScenarioAgentGenerator : MonoBehaviour
    {
        public string generatorName;
        public AnimationCurve unfolding;

        public TimeCounter timeCounter;

        [Range(0.01f, 100f)] public float amplification = 1f;
        public List<Transform> spawnerPositions;

        private int _currentKeyframeIndex = 0;
        // Variable to keep track of the number of agents that have been
        // spawned since the last keyframe in the animation curve.
        // This variable is updated each time a new agent is spawned and
        // is used to determine when to spawn the next batch of agents
        // based on the values in the animation curve.
        private int agentsAlreadySpawned = 0;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            // Day time to animation curve time
            float curveTime = ConvertTimeSpanToCurveTime(timeCounter.currentTime);

            // Get the number of agents to spawn at the current time
            int agentsToSpawn = Mathf.FloorToInt(unfolding.Evaluate(curveTime) * amplification);

            // Check if there are new agents to spawn
            int newAgentsToSpawn = agentsToSpawn - agentsAlreadySpawned;

            if (newAgentsToSpawn > 0)
            {
                GenerateAgents(newAgentsToSpawn);
                agentsAlreadySpawned += newAgentsToSpawn;
            }
        }

        private float ConvertTimeSpanToCurveTime(TimeSpan time)
        {
            float curveTime = time.Hours;
            curveTime += time.Minutes / 60f;
            curveTime += time.Seconds / 3600f;
            return curveTime;
        }

        private void GenerateAgents(int numberOfAgents)
        {
            Debug.LogFormat("Generating {0} Agents", numberOfAgents);

            // Clear previous positions
            MengeWrapper.clearExternalAgentGeneratorPositions(generatorName);

            if (spawnerPositions.Count == 0)
            {
                Debug.Log("No spawner positions have been set, generating agents at the origin.");
                for (int i = 0; i < numberOfAgents; i++)
                {
                    bool addPositionRes = MengeWrapper.addPositionToExternalAgentGenerator(generatorName, gameObject.transform.position.x, gameObject.transform.position.z);
                    Debug.LogFormat("Adding position to generator {0} at {1}, {2} : {3}", generatorName, gameObject.transform.position.x, gameObject.transform.position.z, addPositionRes);
                }
                bool triggerSpawnRes = MengeWrapper.triggerExternalAgentGeneratorSpawn(generatorName);
                Debug.LogFormat("Triggering spawn for generator {0} : {1}", generatorName, triggerSpawnRes);
            }
            else
            {
                // Create a list of indices and shuffle it
                List<int> indices = Enumerable.Range(0, spawnerPositions.Count).ToList();
                indices = Shuffle(indices);

                for (int i = 0; i < numberOfAgents; i++)
                {
                    int spawnerIndex = indices[i % indices.Count]; // Use modulo operator to prevent IndexOutOfRangeException

                    Transform spawnerTransform = spawnerPositions[spawnerIndex];

                    bool addPositionRes = MengeWrapper.addPositionOrientationToExternalAgentGenerator(generatorName, spawnerTransform.position.x, spawnerTransform.position.z, spawnerTransform.forward.x, spawnerTransform.forward.z);

                    Debug.LogFormat("Adding position to generator {0} at pos [{1}, {2}] rot [{3}, {4}] : {5}", generatorName, spawnerTransform.position.x, spawnerTransform.position.z, spawnerTransform.forward.x, spawnerTransform.forward.z, addPositionRes);
                }
                MengeWrapper.triggerExternalAgentGeneratorSpawn(generatorName);
            }
        }

        private List<int> Shuffle(List<int> list)
        {
            int n = list.Count;
            System.Random rng = new System.Random();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

    }
}