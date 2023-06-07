using UnityEngine;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;

namespace Menge
{
    namespace BFSM
    {
        [Serializable]
        public class NotCondition : Condition
        {
            public override string Name => "not";

            [SerializeReference] public Condition inCondition;

            public NotCondition(Condition inCondition) {
                this.inCondition = inCondition;
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

                writer.WriteStartElement("Condition");
                writer.WriteAttributeString("type", Name);
                inCondition.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        [Serializable]
        public class AndCondition : Condition
        {
            public override string Name => "and";

            [SerializeReference] public Condition inConditionA;
            [SerializeReference] public Condition inConditionB;

            public AndCondition(Condition inConditionA, Condition inConditionB) {
                this.inConditionA = inConditionA;
                this.inConditionB = inConditionB;
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

                writer.WriteStartElement("Condition");
                writer.WriteAttributeString("type", Name);
                inConditionA.WriteXml(writer);
                inConditionB.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        [Serializable]
        public class OrCondition : Condition
        {
            public override string Name => "or";

            [SerializeReference] public Condition inConditionA;
            [SerializeReference] public Condition inConditionB;

            public OrCondition(Condition inConditionA, Condition inConditionB) {
                this.inConditionA = inConditionA;
                this.inConditionB = inConditionB;
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

                writer.WriteStartElement("Condition");
                writer.WriteAttributeString("type", Name);
                inConditionA.WriteXml(writer);
                inConditionB.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }
}