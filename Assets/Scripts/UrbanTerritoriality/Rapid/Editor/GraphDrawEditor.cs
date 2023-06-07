
// Copyright (c) 2012 Henrik Johansson

using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rapid.Tools
{

	public class GraphDrawEditor : EditorWindow
	{
		#region STATIC MEMBERS

		[MenuItem("Window/Graph Debugger %g")]
		static void OpenGraphViewer()
		{
			EditorWindow.GetWindow(typeof(GraphDrawEditor));
		}

		#endregion
		

		readonly GUILayoutOption[] LargeButton = new[] { GUILayout.MinWidth(70f), GUILayout.MaxWidth(120f), GUILayout.MinHeight(40f) };
		readonly GUILayoutOption[] SmallButton = new[] { GUILayout.Width(70f) };
		readonly GUILayoutOption[] FileLabel = new[] { GUILayout.MinWidth(280f), GUILayout.ExpandWidth(false), GUILayout.Height(22f) };
		readonly GUILayoutOption[] ToolBoxParams = new[] { GUILayout.MaxHeight(50f) };
		readonly GUILayoutOption[] FileBoxParams = new[] { GUILayout.MaxHeight(120f) };

		int ColumnSize = 10;
		float GridInsetX = 60f;
		float GridInsetY = 40f;
		float SpacingY = 56f;

		private float _timeDelta = 0f;
		private float _lastRealTime = 0f;

		private bool _wasCompiling = false;
		private bool _wasPlaying = false;
		private bool _initialized = false;
		private string _lastDumpDirectory = "Logs";
		private bool _loadedFileList = false;
		private List<FileInfo> _fileList = new List<FileInfo>();
		private List<string> _fileNameList = new List<string>(); 
		private Vector2 _fileListScrollPos = Vector2.zero;
		private List<GraphLogBuffer> _loadedGraphs = new List<GraphLogBuffer>();
		private List<GraphGridEditor> _grids = new List<GraphGridEditor>();

		private bool _globalLink = false;
		private GraphGridEditor _dragLink = null;
		private GraphGridEditor _dragGrid = null;
		private Vector2 _gridSize = new Vector2(1000f, 160f);
		private int _columns = 10;
		private int _rows = 4;
		private float _scale = 1f;

		bool _showOptions = true;

		/*
		public List<GraphLogBuffer> LoadedGraphs
		{
			get { return _loadedGraphs; }
		}
		*/
		public List<GraphGridEditor> ActiveGrids
		{
			get { return _grids; }
		}

		Vector2 GridInset
		{
			get { return new Vector2(GridInsetX, GridInsetY); }
		}
		float LimitY
		{
			get { return position.height + _gridSize.y + SpacingY; }
		}

		void VerticalSpace(float pixels)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(pixels);
			GUILayout.EndVertical();
		}

		Vector2 GetScroll(Event e)
		{
			return e.type == EventType.ScrollWheel || e.type == EventType.ScrollWheel ? e.delta : Vector2.zero;
		}

		Vector2 GetMouseDrag(Event e)
		{
			return e.type == EventType.MouseDrag ? -Event.current.delta : Vector2.zero;
		}

		private void OnEnable()
		{
			title = "Graphs";

			if(EditorPrefs.HasKey("GraphShowPause"))
				_showOptions = EditorPrefs.GetBool("GraphShowPause");
			else{
				_showOptions = true;
				EditorPrefs.SetBool("GraphShowOptions", true);
			}

			if(EditorPrefs.HasKey("GraphGridHeight"))
			{
				_gridSize.y = EditorPrefs.GetFloat("GraphGridHeight");
			}
			else{
				EditorPrefs.SetFloat("GraphGridHeight", 160f);
				_gridSize.y = 160f;
			}
			UpdateRows();

			base.minSize = new Vector2(400f, 300f);
		}

		void UpdateRows()
		{
			_rows = Mathf.CeilToInt(_gridSize.y / 40f);
		}

		Vector2 _scrollPos = Vector2.zero;

		private void OnGUI()
		{
			_timeDelta = Time.realtimeSinceStartup - _lastRealTime;
			_lastRealTime = Time.realtimeSinceStartup;

			bool isPlaying = Application.isPlaying;
			Vector2 mouseDrag = GetMouseDrag(Event.current);

			var width = base.position.width;
			_scale = width * 0.001f;
			_columns = (int)(_scale * ColumnSize);
			_gridSize.x = width - 110f;

			if (_wasCompiling)
			{
				_loadedFileList = false;
				_wasCompiling = false;
			}

			if(isPlaying)
			{
				if(_showOptions)
				{
					GUILayout.BeginHorizontal(GUI.skin.box, ToolBoxParams);
					{
						if (EditorApplication.isPaused)
						{
							if (GUILayout.Button("Play", LargeButton))
							{
								EditorApplication.isPaused = false;
								return;
							}
						}
						else
						{
							if (GUILayout.Button("Pause", LargeButton))
							{
								EditorApplication.isPaused = true;
								return;
							}
						}
						
						GUILayout.FlexibleSpace();
						
						if (GUILayout.Button("Hide", LargeButton))
						{
							_showOptions = false;
							return;
						}

						var xmax = position.width;

						Rect rect = new Rect(280f, 18f, 100f, 40f);
						if(rect.xMax < xmax)
						{
							GUI.Label(rect, "Grid height");
							rect.x += 72f;
							var h = GUI.HorizontalSlider(rect, _gridSize.y, 20f, 400f);
							if(!Mathf.Approximately(h, _gridSize.y))
							{
								_gridSize.y = h;
								EditorPrefs.SetFloat("GraphGridHeight", h);
								UpdateRows();
							}
						}
					}
					GUILayout.EndHorizontal();
				}

				_wasPlaying = true;
				_loadedFileList = false;
			}
			else
			{
				if (_wasPlaying) OnStop();

				if (!_loadedFileList) LoadFileList();

				bool loadAll = false;
				
				Rect rect = new Rect(280f, 18f, 60f, 40f);

				GUILayout.BeginHorizontal(GUI.skin.box, ToolBoxParams);
				{
					if (GUILayout.Button("Refresh filelist", LargeButton))
						LoadFileList();
					
					if (_fileList.Count > 0)
					{
						loadAll = GUILayout.Button("Load all", LargeButton);
					}

					float br_x = 268f;
					Handles.color = Color.white*0.45f;
					Handles.DrawLine(new Vector3(br_x, 8f, 0f), new Vector3(br_x, 46f, 0f));

					bool toggleglobal = GUI.Toggle(rect, _globalLink, "Link all");
					if(_globalLink != toggleglobal)
					{
						_globalLink = toggleglobal;
						
						GraphGrid last = null;
						foreach(var g in _grids)
						{
							if(_globalLink)
							{
								if(last != null)
									LinkGrids(g, last);
								
								last = g;
							}
							else
							{
								g.UnLink();
							}
						}
					}
					
					EditorGUILayout.Space();
					
					
					GUILayout.FlexibleSpace();


					EditorGUILayout.Space();

					if (_fileList.Count > 0 && GUILayout.Button("Delete all", LargeButton))
					{
						if(EditorUtility.DisplayDialog("Delete all graphs?", "Are you sure you want to delete all graphs? (This action is irreversible)", "Yes", "No"))
						{
							DeleteAll();
						}
					}

					if (_loadedGraphs.Count > 0)
					{
						if (GUILayout.Button("Clear all", LargeButton))
						{
							ClearAll();
							LoadFileList();
						}
					}
				}
				GUILayout.EndHorizontal();


				var xmax = (_loadedGraphs.Count > 0) ? position.width-200f : position.width;

				rect.x = rect.x + 80f;
				rect.width = 90f;
				if(rect.xMax-100f < xmax)
				{
					var showPause = GUI.Toggle(rect, _showOptions, "Show options");
					if(showPause != _showOptions)
					{
						_showOptions = showPause;
						EditorPrefs.SetBool("GraphShowPause", _showOptions);
					}
				}
				
				rect.x += 110f;
				rect.width = 100f;
				if(rect.xMax < xmax)
				{
					GUI.Label(rect, "Grid height");
					rect.x += 72f;
					var h = GUI.HorizontalSlider(rect, _gridSize.y, 20f, 400f);
					if(!Mathf.Approximately(h, _gridSize.y))
					{
						_gridSize.y = h;
						EditorPrefs.SetFloat("GraphGridHeight", h);
						UpdateRows();
					}
				}

				if (_fileList.Count > 0)
				{
					EditorGUILayout.BeginVertical(GUI.skin.box, FileBoxParams);
					_fileListScrollPos = EditorGUILayout.BeginScrollView(_fileListScrollPos);

					for (int i = 0; i < _fileList.Count; ++i)
					{
						var file = _fileList[i];

						GUILayout.BeginHorizontal();

						GUILayout.Label(_fileNameList[i], FileLabel);

						if (loadAll || GUILayout.Button("Load", SmallButton))
						{
							_fileList.RemoveAt(i);
							_fileNameList.RemoveAt(i);
							--i;
							LoadGraph(file.FullName);
						}

						EditorGUILayout.Space();

						if (GUILayout.Button("Delete", SmallButton))
						{
							if (EditorUtility.DisplayDialog("Delete " + file.Name + "?", "Are you sure you want to delete " + file.Name + "?", "Yes", "No"))
							{
								this.ShowNotification(new GUIContent("Deleted " + file.Name));
								file.Delete();
								_fileList.RemoveAt(i);
								_fileNameList.RemoveAt(i);
								--i;
							}
						}

						GUILayout.FlexibleSpace();

						GUILayout.EndHorizontal();
					}

					EditorGUILayout.EndScrollView();
					GUILayout.EndVertical();
				}
				//


				// Draw loaded graphs
				if (_grids.Count > 0)
				{
					Vector2 scroll = GetScroll(Event.current);

					DrawLoadedGraphs(mouseDrag, scroll.y);

					base.Repaint();
					return;
				}
			}


			if (Graph.Instance == null || Graph.Instance.Graphs == null)
			{
				_initialized = false;

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("No graphs in memory.");

				base.Repaint();

				return;
			}

			if(!_initialized)
			{
				_initialized = true;
				_lastDumpDirectory = Graph.Instance.DumpDirectory;
			}

			{ // Draw all graphs currently loaded in memory
			
				DrawGraphsInMemory();

			}

			base.Repaint();


			_wasCompiling = EditorApplication.isCompiling;
		}

		float GetHeight(GraphGrid grid)
		{
			return _gridSize.y * grid.HeightScale + SpacingY;
		}

		GraphGridEditor GetGrid(int index)
		{
			if (index >= _grids.Count)
				ExpandGrids(index);

			return _grids[index];
		}

		void ExpandGrids(int count)
		{
			for (int i = _grids.Count; i <= count; ++i)
				_grids.Add(new GraphGridEditor());
		}

		void DrawGraphsInMemory()
		{
			var min = GridInset;
			var max = min + _gridSize;
			
			_scrollPos = GUILayout.BeginScrollView(_scrollPos);
			{
				var minBoundY = _scrollPos.y;
				var maxBoundY = _scrollPos.y + LimitY;

				GUILayout.BeginVertical();
				
				int index = 0;
				foreach (var kv in Graph.Instance.Graphs)
				{
					var grid = GetGrid(index++);
					
					var log = kv.Value as GraphLogBuffer;
					
					if (log == null) continue;
					
					float yspace = GetHeight(grid);

					if(max.y < minBoundY || min.y+yspace > maxBoundY)
					{
					}
					else
					{
						grid.Log = log;
						if (!EditorGUIUtility.isProSkin) SetLightColor(grid);
						
						grid.SetArea(min, max, _timeDelta);
						grid.SetTimeBounds(log.TimeStart, log.TimeLast, _columns);
						grid.Rows = _rows;
						grid.Draw(Event.current.mousePosition);
						grid.Log = null;
					}

					min.y += yspace;
					max.y += yspace;
					GUILayout.Space(yspace);
					//if (max.y > limitY) break;
				}

				GUILayout.Space(SpacingY);
				
				GUILayout.EndVertical();
			}
			GUILayout.EndScrollView();
		}

		void DrawLoadedGraphs(Vector2 move, float zoomX)
		{
			float limitY = LimitY;
			Vector2 min = GridInset;
			Vector2 max = min + _gridSize;

			bool isLeftMouse = Event.current.button == 0;
			bool clicked = Event.current.type == EventType.MouseDown;
			bool leftClicked = clicked && isLeftMouse;
			bool rightClicked = clicked && Event.current.button == 1;
			bool mouseUpLeft = Event.current.type == EventType.MouseUp && isLeftMouse;

			GraphGrid hoverlink = null;
			
			var mousePos = Event.current.mousePosition;
			bool gridScrolling = mousePos.x >= GridInsetX && mousePos.x <= position.width - GridInsetX;

			if(gridScrolling) GUILayout.BeginScrollView(_scrollPos);
			else _scrollPos = GUILayout.BeginScrollView(_scrollPos);
			{

				var minBoundY = _scrollPos.y;
				var maxBoundY = _scrollPos.y + limitY;

				mousePos = Event.current.mousePosition;

				for (int i = 0; i < _grids.Count; ++i)
				{
					var grid = _grids[i];
					
					float yspace = GetHeight(grid);

					if (max.y < minBoundY || min.y+yspace > maxBoundY)
					{
						min.y += yspace;
						max.y += yspace;
						GUILayout.Space(yspace);
						continue;
					}

					grid.SetArea(min, max, _timeDelta);
					grid.Columns = _columns;
					grid.Rows = _rows;
					
					if (grid.Draw(mousePos))
					{
						_loadedGraphs.Remove(grid.Log);
						grid.Dispose();
						_grids.RemoveAt(i);
						--i;
						LoadFileList();
						continue;
					}
					
					grid.DrawLinkButton(mousePos);
					
					bool contains = grid.Contains(mousePos);
					bool insideLink = grid.LinkRect.Contains(mousePos);
					
					if(contains)
					{
						if(leftClicked)
						{
							_dragGrid = grid;
							Event.current.Use();
						}
						grid.ZoomWorldX(mousePos.x, zoomX);
					}
					if(insideLink)
					{
						hoverlink = grid;
						
						if(leftClicked)
						{
							_dragLink = grid;
							Event.current.Use();
						}
						else if(mouseUpLeft && _dragLink != null)
						{
							if(_dragLink != grid)
								LinkGrids(_dragLink, grid);
							_dragLink = null;
						}
						else if(rightClicked)
						{
							grid.UnLink();
						}
					}
					
					min.y += yspace;
					max.y += yspace;
					GUILayout.Space(yspace);
				}
				
				GUILayout.Space(SpacingY);
				
				
				
				if(hoverlink != null)
				{
					if(hoverlink.Link != null)
						hoverlink.Link.DrawLinks();
				}
				
				if(_dragGrid != null)
				{
					if(mouseUpLeft)
						_dragGrid = null;
					else
						_dragGrid.MoveWorld(move);
				}
				
				if(_dragLink != null)
				{
					if(mouseUpLeft)
					{
						if(_dragLink.Link != null)
							_dragLink.Link.Remove(_dragLink);
						_dragLink = null;
						return ;
					}
					
					Handles.color = _dragLink.BorderColor;
					Handles.DrawLine(Event.current.mousePosition, _dragLink.LinkRect.center);
				}

			}
			GUILayout.EndScrollView();
		}

		void LinkGrids(GraphGrid a, GraphGrid b)
		{
			if(a.Link == null && b.Link == null)
				new GraphLinkList(a, b);
			else if(a.Link == b.Link)
				return;
			else if(a.Link != null && b.Link != null)
			{
				a.Link.CombineList(b.Link);
			}
			else
			{
				var l = a.Link != null ? a.Link : b.Link;
				l.Add(a);
				l.Add(b);
			}
		}

		void DeleteAll()
		{
			var dirinf = new DirectoryInfo(TargetDirectory);
			var files = new List<FileInfo>(dirinf.GetFiles("*.graphlog"));
			for(int i=0; i<files.Count; ++i)
			{
				File.Delete(files[i].FullName);
			}
			ClearAll();
			LoadFileList();
			ShowNotification(new GUIContent("Deleted all files (" + files.Count +")"));
		}

		void ClearAll()
		{
			foreach (var g in _grids)
				g.Dispose();
			_grids.Clear();
			_loadedGraphs.Clear();
		}

		void OnStop()
		{
			_wasPlaying = false;
			ClearAll();
			Graph.Dispose();
		}

		string TargetDirectory
		{
			get { return !string.IsNullOrEmpty(_lastDumpDirectory) ? _lastDumpDirectory : (GraphLogger.AppRoot + "Logs" + Path.DirectorySeparatorChar); }
		}

		void LoadFileList()
		{
			_loadedFileList = true;
			
			var targetDir = TargetDirectory;

			_fileList.Clear();
			_fileNameList.Clear();

			if (!Directory.Exists(targetDir))
				return;

			var dirinf = new DirectoryInfo(targetDir);
			_fileList = new List<FileInfo>(dirinf.GetFiles("*.graphlog"));

			foreach (var g in _loadedGraphs)
			{
				for (int i = 0; i < _fileList.Count; ++i)
				{
					var f = _fileList[i];
					if (f.Name.Replace(".graphlog", "") != g.Name) continue;
					_fileList.RemoveAt(i);
					--i;
				}
			}

			for (int i = 0; i < _fileList.Count; ++i)
			{
				_fileNameList.Add(_fileList[i].Name.Replace(".graphlog", ""));
			}
		}

		void LoadGraph(string path)
		{
			try
			{
				using (var stream = new StreamReader(path))
				{
					var log = new GraphLogBuffer(stream);
					_loadedGraphs.Add(log);
					var grid = new GraphGridEditor(log);

					if (!EditorGUIUtility.isProSkin) SetLightColor(grid);

					grid.SetTimeBounds(log.TimeStart, log.TimeLast, _columns);
					_grids.Add(grid);

					if(_globalLink && _grids.Count >= 2)
					{
						LinkGrids(_grids[0], _grids[_grids.Count-1]);
					}
				}
			}
			catch(Exception e)
			{
				Debug.LogError("The file could not be read: " + e.Message);
			}
		}

		void SetLightColor(GraphGrid grid)
		{
			grid.SetGridColor(grid.Log.Style.MainColor == Color.white ? Color.black : grid.Log.Style.MainColor, GraphGrid.LightShade);
		}
	};

}