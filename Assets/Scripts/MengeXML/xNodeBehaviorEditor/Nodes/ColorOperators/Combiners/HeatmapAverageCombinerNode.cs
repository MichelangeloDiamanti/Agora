using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("Color Operations/Combiners/Average")]
public class HeatmapAverageCombinerNode : HeatmapBaseCombinerNode
{
    public List<float> weights;

    protected override void Init()
    {
        if (weights == null) weights = new List<float>();
        base.CombineTexturesMaterialName = "Shader Graphs/averageTexturesShaderGraph";
        base.Init();
    }

    public override void ApplyTextureOperation(Texture src, RenderTexture dest, int operationIndex)
    {
        float weight = weights[operationIndex - 1];
        CombineTexturesMaterial.SetFloat(Shader.PropertyToID("_lerpWeight"), weight);

        base.ApplyTextureOperation(src, dest, operationIndex);
    }

}