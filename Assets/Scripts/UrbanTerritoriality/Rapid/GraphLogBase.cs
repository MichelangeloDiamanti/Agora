
// Copyright (c) 2012 Henrik Johansson

using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Rapid.Tools
{

	public enum LogTimeMode
	{
		Frames,
		TimeSinceStartup,
		RealTimeSinceStartup,
		TimeSinceLevelLoad,
		ActualTime
	};

	public enum LogGraphType
	{
		Default,
		EulerAngles,
		Space2D
	};

	public abstract class GraphLogBase
	{
		#region HELPERS

		static protected readonly char[] SplitSpace = new[] { ' ' };
		static protected readonly char[] SplitEquality = new[] { '=' };
		static protected readonly char[] SplitLabels = new[] { '|' };
		static protected readonly char[] SplitConstants = new [] { ';' };

		protected static string TryGetElement(string[] array, int index)
		{
			return array == null ? "" : index >= array.Length ? "" : array[index];
		}

		#endregion


		#region DEFAULTS

		public static string DefaultGroup = "generic";
		public static string DefaultName = "unassigned";

		#endregion


		protected GraphLogBase _link;
		protected float _timeStart;

		public string Group { get; protected set; }
		public string Name { get; protected set; }
		public bool Enabled { get; set; }
		public int GraphCount { get; protected set; }
		public string[] Labels { get; protected set; }
		public GraphLogConstant[] Constants { get; protected set; }
		public LogTimeMode TimeMode { get; internal set; }
		public DateTime TimeCreated { get; internal set; }
		public LogGraphType GraphMode { get; set; }
		public GraphLogStyle Style { get; set; }

		public GraphLogBase Link
		{
			get { return _link; }
			set
			{
				_link = value;
				if (_link != null && _link.Link == this)
					_link.Link = null;
			}
		}

		public virtual float TimeStart
		{
			get { return _timeStart; }
			protected set { _timeStart = value; }
		}
		public float TimeLast { get; protected set; }
		public float TimeRecorded { get { return TimeLast - TimeStart; } }

		public float CurrentTime
		{
			get
			{
				switch (TimeMode)
				{
					case LogTimeMode.Frames:
						return Time.frameCount;
					case LogTimeMode.TimeSinceStartup:
						return Time.time;
					case LogTimeMode.RealTimeSinceStartup:
						return Time.realtimeSinceStartup;
					case LogTimeMode.TimeSinceLevelLoad:
						return Time.timeSinceLevelLoad;
					case LogTimeMode.ActualTime:
						return (float)(System.DateTime.Now.Ticks / 10000000);
					default:
						return Time.time;
				}
			}
		}


		protected GraphLogBase()
		{
			Enabled = true;
			Group = DefaultGroup;
			Name = DefaultName;
			GraphCount = 1;
			Labels = null;
			Constants = null;
			_timeStart = 0f;
			Style = null;
			Link = null;

			TimeCreated = DateTime.Now;
		}
		protected GraphLogBase(string name, string group, int graphCount, string[] graphLabels, GraphLogConstant[] constants, LogTimeMode timeMode, GraphLogStyle style)
		{
			Enabled = true;
			Link = null;

			Group = group;
			Name = name;

			GraphCount = graphCount < 1 ? 1 : graphCount;
			Labels = graphLabels;
			Constants = constants;

			TimeMode = timeMode;

			_timeStart = CurrentTime;
			if(_timeStart<0f) _timeStart = 0f;

			Style = style;
			TimeCreated = DateTime.Now;
		}


		public string GetGraphLabel(int index)
		{
			if (Labels == null || index >= Labels.Length)
				return "";
			return Labels[index];
		}

		public virtual void Log(float value)
		{
			if (Link != null) Link.Log(value);
		}
		public virtual void Log(float value1, float value2)
		{
			if (Link != null) Link.Log(value1, value2);
		}
		public virtual void Log(float value1, float value2, float value3)
		{
			if (Link != null) Link.Log(value1, value2, value3);
		}
		public virtual void Log(float value1, float value2, float value3, float value4)
		{
			if (Link != null) Link.Log(value1, value2, value3, value4);
		}
		public virtual void Log(float value1, float value2, float value3, float value4, float value5)
		{
			if (Link != null) Link.Log(value1, value2, value3, value4, value5);
		}

		public void Log(Vector2 vec)
		{
			Log(vec.x, vec.y);
		}
		public void Log(Vector2 vec1, Vector2 vec2)
		{
			Log(vec1.x, vec1.y, vec2.x, vec2.y);
		}
		public void Log(Vector3 vec)
		{
			Log(vec.x, vec.y, vec.z);
		}
		public void Log(Vector4 vec)
		{
			Log(vec.x, vec.y, vec.z, vec.w);
		}
		public void Log(Quaternion quat)
		{
			var angles = quat.eulerAngles;
			Log(angles.x, angles.y, angles.z);
		}
		public void Log(Color color)
		{
			Log(color.r, color.g, color.b, color.a);
		}
		public virtual void Log(Color32 color)
		{
			Log(color.r, color.g, color.b, color.a);
		}
		public void Log(Rect rect)
		{
			Log(rect.xMin, rect.yMin, rect.xMax, rect.yMax);
		}

		public void Log(int value)
		{
			Log((float)value);
		}
		public void Log(int value1, int value2)
		{
			Log((float)value1, (float)value2);
		}
		public void Log(int value1, int value2, int value3)
		{
			Log((float)value1, (float)value2, (float)value3);
		}
		public void Log(int value1, int value2, int value3, int value4)
		{
			Log((float)value1, (float)value2, (float)value3, (float)value4);
		}
		public void Log(int value1, int value2, int value3, int value4, int value5)
		{
			Log((float)value1, (float)value2, (float)value3, (float)value4, (float)value5);
		}

		public void Log(byte value)
		{
			Log((float)value);
		}
		public void Log(byte value1, byte value2)
		{
			Log((float)value1, (float)value2);
		}
		public void Log(byte value1, byte value2, byte value3)
		{
			Log((float)value1, (float)value2, (float)value3);
		}
		public void Log(byte value1, byte value2, byte value3, byte value4)
		{
			Log((float)value1, (float)value2, (float)value3, (float)value4);
		}
		public void Log(byte value1, byte value2, byte value3, byte value4, byte value5)
		{
			Log((float)value1, (float)value2, (float)value3, (float)value4, (float)value5);
		}

		public void Log(bool value)
		{
			Log(value ? 1f : 0f);
		}
		public void Log(bool value1, bool value2)
		{
			Log(value1 ? 1f : 0f, value2 ? 1f : 0f);
		}
		public void Log(bool value1, bool value2, bool value3)
		{
			Log(value1 ? 1f : 0f, value2 ? 1f : 0f, value3 ? 1f : 0f);
		}
		public void Log(bool value1, bool value2, bool value3, bool value4)
		{
			Log(value1 ? 1f : 0f, value2 ? 1f : 0f, value3 ? 1f : 0f, value4 ? 1f : 0f);
		}
		public void Log(bool value1, bool value2, bool value3, bool value4, bool value5)
		{
			Log(value1 ? 1f : 0f, value2 ? 1f : 0f, value3 ? 1f : 0f, value4 ? 1f : 0f, value5 ? 1f : 0f);
		}

		public virtual void LogEvent(string eventMessage)
		{
			if(Link != null) Link.LogEvent(eventMessage);
		}
		public virtual void LogEvent(string eventMessage, Color color)
		{
			if (Link != null) Link.LogEvent(eventMessage, color);
		}


		public virtual void Write(float time, float value)
		{
			if(Link != null) Link.Write(time, value);
		}
		public virtual void Write(float time, float value1, float value2)
		{
			if(Link != null) Link.Write(time, value1, value2);
		}
		public virtual void Write(float time, float value1, float value2, float value3)
		{
			if(Link != null) Link.Write(time, value1, value2, value3);
		}
		public virtual void Write(float time, float value1, float value2, float value3, float value4)
		{
			if(Link != null) Link.Write(time, value1, value2, value3, value4);
		}
		public virtual void Write(float time, float value1, float value2, float value3, float value4, float value5)
		{
			if(Link != null) Link.Write(time, value1, value2, value3, value4, value5);
		}


		public virtual void Dispose()
		{
			if(Link != null) Link.Dispose();
			Labels = null;
		}


		public void ParseHeader(StreamReader stream)
		{
			ParseGroup(stream.ReadLine());

			ParseName(stream.ReadLine());

			ParseGraphCount(stream.ReadLine());

			ParseGraphLabels(stream.ReadLine());

			ParseGraphConstants(stream.ReadLine());

			ParseTimeMode(stream.ReadLine());

			Style = new GraphLogStyle(stream);
		}

		private void ParseName(string str)
		{
			if (string.IsNullOrEmpty(str)) return;
			Name = TryGetElement(str.Split(SplitEquality), 1);
			if (Name.Length <= 0) Name = DefaultName;
		}

		private void ParseGroup(string str)
		{
			if (string.IsNullOrEmpty(str)) return;
			Group = TryGetElement(str.Split(SplitEquality), 1);
			if (Group.Length <= 0) Group = DefaultGroup;
		}

		private void ParseGraphCount(string str)
		{
			if(string.IsNullOrEmpty(str))return ;
			int count = 0;
			if (int.TryParse(TryGetElement(str.Split(SplitEquality), 1), out count))
				GraphCount = count;
			else
				GraphCount = 1;
		}

		void ParseGraphLabels(string str)
		{
			if (string.IsNullOrEmpty(str)) return;

			var split = str.Split(SplitEquality);
			if(split.Length < 2) return;

			Labels = split[1].Split(SplitLabels);	
		}

		void ParseGraphConstants(string str)
		{
			if (string.IsNullOrEmpty(str)) return;

			var split = str.Split(SplitEquality);
			if(split.Length < 2) return;

			var consts = split[1].Split(SplitConstants);

			List<GraphLogConstant> constants = new List<GraphLogConstant>();
			for(int i=0; i<consts.Length; ++i)
			{
				if(string.IsNullOrEmpty(consts[i])) continue;
				constants.Add(new GraphLogConstant(consts[i]));
			}
			Constants = constants.ToArray();
		}

		void ParseTimeMode(string str)
		{
			if (string.IsNullOrEmpty(str)) return;

			string modestr = TryGetElement(str.Split(SplitEquality), 1);
			if (string.IsNullOrEmpty(modestr)) return;
			
			try {
				TimeMode = (LogTimeMode)System.Enum.Parse(typeof (LogTimeMode), modestr, true);
			}catch{}
		}

		public string LabelsString()
		{
			if (Labels == null) return "";
			return string.Join("|", Labels);
		}

		public string ConstantsString()
		{
			if(Constants == null) return "";

			string[] consts = new string[Constants.Length];
			for(int i=0; i<Constants.Length; ++i)
				consts[i] = Constants[i].ToString();

			return string.Join(";", consts);
		}

		public override string ToString()
		{
			var str = new StringBuilder();

			str.Append("group=");
			str.AppendLine(Group);

			str.Append("name=");
			str.AppendLine(Name);

			str.Append("graphs=");
			str.AppendLine(GraphCount.ToString());

			str.Append("labels=");
			str.AppendLine(LabelsString());

			str.Append("constants=");
			str.AppendLine(ConstantsString());

			str.Append("timeMode=");
			str.AppendLine(TimeMode.ToString());

			str.AppendLine(Style.ToString());

			return str.ToString();
		}

	};

}