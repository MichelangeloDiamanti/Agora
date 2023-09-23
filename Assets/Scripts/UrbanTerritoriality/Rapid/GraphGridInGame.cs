
// Copyright (c) 2012 Henrik Johansson

using UnityEngine;

namespace Rapid.Tools
{

	public class GraphGridInGame : GraphGrid
	{
		public Camera RenderCamera;
		public Material RenderMaterial;
		public bool ScreenSpace = true;
		Color _lastColor = Color.white;


		public GraphGridInGame(Camera renderCamera, Material renderMaterial, bool screenSpace = true)
		{
			InvertY = false;
			UseText = false;

			RenderCamera = renderCamera;
			RenderMaterial = renderMaterial;
			ScreenSpace = screenSpace;
		}
		public GraphGridInGame(Camera renderCamera, Material renderMaterial, bool screenSpace, GraphLogBuffer log)
		{
			InvertY = false;
			UseText = false;

			RenderCamera = renderCamera;
			RenderMaterial = renderMaterial;
			ScreenSpace = screenSpace;
			Log = log;
		}


		public override bool Draw ()
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
				
				GL.Begin(GL.LINES);
				{
					DrawGrid(false);
					DrawLog(new Vector2(-99999f, -99999f));
				}
				GL.End();
			}
			GL.PopMatrix();

			// This value was used to signal if a graph was closed (returning true) by the user in the editor
			// but we won't make use of that in this case.
			return false;
		}

		public override bool DrawLog (Vector2 mousePosition)
		{
			if(Log == null) return false;

			var timestamps = Log.Timestamps;
			if (timestamps != null && timestamps.Count >= 2)
			{
				// Use some approximation when doing live logging
				DrawGraphsRoughly(DrawDelta * 0.1f);
			}
			
			var events = Log.Events;
			if (events != null && events.Count > 0)
				DrawEvents(mousePosition);

			return false;
		}
		
		public override Color GetDrawColor(Color color)
		{
			return _lastColor;
		}
		public override void SetDrawColor(Color color)
		{
			GL.Color(color);
			_lastColor = color;
		}
		public override void DrawLine(Vector3 a, Vector3 b)
		{
			GL.Vertex(a);
			GL.Vertex(b);
		}
		public override void DrawSolidDisc(Vector3 position, float radius)
		{
			GL.End();

			position.x = Mathf.Round(position.x);
			position.y = Mathf.Round(position.y);

			GL.Begin(GL.TRIANGLE_STRIP);
			{
				var len = CirclePoints20.Length-1;
				for(int i=0; i<len; ++i)
				{
					GL.Vertex(position);
					GL.Vertex(position + CirclePoints20[i]*radius);
					GL.Vertex(position + CirclePoints20[i+1]*radius);
				}
				GL.Vertex(position);
				GL.Vertex(position + CirclePoints20[len]*radius);
				GL.Vertex(position + CirclePoints20[0]*radius);
			}
			GL.End();


			GL.Begin(GL.LINES);
		}

		static readonly Vector3[] CirclePoints20 = new []
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(-0.309017f, 0.9510565f, 0f),
			new Vector3(-0.5877853f, 0.8090169f, 0f),
			new Vector3(-0.8090171f, 0.5877852f, 0f),
			new Vector3(-0.9510566f, 0.3090169f, 0f),
			new Vector3(-1f, -1.693467E-07f, 0f),
			new Vector3(-0.9510565f, -0.3090172f, 0f),
			new Vector3(-0.8090169f, -0.5877855f, 0f),
			new Vector3(-0.5877851f, -0.8090172f, 0f),
			new Vector3(-0.3090167f, -0.9510567f, 0f),
			new Vector3(3.294841E-07f, -1f, 0f),
			new Vector3(0.3090174f, -0.9510565f, 0f),
			new Vector3(0.5877857f, -0.8090169f, 0f),
			new Vector3(0.8090174f, -0.587785f, 0f),
			new Vector3(0.9510568f, -0.3090166f, 0f),
			new Vector3(1f, 4.796966E-07f, 0f),
			new Vector3(0.9510565f, 0.3090175f, 0f),
			new Vector3(0.8090168f, 0.5877858f, 0f),
			new Vector3(0.5877848f, 0.8090175f, 0f),
			new Vector3(0.3090164f, 0.9510568f, 0f),
		};
	};

}