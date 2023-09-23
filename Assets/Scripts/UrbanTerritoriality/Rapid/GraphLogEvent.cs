
// Copyright (c) 2012 Henrik Johansson

using System.IO;
using System.Text;
using UnityEngine;

namespace Rapid.Tools
{

	public struct GraphLogEvent
	{
		#region STATIC MEMBERS

		internal static readonly char[] Split = new[] { '|' };
		internal const string Separator = "|";

		#endregion


		public float Time;
		public string Message;
		public Color Color;


		public GraphLogEvent(float time, string message, Color color)
		{
			Time = time;
			Message = message;
			Color = color;
		}


		public void Write(StreamWriter writer)
		{
			writer.Write(Time);
			writer.Write(Separator);
			writer.WriteLine(Message);
			writer.Write(Separator);
			writer.Write(GraphLogStyle.SerializeRGB(Color));
		}

		public void Read(StreamReader reader)
		{
			Parse(reader.ReadLine());
		}

		public void Parse(string eventString)
		{
			var strs = eventString.Split(Split);
			float.TryParse(strs[1], out Time);
			Message = strs[2];
			Color = GraphLogStyle.TryParseColor32(strs[3]);
		}

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			str.Append(Time);
			str.Append(Separator);
			str.Append(Message);
			str.Append(Separator);
			str.Append(GraphLogStyle.SerializeRGB(Color));
			return str.ToString();
		}
	};

}