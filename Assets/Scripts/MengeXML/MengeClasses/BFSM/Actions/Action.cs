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
        public abstract class Action : IXmlSerializable
        {
            private string name;

            public abstract string Name { get; }

            public abstract XmlSchema GetSchema();

            public abstract void ReadXml(XmlReader reader);

            public abstract void WriteXml(XmlWriter writer);
        }

        public enum ActionTypes
        {
            SET_PROPERTY_ACTION,
            OFFSET_PROPERTY_ACTION,
            SCALE_PROPERTY_ACTION
        }
    }
}
