using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;
using System;

[CreateNodeMenu("Goals/Heatmap")]
public class AbsoluteHeatmapGoalNode : GoalNode
{
    [SerializeReference] public AbsoluteHeatmapGoal absoluteHeatmapGoal;

    public GoalSet goalSet; // the goal set this goal belongs to

    public override Goal getGoal()
    {
        return absoluteHeatmapGoal;
    }

    public override GoalSet getGoalSet()
    {
        return goalSet;
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "goalSet")
        {
            updateGoalset();
            return goalSet;
        }
        if (port.fieldName == "absoluteHeatmapGoal")
        {
            updateHeatmapTexture();
            return absoluteHeatmapGoal;
        }
        return null;
    }

    public void updateGoalset()
    {
        NodePort goalsetPort = GetInputPort("goalSet");
        if (goalsetPort != null && goalsetPort.IsConnected)
        {
            goalSet = GetInputValue<GoalSet>("goalSet");
        }
    }

    public void updateHeatmapTexture()
    {
        NodePort heatMapTexturePort = GetInputPort("absoluteHeatmapGoal.heatmap");
        if (heatMapTexturePort != null && heatMapTexturePort.IsConnected)
        {
            absoluteHeatmapGoal.heatmap = GetInputValue<Texture2D>("absoluteHeatmapGoal.heatmap");
        }
    }

    protected override void Init()
    {
        if (absoluteHeatmapGoal == null) absoluteHeatmapGoal = new AbsoluteHeatmapGoal();
        base.Init();
    }
}