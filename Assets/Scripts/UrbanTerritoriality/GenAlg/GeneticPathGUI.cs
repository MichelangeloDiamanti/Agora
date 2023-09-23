using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.GenAlg
{
    /**
     * This component is for displaying a panel 
     * where a user can edit some values used in
     * GeneticPathGenerators.
     */
    public class GeneticPathGUI : MonoBehaviour
    {
        /**
         * The GeneticPathApplication object.
         */
        public GeneticPathApplication gpApp;

        private FloatGUIInput field_HeatmapClearCost = null;
        private FloatGUIInput field_HeatmapTerritoryCost = null;
        private FloatGUIInput field_HeatmapWeight = null;
        private FloatGUIInput field_ColliderWeight = null;
        private FloatGUIInput field_MaxLineRatioWeight = null;
        private FloatGUIInput field_MaxAngleWeight = null;
        private FloatGUIInput field_AgentAngleWeight = null;

        private int padding = 10;
        private bool initialized = false;
        private FloatGUIInput[] inputFields;
        private int panelTitleHeight = 40;
        private int height;
        private int width;

        void OnGUI()
        {
            if (!initialized)
            {
                field_HeatmapClearCost =
                    new FloatGUIInput(
                        gpApp.clearCostPerMeter, "Heatmap Clear Cost");
                field_HeatmapTerritoryCost = 
                    new FloatGUIInput(
                        gpApp.territoryCostPerMeter, "Heatmap Territory Cost");
                field_HeatmapWeight =
                    new FloatGUIInput(
                        gpApp.heatmapWeight, "Heatmap Weight");
                field_ColliderWeight =
                    new FloatGUIInput(
                        gpApp.colliderWeight, "Collider Weight");
                field_MaxLineRatioWeight =
                    new FloatGUIInput(
                        gpApp.maxLineRatioWeight, "Max Line Ratio Weight");
                field_MaxAngleWeight =
                    new FloatGUIInput(
                        gpApp.maxAngleWeight, "Max Angle Weight");
                field_AgentAngleWeight =
                    new FloatGUIInput(
                        gpApp.agentAngleWeight, "Agent Angle Weight");

                inputFields = new FloatGUIInput[]
                {
                    field_HeatmapClearCost,
                    field_HeatmapTerritoryCost,
                    field_HeatmapWeight,
                    field_ColliderWeight,
                    field_MaxLineRatioWeight,
                    field_MaxAngleWeight,
                    field_AgentAngleWeight
                };

                height = panelTitleHeight +
                    (FloatGUIInput.Height + padding) * inputFields.Length;
                width = padding * 2 + FloatGUIInput.Width;

                initialized = true;
            }

            GUI.BeginGroup(new Rect(padding,
                Screen.height - padding - height,
                width,
                height));
            GUI.color = new Color(1, 1, 1, 1);
            GUI.Box(new Rect(0, 0, width, height), "Behavior parameters");
            int n = inputFields.Length;
            for (int i = 0; i < n; i++)
            {
                inputFields[i].CreateGUI(padding,
                    panelTitleHeight +
                    (FloatGUIInput.Height + padding) * i - padding);
            }
            GUI.EndGroup();
            if (GUI.changed)
            {
                gpApp.clearCostPerMeter = field_HeatmapClearCost.Value;
                gpApp.territoryCostPerMeter = field_HeatmapTerritoryCost.Value;
                gpApp.colliderWeight = field_ColliderWeight.Value;
                gpApp.heatmapWeight = field_HeatmapWeight.Value;
                gpApp.maxLineRatioWeight = field_MaxLineRatioWeight.Value;
                gpApp.maxAngleWeight = field_MaxAngleWeight.Value;
                gpApp.agentAngleWeight = field_AgentAngleWeight.Value;
            }
        }
    }
}
