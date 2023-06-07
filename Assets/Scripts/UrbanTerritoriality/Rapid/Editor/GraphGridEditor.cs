
// Copyright (c) 2012 Henrik Johansson

using UnityEditor;
using UnityEngine;

namespace Rapid.Tools
{

	public class GraphGridEditor : GraphGrid
	{
		public GraphGridEditor()
		{
		}
		public GraphGridEditor(GraphLogBuffer log)
		{
			Log = log;
		}


		public override Color GetDrawColor(Color color)
		{
			return Handles.color;
		}
		public override void SetDrawColor(Color color)
		{
			Handles.color = color;
		}
		public override void DrawLine(Vector3 a, Vector3 b)
		{
            Handles.DrawLine(a, b);
		}
		public override void DrawSolidDisc(Vector3 position, float radius)
		{
			position.x = Mathf.Round(position.x);
			position.y = Mathf.Round(position.y);

			Handles.DrawSolidDisc(position, -Vector3.forward, radius);
		}
	};

}