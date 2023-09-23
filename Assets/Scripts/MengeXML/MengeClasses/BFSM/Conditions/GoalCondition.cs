using System;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public class GoalCondition : Condition
        {
            public override string Name => "goal_reached";

            public float distance;

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
                writer.WriteAttributeString("distance", distance.ToString(nfi));
                writer.WriteEndElement();
            }
        }
    }
}