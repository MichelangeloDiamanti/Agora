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
        public class ExplicitGoalSelector : GoalSelector
        {
            public override string Name => "explicit";

            public int goalSetId;

            [SerializeReference] public Goal goal;

            public override XmlSchema GetSchema()
            {
                return null;
            }

            public override void ReadXml(XmlReader reader)
            {
                throw new NotImplementedException();
            }

            public override void WriteXml(XmlWriter writer)
            {
                writer.WriteStartElement("GoalSelector");
                writer.WriteAttributeString("type", Name);
                writer.WriteAttributeString("goal_set", goalSetId.ToString());
                writer.WriteAttributeString("goal", goal.id.ToString());
                writer.WriteEndElement();
            }
        }
    }
}