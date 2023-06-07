using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;

namespace Menge
{
    namespace BFSM
    {
        // A simple point goal.  The goal is trivially this point.
        [Serializable]
        [XmlRoot(ElementName = "Goal")]
        public class AbsoluteHeatmapGoal : Goal
        {
            public Texture2D heatmap;

            public float scale;

            public override string Name => "heatmap";

            public AbsoluteHeatmapGoal()
            {
                id = 0;
                capacity = int.MaxValue;
                weight = 1.0f;
                heatmap = null;
                scale = 1.0f;
            }

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

                writer.WriteStartElement("Goal");
                writer.WriteAttributeString("type", Name);
                writer.WriteAttributeString("id", id.ToString(nfi));
                writer.WriteAttributeString("capacity", capacity.ToString(nfi));
                writer.WriteAttributeString("weight", weight.ToString(nfi));

                string texturePath = AssetDatabase.GetAssetPath(heatmap);
                string textureFileNameAndExtension = texturePath.Substring(texturePath.LastIndexOf(heatmap.name));
                writer.WriteAttributeString("file_name", textureFileNameAndExtension);

                writer.WriteAttributeString("scale", scale.ToString(nfi));

                writer.WriteEndElement();
            }

            // public static PointGoal ParseFromXML(XmlReader reader)
            // {
            //     return new PointGoal()
            //     {
            //         type = reader.GetAttribute("type"),
            //         id = int.Parse(reader.GetAttribute("id")),
            //         weight = reader.GetAttribute("weight") != null ? float.Parse(reader.GetAttribute("weight")) : 1.0f,
            //         capacity = reader.GetAttribute("capacity") != null ? int.Parse(reader.GetAttribute("capacity")) : int.MaxValue,
            //         x = float.Parse(reader.GetAttribute("x")),
            //         y = float.Parse(reader.GetAttribute("y"))
            //     };
            // }
        }
    }
}