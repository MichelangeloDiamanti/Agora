using System;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;

namespace Menge
{
    namespace BFSM
    {

        public class ConstantVelocityComponent : VelocityComponent
        {
            public override string Name => "const";

            public float x;
            public float y;

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
                writer.WriteAttributeString("x", x.ToString(nfi));
                writer.WriteAttributeString("y", y.ToString(nfi));
                writer.WriteEndElement();
            }
        }
    }
}