using UnityEngine;
using XNode;
using UnityEngine.Experimental.Rendering;

[CreateNodeMenu("Color Operations/Invert")]
public class HeatmapInverterNode : Node
{
    [Input] public Texture inputHeatmap;

    public float minValue = 0.0f;
    public float maxValue = 1.0f;
    public bool clamp = true;

    [Output] public Texture outputHeatmap;

    public Material InvertTextureMaterial;
    public string InvertTextureMaterialName;

    protected override void Init()
    {
        base.Init();
        InvertTextureMaterialInit();
    }

    void InvertTextureMaterialInit()
    {
        if (InvertTextureMaterialName == null || InvertTextureMaterialName == "")
        {
            // InvertTextureMaterialName = "Shader Graphs/invertTextureShaderGraph";
            InvertTextureMaterialName = "Shader Graphs/minMaxInvertTextureShaderGraph";
            Debug.LogWarning("InvertTextureMaterialName NULL, Changed to default: " + InvertTextureMaterialName);
        }
        if(InvertTextureMaterial == null) InvertTextureMaterial = new Material(Shader.Find(InvertTextureMaterialName));
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "outputHeatmap")
        {
            UpdateOutputHeatmap();
            return outputHeatmap;
        }
        return null;
    }

    public void UpdateOutputHeatmap()
    {
        InvertTextureMaterialInit();
        Texture input = GetInputValue<Texture>("inputHeatmap", null);

        if (input != null)
        {
            RenderTexture rt = RenderTexture.GetTemporary(input.width, input.height, 0, input.graphicsFormat);

            InvertTextureMaterial.SetTexture(Shader.PropertyToID("_tex1"), input);
            InvertTextureMaterial.SetFloat(Shader.PropertyToID("_minValue"), minValue);
            InvertTextureMaterial.SetFloat(Shader.PropertyToID("_maxValue"), maxValue);
            InvertTextureMaterial.SetInt(Shader.PropertyToID("_clamp"), clamp ? 1 : 0);
            Graphics.Blit(input, rt, InvertTextureMaterial);

            // Update or create the output RenderTexture
            outputHeatmap = UpdateOrCreateRenderTexture(outputHeatmap as RenderTexture, rt.width, rt.height, input.graphicsFormat);

            Graphics.Blit(rt, outputHeatmap as RenderTexture);

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
        }
        else
        {
            outputHeatmap = null;
        }
    }

    protected RenderTexture UpdateOrCreateRenderTexture(RenderTexture tex, int width, int height, GraphicsFormat format)
    {
        if (tex == null || tex.width != width || tex.height != height)
        {
            if (tex != null)
            {
                tex.Release();
            }
            tex = new RenderTexture(width, height, 0, format);
            tex.Create();
        }
        return tex;
    }
}

