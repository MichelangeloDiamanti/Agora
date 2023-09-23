using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;
using UnityEngine;
using UnityEditor;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public class ColorCondition : Condition
        {
            public override string Name => "match_color";

            public Texture2D relativeHeatmap;

            public float scale;
            public Vector2 offset;

            public Color conditionColor;

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

                string texturePath = AssetDatabase.GetAssetPath(relativeHeatmap);
                string textureFileNameAndExtension = texturePath.Substring(texturePath.LastIndexOf(relativeHeatmap.name));
                writer.WriteAttributeString("file_name", textureFileNameAndExtension);


                string colorString = string.Format("{0} {1} {2}", (int)(conditionColor.r * 255), (int)(conditionColor.g * 255), (int)(conditionColor.b * 255));
                writer.WriteAttributeString("color_rgb", colorString);

                writer.WriteAttributeString("scale", scale.ToString(nfi));
                writer.WriteAttributeString("offsetX", offset.x.ToString(nfi));
                writer.WriteAttributeString("offsetY", offset.y.ToString(nfi));


                writer.WriteEndElement();
            }
        }
    }
}