using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;

[CreateNodeMenu("Conditions/Not Condition")]
public class NotConditionNode : Node
{
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
            Condition baseCondition = this.GetInputValue<Condition>("condition.inCondition");
            return new NotCondition(baseCondition);
        }
        return null;
    }

}