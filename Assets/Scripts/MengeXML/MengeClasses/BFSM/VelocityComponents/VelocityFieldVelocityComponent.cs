using System;
using System.Xml;
using System.Xml.Schema;

namespace Menge
{
    namespace BFSM
    {

        public class VelocityFieldVelocityComponent : VelocityComponent
        {
            public override string Name => "vel_field";

            public string fileName;
            public bool useNearest;

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
                writer.WriteAttributeString("file_name", fileName);
                writer.WriteAttributeString("use_nearest", Convert.ToInt32(useNearest).ToString());
                writer.WriteEndElement();
            }

        }
    }
}