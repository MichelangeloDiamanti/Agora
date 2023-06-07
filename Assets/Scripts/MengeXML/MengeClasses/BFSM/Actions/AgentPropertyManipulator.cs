using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using Menge.Math;
using Menge.BFSM;

namespace Menge
{
    namespace Agents
    {
        [Serializable]
        public class AgentPropertyManipulator : IXmlSerializable
        {
            public PropertyOperand Property;

            [SerializeReference] public FloatGenerator Distribution;

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                throw new NotImplementedException();
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteAttributeString("property", Property.ToString());
                Distribution.WriteXml(writer);
            }
        }
    }
}