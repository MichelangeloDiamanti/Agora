
// Copyright (c) 2012 Henrik Johansson

using System.IO;
using UnityEngine;

namespace Rapid.Tools
{

	public class GraphLogStream : GraphLogBase
	{
		#region CONSTANTS

		internal const char ValueSeparator = ' ';
		internal const char EventIdentifier = '#';

		#endregion


		public StreamWriter Stream { get; private set; }


		public GraphLogStream(string name, string group, int graphCount, string[] graphLabels, GraphLogConstant[] constants, LogTimeMode timeMode, GraphLogStyle style, string path, bool append)
			: base(name, group, graphCount, graphLabels, constants, timeMode, style)
		{
			Create(path, append);
		}


		public void Create(string path, bool append)
		{
			Stream = new StreamWriter(path, append);

			if (!append) WriteHeader();
		}

		public void WriteHeader()
		{
			Stream.Write(ToString());
		}

		public void WriteTime()
		{
			if(TimeMode == LogTimeMode.Frames)
			{
				TimeLast = CurrentTime;
				Stream.Write(Time.frameCount);
			}
			else
			{
				TimeLast = CurrentTime;
				Stream.Write(TimeLast);
			}
		}

		public override void Dispose()
		{
			base.Dispose();

			Stream.Flush();
			Stream.Close();
			Stream = null;
		}

		public override void Log(float value)
		{
			base.Log(value);

			if(!Enabled)return;

			WriteTime();
			Stream.Write(ValueSeparator);
			Stream.Write(value);
			Stream.WriteLine();
		}

		public override void Log(float value1, float value2)
		{
			base.Log(value1, value2);

			if (!Enabled) return;

			WriteTime();
			Stream.Write(ValueSeparator);
			Stream.Write(value1);
			Stream.Write(ValueSeparator);
			Stream.Write(value2);
			Stream.WriteLine();
		}

		public override void Log(float value1, float value2, float value3)
		{
			base.Log(value1, value2, value3);

			if (!Enabled) return;

			WriteTime();
			Stream.Write(ValueSeparator);
			Stream.Write(value1);
			Stream.Write(ValueSeparator);
			Stream.Write(value2);
			Stream.Write(ValueSeparator);
			Stream.Write(value3);
			Stream.WriteLine();
		}

		public override void Log(float value1, float value2, float value3, float value4)
		{
			base.Log(value1, value2, value3);

			if (!Enabled) return;

			WriteTime();
			Stream.Write(ValueSeparator);
			Stream.Write(value1);
			Stream.Write(ValueSeparator);
			Stream.Write(value2);
			Stream.Write(ValueSeparator);
			Stream.Write(value3);
			Stream.Write(ValueSeparator);
			Stream.Write(value4);
			Stream.WriteLine();
		}

		public override void Log(float value1, float value2, float value3, float value4, float value5)
		{
			base.Log(value1, value2, value3);

			if (!Enabled) return;

			WriteTime();
			Stream.Write(ValueSeparator);
			Stream.Write(value1);
			Stream.Write(ValueSeparator);
			Stream.Write(value2);
			Stream.Write(ValueSeparator);
			Stream.Write(value3);
			Stream.Write(ValueSeparator);
			Stream.Write(value4);
			Stream.Write(ValueSeparator);
			Stream.Write(value5);
			Stream.WriteLine();
		}

		public override void Log(Color32 color)
		{
			base.Log(color);

			if (!Enabled) return;

			WriteTime();
			Stream.Write(ValueSeparator);
			Stream.Write(color.r);
			Stream.Write(ValueSeparator);
			Stream.Write(color.g);
			Stream.Write(ValueSeparator);
			Stream.Write(color.b);
			Stream.Write(ValueSeparator);
			Stream.Write(color.a);
			Stream.WriteLine();
		}

		public override void LogEvent(string eventMessage)
		{
			base.LogEvent(eventMessage);

			Stream.Write(EventIdentifier);
			Stream.Write(GraphLogEvent.Separator);
			WriteTime();
			Stream.Write(GraphLogEvent.Separator);
			Stream.Write(eventMessage);
			Stream.Write(GraphLogEvent.Separator);
			Stream.Write(GraphLogStyle.SerializeRGB(Style.EventColor));
			Stream.WriteLine();
		}
		public override void LogEvent(string eventMessage, Color color)
		{
			base.LogEvent(eventMessage);

			Stream.Write(EventIdentifier);
			Stream.Write(GraphLogEvent.Separator);
			WriteTime();
			Stream.Write(GraphLogEvent.Separator);
			Stream.Write(eventMessage);
			Stream.Write(GraphLogEvent.Separator);
			Stream.Write(GraphLogStyle.SerializeRGB(color));
			Stream.WriteLine();
		}
	};

}