using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;
using System;

[CreateNodeMenu("Goals/AABB")]
public class AABBGoalNode : GoalNode
{
    [SerializeReference] public AABBGoal aabbGoal;

    public GoalSet goalSet; // the goal set this goal belongs to

    public override Goal getGoal()
    {
        return aabbGoal;
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
        if (port.fieldName == "aabbGoal")
        {
            return aabbGoal;
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
        if (aabbGoal == null) aabbGoal = new AABBGoal();
        base.Init();
    }
}