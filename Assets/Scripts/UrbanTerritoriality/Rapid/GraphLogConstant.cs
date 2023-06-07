
// Copyright (c) 2012 Henrik Johansson

using System.IO;
using System.Text;
using UnityEngine;

namespace Rapid.Tools
{

	public struct GraphLogConstant
	{
		public string Name;
		public float Value;
		public Color Color;

		
		public GraphLogConstant(string name, float value, Color color)
		{
			Name = name;
			Value = value;
			Color = color;
		}
		public GraphLogConstant(string str)
		{
			Name = "";
			Value = 0f;
			Color = Color.white;
			Parse(str);
		}

		
		public void Write(StreamWriter writer)
		{
			writer.WriteLine(Name);
			writer.Write(GraphLogEvent.Separator);
			writer.Write(Value);
			writer.Write(GraphLogEvent.Separator);
			writer.Write(GraphLogStyle.SerializeRGB(Color));
		}
		
		public void Parse(string str)
		{
			var strs = str.Split(GraphLogEvent.Split);
			Name = strs[0];
			float.TryParse(strs[1], out Value);
			Color = GraphLogStyle.TryParseColor32(strs[2]);
		}

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			str.Append(Name);
			str.Append(GraphLogEvent.Separator);
			str.Append(Value);
			str.Append(GraphLogEvent.Separator);
			str.Append(GraphLogStyle.SerializeRGB(Color));
			return str.ToString();
		}
	};

}