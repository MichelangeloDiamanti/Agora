using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace MengeXML
{
    #region Behavior

    [Serializable]
    public class BFSM
    {
        [XmlElement("GoalSet")]
        public List<GoalSet> goalSets;

        [XmlElement("State")]
        public List<State> states;

        [XmlElement("Transition")]
        public List<Transition> transitions;
    }

    [Serializable]
    public class GoalSet
    {
        [XmlAttribute("id")]
        public int id;

        [XmlElement("Goal")]
        public List<Goal> goals;
    }

    [Serializable]
    public class Goal
    {
        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("id")]
        public int Id;
        [XmlAttribute("x")]
        public float X;
        [XmlAttribute("y")]
        public float Y;
    }


    [Serializable]
    public class State
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("final")]
        public bool Final;
        [XmlElement("GoalSelector")]
        public GoalSelector goalSelector;

        [XmlElement("VelComponent")]
        public VelocityComponent velocityComponent;
    }

    [Serializable]
    public class GoalSelector
    {
        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("goal_set")]
        public int GoalSet;
        [XmlAttribute("goal")]
        public int Goal;

        public override string ToString()
        {
            return string.Format("Type: {0} | GoalSet: {1} | Goal {2}", Type, GoalSet, Goal);
        }
    }

    [Serializable]
    public class VelocityComponent
    {
        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("heading_threshold")]
        public float HeadingThreshold;
        [XmlAttribute("file_name")]
        public string FileName;

        public override string ToString()
        {
            return string.Format("Type: {0} | HeadingThreshold: {1} | FileName {2}", Type, HeadingThreshold, FileName);
        }
    }

    [Serializable]
    public class Transition
    {
        [XmlAttribute("from")]
        public string From;
        [XmlAttribute("to")]
        public string To;

        [XmlElement("Condition")]
        public Condition condition;
    }

    [Serializable]
    public class Condition
    {
        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("dist")]
        public string Dist;

        [XmlAttribute("distance")]
        public float Distance;

        [XmlAttribute("min")]
        public float Min;
        [XmlAttribute("max")]
        public float Max;
        [XmlAttribute("seed")]
        public float Seed;
        [XmlAttribute("per_agent")]
        public float PerAgent;

        public override string ToString()
        {
            string s = "";
            switch (Type)
            {
                case "goal_reached":
                    s = String.Format("Type: {0} | Distance: {1}", Type, Distance);
                    break;
                case "timer":
                    s = String.Format("Type: {0} | Random Dist: {1} | Min: {2} | Max: {3} | Seed: {4} | Per Agent: {5}", Type, Dist, Min, Max, Seed, PerAgent);
                    break;
                default:
                    break;
            }
            return s;
        }
    }

    #endregion


    #region Scene     
    [Serializable]
    public class Experiment
    {
        [XmlAttribute("version")]
        public string Version;

        [XmlElement("Common")]
        public ExperimentCommonParameters CommonParameters;

        [XmlElement("SpatialQuery")]
        public SpatialQuery SpatialQuery;

        [XmlElement("Elevation")]
        public Elevation Elevation;

        [XmlElement("ObstacleSet")]
        public ObstacleSet ObstacleSet;

        [XmlElement("AgentProfile")]
        public List<AgentProfile> AgentProfiles;

        [XmlElement("AgentGroup")]
        public List<AgentGroup> AgentGroups;
    }


    [Serializable]
    public class ExperimentCommonParameters
    {
        [XmlAttribute("time_step")]
        public float TimeStep;
    }


    [Serializable]
    public class SpatialQuery
    {
        [XmlAttribute("type")]
        public string Type;

        [XmlAttribute("test_visibility")]
        public bool TestVisibility;

        [XmlAttribute("file_name")]
        public string FileName;
    }


    [Serializable]
    public class Elevation
    {
        [XmlAttribute("type")]
        public string Type;

        [XmlAttribute("file_name")]
        public string FileName;
    }


    [Serializable]
    public class ObstacleSet
    {
        [XmlAttribute("type")]
        public string Type;

        [XmlAttribute("file_name")]
        public string FileName;

        [XmlAttribute("class")]
        public int Class;
    }


    [Serializable]
    public class AgentProfile
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlElement("Common")]
        public AgentProfileCommonParameters CommonParameters;

        [XmlElement("ORCA")]
        public AgentProfileORCAParameters ORCAParameters;
    }


    [Serializable]
    public class AgentProfileCommonParameters
    {
        [XmlAttribute("max_angle_vel")]
        public float MaxAngularVelocity;

        [XmlAttribute("max_neighbors")]
        public int MaxNeighbors;

        [XmlAttribute("obstacleSet")]
        public int ObstacleSet;

        [XmlAttribute("neighbor_dist")]
        public float NeighborDist;

        [XmlAttribute("r")]
        public float Radius;

        [XmlAttribute("class")]
        public int Class;

        [XmlAttribute("pref_speed")]
        public float PreferredSpeed;

        [XmlAttribute("max_speed")]
        public float MaxSpeed;

        [XmlAttribute("max_accel")]
        public float MaxAcceleration;
    }


    [Serializable]
    public class AgentProfileORCAParameters
    {
        [XmlAttribute("tau")]
        public float Tau;
        [XmlAttribute("tauObst")]
        public float TauObst;
    }


    [Serializable]
    public class AgentGroup
    {
        [XmlElement("ProfileSelector")]
        public AgentGroupProfileSelector ProfileSelector;

        [XmlElement("StateSelector")]
        public AgentGroupStateSelector StateSelector;

        [XmlElement("Generator")]
        public AgentGroupGenerator Generator;
    }


    [Serializable]
    public class AgentGroupProfileSelector
    {
        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("name")]
        public string name;
    }


    [Serializable]
    public class AgentGroupStateSelector
    {
        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("name")]
        public string name;
    }


    [Serializable]
    public class AgentGroupGenerator
    {
        [XmlAttribute("type")]
        public string Type;

        [XmlElement("Agent")]
        public List<AgentGroupAgent> Agents;
    }


    [Serializable]
    public class AgentGroupAgent
    {
        [XmlAttribute("p_x")]
        public float PositionX;

        [XmlAttribute("p_y")]
        public float PositionY;

        public void SetPosition(Vector3 pos)
        {
            PositionX = pos.x;
            PositionY = pos.z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(PositionX, 0, PositionY);
        }

        [Button("Select in SceneView", ButtonSizes.Small)]
        public void SelectInSceneView()
        {
            SceneView.lastActiveSceneView.LookAt(new Vector3(PositionX, 0, PositionY));
        }
    }
    #endregion

    // [Serializable]
    // public class AgentGroupAgent : ISerializationCallbackReceiver
    // {
    //     private float _positionX;
    //     private float _positionY;

    //     [XmlAttribute("p_x"), ShowInInspector]
    //     public float PositionX
    //     {
    //         get => _positionX;
    //         set
    //         {
    //             _positionX = value;
    //         }
    //     }

    //     [XmlAttribute("p_y"), ShowInInspector]
    //     public float PositionY
    //     {
    //         get => _positionY;
    //         set
    //         {
    //             _positionY = value;
    //         }
    //     }

    //     public void OnAfterDeserialize()
    //     {
    //         Debug.LogFormat("OnAfterDeserialize [{0},{1}]", PositionX, PositionY);
    //     }

    //     public void OnBeforeSerialize()
    //     {

    //         Debug.LogFormat("OnBeforeSerialize [{0},{1}]", PositionX, PositionY);
    //     }

    //     // public Transform PositionTransform;


    //     [Button("Select in SceneView", ButtonSizes.Small)]
    //     public void SelectInSceneView()
    //     {
    //         SceneView.lastActiveSceneView.LookAt(new Vector3(PositionX, 0, PositionY));
    //     }
    // }
}
