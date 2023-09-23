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
        public class MirrorGoalSelector : GoalSelector
        {
            public override string Name => "mirror";

            public bool mirrorX;
            public bool mirrorY;

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
                writer.WriteAttributeString("mirror_x", Convert.ToInt32(mirrorX).ToString());
                writer.WriteAttributeString("mirror_y", Convert.ToInt32(mirrorY).ToString());
                writer.WriteEndElement();
            }
        }
    }
}