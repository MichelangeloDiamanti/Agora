
// Copyright (c) 2012 Henrik Johansson

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Rapid.Tools
{

	public class GraphLogStyle
	{
		#region HELPERS

		public static char[] ColorSplit = new [] {';'};

		public static string SerializeRGB(Color32 color)
		{
			return color.r + ";" + color.g + ";" + color.b;
		}

		public static Color32 TryParseColor32(string str)
		{
			Color32 color;
			TryParseColor32(str, out color);
			return color;
		}

		public static bool TryParseColor32(string str, out Color32 color)
		{
			color = Color.white;
			if (string.IsNullOrEmpty(str)) return false;
			var vals = str.Split(ColorSplit);
			if (vals.Length < 3) return false;
			byte.TryParse(vals[0], out color.r);
			byte.TryParse(vals[1], out color.g);
			byte.TryParse(vals[2], out color.b);
			if (vals.Length > 3) byte.TryParse(vals[3], out color.a);
			return true;
		}

		#endregion


		public string Name { get; private set; }
		public Color MainColor { get; set; }
		public Color EventColor { get; set; }
		public Color[] Colors { get; set; }

		
		public GraphLogStyle(string name)
			: this(name, Color.white, Color.cyan, new []{Color.red, Color.green, Color.blue, Color.yellow, Color.magenta})
		{
		}
		public GraphLogStyle(string name, Color mainColor)
		{
			Name = string.IsNullOrEmpty(name) ? "undefined" : name;
			MainColor = mainColor;
			EventColor = Color.cyan;
			Colors = new []{Color.red, Color.green, Color.blue, Color.yellow, Color.magenta};
		}
		public GraphLogStyle(string name, Color mainColor, Color eventColor, Color[] colors)
		{
			Name = string.IsNullOrEmpty(name) ? "undefined" : name;
			MainColor = mainColor;
			EventColor = eventColor;
			Colors = colors;
		}
		public GraphLogStyle(StreamReader stream)
		{
			Name = "undefined";

			string styleString = stream.ReadLine();
			ParseString(styleString);
		}


		public Color GetColor(int index)
		{
			if (index >= Colors.Length) return Colors[index % Colors.Length];
			return Colors[index];
		}

		public string ReferenceString()
		{
			return "style=" + Name;
		}

		public void ParseString(string style)
		{
			var strings = style.Split(new [] {'='});

			Name = strings[0].Replace("style.", "");
			if (string.IsNullOrEmpty(Name)) Name = "undefined";

			strings = strings[1].Split(new[] {' '});

			MainColor = TryParseColor32(strings[0]);

			EventColor = TryParseColor32(strings[1]);

			int colrs = strings.Length;
			List<Color> colors = new List<Color>();
			for(int i=2; i<colrs; ++i)
			{
				if (string.IsNullOrEmpty(strings[i])) continue;
				Color32 color;
				if (TryParseColor32(strings[i], out color))
					colors.Add(color);
			}
			Colors = colors.ToArray();
		}
		public override string ToString()
		{
			var str = new System.Text.StringBuilder("style.");
			str.Append(Name);
			str.Append("=");

			str.Append(SerializeRGB(MainColor));
			str.Append(" ");

			str.Append(SerializeRGB(EventColor));
			str.Append(" ");

			foreach (var color in Colors)
			{
				str.Append(SerializeRGB(color));
				str.Append(" ");
			}

			return str.ToString();
		}
	};

}