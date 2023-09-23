
// Copyright (c) 2012 Henrik Johansson

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rapid.Tools
{

	public abstract class GraphGrid : IDisposable
	{
		public enum BorderAnchor : byte
		{
			Top=0,
			Bottom,
			Right,
			Left
		};


        #region MATH HELPERS

        protected static float Epsilon = Mathf.Epsilon;

		protected static bool IsZero(float v)
		{
			return v < Epsilon && v > -Epsilon;
		}
		protected static bool Approx(float v, float a)
		{
			return v < (a + Epsilon) && v > (a - Epsilon);
		}
		protected static bool Approx(float v, float a, float delta)
		{
			return v < (a + delta) && v > (a - delta);
		}
		protected static bool IsRound(float v)
		{
			return Approx(v, Mathf.Round(v), 0.001f);
		}

		protected static void LinearTo(ref float input, float target, float speed)
		{
			if (input > target)
			{
				input -= speed;
				if (input < target) input = target;
			}
			else if (input < target)
			{
				input += speed;
				if (input > target) input = target;
			}
		}

		private const int Kilos = 1000;
		private const int Mega = Kilos*1000;
		private const int Gigs = Mega*1000;

		protected static string FormatBigNumber(int number)
		{
			var abs = System.Math.Abs(number);
			if (abs >= Gigs)
			{
				int mega = ((number % Gigs) / 100000000);
				return ((int)(number / Gigs)) + (mega != 0 ? "."+mega : "") + " g ~";
			}
			if(abs >= Mega)
			{
				int kilo = ((number % Mega) / 100000);
				return ((int)(number / Mega)) + (kilo != 0 ? "."+kilo : "") + " m ~";
			}
			if (abs >= Kilos)
			{
				int remainder = (int) (number%Kilos) / 100;
				return ((int)(number / Kilos)) + (remainder!=0 ? "."+remainder : "") + " k ~";
			}
			return number.ToString();
		}

		private const float SecondsOnAMinute = 60;
		private const float SecondsOnAHour = (60 * 60);

		protected static string FormatTimeSpan(float seconds)
		{
			if (seconds >= SecondsOnAMinute)
			{
				if (seconds >= SecondsOnAHour)
				{
					int hours = Mathf.FloorToInt(seconds / SecondsOnAHour);
					int minutes = Mathf.FloorToInt((seconds % SecondsOnAHour) / SecondsOnAMinute);
					return minutes > 0 ? hours + "h " + minutes + "m" : hours+"h";
				}
				{
					int minutes = Mathf.FloorToInt(seconds / SecondsOnAMinute);
					if(minutes > 0)
					{
						int sec = Mathf.FloorToInt(seconds % SecondsOnAMinute);
						return sec > 0 ? minutes + "m " + sec + "s" : minutes + "m";
					}
				}
			}
			return seconds.ToString("0.00") + "s";
		}
		
		protected static string FormatTimeSpanShort(float seconds, string format)
		{
			if (seconds < SecondsOnAMinute)
				return seconds.ToString(format);

			var minutes = Mathf.FloorToInt(seconds / SecondsOnAMinute);
			if(minutes >= 1)
			{
				var sec = (seconds % SecondsOnAMinute);
				return minutes.ToString("00") + ":" + sec.ToString(format);
			}

			return seconds.ToString(format);
		}
		#endregion

		#region DEFAULTS

		internal const float TitleBarHeight = 24f;

		internal const float EventSpotRadius = 3f;
		internal const float HotSpotRadius = 8f;
		internal static Color HotSpotEdgeColor = new Color(0.15f, 0.15f, 0.15f, 0.75f);

		public static float[] DarkShade = new[] { 0.5f, 0.25f, 0.1f };
		public static float[] LightShade = new[] { 0.8f, 0.5f, 0.3f };

		public static Texture2D BlackSemiTransparent = null;

		#endregion


		// Used for auto layouting
		private GUIContent _tempContent = new GUIContent();

		private bool _invertY = true;
		private bool _useText = true;

		private int _columns = 0;
		private int _rows = 0;

		private float _startTime = 0f;
		private float _endTime = 1f;
		private float _scaleTime = 1f;
		
		public float MaxTimeDifference = 1f;

		private float _startY = 0f;
		private float _endY = 1f;
		private float _scaleY = 1f;

		public Vector2 BoundsMin { get; private set; }
		public Vector2 BoundsMax { get; private set; }
		public Vector2 GridMin { get; private set; }
		public Vector2 GridMax { get; private set; }
		private Vector2 _insetMin;
		private Vector2 _insetMax;
		public Vector2 InsetMin { get { return _insetMin; } set { _insetMin = value; UpdateArea(); } }
		public Vector2 InsetMax { get { return _insetMax; } set { _insetMax = value; UpdateArea(); } }

		public int SubDivisionsX;
		public int SubDivisionsY;

		public float RulerSize;
		public int FontSize;
		public float FieldWidth = 70f;

		private List<string> _rulerTextsX;
		private List<string> _rulerTextsY;
		private string _formatString = "0.00";
		private string _valuePostFix = "";

		public Color TitleColor;
		public Color BorderColor;
		public Color LineColor;
		public Color SubColor;

		public int Columns
		{
			get { return _columns; }
			set
			{
				if (value == _columns) return;
				_columns = value < 1 ? 1 : value;
				UpdateRulersX();
			}
		}
		public int Rows
		{
			get { return _rows; }
			set
			{
				if (value == _rows) return;
				_rows = value < 1 ? 1 : value;
				UpdateRulersY();
			}
		}

		public float StartTime
		{
			get { return _startTime; }
			set
			{
				if (Approx(value,_startTime)) return;
				_startTime = value;
				UpdateRulersX();
			}
		}
		public float EndTime
		{
			get { return _endTime; }
			set
			{
				if (Approx(value, _endTime)) return;
				_endTime = value;
				UpdateRulersX();
			}
		}
		public float StartY
		{
			get { return _startY; }
			set
			{
				if (Approx(value, _startY)) return;
				_startY = value;
				UpdateRulersY();
			}
		}
		public float EndY
		{
			get { return _endY; }
			set
			{
				if (Approx(value, _endY)) return;
				_endY = value;
				UpdateRulersY();
			}
		}
		
		public Vector2 GridSize
		{
			get { return GridMax - GridMin; }
		}
		public float GridWidth
		{
			get { return GridMax.x - GridMin.x; }
		}
		public float GridHeight
		{
			get { return GridMax.y - GridMin.y; }
		}

		public Vector2 BoundsSize
		{
			get { return BoundsMax - BoundsMin; }
		}
		public float BoundsWidth
		{
			get { return BoundsMax.x - BoundsMin.x; }
		}
		public float BoundsHeight
		{
			get { return BoundsMax.y - BoundsMin.y; }
		}
		public Vector2 Center
		{
			get { return BoundsMin + BoundsSize*0.5f; }
		}
		public float CenterX
		{
			get { return BoundsMin.x + BoundsWidth*0.5f; }
		}
		public float CenterY
		{
			get { return BoundsMin.y + BoundsHeight*0.5f; }
		}


		public string PostFixY
		{
			get { return _valuePostFix; }
			set
			{
				if (value == _valuePostFix) return;
				_valuePostFix = value;
				UpdateRulersY();
			}
		}

		private float _drawRoughness = 0.01f;
		/// <summary>
		/// Used to approximate line drawing (reduce lines drawn)
		/// 0 means no reduction, anything above 0.1 means heavy reduction
		/// </summary>
		public float DrawRoughness
		{
			get { return _drawRoughness; }
			set { _drawRoughness = value; }
			//set { _drawRoughness = Mathf.Clamp(value, 0f, 0.1f); }
		}

		/// <summary>
		/// The absolute comparison value used when reducing line drawing
		/// </summary>
		public float DrawDelta
		{
			get { return Mathf.Abs((_endY*2f) - _startY)*_drawRoughness; }
		}

		private GraphLogBuffer _log = null;
		public GraphLogBuffer Log
		{
			get { return _log; }
			set
			{
				_log = value;
				if (_log == null) return;
				SetGridColor(_log.Style.MainColor);
				SetTimeValueBounds(_log.TimeStart, Log.TimeLast, _log.Min, _log.Max);
			}
		}

		public bool Minimized { get; set; }
		private float _minimizeAnimator = 1f;
		public float HeightScale { get { return _minimizeAnimator; } }
		
		public float ZoomedPercentX
		{
			get { return Log == null ? 1f : (_endTime - _startTime) / Log.TimeRecorded; }
		}

		public Rect LinkRect
		{
			get { return new Rect(BoundsMin.x, BoundsMin.y - (TitleBarHeight + 2f), TitleBarHeight, TitleBarHeight); }
		}
		public Rect TitleBarRect
		{
			get { return new Rect(BoundsMin.x + (TitleBarHeight + 2f), BoundsMin.y - (TitleBarHeight + 2f), 100f, TitleBarHeight); }
		}

		public GraphLinkList Link { get; set; }

		public bool InvertY
		{
			get { return _invertY; }
			set { _invertY = value; }
		}
		public bool UseText
		{
			get { return _useText; }
			set { _useText = value; }
		}


		protected GraphGrid()
		{
			if(BlackSemiTransparent == null)
			{
				Color clr = new Color(0f,0f,0f,0.8f);
				BlackSemiTransparent = new Texture2D(2,2);
				BlackSemiTransparent.SetPixel(0,0,clr);
				BlackSemiTransparent.SetPixel(1,0,clr);
				BlackSemiTransparent.SetPixel(0,1,clr);
				BlackSemiTransparent.SetPixel(1,1,clr);
				BlackSemiTransparent.Apply();
			}

			Link = null;

			_minimizeAnimator = 1f;
			Minimized = false;

			_insetMin = _insetMax = Vector2.one * 2f;

			Columns = 10;
			Rows = 4;
			SubDivisionsX = 1;
			SubDivisionsY = 1;

			StartTime = 0f;
			StartY = 0f;
			EndTime = 10f;
			EndY = 1f;

			RulerSize = 4f;
			FontSize = 9;

			_rulerTextsX = new List<string>();
			_rulerTextsY = new List<string>();

			SetGridColor(Color.white);

			TitleColor = Color.white;

			UpdateRulersX();
			UpdateRulersY();
		}


		public void Dispose()
		{
			UnLink();
			Log = null;
		}

		public void UnLink()
		{
			if(Link == null) return;

			Link.Remove(this);
			Link = null;
		}
		
		#region COORDINATE HELPERS
		
		public bool Contains(Vector2 point)
		{
			return ContainsX(point.x) && ContainsY(point.y);
		}
		public bool ContainsX(float x)
		{
			return x <= BoundsMax.x && x >= BoundsMin.x;
		}
		public bool ContainsY(float y)
		{
			return y <= BoundsMax.y && y >= BoundsMin.y;
		}

		public float ToWorldX(float x)
		{
			return Mathf.Lerp(GridMin.x, GridMax.x, (x - _startTime) * _scaleTime);
		}
		public float ToWorldY(float y)
		{
			return _invertY ?
				Mathf.Lerp(GridMax.y, GridMin.y, (y - _startY) * _scaleY) :
				Mathf.Lerp(GridMin.y, GridMax.y, y * _scaleY);
		}
		public Vector2 ToWorld(float x, float y)
		{
			return new Vector2(ToWorldX(x), ToWorldY(y));
		}

		public float ToLocalX(float x)
		{
			float percent = Mathf.InverseLerp(GridMin.x, GridMax.x, x);
			return _startTime + (_endTime - _startTime) * percent;
		}
		public float ToLocalY(float y)
		{
			float percent = _invertY ? Mathf.InverseLerp(GridMax.y, GridMin.y, y) : Mathf.InverseLerp(GridMin.y, GridMax.y, y);
			return _startY + (_endY - _startY) * percent;
		}
		public Vector2 ToLocal(float x, float y)
		{
			return new Vector2(ToLocalX(x), ToLocalY(y));
		}

		public float ToLocalUnitsX(float x)
		{
			return (x / GridWidth) * (_endTime - _startTime);
		}
		public float ToLocalUnitsY(float y)
		{
			return (y / GridHeight) * (_endY - _startY);
		}

		public int GetColumn(float worldX)
		{
			float percent = Mathf.InverseLerp(GridMin.x, GridMax.x, worldX);
			return Mathf.RoundToInt(_columns * percent);
		}
		public int GetRow(float worldY)
		{
			float percent = _invertY ? Mathf.InverseLerp(GridMax.y, GridMin.y, worldY) : Mathf.InverseLerp(GridMin.y, GridMax.y, worldY);
			return Mathf.RoundToInt(_rows * percent);
		}

		#endregion

		protected void UpdateArea()
		{
			GridMin = BoundsMin + _insetMin;
			GridMax = BoundsMax - _insetMax;
		}

		public void SetGridColor(Color color)
		{
			TitleColor = BorderColor = LineColor = SubColor = color;
			BorderColor.a = DarkShade[0];
			LineColor.a = DarkShade[1];
			SubColor.a = DarkShade[2];
		}
		public void SetGridColor(Color color, float[] shade)
		{
			TitleColor = BorderColor = LineColor = SubColor = color;
			BorderColor.a = shade[0];
			LineColor.a = shade[1];
			SubColor.a = shade[2];
		}

		public void SetArea(Vector2 min, Vector2 max, float deltaTime)
		{
			LinearTo(ref _minimizeAnimator, Minimized ? 0f : 1f, deltaTime * 5f);
			max.y = Mathf.Lerp(min.y, max.y, _minimizeAnimator);
			SetArea(min, max);
		}
		public void SetArea(Vector2 min, Vector2 max)
		{
			BoundsMin = min;
			BoundsMax = max;
			GridMin = BoundsMin + _insetMin;
			GridMax = BoundsMax - _insetMax;
		}
		public void SetArea(Vector2 min, Vector2 max, Vector2 insetMin, Vector2 insetMax)
		{
			_insetMin = insetMin;
			_insetMax = insetMax;

			BoundsMin = min;
			BoundsMax = max;
			GridMin = BoundsMin + _insetMin;
			GridMax = BoundsMax - _insetMax;
		}

		public void SetTimeBounds(float startTime, float endTime)
		{
			if (Approx(startTime, _startTime) && Approx(endTime, _endTime)) return;
			_startTime = startTime;
			_endTime = endTime;
			UpdateRulersX();
		}
		public void SetTimeBounds(float startX, float endX, int columns)
		{
			if (Approx(startX, _startTime) && Approx(endX, _endTime) && columns == _columns) return;
			_startTime = startX;
			_endTime = endX;
			_columns = columns;
			UpdateRulersX();
		}

		public void SetValueBounds(float startY, float endY)
		{
			if (Approx(startY, _startY) && Approx(endY, _endY)) return;
			_startY = startY;
			_endY = endY;
			UpdateRulersY();
		}
		public void SetValueBounds(float startY, float endY, int rows)
		{
			if (Approx(startY, _startY) && Approx(endY, _endY) && rows==_rows) return;
			_startY = startY;
			_endY = endY;
			_rows = rows;
			UpdateRulersY();
		}

		public void SetTimeValueBounds(float startTime, float endTime, float startY, float endY)
		{
			_startTime = startTime;
			_endTime = endTime;
			UpdateRulersX();

			_startY = startY;
			_endY = endY;
			UpdateRulersY();
		}

		void UpdateRulersX()
		{
			float width = (_endTime - _startTime);
			_scaleTime = 1f / width;

			if(!_useText) return;

			var lx = Columns < 1 ? 1 : Columns;
			GetTimeInterval(_startTime, width / lx, lx, "0.00", _rulerTextsX);
		}

		void UpdateRulersY()
		{
			float height = (_endY - _startY);
			_scaleY = 1f / height;

			if(!_useText) return;

			var ly = Rows < 1 ? 1 : Rows;
			if(string.IsNullOrEmpty(_valuePostFix))
				GetNumberInterval(_startY, height / ly, ly, _formatString, _rulerTextsY);
			else
				GetNumberInterval(_startY, height / ly, ly, _formatString, _valuePostFix, _rulerTextsY);
		}

		public void MoveX(float amount)
		{
			if(Log == null)
			{
				_startTime += amount;
				_endTime += amount;
				return;
			}

			if(Link != null)
			{
				Link.MoveX(amount);
				return;
			}

			if(amount > 0f)
			{
				var x = Math.Min(_endTime + amount, Log.TimeLast);
				var clampedMove = x - _endTime;
				_endTime = x;
				_startTime += clampedMove;
			}
			else
			{
				var x = Math.Max(_startTime + amount, Log.TimeStart);
				var clampedMove = x - _startTime;
				_startTime = x;
				_endTime += clampedMove;
			}

			UpdateRulersX();
		}

		public void ZoomX(float amount, float relativeX)
		{
			if(IsZero(amount)) return;

			if(Link != null)
			{
				Link.ZoomX(amount, relativeX);
				return;
			}

			float startDist = (_startTime - relativeX);
			float endDist = (_endTime - relativeX);

			_startTime = _startTime + startDist * _scaleTime * amount;
			_endTime = _endTime + endDist * _scaleTime * amount;

			// Don't allow negative zooming
			if (_endTime <= _startTime) _endTime = _startTime + 0.0001f;

			ClampStartEndX();

			UpdateRulersX();
		}

		void ClampStartEndX()
		{
			if (Log == null)return;
			if (_startTime < Log.TimeStart) _startTime = Log.TimeStart;
			if (_endTime > Log.TimeLast) _endTime = Log.TimeLast;
		}
		
		public void MoveWorld(Vector2 move)
		{
			if(Minimized)
				return;
			
			if(System.Math.Abs(move.x) <= 0f) return;

			MoveX( ToLocalUnitsX(move.x) );
		}

		public void ZoomWorldX(float mouseX, float zoomX)
		{
			if(IsZero(zoomX)) return;
			var scale = System.Math.Max(ZoomedPercentX, 0.025f);
			ZoomX(zoomX * scale, ToLocalX(mouseX));
		}

		#region TEXT HELPERS

		public void GetNumberInterval(float start, float step, int num, string format, IList<string> result)
		{
			result = result ?? new List<string>(num);
			result.Clear();
			var f = start;
			for (int i = 0; i <= num; ++i, f += step)
			{
				if (IsRound(f))
					result.Add((Mathf.RoundToInt(f)).ToString());
				else
					result.Add(f.ToString(format));
			}
		}

		public void GetNumberInterval(float start, float step, int num, string format, string postfix, IList<string> result)
		{
			result = result ?? new List<string>(num);
			result.Clear();
			var f = start;
			for (int i = 0; i <= num; ++i, f += step)
			{
				if (IsRound(f))
					result.Add((Mathf.RoundToInt(f)).ToString() + postfix);
				else
					result.Add(f.ToString(format) + postfix);
			}
		}
		
		public void GetTimeInterval(float start, float step, int num, string format, IList<string> result)
		{
			result = result ?? new List<string>(num);
			result.Clear();
			var f = start;
			for (int i = 0; i <= num; ++i, f += step)
			{
				if(IsZero(f))
				{
					result.Add("0");
					continue;
				}

                //result.Add(FormatTimeSpanShort(f, "0.00"));
                result.Add(FormatTimeSpan(f));
            }
		}
		#endregion


		#region DRAWING
		
		public virtual bool Draw()
		{
			return Draw (Vector2.zero);
		}

		public virtual bool Draw(Vector2 mousePosition)
		{
			bool mouseActive = !Minimized && Contains(mousePosition);
			
			DrawGrid();
			
			if(_minimizeAnimator <= Epsilon)
			{
				return Log != null && DrawTitlebar();
			}

			bool close = DrawLog(mousePosition);

			if (mouseActive)
				DrawCrossHair(mousePosition, BorderColor * 0.9f);
			
			return close;
		}

		public virtual void DrawGrid(bool drawRulers = true)
		{
			if(BoundsHeight <= 0.1f || BoundsWidth <= 0.1f)
			{
				DrawLineRect(BoundsMin, BoundsMax, BorderColor);
				return;
			}

			Vector2 bl = new Vector2(BoundsMin.x + _insetMin.x, BoundsMax.y - _insetMin.y);
			Vector2 startPos = new Vector2(BoundsMin.x, BoundsMax.y);
			Vector2 size = BoundsSize - _insetMin - _insetMax;

			DrawSubGrid(bl, size, Columns, Rows, SubDivisionsX, SubDivisionsY, LineColor, SubColor);

			DrawLineRect(BoundsMin, BoundsMax, BorderColor);

			if(drawRulers)
			{
				DrawGridRulers(bl, startPos, size, StartTime, StartY, EndTime, EndY, Columns, Rows,
					RulerSize, _rulerTextsX, _rulerTextsY, FontSize, FieldWidth, BorderColor, TitleColor);
			}

			if(Log != null && Log.Constants != null)
			{
				Vector3 left = new Vector3(BoundsMin.x, 0f, 0f);
				Vector3 right = new Vector3(BoundsMax.x, 0f, 0f);

				for(int i=0; i<Log.Constants.Length; ++i)
				{
					var constant = Log.Constants[i];
					left.y = right.y = ToWorldY(constant.Value);

					SetDrawColor(constant.Color);
					DrawLine(left, right);

					// Draw constants on the right side of the grid
					DrawBorderElement(constant.Name, constant.Color, right.y, BorderAnchor.Right);
				}
			}
		}

		public void DrawCrossHair(Vector2 position, Color color)
		{
			SetDrawColor(color);

			Vector2 top = new Vector2(position.x, BoundsMax.y);
			Vector2 bottom = new Vector2(position.x, BoundsMin.y);
			Vector2 left = new Vector2(BoundsMin.x, position.y);
			Vector2 right = new Vector2(BoundsMax.x, position.y);

			DrawLine(top, bottom);
			DrawLine(left, right);


			var localX = ToLocalX(position.x);
			var localY = ToLocalY(position.y);


			var oldAlign = GUI.skin.box.alignment;
			var oldColor = GUI.skin.box.normal.textColor;
			var oldTex = GUI.skin.box.normal.background;
			GUI.skin.box.normal.background = BlackSemiTransparent;
			GUI.skin.box.normal.textColor = Color.white;

			Rect pos = new Rect();
			Vector2 size;

			Vector2 closestPoint = Vector2.zero;
			float closestDist = float.MaxValue;
			Color pcolor = Color.white;
			
			if(Log != null && Log.HasTimestamps && Log.HasGraphs)
			{
				int xindex = Log.SearchIndex(localX);
				if(xindex < 0) xindex = 0;

				var leftIndex = xindex;
				var rightIndex = xindex;

				var leftTime = Log.Timestamps[xindex];
				var rightTime = leftTime;

				if(leftTime > localX)
				{
					leftIndex = Math.Max(xindex - 1, 0);
					leftTime = Log.Timestamps[leftIndex];
				}
				else
				{
					rightIndex = Math.Min(xindex + 1, Log.Timestamps.Count-1);
					rightTime = Log.Timestamps[rightIndex];
				}

				var values = Log.Graphs;
				for (int i = 0; i < values.Length; ++i)
				{
					var c = Log.Style.GetColor(i);
					SetDrawColor(c);

					var arr = values[i];

					var leftVal = arr[leftIndex];
					var rightVal = arr[rightIndex];
					var leftpos = ToWorld(leftTime, leftVal);
					var rightpos = ToWorld(rightTime, rightVal);

					DrawSolidDisc(leftpos, 3f);
					DrawSolidDisc(rightpos, 3f);

					var invlerp = Mathf.InverseLerp(leftTime, rightTime, localX);
					Vector2 interp = Vector2.Lerp(leftpos, rightpos, invlerp);

					Vector2 distvec = interp - position;
					float distsqr = distvec.sqrMagnitude;
					if(distsqr < closestDist)
					{
						closestDist = distsqr;
						
						if(invlerp <= 0.25f)
							closestPoint = leftpos;
						else if(invlerp >= 0.75f)
							closestPoint = rightpos;
						else
							closestPoint = interp;

						pcolor = c;
					}
				}
			}

			if(closestDist <= (HotSpotRadius * HotSpotRadius))
			{
				left.y = closestPoint.y;
				localX = ToLocalX(closestPoint.x);
				localY = ToLocalY(closestPoint.y);
				position = closestPoint;

				SetDrawColor(pcolor);
				DrawLine(closestPoint, left);
				DrawSolidDisc(closestPoint, 6f);

				GUI.skin.box.normal.textColor = pcolor;
			}
			
			_tempContent.text = FormatTimeSpanShort(localX, "0.0000");
			size = GUI.skin.label.CalcSize(_tempContent) + new Vector2(4f, 4f);

            pos.width = size.x;
			pos.height = size.y;
			pos.x = position.x - size.x * 0.5f;
			pos.y = top.y;
			GUI.skin.box.alignment = TextAnchor.UpperCenter;
			GUI.Box(pos, _tempContent);
			
			_tempContent.text = localY.ToString(localY >= 1000f ? "0.00" : "0.0000");
			size = GUI.skin.label.CalcSize(_tempContent) + new Vector2(4f, 4f);
			pos.x = left.x - size.x;
			pos.y = position.y - size.y*0.5f;
			pos.width = size.x;
			pos.height = size.y;
			GUI.skin.box.alignment = TextAnchor.MiddleRight;
			GUI.Box(pos, _tempContent);

			
			GUI.skin.box.normal.background = oldTex;
			GUI.skin.box.alignment = oldAlign;
			GUI.skin.box.normal.textColor = oldColor;
		}

		/// <summary>
		/// Draws a border element.
		/// </summary>
		/// <param name='position'>
		/// World space position.
		/// </param>
		protected void DrawBorderElement(string text, Color textColor, float position, BorderAnchor side)
		{
			var oldAlign = GUI.skin.box.alignment;
			var oldColor = GUI.skin.box.normal.textColor;
			var oldTex = GUI.skin.box.normal.background;
			GUI.skin.box.normal.background = BlackSemiTransparent;
			GUI.skin.box.normal.textColor = textColor;

			_tempContent.text = text;
			Vector2 size = GUI.skin.label.CalcSize(_tempContent) + new Vector2(4f, 4f);

			TextAnchor anchor = TextAnchor.MiddleCenter;
			Vector2 pos = Center;

			switch(side)
			{
			case BorderAnchor.Top:
				anchor = TextAnchor.LowerCenter;
				pos = new Vector2(position - size.x * 0.5f, BoundsMin.y);
				break;
			case BorderAnchor.Bottom:
				anchor = TextAnchor.UpperCenter;
				pos = new Vector2(position - size.x * 0.5f, BoundsMax.y);
				break;
			case BorderAnchor.Right:
				anchor = TextAnchor.MiddleLeft;
				pos = new Vector2(BoundsMin.x, position - size.y * 0.5f);
				break;
			case BorderAnchor.Left:
				anchor = TextAnchor.MiddleRight;
				pos = new Vector2(BoundsMax.x, position - size.y * 0.5f);
				break;
			}

			GUI.skin.box.alignment = anchor;

			Rect rect = new Rect(0f,0f,0f,0f);
			rect.width = size.x;
			rect.height = size.y;
			rect.x = pos.x;
			rect.y = pos.y;
			GUI.Box(rect, _tempContent);

			GUI.skin.box.normal.background = oldTex;
			GUI.skin.box.alignment = oldAlign;
			GUI.skin.box.normal.textColor = oldColor;
		}

		public void DrawTexts(Vector2 position, Vector2 step, IList<string> texts, float fieldWidth, float fieldHeight, Color color)
		{
			Rect pos = new Rect(position.x, position.y, fieldWidth, fieldHeight);

			for (int i = 0; i < texts.Count; ++i)
			{
				DrawText(texts[i], pos);
				pos.x += step.x;
				pos.y += step.y;
			}
		}

		public void DrawLineRect(Vector2 min, Vector2 max, Color color)
		{
			SetDrawColor(color);
			Vector3 tl = new Vector3(min.x, max.y, 0f);
			Vector3 br = new Vector3(max.x, min.y, 0f);
			DrawLine(min, tl);
			DrawLine(tl, max);
			DrawLine(max, br);
			DrawLine(br, min);
		}

		public void DrawLinesInterval(Vector2 lineA, Vector2 lineB, Vector2 step, int lines, Color color)
		{
			Vector3 la = lineA;
			Vector3 lb = lineB;
			SetDrawColor(color);
			for (int i = 0; i < lines; ++i)
			{
				DrawLine(la, lb);
				la.x += step.x;
				la.y += step.y;
				lb.x += step.x;
				lb.y += step.y;
			}
		}

		public void DrawGridRulers(Vector2 bl, Vector2 startPos, Vector2 size,
									float startX, float startY, float endX, float endY, int columns, int rows,
									float rulerSize, IList<string> rulersX, IList<string> rulersY,
									int fontSize, float fieldWidth, Color color, Color textColor)
		{
			if (columns < 1) columns = 1;
			if (rows < 1) rows = 1;

			var oldAlign = GUI.skin.label.alignment;

			var oldFontSize = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = fontSize;
			var fieldHeight = fontSize * 2f;

			var oldColor = GUI.skin.label.normal.textColor;
			GUI.skin.label.normal.textColor = textColor;

			var move = new Vector2(0f, -(size.y / rows));
			var lineA = new Vector2(startPos.x, bl.y);
			var lineB = lineA;
			lineB.x -= rulerSize;
			DrawLinesInterval(lineA, lineB, move, rows+1, color);

			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			var offset = new Vector2(-fieldWidth, -fieldHeight * 0.5f);
			DrawTexts(lineB + offset, move, rulersY, fieldWidth, fieldHeight, color);

			move.x = size.x / columns;
			move.y = 0f;
			lineA = new Vector2(bl.x, startPos.y);
			lineB = lineA;
			lineB.y += rulerSize;
			DrawLinesInterval(lineA, lineB, move, columns+1, color);

			GUI.skin.label.alignment = TextAnchor.UpperCenter;
			offset = new Vector2(-fieldWidth * 0.5f, 0f);
			DrawTexts(lineB + offset, move, rulersX, fieldWidth, fieldHeight, color);

			GUI.skin.label.fontSize = oldFontSize;
			GUI.skin.label.alignment = oldAlign;
			GUI.skin.label.normal.textColor = oldColor;
		}

		public void DrawSubGrid(Vector2 bottomLeft, Vector2 size, int columns, int rows, int subColumns, int subRows, Color lineColor, Color subColor)
		{
			SetDrawColor(lineColor);

			++subColumns;
			++subRows;

			int stepsX = columns * subColumns;
			int stepsY = rows * subRows;
			var move = new Vector2(size.x / stepsX, size.y / stepsY);
			int subCounter = 0;

			var a = bottomLeft;
			var b = new Vector3(bottomLeft.x + size.x, bottomLeft.y, 0f);
			for (int y = 0; y < stepsY; ++y)
			{
				DrawLine(a, b);
				a.y -= move.y;
				b.y -= move.y;
				if (++subCounter >= subRows)
				{
					subCounter = 0;
					SetDrawColor(lineColor);
				}
				else
					SetDrawColor(subColor);
			}
			DrawLine(a, b);

			a = bottomLeft;
			b = new Vector3(bottomLeft.x, bottomLeft.y - size.y, 0f);
			subCounter = 0;
			for (int x = 0; x < stepsX; ++x)
			{
				DrawLine(a, b);
				a.x += move.x;
				b.x += move.x;
				if (++subCounter >= subColumns)
				{
					subCounter = 0;
					SetDrawColor(lineColor);
				}
				else
					SetDrawColor(subColor);
			}
			DrawLine(a, b);
		}

		#endregion


		#region DRAW LOGS

		public virtual bool DrawLog(Vector2 mousePosition)
		{
			if(Log == null) return false;

			bool close = DrawTitlebar();

			var timestamps = Log.Timestamps;
			if (timestamps != null && timestamps.Count >= 2)
			{
				if(Application.isPlaying)
					DrawGraphsRoughly(DrawDelta * 0.1f); // Use some (but snall) approximation when doing live logging
				else
					DrawGraphsRoughly(DrawDelta * ZoomedPercentX);
			}

			var events = Log.Events;
			if (events != null && events.Count > 0)
				DrawEvents(mousePosition);

			return close;
		}

		public bool DrawLinkButton(Vector2 mousePosition)
		{
			var container = LinkRect;
			GUI.Box(container, GUIContent.none);

			if(Link != null)
			{
				SetDrawColor(BorderColor);
				DrawSolidDisc(container.center, 3f);
			}

			return container.Contains(mousePosition);
		}

		public bool DrawTitlebar()
		{
			var oldColor = GUI.skin.label.normal.textColor;
			
			var container = TitleBarRect;
			if(Application.isPlaying)
				container.xMin = BoundsMin.x;

			Rect pos = container;
			pos.height -= 4f;
			pos.y += 2f;
			float limitX = BoundsMax.x - (36f + 20f);
			
			// CONTAINER
			container.width = BoundsMax.x - pos.x;
			GUI.Box(container, GUIContent.none);
			//

			// NAME & GROUP
			GUI.skin.label.normal.textColor = TitleColor;
			GUI.skin.label.fontStyle = FontStyle.Bold;
			_tempContent.text = Log.Name;
			pos.width = GUI.skin.label.CalcSize(_tempContent).x;
			GUI.Label(pos, _tempContent);
			pos.x += pos.width + 4f;
			GUI.skin.label.fontStyle = FontStyle.Normal;
			
			// LABELS
			if (Log.Labels != null)
			{
				for (int i = 0; i < Log.Labels.Length; ++i)
				{
					var label = Log.Labels[i];
					if (string.IsNullOrEmpty(label)) continue;

					_tempContent.text = label;
					pos.width = GUI.skin.label.CalcSize(_tempContent).x;

					GUI.skin.label.normal.textColor = Log.Style.GetColor(i);
					GUI.Label(pos, label);
					float smallSpacing = pos.width + 4f;
					pos.x += smallSpacing;
				}
			}

			GUI.skin.label.normal.textColor = oldColor;

			// CLOSE / MINIMIZE BUTTON
			var x = pos.x;
			pos.x = BoundsMax.x - 22f;
			pos.width = 20f;
			if (!Application.isPlaying)
			{
				if (GUI.Button(pos, "x"))
					return true;

				pos.x -= 24f;
			}
			if (GUI.Button(pos, Minimized ? "+" : "-"))
				Minimized = !Minimized;

			Rect dotRect = pos;
			dotRect.x = limitX - 10f;
			dotRect.width = 12f;

			// BUFFER & STREAM INFO
			pos.x = x + 24f;
			if (Log.Timestamps.Count > 0)
			{
				pos.width = 80f;

				if (pos.xMax > limitX)
				{
					GUI.Label(dotRect, "...");
					return false;
				}

				GUI.Label(pos, "Frames " + FormatBigNumber(Log.BufferSize));
				pos.x += 80f;

				if (pos.xMax > limitX)
				{
					GUI.Label(dotRect, "...");
					return false;
				}

				if (Application.isPlaying)
				{
					Rect bpos = pos;
					bpos.width = 20f;
					if (GUI.Button(bpos, "+")) Log.BufferSize = Log.BufferSize + 250;
					bpos.x += 20f;
					if (GUI.Button(bpos, "-")) Log.BufferSize = System.Math.Max(Log.BufferSize - 250, 100);

					pos.x += 80f;
				}
				else pos.x += 40f;

				if (pos.xMax > limitX)
				{
					GUI.Label(dotRect, "...");
					return false;
				}

				GUI.Label(pos, "Time " + FormatTimeSpan(Log.TimeRecorded));

				pos.x += pos.width + 20f;
				pos.width = 120f;
				if (pos.xMax > limitX)
				{
					if (Application.isPlaying) GUI.Label(dotRect, "...");
					return false;
				}

				if (Application.isPlaying)
				{
					if (GUI.Button(pos, Log.Enabled ? "Stop buffering" : "Continue buffering"))
						Log.Enabled = !Log.Enabled;

					pos.x += pos.width + 60f;
					if (pos.xMax > limitX)
					{
						GUI.Label(dotRect, "...");
						return false;
					}

					if (Log.Link != null)
					{
						var stream = Log.Link as GraphLogStream;

						GUI.Label(pos, "Streamed: " + FormatTimeSpan(stream.TimeRecorded));

						pos.x += pos.width;
						if (pos.xMax > limitX)
						{
							GUI.Label(dotRect, "...");
							return false;
						}

						if (GUI.Button(pos, stream.Enabled ? "Stop streaming" : "Continue streaming"))
							stream.Enabled = !stream.Enabled;
					}
				}
				else
				{
					// date


					// delete button
				}
			}
			//

			return false;
		}

		public void DrawGraphs()
		{
			var values = Log.Graphs;

			var timestamps = Log.Timestamps;
			float lastTime = 0f;

			Vector3 lastPos;
			Vector3 pos = Vector3.zero;
			Vector2 p;

			for (int i = 0; i < values.Length; ++i)
			{
				SetDrawColor(Log.Style.GetColor(i));

				var arr = values[i];
				lastTime = timestamps[0];
				lastPos = ToWorld(timestamps[0], arr[0]);

				for (int j = 1; j < arr.Count; ++j)
				{
					var value = arr[j];

					var time = timestamps[j];

					p = ToWorld(time, value);
					pos.x = p.x;
					pos.y = p.y;

					// Create a gap in the graph if timestamps differs to much
					if ((time - lastTime) < MaxTimeDifference)
					{
						DrawLine(pos, lastPos);
					}

					lastPos = pos;
					lastTime = time;
				}
			}
		}

		public void DrawGraphsRoughly(float minDelta)
		{
			float lastValue;
			bool skipped = false;

			float lastTime = 0f;

			Vector3 lastPos = Vector3.zero;
			Vector3 pos = Vector3.zero;
			Vector2 p;

			int startIndex = Log.SearchIndex(_startTime);
			if (startIndex < 1) startIndex = 1;
			else if (startIndex >= Log.Timestamps.Count) startIndex = Log.Timestamps.Count - 1;

			var timestamps = Log.Timestamps;
			var values = Log.Graphs;
			for (int i = 0; i < values.Length; ++i)
			{
				SetDrawColor(Log.Style.GetColor(i));

				var arr = values[i];
				lastValue = float.MaxValue;
				lastTime = timestamps[0];
				lastPos = ToWorld(timestamps[0], arr[0]);

				for (int j = startIndex; j < arr.Count; ++j)
				{
					var value = arr[j];

					var time = timestamps[j];

					if(time > _endTime) break;

					var timeDiff = (time - lastTime);

					if (timeDiff < MaxTimeDifference && Approx(value, lastValue, minDelta))
					{
						lastTime = time;
						skipped = true;
						continue;
					}

					if (skipped)
					{
						p = ToWorld(lastTime, lastValue);
						pos.x = p.x;
						pos.y = p.y;
						DrawLine(pos, lastPos);
						lastPos = pos;
						skipped = false;
					}

					p = ToWorld(time, value);
					pos.x = p.x;
					pos.y = p.y;

					// Create a gap in the graph if timestamps differs to much
					if (timeDiff < MaxTimeDifference)
					{
						DrawLine(pos, lastPos);
					}

					lastPos = pos;
					lastTime = time;
					lastValue = value;
				}

				if (skipped)
				{
					p = ToWorld(Log.TimeLast, lastValue);
					pos.x = p.x;
					pos.y = p.y;
					DrawLine(pos, lastPos);
				}
				skipped = false;
			}
		}

		public void DrawEvents(Vector2 mousePosition)
		{
			var events = Log.Events;
			float centerY = CenterY;

			Vector2 pos2d = Vector2.zero;
			Vector3 pos = Vector3.zero;
			pos2d.y = pos.y = centerY;

			Vector2 closestPos = Vector2.zero;
			GraphLogEvent closestEvent = new GraphLogEvent(0f, "", Color.white);
			float closestDist = float.MaxValue;

			for (int i = Log.StartEvent; i < events.Count; ++i)
			{
				var evt = events[i];

				if(evt.Time < _startTime) continue;
				if(evt.Time > _endTime) break;

				pos2d.x = pos.x = ToWorldX(evt.Time);

				var dist = (pos2d - mousePosition).sqrMagnitude;
				if (dist < closestDist)
				{
					closestDist = dist;
					closestEvent = evt;
					closestPos = pos2d;
				}

				SetDrawColor(evt.Color);
				DrawSolidDisc(pos, EventSpotRadius);
			}

			// Draw highlighted event
			if (closestDist < (HotSpotRadius * HotSpotRadius))
			{
				SetDrawColor(HotSpotEdgeColor);
				DrawSolidDisc(closestPos, HotSpotRadius + 3f);
				SetDrawColor(closestEvent.Color);
				DrawSolidDisc(closestPos, HotSpotRadius);
				
				var oldAlign = GUI.skin.box.alignment;
				var oldColor = GUI.skin.box.normal.textColor;
				var oldTex = GUI.skin.box.normal.background;
				GUI.skin.box.alignment = TextAnchor.MiddleLeft;
				GUI.skin.box.normal.background = BlackSemiTransparent;
				GUI.skin.box.normal.textColor = Color.white;

				var x = closestPos.x;
				var insetx = 4f;
				_tempContent.text = closestEvent.Message;
				var wid = GUI.skin.textField.CalcSize(_tempContent).x + insetx;
				if(x + wid > BoundsMax.x) x = BoundsMax.x - wid;

				var time = "@ " + FormatTimeSpanShort(closestEvent.Time, "0.0000");
				_tempContent.text = time;
				Rect rect = new Rect(x, closestPos.y - 52f, GUI.skin.label.CalcSize(_tempContent).x + insetx + 2f, 20f);
				GUI.Box(rect, time);

				rect.y += 20f;
				rect.width = wid - insetx + 2f;
				GUI.Box(rect, closestEvent.Message);

				GUI.skin.box.alignment = oldAlign;
				GUI.skin.box.normal.background = oldTex;
				GUI.skin.box.normal.textColor = oldColor;
			}
		}

		#endregion


		#region ABSTRACT METHODS

		public abstract Color GetDrawColor(Color color);

		public abstract void SetDrawColor(Color color);

		public abstract void DrawLine(Vector3 a, Vector3 b);

		public abstract void DrawSolidDisc(Vector3 position, float radius);

		public virtual void DrawText(string text, Rect position)
		{
			GUI.Label(position, text);
		}

		#endregion
	};

}
