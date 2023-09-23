
// Copyright (c) 2012 Henrik Johansson

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Rapid.Tools
{
	
	public static class Graph
	{
		public static GraphLogger Instance = null;


		public static void Initialize()
		{
			if (Instance != null) return;

			Instance = new GraphLogger();
		}
		public static void Initialize(bool warnMe)
		{
			if (Instance != null) return;

			Instance = new GraphLogger(true, false, "Logs", true, warnMe);
		}
		public static void Initialize(string dumpDirectory)
		{
			if (Instance != null) return;

			Instance = new GraphLogger(true, false, dumpDirectory);
		}
		public static void Initialize(string dumpDirectory, bool warnMe)
		{
			if (Instance != null) return;

			Instance = new GraphLogger(true, false, dumpDirectory, true, warnMe);
		}

		public static void Dispose()
		{
			if(Instance == null) return;

			Instance.Dispose();
			Instance = null;

			if(GraphGrid.BlackSemiTransparent != null)
				UnityEngine.Texture2D.Destroy(GraphGrid.BlackSemiTransparent);
			GraphGrid.BlackSemiTransparent = null;
		}
		
		public static void LogEvent(string name, string message)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.LogEvent(name, message);
		}
		public static void LogEvent(string name, string message, Color color)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.LogEvent(name, message, color);
		}

		public static void Log(string name, float value)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value);
		}
		public static void Log(string name, float value1, float value2)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2);
		}
		public static void Log(string name, float value1, float value2, float value3)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3);
		}
		public static void Log(string name, float value1, float value2, float value3, float value4)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3, value4);
		}
		public static void Log(string name, float value1, float value2, float value3, float value4, float value5)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3, value4, value5);
		}
		
		public static void Log(string name, int value)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value);
		}
		public static void Log(string name, int value1, int value2)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2);
		}
		public static void Log(string name, int value1, int value2, int value3)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3);
		}
		public static void Log(string name, int value1, int value2, int value3, int value4)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3, value4);
		}
		public static void Log(string name, int value1, int value2, int value3, int value4, int value5)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3, value4, value5);
		}

		public static void Log(string name, byte value1, byte value2)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2);
		}
		public static void Log(string name, byte value1, byte value2, byte value3)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3);
		}
		public static void Log(string name, byte value1, byte value2, byte value3, byte value4)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3, value4);
		}
		public static void Log(string name, byte value1, byte value2, byte value3, byte value4, byte value5)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3, value4, value5);
		}

		public static void Log(string name, bool value1, bool value2)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2);
		}
		public static void Log(string name, bool value1, bool value2, bool value3)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3);
		}
		public static void Log(string name, bool value1, bool value2, bool value3, bool value4)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3, value4);
		}
		public static void Log(string name, bool value1, bool value2, bool value3, bool value4, bool value5)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3, value4, value5);
		}

		public static void Log(string name, Vector2 vec)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec);
		}
		public static void Log(string name, Vector2 vec1, Vector2 vec2)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec1, vec2);
		}
		public static void Log(string name, Vector3 vec)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec);
		}
		public static void Log(string name, Vector4 vec)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec);
		}
		public static void Log(string name, Quaternion quat)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, quat);
		}
		public static void Log(string name, Color color)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, color);
		}
		public static void Log(string name, Color32 color)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, color);
		}
		public static void Log(string name, Rect rect)
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, rect);
		}


		public static void Log<T>(string name, Vector2 vec, T value)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec, value);
		}
		public static void Log<T>(string name, Vector2 vec, T value1, T value2)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec, value1, value2);
		}
		public static void Log<T>(string name, Vector2 vec, T value1, T value2, T value3)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec, value1, value2, value3);
		}
		public static void Log<T>(string name, Vector2 vec1, Vector2 vec2, T value)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec1, vec2, value);
		}
		public static void Log<T>(string name, Vector3 vec, T value)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec, value);
		}
		public static void Log<T>(string name, Vector3 vec, T value1, T value2)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec, value1, value2);
		}
		public static void Log<T>(string name, Quaternion quat, T value)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, quat, value);
		}
		public static void Log<T>(string name, Quaternion quat, T value1, T value2)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, quat, value1, value2);
		}
		public static void Log<T>(string name, Vector4 vec, T value)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, vec, value);
		}
		public static void Log<T>(string name, Rect rect, T value)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, rect, value);
		}
		public static void Log<T>(string name, Color color, T value)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, color, value);
		}
		public static void Log<T>(string name, Color32 color, T value)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, color, value);
		}


		public static void Log<T>(string name, T value)
			where T : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value);
		}
		public static void Log<T1, T2>(string name, T1 value1, T2 value2)
			where T1 : struct
			where T2 : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2);
		}
		public static void Log<T1, T2, T3>(string name, T1 value1, T2 value2, T3 value3)
			where T1 : struct
			where T2 : struct
			where T3 : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3);
		}
		public static void Log<T1, T2, T3, T4>(string name, T1 value1, T2 value2, T3 value3, T4 value4)
			where T1 : struct
			where T2 : struct
			where T3 : struct
			where T4 : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3, value4);
		}
		public static void Log<T1, T2, T3, T4, T5>(string name, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
			where T1 : struct
			where T2 : struct
			where T3 : struct
			where T4 : struct
			where T5 : struct
		{
			if (Instance == null) Instance = new GraphLogger();
			Instance.Log(name, value1, value2, value3, value4, value5);
		}
	};

}