using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using UnityEngine;
using Menge.Math;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public class Transition : IXmlSerializable
        {
            public State from;

            [SerializeReference] public Condition condition;

            public State to;

            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                throw new System.NotImplementedException();
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteAttributeString("from", from.name);
                writer.WriteAttributeString("to", to.name);
                condition.WriteXml(writer);
            }
        }
    }
}