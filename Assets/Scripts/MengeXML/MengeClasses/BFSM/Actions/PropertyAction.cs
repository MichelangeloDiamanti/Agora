using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using Menge.Agents;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public class SetPropertyAction : Action
        {
            public override string Name => "set_property";

            public AgentPropertyManipulator manipulator;

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
                writer.WriteStartElement("Action");
                writer.WriteAttributeString("type", Name);
                manipulator.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        [Serializable]
        public class OffsetPropertyAction : Action
        {
            public override string Name => "offset_property";

            public AgentPropertyManipulator manipulator;

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
                writer.WriteStartElement("Action");
                writer.WriteAttributeString("type", Name);
                manipulator.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        [Serializable]
        public class ScalePropertyAction : Action
        {
            public override string Name => "scale_property";

            public AgentPropertyManipulator manipulator;

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
                writer.WriteStartElement("Action");
                writer.WriteAttributeString("type", Name);
                manipulator.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }
}