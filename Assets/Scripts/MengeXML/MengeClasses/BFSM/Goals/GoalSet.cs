using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public class GoalSet : IXmlSerializable
        {
            public int id;

            [SerializeReference] public List<Goal> goals;

            public Goal GetGoal(int id)
            {
                Goal goal = null;
                int instances = 0;
                foreach (Goal g in goals)
                {
                    if (g.id == id)
                    {
                        if (instances == 0) goal = g;
                        instances++;
                    }
                }
                if (goal == null) Debug.LogWarningFormat("Goal id: {0} does not exist in this set. But you're trying to access it.", id);
                if (instances > 1) Debug.LogWarningFormat("Goalset {0} contains multiple goal with id: {1}. Returing the first found instance when getting the goal.", this.id, id);
                return goal;
            }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                // this.id = int.Parse(reader.GetAttribute("id"));
                // this.goals = new List<Goal>();

                // reader.MoveToContent();

                // bool endElement = false;
                // while (!endElement)
                // {
                //     reader.Read();

                //     if (reader.NodeType != XmlNodeType.EndElement)
                //     {
                //         string _type = reader.GetAttribute("type");
                //         int _id = int.Parse(reader.GetAttribute("id"));

                //         switch (_type)
                //         {
                //             case "point":
                //                 goals.Add(PointGoal.ParseFromXML(reader));
                //                 break;
                //             case "AABB":
                //                 goals.Add(AABBGoal.ParseFromXML(reader));
                //                 break;
                //             case "circle":
                //                 goals.Add(CircleGoal.ParseFromXML(reader));
                //                 break;
                //             case "OBB":
                //                 goals.Add(OBBGoal.ParseFromXML(reader));
                //                 break;
                //             default:
                //                 break;
                //         }
                //     }
                //     else
                //     {
                //         endElement = true;
                //     }
                // }

            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteAttributeString("id", id.ToString());
                foreach (Goal g in goals)
                {
                    g.WriteXml(writer);
                }
            }
        }

    }
}