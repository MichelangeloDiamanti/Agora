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
        // A simple point goal.  The goal is trivially this point.
        [Serializable]
        [XmlRoot(ElementName = "Goal")]
        public class PointGoal : Goal
        {
            public float x;

            public float y;

            public override string Name => "point";

            public PointGoal()
            {
                id = 0;
                capacity = int.MaxValue;
                weight = 1.0f;
                x = 0.0f;
                y = 0.0f;
            }

            public PointGoal(float x, float y)
            {
                id = 0;
                capacity = int.MaxValue;
                weight = 1.0f;
                this.x = x;
                this.y = y;
            }

            public PointGoal(Vector2 xy){
                id = 0;
                capacity = int.MaxValue;
                weight = 1.0f;
                this.x = xy.x;
                this.y = xy.y;
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