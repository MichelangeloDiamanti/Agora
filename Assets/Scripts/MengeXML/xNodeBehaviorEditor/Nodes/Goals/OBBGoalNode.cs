using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;
using System;

[CreateNodeMenu("Goals/OBB")]
public class OBBGoalNode : GoalNode
{
    [SerializeReference] public OBBGoal obbGoal;

    public GoalSet goalSet; // the goal set this goal belongs to

    public override Goal getGoal()
    {
        return obbGoal;
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
        if (port.fieldName == "obbGoal")
        {
            return obbGoal;
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
        if (obbGoal == null) obbGoal = new OBBGoal();
        base.Init();
    }
}