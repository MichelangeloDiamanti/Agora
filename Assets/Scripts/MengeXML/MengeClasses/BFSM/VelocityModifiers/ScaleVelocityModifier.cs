using System;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]

        public class ScaleVelocityModifier : VelocityModifier
        {
            public override string Name => "scale";

            public float scale;

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

                writer.WriteStartElement("VelModifier");
                writer.WriteAttributeString("type", Name);
                writer.WriteAttributeString("scale", scale.ToString(nfi));
                writer.WriteEndElement();
            }
        }

    }
}