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
    namespace Math
    {
        [Serializable]
        public abstract class FloatGenerator : IXmlSerializable
        {
            private string name;

            public abstract string Name { get; }

            public abstract XmlSchema GetSchema();

            public abstract void ReadXml(XmlReader reader);

            public abstract void WriteXml(XmlWriter writer);
        }

        public enum GeneratorTypes
        {
            CONSTANT_FLOAT = 0,
            NORMAL_FLOAT = 1,
            UNIFORM_FLOAT = 2,
        }

        [Serializable]
        public class ConstFloatGenerator : FloatGenerator
        {
            public float value;

            public override string Name => "c";

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

                writer.WriteAttributeString("dist", Name);
                writer.WriteAttributeString("value", value.ToString(nfi));
            }
        }

        [Serializable]
        public class NormalFloatGenerator : FloatGenerator
        {
            public float mean;
            public float std;
            public float min;
            public float max;

            public override string Name => "n";

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

                writer.WriteAttributeString("dist", Name);
                writer.WriteAttributeString("mean", mean.ToString(nfi));
                writer.WriteAttributeString("std", std.ToString(nfi));
                writer.WriteAttributeString("min", min.ToString(nfi));
                writer.WriteAttributeString("max", max.ToString(nfi));
            }
        }

        [Serializable]
        public class UniformFloatGenerator : FloatGenerator
        {
            public float min;
            public float size;

            public override string Name => "u";

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

                writer.WriteAttributeString("dist", Name);
                writer.WriteAttributeString("min", min.ToString(nfi));
                writer.WriteAttributeString("size", size.ToString(nfi));
            }
        }
    }
}