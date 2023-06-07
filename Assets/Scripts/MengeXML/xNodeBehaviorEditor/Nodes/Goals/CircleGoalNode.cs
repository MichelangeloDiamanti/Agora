using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;
using System;

[CreateNodeMenu("Goals/Circle")]
public class CircleGoalNode : GoalNode
{
    [SerializeReference] public CircleGoal circleGoal;

    public GoalSet goalSet; // the goal set this goal belongs to

    public override Goal getGoal()
    {
        return circleGoal;
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
        if (port.fieldName == "circleGoal")
        {
            return circleGoal;
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
        if (circleGoal == null) circleGoal = new CircleGoal();
        base.Init();
    }
}