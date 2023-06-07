using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]

        public abstract class VelocityModifier : IXmlSerializable
        {
            private string name;

            public abstract string Name { get; }

            public abstract XmlSchema GetSchema();

            public abstract void ReadXml(XmlReader reader);

            public abstract void WriteXml(XmlWriter writer);
        }

        public enum VelocityModifierTypes
        {
            SCALE,
            FORMATION
        }
    }
}