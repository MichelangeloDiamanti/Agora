using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;

[CreateNodeMenu("Velocity Modifiers/Global Heatmap")]
public class AbsoluteHeatmapVelocityModifierNode : Node
{
    [SerializeReference] public AbsoluteHeatmapVelocityModifier absoluteHeatmapVelocityModifier;

    // public override void OnCreateConnection(NodePort from, NodePort to)
    // {
    //     base.OnCreateConnection(from, to);
    // }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "outputVelocityModifier")
        {
            updateHeatmapTexture();
            return absoluteHeatmapVelocityModifier;
        }
        return null;
    }

    public void updateHeatmapTexture()
    {
        NodePort heatMapTexturePort = GetInputPort("absoluteHeatmapVelocityModifier.heatmap");
        if(heatMapTexturePort != null && heatMapTexturePort.IsConnected)
        {
			absoluteHeatmapVelocityModifier.heatmap = GetInputValue<Texture2D>("absoluteHeatmapVelocityModifier.heatmap");
        }
    }

    protected override void Init()
    {
        if (absoluteHeatmapVelocityModifier == null) absoluteHeatmapVelocityModifier = new AbsoluteHeatmapVelocityModifier();
        base.Init();
    }
}