using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public abstract class GoalSelector : IXmlSerializable
        {
            private string name;

            public abstract string Name { get; }

            public abstract XmlSchema GetSchema();

            public abstract void ReadXml(XmlReader reader);

            public abstract void WriteXml(XmlWriter writer);
        }

        public enum GoalSelectorTypes
        {
            NONE,
            IDENTITY_GOAL_SELECTOR,
            EXPLICIT_GOAL_SELECTOR,
            HEATMAP_GOAL_SELECTOR,
            MIRROR_GOAL_SELECTOR,
        }

    }
}