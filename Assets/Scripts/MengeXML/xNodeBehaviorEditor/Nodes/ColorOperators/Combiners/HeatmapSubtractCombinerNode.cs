using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using XNode;
using Menge.BFSM;
using System.Collections.Specialized;
using System;

[CreateNodeMenu("Color Operations/Combiners/Subtract")]
public class HeatmapSubtractCombinerNode : HeatmapBaseCombinerNode
{
    protected override void Init()
    {
        base.CombineTexturesMaterialName = "Shader Graphs/subtractTexturesShaderGraph";
        base.Init();
    }

}