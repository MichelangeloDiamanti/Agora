
// Copyright (c) 2012 Henrik Johansson

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rapid.Tools
{

	public class GraphLinkList : IEnumerable<GraphGrid>
	{
		#region STATIC MEMBERS

		static int SortDescending(GraphGrid a, GraphGrid b){ return a.BoundsMin.y < b.BoundsMin.y ? -1 : 1; }

		#endregion

		public float ScaleTime { get; set; }
		public float MinTime { get; set; }
		public float MaxTime { get; set; }
		public float StartTime { get; set; }
		public float EndTime { get; set; }
		public List<GraphGrid> Grids { get; private set; }


		public GraphLinkList(GraphGrid a, GraphGrid b)
		{
			ScaleTime = 1f;
			MinTime = float.MaxValue;
			MaxTime = float.MinValue;

			Grids = new List<GraphGrid>(2);
			OnAdd(a);
			OnAdd(b);
			
			StartTime = MinTime;
			EndTime = MaxTime;
			
			a.SetTimeBounds(StartTime, EndTime);
			b.SetTimeBounds(StartTime, EndTime);
			
			float width = (EndTime - StartTime);
			ScaleTime = 1f / width;

			Grids.Sort(SortDescending);
		}


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator) GetEnumerator();
		}
		public IEnumerator<GraphGrid> GetEnumerator ()
		{
			return Grids.GetEnumerator();
		}

		public void Clear()
		{
			foreach(var g in Grids)
			{
				g.Link = null;
			}
			Grids.Clear();
		}

		void OnAdd(GraphGrid grid)
		{
			grid.Link = this;
			Grids.Add(grid);
			if(grid.Log.TimeStart < MinTime) MinTime = grid.Log.TimeStart;
			if(grid.Log.TimeLast > MaxTime) MaxTime = grid.Log.TimeLast;
		}

		public void CombineList(GraphLinkList other)
		{
			if(other == this) return;

			while(other.Grids.Count > 0)
			{
				var g = other.Grids[0];
				other.Grids.RemoveAt(0);
				Add (g);
			}
		}

		public void Add(GraphGrid grid)
		{
			grid.Link = this;

			int index = Grids.IndexOf(grid);
			if(index >= 0) return;

			OnAdd(grid);
			grid.SetTimeBounds(StartTime, EndTime);
			Grids.Sort(SortDescending);
		}
		public void Remove(GraphGrid grid)
		{
			int index = Grids.IndexOf(grid);
			if(index < 0)
				return;

			Grids[index].Link = null;
			Grids.RemoveAt(index);

			if(Grids.Count == 1)
			{
				Grids[0].UnLink();
				return;
			}
			if(Grids.Count == 0)
			{
				Grids = null;
				return;
			}
			UpdateBounds();
		}

		void UpdateBounds()
		{
			MinTime = float.MaxValue;
			MaxTime = float.MinValue;
			foreach(var g in Grids)
			{
				if(g.Log.TimeStart < MinTime) MinTime = g.Log.TimeStart;
				if(g.Log.TimeLast > MaxTime) MaxTime = g.Log.TimeLast;
			}

			StartTime = Math.Max(StartTime, MinTime);
			EndTime = Math.Min(EndTime, MaxTime);

			float width = (EndTime - StartTime);
			ScaleTime = 1f / width;

			UpdateGrids();
		}

		public void DrawLinks()
		{
			if(Grids.Count <= 0) return;

			var grid = Grids[0];
			Vector2 last = grid.LinkRect.center;
			grid.SetDrawColor(grid.BorderColor);
			grid.DrawSolidDisc(last, 6f);

			for(int i=1; i<Grids.Count; ++i)
			{
				grid = Grids[i];
				Vector2 p = grid.LinkRect.center;
				grid.SetDrawColor(grid.BorderColor);
				grid.DrawLine(last, p);
				grid.DrawSolidDisc(p, 6f);
				last = p;
			}
		}
		public void MoveX(float amount)
		{
			if(amount > 0f)
			{
				var x = Math.Min(EndTime + amount, MaxTime);
				var clampedMove = x - EndTime;
				EndTime = x;
				StartTime += clampedMove;
			}
			else
			{
				var x = Math.Max(StartTime + amount, MinTime);
				var clampedMove = x - StartTime;
				StartTime = x;
				EndTime += clampedMove;
			}

			UpdateGrids();
		}
		public void ZoomX(float amount, float relativeX)
		{
			float startDist = (StartTime - relativeX);
			float endDist = (EndTime - relativeX);

			StartTime = StartTime + startDist * ScaleTime * amount;
			EndTime = EndTime + endDist * ScaleTime * amount;

			// Don't allow negative zooming
			if (EndTime <= StartTime) EndTime = StartTime + 0.0001f;

			ClampTimeRange();

			UpdateGrids();
		}

		void ClampTimeRange()
		{
			if (StartTime < MinTime) StartTime = MinTime;
			if (EndTime > MaxTime) EndTime = MaxTime;

			float width = (EndTime - StartTime);
			ScaleTime = 1f / width;
		}

		public void UpdateGrids()
		{
			foreach(var g in Grids)
				g.SetTimeBounds(StartTime, EndTime);
		}
	};

}