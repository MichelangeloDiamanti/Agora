using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;
using Menge.Math;

[CreateNodeMenu("Conditions/Timer Condition")]
public class TimerConditionNode : Node
{
    public TimerCondition timerCondition;

    // Use this for initialization
    protected override void Init()
    {
        timerCondition = new TimerCondition();
        // timerCondition.distribution = new ConstFloatGenerator();
        base.Init();
    }

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "condition.outCondition")
        {
            return timerCondition;
        }
        return null;
    }

}