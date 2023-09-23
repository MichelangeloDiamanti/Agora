
// Copyright (c) 2012 Henrik Johansson

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Rapid.Tools
{

	public class GraphLogBuffer : GraphLogBase
	{
		public enum BufferOrder
		{
			TimeThenValues,
			ValuesThenTime
		};

		#region HELPERS

		public void ShiftContents<T>(IList<T> list, int length)
		{
			for (int i = 0; i < length; ++i)
			{
				list[i] = list[i + 1];
			}
		}

		#endregion


		private int _bufferSize = 1;

		public List<float> Timestamps { get; protected set; }
		public List<GraphLogEvent> Events { get; protected set; }
		public List<float>[] Graphs { get; protected set; }
		public float Min { get; protected set; }
		public float Max { get; protected set; }
		public int BufferSize
		{
			get { return _bufferSize; }
			set
			{
				var prevSize = Timestamps != null ? Timestamps.Count : 0;
				_bufferSize = Math.Max(value, 1);

				if(prevSize > _bufferSize)
				{
					RemoveEntries(prevSize - _bufferSize);
				}
			}
		}

		public int StartEvent { get; protected set; }

		public override float TimeStart
		{
			get { return base.TimeStart; }
			protected set
			{
				base.TimeStart = value;
				UpdateStartEvent();
			}
		}

		public bool HasTimestamps
		{
			get { return Timestamps != null && Timestamps.Count>0; }
		}
		public bool HasGraphs
		{
			get { return Graphs != null && Graphs.Length>0 && Graphs[0].Count > 0; }
		}


		public GraphLogBuffer(string name, string group, int graphCount, string[] graphLabels, GraphLogConstant[] constants, LogTimeMode timeMode, GraphLogStyle style, int bufferSize)
			: base(name, group, graphCount, graphLabels, constants, timeMode, style)
		{
			graphCount = graphCount < 1 ? 1 : graphCount;
			BufferSize = bufferSize;
			Timestamps = new List<float>();
			Events = new List<GraphLogEvent>();
			InitializeBuffer(graphCount);
			Clear();
		}
		public GraphLogBuffer(StreamReader stream)
			: base()
		{
			Graphs = null;
			Timestamps = new List<float>();
			Events = new List<GraphLogEvent>();
			ReadLog(stream);
		}


		public override void Dispose()
		{
			base.Dispose();
			Timestamps = null;
			Graphs = null;
		}

		public void InitializeBuffer(int graphs)
		{
			Graphs = new List<float>[graphs];
			for (int i = 0; i < graphs; ++i)
				Graphs[i] = new List<float>();
		}

		public void Clear()
		{
			Min = Max = 0f;
			_timeStart = TimeLast = 0f;

			Timestamps.Clear();

			Events.Clear();
			StartEvent = 0;

			if (Graphs != null)
			{
				for (int i = 0; i < Graphs.Length; ++i)
					Graphs[i].Clear();
			}
		}

		public void ExpandMinMax(float value)
		{
			if (value > Max) Max = value;
			else if (value < Min) Min = value;
		}

		void AddValue(float val, IList<float> list, bool expandMinMax = true)
		{
			if(expandMinMax) ExpandMinMax(val);

			if(BufferSize <= 0 || list.Count < BufferSize)
				list.Add(val);
			else
			{
				int len = BufferSize - 1;
				ShiftContents(list, len);
				list[len] = val;
			}
		}

		void RemoveOldEvents()
		{
			int count = Events.Count;
			if (count < 1) return;

			while(count > 0 && Events[0].Time < TimeStart)
			{
				Events.RemoveAt(0);
				--count;
			}
		}

		void RemoveEntries(int count)
		{
			Timestamps.RemoveRange(0, count);
			for (int j = 0; j < GraphCount; ++j)
				Graphs[j].RemoveRange(0, count);
			TimeStart = Timestamps[0];
		}

		void AddTimestamp()
		{
			AddTimestamp(CurrentTime);
		}
		void AddTimestamp(float time)
		{
			AddValue(time, Timestamps, false);
			
			if (BufferSize > 0)
			{
				TimeStart = Timestamps[0];
				RemoveOldEvents();
			}
			else if (TimeStart < 0f)
				TimeStart = time;
			
			TimeLast = time;
		}

		void UpdateStartEvent()
		{
			var count = Events.Count-1;
			while (StartEvent < count && Events[StartEvent].Time < TimeStart)
			{
				++StartEvent;
			}
		}

		public float Sample(float time, int graphIndex)
		{
			if(graphIndex < GraphCount && graphIndex >= 0) return -1f;
			var index = SearchIndex(time);
			return index < 0 ? -1f : Graphs[graphIndex][index];
		}

		public float SampleClosestTime(float time)
		{
			var index = SearchIndex(time);
			return index >= 0 ? Timestamps[index] : -1f;
		}

		public int SearchIndex(float time)
		{
			int count = Timestamps.Count;

			if (time <= TimeStart) return 0;
			if (time >= TimeLast) return count - 1;

			if (count <= 2) return time > ((TimeLast-TimeStart)*0.5f) ? 1 : 0;

			int i = count/2;
			int lower = 0;
			int upper = count-1;
			int closest = -1;
			float closestDiff = float.MaxValue;
			while ((upper-lower) > 2)
			{
				float diff = Timestamps[i]-time;

				if(Math.Abs(diff) < closestDiff)
				{
					closest = i;
					closestDiff = Math.Abs(diff);
				}

				if(diff > 0f) upper = i;
				else lower = i;

				i = lower + ((upper - lower) / 2);
			}
			return closest;
		}

		public override void Log(float value)
		{
			base.Log(value);
			if(!Enabled) return;
			AddTimestamp();
			AddValue(value, Graphs[0]);
		}
		public override void Log(float value1, float value2)
		{
			base.Log(value1, value2);
			if (!Enabled) return;
			AddTimestamp();
			AddValue(value1, Graphs[0]);
			AddValue(value2, Graphs[1]);
		}
		public override void Log(float value1, float value2, float value3)
		{
			base.Log(value1, value2, value3);
			if (!Enabled) return;
			AddTimestamp();
			AddValue(value1, Graphs[0]);
			AddValue(value2, Graphs[1]);
			AddValue(value3, Graphs[2]);
		}
		public override void Log(float value1, float value2, float value3, float value4)
		{
			base.Log(value1, value2, value3, value4);
			if (!Enabled) return;
			AddTimestamp();
			AddValue(value1, Graphs[0]);
			AddValue(value2, Graphs[1]);
			AddValue(value3, Graphs[2]);
			AddValue(value4, Graphs[3]);
		}
		public override void Log(float value1, float value2, float value3, float value4, float value5)
		{
			base.Log(value1, value2, value3, value4, value5);
			if (!Enabled) return;
			AddTimestamp();
			AddValue(value1, Graphs[0]);
			AddValue(value2, Graphs[1]);
			AddValue(value3, Graphs[2]);
			AddValue(value4, Graphs[3]);
			AddValue(value5, Graphs[4]);
		}

		public override void LogEvent(string eventMessage)
		{
			base.LogEvent(eventMessage, Style.EventColor);

			var time = CurrentTime;
			TimeLast = time;
			Events.Add(new GraphLogEvent(time, eventMessage, Style.EventColor));
		}
		public override void LogEvent(string eventMessage, Color color)
		{
			base.LogEvent(eventMessage, color);

			var time = CurrentTime;
			TimeLast = time;
			Events.Add(new GraphLogEvent(time, eventMessage, color));
		}

		public override void Write (float time, float value)
		{
			base.Write (time, value);
			AddTimestamp(time);
			AddValue(value, Graphs[0]);
		}
		public override void Write (float time, float value1, float value2)
		{
			base.Write (time, value1, value2);
			AddTimestamp(time);
			AddValue(value1, Graphs[0]);
			AddValue(value2, Graphs[1]);
		}
		public override void Write (float time, float value1, float value2, float value3)
		{
			base.Write (time, value1, value2, value3);
			AddTimestamp(time);
			AddValue(value1, Graphs[0]);
			AddValue(value2, Graphs[1]);
			AddValue(value3, Graphs[2]);
		}
		public override void Write (float time, float value1, float value2, float value3, float value4)
		{
			base.Write (time, value1, value2, value3, value4);
			AddTimestamp(time);
			AddValue(value1, Graphs[0]);
			AddValue(value2, Graphs[1]);
			AddValue(value3, Graphs[2]);
			AddValue(value4, Graphs[3]);
		}
		public override void Write (float time, float value1, float value2, float value3, float value4, float value5)
		{
			base.Write (time, value1, value2, value3, value4, value5);
			AddTimestamp(time);
			AddValue(value1, Graphs[0]);
			AddValue(value2, Graphs[1]);
			AddValue(value3, Graphs[2]);
			AddValue(value4, Graphs[3]);
			AddValue(value5, Graphs[4]);
		}

		public void ReadStream(Stream stream, BufferOrder order, int graphs)
		{
			using(var r = new BinaryReader(stream))
			{
				ReadData(r, order, graphs);
			}
		}
		public void ReadData(BinaryReader reader, BufferOrder order, int graphs)
		{
			switch(order)
			{
			case BufferOrder.TimeThenValues:

				while(reader.BaseStream.CanRead)
				{
					AddTimestamp(reader.ReadSingle());
					for(int i=0; i<graphs; ++i)
						AddValue(reader.ReadSingle(), Graphs[i]);
				}

				break;

			case BufferOrder.ValuesThenTime:
				
				while(reader.BaseStream.CanRead)
				{
					for(int i=0; i<graphs; ++i)
						AddValue(reader.ReadSingle(), Graphs[i]);
					AddTimestamp(reader.ReadSingle());
				}

				break;
			}
		}

		public void ReadLog(StreamReader stream)
		{
			Clear();

			ParseHeader(stream);

			InitializeBuffer(GraphCount);

			string firstEntry = stream.ReadLine();

			if(string.IsNullOrEmpty(firstEntry)) return;

			ReadLine(firstEntry);

			try
			{
				if (firstEntry[0] == '#') _timeStart = Events[0].Time;
				else _timeStart = Timestamps[0];
			}catch
			{
				_timeStart = 0f;
			}

			// Read all values & events
			string line;
			while ((line = stream.ReadLine()) != null)
			{
				ReadLine(line);
			}

			BufferSize = Timestamps.Count;
		}

		public void ReadLine(string str)
		{
			if(string.IsNullOrEmpty(str)) return;

			// # is the identifier for events
			if(str[0] == '#')
			{
				ReadEvent(str);
			}
			else
			{
				ReadValues(str);
			}
		}

		public void ReadEvent(string line)
		{
			var values = line.Split(GraphLogEvent.Split);

			float time = 0f;
			if(float.TryParse(TryGetElement(values, 1), out time))
			{
				if (time > TimeLast) TimeLast = time;
			}

			Events.Add(new GraphLogEvent(time, TryGetElement(values, 2), GraphLogStyle.TryParseColor32(TryGetElement(values, 3))));
		}

		public void ReadValues(string line)
		{
			var values = line.Split(SplitSpace);
			
			float time = 0f;
			float.TryParse(TryGetElement(values, 0), out time);
			Timestamps.Add(time);

			int count = Math.Min(GraphCount, values.Length-1);
			for(int i=0; i < count; ++i)
			{
				float value;
				float.TryParse(values[i + 1], out value);
				Graphs[i].Add(value);
				if (value < Min) Min = value;
				else if (value > Max) Max = value;
			}
			if (time > TimeLast) TimeLast = time;
		}
	};
	
}