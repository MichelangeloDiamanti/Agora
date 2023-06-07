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
        // A circular goal region with uniform probability
        [Serializable]
        public class CircleGoal : Goal
        {
            public float x;

            public float y;

            public float radius;

            public override string Name => "circle";

            public CircleGoal()
            {
                id = 0;
                capacity = int.MaxValue;
                weight = 1.0f;
                x = 0.0f;
                y = 0.0f;
                radius = 0.0f;
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
                writer.WriteAttributeString("x", x.ToString(nfi));
                writer.WriteAttributeString("y", y.ToString(nfi));
                writer.WriteAttributeString("radius", radius.ToString(nfi));
                writer.WriteEndElement();
            }

            // public static CircleGoal ParseFromXML(XmlReader reader)
            // {
            //     return new CircleGoal()
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