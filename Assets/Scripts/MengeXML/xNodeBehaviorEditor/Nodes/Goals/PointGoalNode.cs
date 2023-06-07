using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;
using System;

[CreateNodeMenu("Goals/Point")]
public class PointGoalNode : GoalNode
{
    [SerializeReference] public PointGoal pointGoal;

    public GoalSet goalSet; // the goal set this goal belongs to

    public override Goal getGoal()
    {
        return pointGoal;
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
        if (port.fieldName == "pointGoal")
        {
            return pointGoal;
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

    protected override void Init()
    {
        if (pointGoal == null) pointGoal = new PointGoal();
        base.Init();
    }
}