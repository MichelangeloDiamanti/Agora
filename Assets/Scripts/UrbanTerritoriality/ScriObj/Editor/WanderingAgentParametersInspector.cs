using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UrbanTerritoriality.ScriObj
{
    /** Custom inspector for WanderingAgentParameters */
    [CustomEditor(typeof(WanderingAgentParameters))]
    public class WanderingAgentParametersInspector : Editor
    {
        /** The WanderingAgentParameters object */
        protected WanderingAgentParameters agentParameters;

        protected bool showAdvancedSettings = false;

        /** Unity OnEnable method */
        void OnEnable()
        {
            agentParameters = (WanderingAgentParameters)target;
        }

        /** Fills the inspector with the user interface. */
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
			
			GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                margin = new RectOffset(5, 5, 5, 5),
                padding = new RectOffset(5, 5, 5, 5)
            };

            /** Record object for undo */
            Undo.RecordObject(agentParameters, "WanderingAgentParameters Change");
			
			agentParameters.perceptualDistance = EditorGUILayout.FloatField(new GUIContent(
				"Perceptual Distance",
				"The perceptual distance of the agent."),
				agentParameters.perceptualDistance);
				
			agentParameters.nrOfPickedPoints = EditorGUILayout.IntField(new GUIContent(
				"Nr Of Picked Points",
				"Number of new destination points to consider " +
				"each time the agent picks a new destination."),
				agentParameters.nrOfPickedPoints);
				
			agentParameters.maxTimeBetweenPicks = EditorGUILayout.FloatField(new GUIContent(
				"Max Time Between Picks",
				"Maximum time between picking a new destination."),
				agentParameters.maxTimeBetweenPicks);
				
			agentParameters.showGizmo = GUILayout.Toggle(
			agentParameters.showGizmo, new GUIContent(
			"Show Gizmo",
			"Weather or not to show the gizmo for " +
            "the agent in the scene window."));
			
			agentParameters.gizmoColor = EditorGUILayout.ColorField(new GUIContent(
				"Gizmo Color",
				"The main color of the gizmo."),
				agentParameters.gizmoColor);
				
			agentParameters.gizmoPositionOffset = EditorGUILayout.Vector3Field(new GUIContent(
				"Gizmo Position Offset",
				"A position offsett for the gizmo"),
				agentParameters.gizmoPositionOffset);
				
			GUILayout.BeginVertical(style);
			{
				showAdvancedSettings = GUILayout.Toggle(showAdvancedSettings, new GUIContent("Show Advanced Settings",
				"Display some advanced parameters for the agent."));

				if (showAdvancedSettings)
				{

					agentParameters.defaultMapWeight = EditorGUILayout.FloatField(new GUIContent(
						"Default Map Weight",
						"Default weight for map when " +
						"calculating the goodness of a destination."),
						agentParameters.defaultMapWeight);

					agentParameters.defaultAngleWeight = EditorGUILayout.FloatField(new GUIContent(
						"Default Angle Weight",
						"Default weight for angle between agent " +
						"forward vector and direction to destination " +
						"when calculating goodness of a destination"),
						agentParameters.defaultAngleWeight);

					agentParameters.defaultNewnessWeight = EditorGUILayout.FloatField(new GUIContent(
						"Default Newness Weight",
						"Default weight for newness of a destination " +
						"when calculating the goodness of it."),
						agentParameters.defaultNewnessWeight);
				}

            }
			GUILayout.EndVertical();
        }
    }
}

