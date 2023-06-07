using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]

        public class FormationVelocityModifier : VelocityModifier
        {
            public override string Name => "formation";

            public string fileName;

            public FormationScriptableObject formation;

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

                formation.WriteFormationFile(fileName);

                writer.WriteStartElement("VelModifier");
                writer.WriteAttributeString("type", Name);
                writer.WriteAttributeString("file_name", Path.GetFileName(fileName));
                writer.WriteEndElement();
            }


        }

    }
}