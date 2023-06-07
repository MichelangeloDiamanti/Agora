using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;

[CreateNodeMenu("Conditions/Goal Condition")]
public class GoalConditionNode : Node
{
    public GoalCondition goalCondition;

    // Use this for initialization
    protected override void Init()
    {
        base.Init();
    }

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "condition.outCondition")
        {
            return goalCondition;
        }
        return null;
    }

}