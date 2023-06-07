using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;


namespace Menge
{
    namespace BFSM
    {
        // A axis-aligned bounding box goal region with uniform probability
        [Serializable]
        public class AABBGoal : Goal
        {
            public float min_x;

            public float max_x;

            public float min_y;

            public float max_y;

            public override string Name => "AABB";

            public AABBGoal()
            {
                id = 0;
                capacity = int.MaxValue;
                weight = 1.0f;
                min_x = 0.0f;
                max_x = 0.0f;
                min_y = 0.0f;
                max_y = 0.0f;
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
                writer.WriteAttributeString("min_x", min_x.ToString(nfi));
                writer.WriteAttributeString("max_x", max_x.ToString(nfi));
                writer.WriteAttributeString("min_y", min_y.ToString(nfi));
                writer.WriteAttributeString("max_y", max_y.ToString(nfi));
                writer.WriteEndElement();
            }

            // public static AABBGoal ParseFromXML(XmlReader reader)
            // {
            //     return new AABBGoal()
            //     {
            //         type = reader.GetAttribute("type"),
            //         id = int.Parse(reader.GetAttribute("id")),
            //         weight = reader.GetAttribute("weight") != null ? float.Parse(reader.GetAttribute("weight")) : 1.0f,
            //         capacity = reader.GetAttribute("capacity") != null ? int.Parse(reader.GetAttribute("capacity")) : int.MaxValue,
            //         min_x = float.Parse(reader.GetAttribute("min_x")),
            //         max_x = float.Parse(reader.GetAttribute("max_x")),
            //         min_y = float.Parse(reader.GetAttribute("min_y")),
            //         max_y = float.Parse(reader.GetAttribute("max_y"))
            //     };
            // }
        }
    }
}