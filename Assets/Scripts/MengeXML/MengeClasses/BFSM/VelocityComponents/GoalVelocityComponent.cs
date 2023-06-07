using System;
using System.Xml;
using System.Xml.Schema;

namespace Menge
{
    namespace BFSM
    {

        public class GoalVelocityComponent : VelocityComponent
        {
            public override string Name => "goal";
        
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
                writer.WriteStartElement("VelComponent");
                writer.WriteAttributeString("type", Name);
                writer.WriteEndElement();
            }
        }
    }
}