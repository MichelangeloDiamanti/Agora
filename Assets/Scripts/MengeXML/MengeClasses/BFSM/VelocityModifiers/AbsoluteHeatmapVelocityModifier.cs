using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;
using UnityEditor;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]

        public class AbsoluteHeatmapVelocityModifier : VelocityModifier
        {
            public override string Name => "absolute_heatmap";

            public Texture2D heatmap;

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

                string texturePath = AssetDatabase.GetAssetPath(heatmap);
                string textureFileNameAndExtension = texturePath.Substring(texturePath.LastIndexOf(heatmap.name));
                writer.WriteAttributeString("file_name", textureFileNameAndExtension);

                writer.WriteAttributeString("scale", scale.ToString(nfi));

                writer.WriteEndElement();
            }


        }

    }
}