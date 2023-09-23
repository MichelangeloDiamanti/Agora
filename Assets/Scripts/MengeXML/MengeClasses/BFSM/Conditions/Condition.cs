using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public abstract class Condition : IXmlSerializable
        {
            private string name;

            public abstract string Name { get; }
            
            public abstract XmlSchema GetSchema();

            public abstract void ReadXml(XmlReader reader);

            public abstract void WriteXml(XmlWriter writer);
        }

        public enum ConditionTypes
        {
            NOT,
            AND,
            OR,
            GOAL_REACHED,
            TIMER,
            COLOR_CONDITION
        }
    }
}