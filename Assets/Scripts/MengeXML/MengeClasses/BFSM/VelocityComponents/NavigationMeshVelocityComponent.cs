using System;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;

namespace Menge
{
    namespace BFSM
    {

        public class NavigationMeshVelocityComponent : VelocityComponent
        {
            public override string Name => "nav_mesh";

            public string fileName;
            public float headingThreshold;

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

                writer.WriteStartElement("VelComponent");
                writer.WriteAttributeString("type", Name);
                writer.WriteAttributeString("file_name", fileName);
                writer.WriteAttributeString("heading_threshold", headingThreshold.ToString(nfi));
                writer.WriteEndElement();
            }

        }
    }
}