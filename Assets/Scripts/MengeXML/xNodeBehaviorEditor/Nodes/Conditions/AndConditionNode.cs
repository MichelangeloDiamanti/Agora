using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;

[CreateNodeMenu("Conditions/And Condition")]
public class AndConditionNode : Node
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
            Condition baseConditionA = this.GetInputValue<Condition>("condition.inConditionA");
            Condition baseConditioB = this.GetInputValue<Condition>("condition.inConditionB");
            return new AndCondition(baseConditionA, baseConditioB);
        }
        return null;
    }
}