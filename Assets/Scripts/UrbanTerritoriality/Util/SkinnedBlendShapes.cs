/// <summary>
/// SkinnedBlendShapes.cs
/// ====================================================
/// Custom script and inspector for modifying values
/// of blend shapes in a skinned mesh renderer
/// --
/// Use: 
/// Put script on any game object, and set mesh renderer property
/// ====================================================
/// Author: Unnar Krist
/// Date: 22/07/2017
/// </summary>

namespace Utils
{
	using System.Collections.Generic;
	using UnityEngine;
	
	internal class SkinnedBlendShapes : MonoBehaviour
	{
		/// <summary>
		/// Checks if given blend key exists in mesh
		/// </summary>
		public bool ShapeKeyExists(string key) { return IndexOfKey(key) != -1; }
		/// <summary>
		/// Returns the number of blend shapes that exist
		/// </summary>
		public int ShapeCount() { return _renderer ? (_renderer.sharedMesh ? _renderer.sharedMesh.blendShapeCount : 0) : 0; }

		/// <summary>
		/// Finds the index of a shape using given key
		/// </summary>
		public int IndexOfKey(string key)
		{
			if (HasBlendShapes())
			{
				for (int i = 0; i < _renderer.sharedMesh.blendShapeCount; i++)
				{
					if (key == _renderer.sharedMesh.GetBlendShapeName(i)) { return i; }
				}
			}
			return -1;
		}
		/// <summary>
		/// Sets the weight value of blend shape with given key
		/// </summary>
		public void SetWeight(string key, float weight)
		{
			int index = IndexOfKey(key);
			if (index != -1) { SetWeight(index, weight); }
		}
		/// <summary>
		/// Sets the weight value of blend shape at given index
		/// </summary>
		public void SetWeight(int index, float weight)
		{
			if(index >= 0 && index < ShapeCount()) { _renderer.SetBlendShapeWeight(index, weight * 100); }
		}
		/// <summary>
		/// Returns the weight of a blend shape with given key
		/// </summary>
		public float GetWeight(string key) { return GetWeight(IndexOfKey(key)); }
		/// <summary>
		/// Returns the weight of a blend shape at given index
		/// </summary>
		public float GetWeight(int index)
		{
			return (HasBlendShapes() && index < ShapeCount() && index >= 0) ? _renderer.GetBlendShapeWeight(index) * 0.01f : 0;
		}
		/// <summary>
		/// Checks if renderer has blend shapes
		/// </summary>
		public bool HasBlendShapes() { return ShapeCount() > 0; }
		/// <summary>
		/// Checks of the current mesh renderer has a mesh set
		/// </summary>
		public bool HasMesh() { return _renderer ? _renderer.sharedMesh : false; }
		/// <summary>
		/// Returns all blend shape keys found in the current mesh
		/// </summary>
		public IEnumerable<string> GetShapeKeys()
		{
			int shapeCount = 0;
			if (_renderer && _renderer.sharedMesh) { shapeCount = _renderer.sharedMesh.blendShapeCount; }
			for (int i = 0; i < shapeCount; i++) { yield return _renderer.sharedMesh.GetBlendShapeName(i); }
		}
		/// Skinned mesh renderer with blend shapes
		[SerializeField] private SkinnedMeshRenderer _renderer;
	}
}

#region Custom Inspector

#if UNITY_EDITOR

namespace Utils
{
	using UnityEngine;
	using UnityEditor;
	using CB = System.Action;
	using EGL = UnityEditor.EditorGUILayout;
	using UObject = UnityEngine.Object;

	[CustomEditor(typeof(SkinnedBlendShapes))]
	internal class SkinnedBlendShapesInspector : Editor
	{
		public override void OnInspectorGUI() { DrawInspector(); }
		private SkinnedBlendShapes _script = null;
		private SerializedProperty _renderer = null;
		private GUIStyle _rightLabel = null;
		private bool _foldout = false;

		private void OnEnable()
		{
			_script = (SkinnedBlendShapes) target;
			_renderer = serializedObject.FindProperty("_renderer");
		}

		private void DrawInspector()
		{
			if(_rightLabel == null)
			{
				_rightLabel = new GUIStyle(EditorStyles.miniLabel);
				_rightLabel.alignment = TextAnchor.MiddleRight;
			}

			Helpers.Vertical(GUI.skin.box, () =>
			{
				// EditorGUI.PropertyField(EditorGUILayout.GetControlRect(false), _renderer);
				EditorGUI.LabelField(EditorGUILayout.GetControlRect(false), "Renderer");
				EditorGUI.PropertyField(EditorGUILayout.GetControlRect(false), _renderer, GUIContent.none);
				serializedObject.ApplyModifiedProperties();
			});
			
			Helpers.Vertical(GUI.skin.box, () =>
			{
				bool shapesEnabled = BlendShapesEnabled();
				EditorGUI.indentLevel++;
				bool t = GUI.enabled;
				GUI.enabled = shapesEnabled;
				var fr = EditorGUILayout.GetControlRect(false);
				if(shapesEnabled) { _foldout = EditorGUI.Foldout(fr, _foldout, "Blend Shapes", true); }
				else { EditorGUI.Foldout(fr, false, "Blend Shapes", true); }
				EditorGUI.LabelField(fr, (shapesEnabled ? _script.ShapeCount() : 0).ToString(), _rightLabel);
				if(_foldout && shapesEnabled){ DrawBlendValues(); }
				GUI.enabled = t;
				EditorGUI.indentLevel--;
			});
		}

		/// Draws inspector for shape blend values
		private void DrawBlendValues()
		{
			int i = 0;
			foreach(var key in _script.GetShapeKeys())
			{
				Helpers.HorizontalBlock(GUI.skin.box, () =>
				{
					float value = _script.GetWeight(i);
					string message = "Changed Blend Shape value at index " + i;
					Helpers.RUndo(_renderer.objectReferenceValue, message, () =>
					{
						EditorGUILayout.PrefixLabel(i + "\t" + key);
						value = EditorGUILayout.Slider(value, 0f, 1f);
					}, () => _script.SetWeight(i, value));
				});
				i++;
			}
		}

		private bool BlendShapesEnabled()
		{
			if(!_renderer.objectReferenceValue){ return false; }
			if(!_script.HasMesh()) { return false; }
			return true;
		}

		/// utility functions
		private static class Helpers
		{
			public static void RUndo(UObject o, string m, CB b, CB a)
			{
				EditorGUI.BeginChangeCheck(); b(); if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(o, m); a(); }
			}
			public static void HorizontalBlock(GUIStyle s, CB cb) { EGL.BeginHorizontal(s); cb(); EGL.EndHorizontal(); }
			public static void Vertical(GUIStyle s, CB cb) { EGL.BeginVertical(s); cb(); EGL.EndVertical(); }
		}
	}
}
#endif
#endregion