using System;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;
using UnityEngine;
using Menge.Math;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public class TimerCondition : Condition
        {
            public override string Name => "timer";

            public bool perAgent;

            [SerializeReference] public FloatGenerator distribution;

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
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                writer.WriteStartElement("Condition");
                writer.WriteAttributeString("type", Name);
                writer.WriteAttributeString("per_agent", Convert.ToInt32(perAgent).ToString());
                distribution.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }
}