using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public abstract class VelocityComponent : IXmlSerializable
        {
            private string name;

            public abstract string Name { get; }

            public abstract XmlSchema GetSchema();

            public abstract void ReadXml(XmlReader reader);

            public abstract void WriteXml(XmlWriter writer);
        }

        public enum VelocityComponentTypes
        {
            NONE,
            ZERO,
            CONSTANT,
            CONSTANT_DIRECTION,
            GOAL,
            VELOCITY_FIELD,
            ROAD_MAP,
            NAVIGATION_MESH
        }
    }
}
