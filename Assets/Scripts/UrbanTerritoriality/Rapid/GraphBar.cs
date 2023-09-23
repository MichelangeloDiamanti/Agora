
// Copyright (c) 2012 Henrik Johansson

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Rapid.Tools
{

	public class GraphBar
	{
		public enum Anchor : byte
		{
			Top=0,
			Bottom,
			Right,
			Left
		};

		#region STATIC MEMBERS

		public static float TendTo(float input, float target, float speed)
		{
			if(Mathf.Approximately(input, target)) return target;
			return input - ((input - target) * speed);
		}

		#endregion


		public Camera RenderCamera;
		public Material RenderMaterial;
		public bool ScreenSpace = true;

		Color _drawColor = Color.white;
		public Color MainColor;
		public Color SubColor;
		public Color OutlineColor;
		public Color OverflowColor = Color.red;
		public Color RealValueColor = Color.cyan;

		public Anchor AnchorMode = Anchor.Left;
		public Vector2 Position = Vector2.zero;
		public float Thickness = 20f;
		public float Tallness = 300f;

		bool _overflow = false;
		float _lastValue = 0f;
		float _currentValue = 0f;
		float _mainBar = 0f;
		float _subBar = 0f;
		float _smoothingK = 0f;
		float _subSmoothingK = 0f;


		public float Smoothing
		{
			get { return 1f / _smoothingK; }
			set { _smoothingK = 1f / value; }
		}
		public float SubSmoothing
		{
			get { return 1f / _subSmoothingK; }
			set { _subSmoothingK = 1f / value; }
		}

		public Vector2 EndPosition
		{
			get {
				var vertical = Vertical;
				return Position + new Vector2(
					vertical ? Position.x : (AnchorMode == Anchor.Left ? Position.x + Tallness : Position.x),
					vertical ? (AnchorMode == Anchor.Bottom ? Position.y : Position.y + Tallness) : Position.y
				);
			}
		}

		public bool Vertical
		{
			get { return AnchorMode == Anchor.Bottom || AnchorMode == Anchor.Top; }
		}
		float ConvertAnchoredX(float val)
		{
			return AnchorMode == Anchor.Left ? val : -val;
		}
		float ConvertAnchoredY(float val)
		{
			return AnchorMode == Anchor.Bottom ? val : -val;
		}
		Vector2 CurrentSize
		{
			get {
				var vertical = Vertical;
				return new Vector2(vertical ? Thickness : ConvertAnchoredX(Tallness), vertical ? ConvertAnchoredY(Tallness) : Thickness);
			}
		}


		public GraphBar(Camera camera, Material material) : this(camera, material, 0.05f, 0.5f, Color.white, Color.white*0.5f, Color.black)
		{
		}
		public GraphBar(Camera camera, Material material, float smoothing, float subSmoothing, Color mainColor, Color subColor, Color outlineColor)
		{
			RenderCamera = camera;
			RenderMaterial = material;
			Smoothing = smoothing;
			SubSmoothing = subSmoothing;
			MainColor = mainColor;
			SubColor = subColor;
			OutlineColor = outlineColor;
		}


		public void Update(float percent)
		{
			_overflow = percent > 1f;

			_lastValue = _currentValue;
			_currentValue = Mathf.Clamp01(percent);
			
			_mainBar = Mathf.Clamp01(_smoothingK > 0f ? TendTo(_mainBar, _currentValue, _smoothingK * Time.smoothDeltaTime) : _currentValue);
			_subBar = Mathf.Clamp01(_subSmoothingK > 0f ? TendTo(_subBar, _lastValue, _subSmoothingK * Time.smoothDeltaTime) : _lastValue);
		}

		public virtual void Draw()
		{
			GL.PushMatrix();
			{
				if(RenderMaterial != null)
					RenderMaterial.SetPass(0);
				
				if(ScreenSpace)
				{
					if(RenderCamera != null)
					{
						var rect = RenderCamera.pixelRect;
						GL.LoadPixelMatrix(rect.xMin, rect.xMax, rect.yMin, rect.yMax);
					}
					else
					{
						GL.LoadPixelMatrix();
					}
				}
				else
				{
					//GL.LoadIdentity();
					//GL.MultMatrix(ObjectTransform.localToWorldMatrix);
				}
				
				DrawBars();
			}
			GL.PopMatrix();
		}
		
		void SetColor(Color color)
		{
			GL.Color(color);
			_drawColor = color;
		}

		void DrawBars()
		{
			var vertical = Vertical;
			var size = CurrentSize;
			Vector2 barsize;
			Vector3 v1, v2, v3, v4;

			if(!_overflow)
			{
				barsize = size;
				if(vertical) barsize.y *= _subBar;
				else barsize.x *= _subBar;
				GetQuad(Position, barsize, vertical, out v1, out v2, out v3, out v4);
				SetColor(SubColor);
				DrawBar(v1, v2, v3, v4);
			}

			barsize = size;
			if(vertical) barsize.y *= _mainBar;
			else barsize.x *= _mainBar;
			GetQuad(Position, barsize, vertical, out v1, out v2, out v3, out v4);
			SetColor(_overflow ? OverflowColor : MainColor);
			DrawBar(v1, v2, v3, v4);
			
			barsize = size;
			if(vertical) barsize.y *= _currentValue;
			else barsize.x *= _currentValue;
			GetQuad(Position, barsize, vertical, out v1, out v2, out v3, out v4);
			DrawLine(v2, v3, RealValueColor);

			GetQuad(Position, size, vertical, out v1, out v2, out v3, out v4);
			SetColor(OutlineColor);
			DrawOutlines(v1, v2, v3, v4);
		}

		void DrawLine(Vector3 a, Vector3 b, Color color)
		{
			GL.Begin(GL.LINES);
			{
				GL.Color(color);
				GL.Vertex(a);
				GL.Vertex(b);
			}
			GL.End();
		}

		void DrawOutlines(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
		{
			GL.Begin(GL.LINES);
			{
				GL.Color(_drawColor);
				GL.Vertex(v1);
				GL.Vertex(v2);
				GL.Vertex(v2);
				GL.Vertex(v3);
				GL.Vertex(v3);
				GL.Vertex(v4);
				GL.Vertex(v4);
				GL.Vertex(v1);
			}
			GL.End();
		}

		void DrawBar(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
		{
			GL.Begin(GL.TRIANGLES);
			{
				GL.Color(_drawColor);

				GL.Vertex(v1);
				GL.Vertex(v3);
				GL.Vertex(v2);
				
				GL.Vertex(v3);
				GL.Vertex(v1);
				GL.Vertex(v4);

				// back side

				GL.Vertex(v1);
				GL.Vertex(v2);
				GL.Vertex(v3);

				GL.Vertex(v3);
				GL.Vertex(v4);
				GL.Vertex(v1);
			}
			GL.End();
		}

		void GetQuad(Vector2 position, Vector2 size, bool vertical, out Vector3 v1, out Vector3 v2, out Vector3 v3, out Vector3 v4)
		{
			var half = vertical ? size.x * 0.5f : size.y * 0.5f;
			var v = Vector3.zero;

			if(vertical)
			{
				v.x = position.x - half;
				v.y = position.y;
				v1 = v;

				v.y = position.y + size.y;
				v2 = v;

				v.x = position.x + half;
				v3 = v;

				v.y = position.y;
				v4 = v;
			}
			else
			{
				v.x = position.x;
				v.y = position.y - half;
				v1 = v;
				
				v.x = position.x + size.x;
				v2 = v;

				v.y = position.y + half;
				v3 = v;
				
				v.x = position.x;
				v4 = v;
			}
		}
	};

}