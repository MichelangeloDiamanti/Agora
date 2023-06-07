using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace Menge
{
    namespace BFSM
    {

        [Serializable]
        public abstract class Goal : IXmlSerializable
        {
            private string name;

            public abstract string Name { get; }

            public int id;

            public int capacity;

            public float weight;

            public abstract XmlSchema GetSchema();

            public abstract void ReadXml(XmlReader reader);

            public abstract void WriteXml(XmlWriter writer);

        }

        public enum GoalTypes
        {
            POINT_GOAL,
            CIRCLE_GOAL,
            AXIS_ALIGNED_BOUNDING_BOX,
            ORIENTED_BOUNDING_BOX,
            HEATMAP_GOAL
        }

    }
}