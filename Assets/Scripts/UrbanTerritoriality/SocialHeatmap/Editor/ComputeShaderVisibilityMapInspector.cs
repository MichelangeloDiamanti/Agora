using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UrbanTerritoriality.Enum;

namespace UrbanTerritoriality.Maps
{
	/* Custom inspector for the ComputeShaderVisibilityMap class */
	[CustomEditor(typeof(ComputeShaderVisibilityMap), true)]
	public class ComputeShaderVisibilityMapInspector : Editor
	{
		/** The heatmap */
		protected ComputeShaderVisibilityMap heatmap;

		/** Unity OnEnable method */
		void OnEnable()
		{
			//TODO maybee use SerializedProperty
			heatmap = (ComputeShaderVisibilityMap)target;
		}

		/** Fills the inspector with the user interface. */
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUIStyle style = new GUIStyle(GUI.skin.box)
			{
				margin = new RectOffset(5, 5, 5, 5),
				padding = new RectOffset(5, 5, 5, 5)
			};

			/** Record object for undo */
			Undo.RecordObject(heatmap, "Heatmap Change");

			GUILayout.BeginVertical(style);
			{
				EditorGUILayout.HelpBox("" +
					"Configure these paramaters in order to save a map as an asset on disk.",
					MessageType.Info);
				heatmap.saveAsset = GUILayout.Toggle(heatmap.saveAsset, new GUIContent("Save as Asset",
					"Save the map as an asset at some point in play mode."));
				heatmap.saveTexture = GUILayout.Toggle(heatmap.saveTexture, new GUIContent("Save as Texture",
					"Save the map as a texture with same filename, in same folder when the map is saved."));
				if (heatmap.saveTexture)
				{
					heatmap.textureGradient = (ScriObj.GradientScriptableObject)EditorGUILayout.ObjectField(
						"Gradient for Texture",
						heatmap.textureGradient, typeof(ScriObj.GradientScriptableObject), false);
				}
				heatmap.savePath =
					EditorTools.EditorGUIUtil.AssetPathSelector(style, heatmap.savePath, "asset");

				// Help box that helps reading the full heatmap path in one view. It's easier to double
				// check that the path is indeed the one wanted by the user.
				EditorGUILayout.HelpBox("Asset path is set to: \n\n" + heatmap.savePath, MessageType.Info);

				// AddSaveGUI();
				if (heatmap.HasMapBeenSaved)
				{
					EditorGUILayout.HelpBox("Map saved in file " + heatmap.SavedMapPath, MessageType.Info);
				}
				if (heatmap.HasTextureBeenSaved)
				{
					EditorGUILayout.HelpBox("Texture saved in file " + heatmap.SavedTexturePath, MessageType.Info);
				}

				if (GUILayout.Button("Save Now"))
				{
					heatmap.SaveMapImmediate();
				}
			}
			GUILayout.EndVertical();
		}

		/** Adds a GUI that allows the user to edit the save time. */
		public virtual void AddSaveGUI()
		{
			heatmap.saveMethod = (SaveMethod)EditorGUILayout.EnumPopup(
				new GUIContent("Save Method", "Choose between different methods of saving. Either save after " +
				"a specified time or save when the quality of the map has reached a certain level"),
				heatmap.saveMethod);
			if (heatmap.saveMethod == SaveMethod.TIME)
			{

				GUILayout.Label("Current Time: " + heatmap.CurrentTime);

				heatmap.saveTime = EditorGUILayout.FloatField(new GUIContent("Save Time in Seconds",
					"The time in play mode when the file will be saved to disk."), heatmap.saveTime);
			}
			else if (heatmap.saveMethod == SaveMethod.QUALITY)
			{
				GUILayout.Label("Mean Change: " + heatmap.meanChange);
				GUILayout.Label("Current Time: " + heatmap.CurrentTime);

				if (heatmap.HasMapConverged)
					GUILayout.Label("Convergence Time: " + heatmap.ConvergenceTimer);
				heatmap.meanChangeThreshold = EditorGUILayout.FloatField(new GUIContent("Mean Change Threshold",
				   "The threshold for the mean change for saving the map. " +
				   "The map will be saved once the value goes below this."), heatmap.meanChangeThreshold);
			}
		}

		/** Displays some things and handles input
         * in the scene view */
		public virtual void OnSceneGUI()
		{
			Repaint();
			Vector3 center = heatmap.transform.position;
			Vector2 size = heatmap.size;
			Vector2 halfSize = size * 0.5f;
			Vector3 minX = new Vector3(center.x - halfSize.x, center.y, center.z);
			Vector3 maxX = new Vector3(center.x + halfSize.x, center.y, center.z);
			Vector3 minZ = new Vector3(center.x, center.y, center.z - halfSize.y);
			Vector3 maxZ = new Vector3(center.x, center.y, center.z + halfSize.y);

			float handleSize = 0.02f;
			float handleSelectRadius = 100;
			Color colorSelectable = new Color(1, 1, 0.5f, 1);
			Color colorSelected = new Color(1, 1, 0, 1f);
			Handles.color = heatmap.gizmoColor;

			/* Create a position handle for each of the four sides of the map */
			int handleCount = 4;
			Vector3[] inPos = new Vector3[] { minX, maxX, minZ, maxZ };
			Vector3[] outPos = new Vector3[handleCount];
			for (int i = 0; i < handleCount; i++)
			{
				int posHandleId = GUIUtility.GetControlID(
					("ghi_poshandle" + i.ToString()).GetHashCode(), FocusType.Passive);
				outPos[i] = EditorTools.HandlesUtil.PositionHandle(
					posHandleId, inPos[i], Handles.CubeHandleCap, handleSize, handleSelectRadius,
					Quaternion.identity, colorSelectable, colorSelected);
			}
			minX = outPos[0];
			maxX = outPos[1];
			minZ = outPos[2];
			maxZ = outPos[3];

			float centerX = (maxX.x + minX.x) * 0.5f;
			float centerZ = (maxZ.z + minZ.z) * 0.5f;
			center.x = centerX;
			center.z = centerZ;
			size = new Vector2(maxX.x - minX.x, maxZ.z - minZ.z);
			if (size.x > 0 && size.y > 0)
			{
				Undo.RecordObject(heatmap.transform, "Position Change");
				Undo.RecordObject(heatmap, "Heatmap Size Change");
				heatmap.size = size;
				heatmap.transform.position = center;
			}
		}
	}
}

