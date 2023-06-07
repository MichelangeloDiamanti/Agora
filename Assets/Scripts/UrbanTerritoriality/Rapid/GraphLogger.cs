
// Copyright (c) 2012 Henrik Johansson

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Rapid.Tools
{

	public class GraphLogger
	{
		public enum FileIdMode
		{
			None,
			ByExactDate,
			ByDateMinute
		};


		#region STATIC MEMBERS

		public static string DefaultDumpDirectory = "Logs";

		public static FileIdMode DefaultIdMode = FileIdMode.ByExactDate;

        public static int DefaultBufferSizeGlobal = 500;

		public static string AppRoot
		{
			get
			{
				var root = Application.dataPath;
				if (Application.isEditor)
					return new DirectoryInfo(root).Parent.FullName + Path.DirectorySeparatorChar;
				if (Application.platform == RuntimePlatform.OSXPlayer)
					return new DirectoryInfo(root).Parent.Parent.FullName + Path.DirectorySeparatorChar;
				return root + Path.DirectorySeparatorChar;
			}
		}
		#endregion


		private Dictionary<string, GraphLogStyle> _styles; 
		private Dictionary<string, GraphLogBase> _logs;
		private List<GraphLogStream> _streams;
		private bool _bufferByDefault = true;
		private bool _streamByDefault = false;
		private string _dumpDirectory = DefaultDumpDirectory;
		private bool _appendAll = false;

		public Dictionary<string, GraphLogStyle> Styles { get { return _styles; } } 
		public Dictionary<string, GraphLogBase> Graphs { get { return _logs; } }
		public List<GraphLogStream> Streams { get { return _streams; } }
		public int DefaultBufferSize { get; set; }
		public string DumpDirectory { get { return _dumpDirectory; } }
		public LogTimeMode DefaultTimeMode { get; set; }
		public FileIdMode IdMode { get; set; }
		public GraphLogStyle DefaultStyle { get; set; }


		public GraphLogger()
			: this(true, false, DefaultDumpDirectory, true, true)
		{
		}
		public GraphLogger(bool streamToDiskByDefault)
			: this(streamToDiskByDefault, false, DefaultDumpDirectory)
		{
		}
		public GraphLogger(bool streamToDiskByDefault, bool appendByDefault, string dumpDirectory)
			: this(streamToDiskByDefault, appendByDefault, dumpDirectory, true, true)
		{
		}
		public GraphLogger(bool streamToDiskByDefault, bool appendByDefault, string dumpDirectory, bool bufferByDefault, bool warning)
		{
			if(warning) WarnUsage();

			DefaultBufferSize = DefaultBufferSizeGlobal;
			DefaultTimeMode = LogTimeMode.TimeSinceStartup;
			IdMode = DefaultIdMode;
			DefaultStyle = new GraphLogStyle("default");

			_bufferByDefault = bufferByDefault;
			_streamByDefault = streamToDiskByDefault;
			_appendAll = appendByDefault;

			_styles = new Dictionary<string, GraphLogStyle>();
			_streams = new List<GraphLogStream>();
			_logs = new Dictionary<string, GraphLogBase>();

			_styles.Add(DefaultStyle.Name, DefaultStyle);

			if (string.IsNullOrEmpty(dumpDirectory))
			{
				_dumpDirectory = AppRoot + DefaultDumpDirectory;
				if (!Directory.Exists(_dumpDirectory)) Directory.CreateDirectory(_dumpDirectory);
			}
			else if(!Directory.Exists(dumpDirectory))
			{
				_dumpDirectory = Path.Combine(AppRoot, dumpDirectory);
				if(!Directory.Exists(_dumpDirectory)) Directory.CreateDirectory(_dumpDirectory);
			}
			else
			{
				_dumpDirectory = dumpDirectory;
			}
		}


		public void Dispose()
		{
			foreach (var g in _logs)
			{
				g.Value.Dispose();
			}
			_logs.Clear();
			_streams.Clear();
		}

		void WarnUsage()
		{
			Debug.LogWarning("Graph logging is active, since it can hurt performance allways remember to remove any log calls when not in use.");
		}

		public void AddStyle(GraphLogStyle style)
		{
			if(_styles.ContainsKey(style.Name))
				throw new Exception("Can't add duplicate styles \"" + style.Name + "\".");
			_styles.Add(style.Name, style);
		}
		public GraphLogStyle GetStyle(string name)
		{
			GraphLogStyle s;
			_styles.TryGetValue(name, out s);
			return s;
		}
		
		public string GenerateFileName(string name, DateTime date)
		{
			switch(IdMode)
			{
			case FileIdMode.ByExactDate:
				return name + " (" + string.Format("{0:yyyy.MM.dd hh.mm.ss tt}", date).ToString() + ").graphlog";
			case FileIdMode.ByDateMinute:
				return name + " (" + string.Format("{0:yyyy.MM.dd hh.mm tt", date).ToString() + ").graphlog";
			}
			return name + ".graphlog";
		}
		public string GetGraphLogPath(string name, DateTime date)
		{
			return Path.Combine(_dumpDirectory, GenerateFileName(name, date));
		}

		
		public GraphLogBase CreateLog(string name, string[] graphLabels)
		{
			return CreateLog(name, "default", Math.Max(graphLabels.Length, 1), graphLabels, null);
		}
		public GraphLogBase CreateLog(string name, string[] graphLabels, string style)
		{
			return CreateLog(name, "default", Math.Max(graphLabels.Length, 1), graphLabels, null, style);
		}
		public GraphLogBase CreateLog(string name, string[] graphLabels, GraphLogConstant[] constants, string style)
		{
			return CreateLog(name, "default", Math.Max(graphLabels.Length, 1), graphLabels, constants, style);
		}
		public GraphLogBase CreateLog(string name, string group, string[] graphLabels, GraphLogConstant[] constants)
		{
			return CreateLog(name, group, Math.Max(graphLabels.Length, 1), graphLabels, constants);
		}
		public GraphLogBase CreateLog(string name, string group, string[] graphLabels, GraphLogConstant[] constants, string style)
		{
			return CreateLog(name, group, Math.Max(graphLabels.Length, 1), graphLabels, constants, style);
		}
		public GraphLogBase CreateLog(string name, string group, int graphs, string[] graphLabels, GraphLogConstant[] constants)
		{
			GraphLogBase log = null;

			if (_bufferByDefault)
				log = CreateLogBuffer(name, group, graphs, graphLabels, constants, DefaultBufferSize);

			if (_streamByDefault)
			{
				var streamed = CreateLogStream(name, group, graphs, graphLabels, constants, _appendAll);
				if (log != null) log.Link = streamed;
				else log = streamed;
			}
			return log;
		}
		public GraphLogBase CreateLog(string name, string group, int graphs, string[] graphLabels, GraphLogConstant[] constants, string style)
		{
			GraphLogBase log = null;

			var styleref = TryGetStyle(style);

			if (_bufferByDefault)
				log = CreateLogBuffer(name, group, graphs, graphLabels, constants, DefaultBufferSize, styleref);

			if (_streamByDefault)
			{
				var streamed = CreateLogStream(name, group, graphs, graphLabels, constants, _appendAll, styleref);
				if (log != null) log.Link = streamed;
				else log = streamed;
			}
			return log;
		}

		public GraphLogBase GetLog(string name, int intendedGraphs)
		{
			GraphLogBase log;
			if (!_logs.TryGetValue(name, out log))
				log = CreateLog(name, GraphLogBase.DefaultGroup, intendedGraphs, null, null);
			return log;
		}

		GraphLogBase GetLogXY(string name)
		{
			GraphLogBase log;
			if (!_logs.TryGetValue(name, out log))
				log = CreateLog(name, GraphLogBase.DefaultGroup, 2, new []{"x","y"}, null);
			return log;
		}
		GraphLogBase GetLogXYXY(string name)
		{
			GraphLogBase log;
			if (!_logs.TryGetValue(name, out log))
				log = CreateLog(name, GraphLogBase.DefaultGroup, 2, new[] { "x1", "y1", "x2", "y2" }, null);
			return log;
		}
		GraphLogBase GetLogXYZ(string name)
		{
			GraphLogBase log;
			if (!_logs.TryGetValue(name, out log))
				log = CreateLog(name, GraphLogBase.DefaultGroup, 3, new[] { "x", "y", "z" }, null);
			return log;
		}
		GraphLogBase GetLogXYZW(string name)
		{
			GraphLogBase log;
			if (!_logs.TryGetValue(name, out log))
				log = CreateLog(name, GraphLogBase.DefaultGroup, 4, new[] { "x", "y", "z", "w" }, null);
			return log;
		}
		GraphLogBase GetLogRGBA(string name)
		{
			GraphLogBase log;
			if (!_logs.TryGetValue(name, out log))
				log = CreateLog(name, GraphLogBase.DefaultGroup, 4, new[] { "r", "g", "b", "a" }, null);
			return log;
		}
		GraphLogBase GetLogRECT(string name)
		{
			GraphLogBase log;
			if (!_logs.TryGetValue(name, out log))
				log = CreateLog(name, GraphLogBase.DefaultGroup, 4, new[] { "left", "bottom", "right", "top" }, null);
			return log;
		}

		public GraphLogStyle TryGetStyle(string name)
		{
			GraphLogStyle s;
			return _styles.TryGetValue(name, out s) ? s : DefaultStyle;
		}
		
		GraphLogBuffer CreateLogBuffer(string name, string group, int graphs, string[] graphLabels, GraphLogConstant[] constants, int bufferSize)
		{
			return CreateLogBuffer(name, group, graphs, graphLabels, constants, bufferSize, TryGetStyle(group));
		}
		GraphLogBuffer CreateLogBuffer(string name, string group, int graphs, string[] graphLabels, GraphLogConstant[] constants, int bufferSize, GraphLogStyle style)
		{
			var log = new GraphLogBuffer(name, group, graphs, graphLabels, constants, DefaultTimeMode, style, bufferSize);
			_logs.Add(name, log);
			return log;
		}
		
		GraphLogStream CreateLogStream(string name, string group, int graphs, string[] graphLabels, GraphLogConstant[] constants, bool append)
		{
			return CreateLogStream(name, group, graphs, graphLabels, constants, append, TryGetStyle(group));
		}
		GraphLogStream CreateLogStream(string name, string group, int graphs, string[] graphLabels, GraphLogConstant[] constants, bool append, GraphLogStyle style)
		{
			var date = DateTime.Now;
			var log = new GraphLogStream(name, group, graphs, graphLabels, constants, DefaultTimeMode, style, GetGraphLogPath(name, date), append);
			log.TimeCreated = date;
			if (!_logs.ContainsKey(name)) _logs.Add(name, log);
			_streams.Add(log);
			return log;
		}


		public void LogEvent(string name, string message)
		{
			GetLog(name, 1).LogEvent(message);
		}
		public void LogEvent(string name, string message, Color color)
		{
			GetLog(name, 1).LogEvent(message, color);
		}

		public void Log(string name, float value)
		{
			GetLog(name, 1).Log(value);
		}
		public void Log(string name, float value1, float value2)
		{
			GetLog(name, 2).Log(value1, value2);
		}
		public void Log(string name, float value1, float value2, float value3)
		{
			GetLog(name, 3).Log(value1, value2, value3);
		}
		public void Log(string name, float value1, float value2, float value3, float value4)
		{
			GetLog(name, 4).Log(value1, value2, value3, value4);
		}
		public void Log(string name, float value1, float value2, float value3, float value4, float value5)
		{
			GetLog(name, 5).Log(value1, value2, value3, value4, value5);
		}
		
		public void Log(string name, byte value)
		{
			GetLog(name, 1).Log(value);
		}
		public void Log(string name, byte value1, byte value2)
		{
			GetLog(name, 2).Log(value1, value2);
		}
		public void Log(string name, byte value1, byte value2, byte value3)
		{
			GetLog(name, 3).Log(value1, value2, value3);
		}
		public void Log(string name, byte value1, byte value2, byte value3, byte value4)
		{
			GetLog(name, 4).Log(value1, value2, value3, value4);
		}
		public void Log(string name, byte value1, byte value2, byte value3, byte value4, byte value5)
		{
			GetLog(name, 5).Log(value1, value2, value3, value4, value5);
		}
		
		public void Log(string name, bool value)
		{
			GetLog(name, 1).Log(value ? 1f : 0f);
		}
		public void Log(string name, bool value1, bool value2)
		{
			GetLog(name, 2).Log(value1, value2);
		}
		public void Log(string name, bool value1, bool value2, bool value3)
		{
			GetLog(name, 3).Log(value1, value2, value3);
		}
		public void Log(string name, bool value1, bool value2, bool value3, bool value4)
		{
			GetLog(name, 4).Log(value1, value2, value3, value4);
		}
		public void Log(string name, bool value1, bool value2, bool value3, bool value4, bool value5)
		{
			GetLog(name, 5).Log(value1, value2, value3, value4, value5);
		}

		public void Log(string name, Vector2 vec)
		{
			GetLogXY(name).Log(vec.x, vec.y);
		}
		public void Log(string name, Vector2 vec1, Vector2 vec2)
		{
			GetLogXYXY(name).Log(vec1.x, vec1.y, vec2.x, vec2.y);
		}
		public void Log(string name, Vector3 vec)
		{
			GetLogXYZ(name).Log(vec.x, vec.y, vec.z);
		}
		public void Log(string name, Vector4 vec)
		{
			GetLogXYZW(name).Log(vec.x, vec.y, vec.z, vec.w);
		}
		public void Log(string name, Quaternion quat)
		{
			var angles = quat.eulerAngles;
			GetLogXYZ(name).Log(angles.x, angles.y, angles.z);
		}
		public void Log(string name, Color color)
		{
			GetLogRGBA(name).Log(color.r, color.g, color.b, color.a);
		}
		public void Log(string name, Color32 color)
		{
			GetLogRGBA(name).Log(color.r, color.g, color.b, color.a);
		}
		public void Log(string name, Rect rect)
		{
			GetLogRECT(name).Log(rect.xMin, rect.yMin, rect.xMax, rect.yMax);
		}


		public void Log<T>(string name, Vector2 vec, T value)
			where T : struct
		{
			GetLog(name, 3).Log(vec.x, vec.y, Convert(value));
		}
		public void Log<T>(string name, Vector2 vec, T value1, T value2)
			where T : struct
		{
			GetLog(name, 4).Log(vec.x, vec.y, Convert(value1), Convert(value2));
		}
		public void Log<T>(string name, Vector2 vec, T value1, T value2, T value3)
			where T : struct
		{
			GetLog(name, 5).Log(vec.x, vec.y, Convert(value1), Convert(value2), Convert(value3));
		}
		public void Log<T>(string name, Vector2 vec1, Vector2 vec2, T value)
			where T : struct
		{
			GetLog(name, 5).Log(vec1.x, vec1.y, vec2.x, vec2.y, Convert(value));
		}
		
		public void Log<T>(string name, Vector3 vec, T value)
			where T : struct
		{
			GetLog(name, 4).Log(vec.x, vec.y, vec.z, Convert(value));
		}
		public void Log<T>(string name, Vector3 vec, T value1, T value2)
			where T : struct
		{
			GetLog(name, 5).Log(vec.x, vec.y, vec.z, Convert(value1), Convert(value2));
		}
		
		public void Log<T>(string name, Quaternion quat, T value)
			where T : struct
		{
			Vector3 vec = quat.eulerAngles;
			GetLog(name, 4).Log(vec.x, vec.y, vec.z, Convert(value));
		}
		public void Log<T>(string name, Quaternion quat, T value1, T value2)
			where T : struct
		{
			Vector3 vec = quat.eulerAngles;
			GetLog(name, 5).Log(vec.x, vec.y, vec.z, Convert(value1), Convert(value2));
		}

		public void Log<T>(string name, Vector4 vec, T value)
			where T : struct
		{
			GetLog(name, 5).Log(vec.x, vec.y, vec.z, vec.w, Convert(value));
		}
		
		public void Log<T>(string name, Rect rect, T value)
			where T : struct
		{
			GetLog(name, 5).Log(rect.xMin, rect.yMin, rect.xMax, rect.yMax, Convert(value));
		}

		public void Log<T>(string name, Color color, T value)
			where T : struct
		{
			GetLog(name, 5).Log(color.r, color.g, color.b, color.a, Convert(value));
		}
		public void Log<T>(string name, Color32 color, T value)
			where T : struct
		{
			GetLog(name, 5).Log(color.r, color.g, color.b, color.a, Convert(value));
		}


		public void Log<T>(string name, T value)
			where T : struct
		{
			GetLog(name, 1).Log(Convert(value));
		}
		public void Log<T1, T2>(string name, T1 value1, T2 value2)
			where T1 : struct
			where T2 : struct
		{
			GetLog(name, 2).Log(Convert(value1), Convert(value2));
		}
		public void Log<T1, T2, T3>(string name, T1 value1, T2 value2, T3 value3)
			where T1 : struct
			where T2 : struct
			where T3 : struct
		{
			GetLog(name, 3).Log(Convert(value1), Convert(value2), Convert(value3));
		}
		public void Log<T1, T2, T3, T4>(string name, T1 value1, T2 value2, T3 value3, T4 value4)
			where T1 : struct
			where T2 : struct
			where T3 : struct
			where T4 : struct
		{
			GetLog(name, 4).Log(Convert(value1), Convert(value2), Convert(value3), Convert(value4));
		}
		public void Log<T1, T2, T3, T4, T5>(string name, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
			where T1 : struct
			where T2 : struct
			where T3 : struct
			where T4 : struct
			where T5 : struct
		{
			GetLog(name, 5).Log(Convert(value1), Convert(value2), Convert(value3), Convert(value4), Convert(value5));
		}

		public float Convert<T>(T value)
			where T : struct
		{
			return System.Convert.ToSingle(value);
		}
	};

}