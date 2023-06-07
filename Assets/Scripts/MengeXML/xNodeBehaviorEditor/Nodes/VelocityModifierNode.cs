using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;

public class VelocityModifierNode : Node
{
    [SerializeReference, Output] public VelocityModifier velocityModifier;

    //[Output] public VelocityModifier outVelocityModifier;

    public VelocityModifierTypes selectedVelocityModifierType;

    //public VelocityModifierTypes selectedVelocityModifierType;

    protected override void Init()
    {
        if (velocityModifier == null) velocityModifier = new ScaleVelocityModifier();
        base.Init();
    }

    public override object GetValue(NodePort port)
    {
        if(port.fieldName == "velocityModifier")
        {
            return velocityModifier;
        }
        return null;
    }

}