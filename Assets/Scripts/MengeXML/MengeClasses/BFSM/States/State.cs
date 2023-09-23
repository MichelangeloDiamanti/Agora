using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Globalization;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public class State : IXmlSerializable
        {
            public string name;

            public bool final;

            [SerializeReference] public List<Action> actions;

            [SerializeReference] public GoalSelector goalSelector;

            [SerializeReference] public VelocityComponent velocityComponent;

            [SerializeReference] public VelocityModifier velocityModifier;


            public State()
            {
                actions = new List<Action>();
                // goalSelector = new IdentityGoalSelector();
                // velocityComponent = new ZeroVelocityComponent();
                // velocityModifier = new ScaleVelocityModifier();

                goalSelector = null;
                velocityComponent = null;
                velocityModifier = null;
            }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                throw new NotImplementedException();
            }

            public void WriteXml(XmlWriter writer)
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                writer.WriteAttributeString("name", name);
                writer.WriteAttributeString("final", Convert.ToInt32(final).ToString(nfi));

                foreach (Action a in actions)
                {
                    a.WriteXml(writer);
                }

                if (goalSelector != null) goalSelector.WriteXml(writer);

                if (velocityComponent != null) velocityComponent.WriteXml(writer);

                if (velocityModifier != null) velocityModifier.WriteXml(writer);

                // writer.WriteAttributeString("id", id.ToString(nfi));
                // writer.WriteAttributeString("capacity", capacity.ToString(nfi));
                // writer.WriteAttributeString("weight", weight.ToString(nfi));
                // writer.WriteAttributeString("x", x.ToString(nfi));
                // writer.WriteAttributeString("y", y.ToString(nfi));
            }
        }
    }
}