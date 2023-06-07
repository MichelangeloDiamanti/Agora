using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNode;
using Menge.BFSM;

[CreateNodeMenu("Goals Selectors/Heatmap")]
public class HeatmapGoalSelectorNode : GoalSelectorNode
{

    [SerializeReference]
    [Output]
    public HeatmapGoalSelector heatmapGoalSelector;

    private string materialShaderName;

    public override Goal getGoal()
    {
        return heatmapGoalSelector.getGoal();
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "heatmapGoalSelector")
        {
            heatmapGoalSelector.spatialHeatmap = GetInputValue<Texture>("heatmapGoalSelector.spatialHeatmap", heatmapGoalSelector.spatialHeatmap);
            heatmapGoalSelector.fieldOfPerceptionlHeatmap = GetInputValue<Texture>("heatmapGoalSelector.fieldOfPerceptionlHeatmap", heatmapGoalSelector.fieldOfPerceptionlHeatmap);
            // heatmapGoalSelector.offset = GetInputValue<Vector2>("heatmapGoalSelector.offset", heatmapGoalSelector.offset);
            heatmapGoalSelector.getGoal();
            return heatmapGoalSelector;
        }
        return null;
    }

    public void UpdateHeatmap()
    {
        heatmapGoalSelector.spatialHeatmap = GetInputValue<Texture>("heatmapGoalSelector.spatialHeatmap", heatmapGoalSelector.spatialHeatmap);
        heatmapGoalSelector.fieldOfPerceptionlHeatmap = GetInputValue<Texture>("heatmapGoalSelector.fieldOfPerceptionlHeatmap", heatmapGoalSelector.fieldOfPerceptionlHeatmap);
        heatmapGoalSelector.updateHeatmap();
    }

    protected override void Init()
    {
        heatmapGoalSelector = new HeatmapGoalSelector();

        LoadMaterialAndShader();

        base.Init();
    }

    public void LoadMaterialAndShader()
    {
        // load the material to combine the textures
        if (materialShaderName == null) materialShaderName  = "Shader Graphs/heatmapGoalSelectorShaderGraph";
        if (heatmapGoalSelector.combineTexturesMaterial == null) heatmapGoalSelector.combineTexturesMaterial = new Material(Shader.Find(materialShaderName));
        if (heatmapGoalSelector.combineTexturesMaterial == null) Debug.LogError("Could not load material");

        // instantiate the heatmap goal selector
        if (heatmapGoalSelector == null) heatmapGoalSelector = new HeatmapGoalSelector();
        // load the compute shader here because it cannot be loaded in the constructor
        if (heatmapGoalSelector.computeShader == null) heatmapGoalSelector.computeShader = Resources.Load<ComputeShader>("Shaders/Compute/HighestPixelRGBCoordinates");
        if (heatmapGoalSelector.computeShader == null) Debug.LogError("Could not load compute shader");
    }
}
