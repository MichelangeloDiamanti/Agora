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
        //  The oriented bounding box (OBB) is defined by a pivot point, a size, and an orientation. An OBB
        //  with no rotation is the same as an AABB whose minimum point is the pivot point and which extends
        //  along the x-axis and the y-axis the given width and height, respectively. Positive angle causes
        //  counter-clockwise rotation.
        [Serializable]
        public class OBBGoal : Goal
        {
            public float x;

            public float y;

            public float width;

            public float height;

            public float angle;

            public override string Name => "OBB";

            public OBBGoal()
            {
                id = 0;
                capacity = int.MaxValue;
                weight = 1.0f;
                x = 0.0f;
                y = 0.0f;
                width = 0.0f;
                height = 0.0f;
                angle = 0.0f;
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
                writer.WriteAttributeString("width", width.ToString(nfi));
                writer.WriteAttributeString("height", height.ToString(nfi));
                writer.WriteAttributeString("angle", angle.ToString(nfi));
                writer.WriteEndElement();
            }

            // public static OBBGoal ParseFromXML(XmlReader reader)
            // {
            //     return new OBBGoal()
            //     {
            //         type = reader.GetAttribute("type"),
            //         id = int.Parse(reader.GetAttribute("id")),
            //         weight = reader.GetAttribute("weight") != null ? float.Parse(reader.GetAttribute("weight")) : 1.0f,
            //         capacity = reader.GetAttribute("capacity") != null ? int.Parse(reader.GetAttribute("capacity")) : int.MaxValue,
            //         x = float.Parse(reader.GetAttribute("x")),
            //         y = float.Parse(reader.GetAttribute("y")),
            //         width = float.Parse(reader.GetAttribute("width")),
            //         height = float.Parse(reader.GetAttribute("height")),
            //         angle = float.Parse(reader.GetAttribute("angle"))
            //     };
            // }
        }
    }
}