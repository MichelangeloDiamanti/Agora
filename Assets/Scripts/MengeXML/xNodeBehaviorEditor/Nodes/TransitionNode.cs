using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;

public class TransitionNode : Node
{
    [SerializeReference] public Transition transition;

    protected override void Init()
    {
        if (transition == null) transition = new Transition();
        base.Init();
    }

    public override object GetValue(NodePort port)
    {
        return this.transition;
    }

    // public override void OnCreateConnection(NodePort from, NodePort to)
    // {
    //     if (to.fieldName == "transition.from")
    //     {
    //         StateNode sn = to.Connection.node as StateNode;
    //         transition.from = sn.state;
    //     }
    //     else if(to.fieldName == "transition.condition")
    //     {
    //         ConditionNode tn = to.Connection.node as ConditionNode;
    //         transition.condition = tn.condition;
    //     }
    //     else if(from.fieldName == "transition.to")
    //     {
    //         StateNode sn = from.Connection.node as StateNode;
    //         transition.to = sn.state;
    //     }

    //     base.OnCreateConnection(from, to);
    // }
}