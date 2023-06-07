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
        public class IdentityGoalSelector : GoalSelector
        {
            public override string Name => "identity";

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
                writer.WriteEndElement();
            }
        }
    }
}