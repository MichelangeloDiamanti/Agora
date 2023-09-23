using UnityEngine;
using XNode;
using UnityEngine.Experimental.Rendering;

[CreateNodeMenu("Color Operations/Gradient")]
public class GradientNode : Node
{
    const int textureGradientResolution = 4096;
    [Input] public Texture inputHeatmap;
    [Output] public Texture outputHeatmap;

    public Gradient mapColorGradient;

    private int _oldGradientHashCode;

    private ComputeShader computeShader;
    private Texture2D textureGradient;

    private int gradientKernel;

    protected override void Init()
    {
        base.Init();
        computeShader = Resources.Load<ComputeShader>("Shaders/Compute/GeneralHeatMapVisualizer");

        Texture input = GetInputValue<Texture>("inputHeatmap", null);

        if (computeShader == null)
            Debug.LogError("computeShader not found in the specified path");
        else
        {
            gradientKernel = computeShader.FindKernel("ApplyGradient");

            if (mapColorGradient != null)
            {
                textureGradient = gradientToTexture(mapColorGradient, textureGradientResolution);
                _oldGradientHashCode = UrbanTerritoriality.Utilities.Util.GradientHashCode(mapColorGradient);
            }
        }
    }

    private Texture2D gradientToTexture(Gradient gradient, int texResolution)
    {
        Texture2D tex = new Texture2D(texResolution, 1);
        for (int i = 0; i < texResolution; i++)
            tex.SetPixel(i, 0, gradient.Evaluate((float)i / texResolution));
        tex.Apply();
        return tex;
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

    void ApplyGradient(RenderTexture normalizedHeatmap, Texture2D gradient, RenderTexture outputHeatmap)
    {
        computeShader.SetTexture(gradientKernel, "NormalizedHeatmapTexture", normalizedHeatmap);
        computeShader.SetTexture(gradientKernel, "GradientTexture", gradient);
        computeShader.SetTexture(gradientKernel, "OutputTexture", outputHeatmap);
        computeShader.SetInt("NormalizedHeatmapTextureWidth", normalizedHeatmap.width);
        computeShader.SetInt("NormalizedHeatmapTextureHeight", normalizedHeatmap.height);
        computeShader.SetInt("GradientTextureWidth", textureGradientResolution);

        int threadGroupsX = Mathf.CeilToInt(normalizedHeatmap.width / 16.0f);
        int threadGroupsY = Mathf.CeilToInt(normalizedHeatmap.height / 16.0f);
        computeShader.Dispatch(gradientKernel, threadGroupsX, threadGroupsY, 1);
    }

    public void UpdateOutputHeatmap()
    {
        Texture input = GetInputValue<Texture>("inputHeatmap", null);

        if (mapColorGradient != null && input != null)
        {
            if (UrbanTerritoriality.Utilities.Util.GradientHashCode(mapColorGradient) != _oldGradientHashCode)
            {
                textureGradient = gradientToTexture(mapColorGradient, textureGradientResolution);
                _oldGradientHashCode = UrbanTerritoriality.Utilities.Util.GradientHashCode(mapColorGradient);
            }

            RenderTexture inputRT = input as RenderTexture;
            if (inputRT == null)
            {
                inputRT = new RenderTexture(input.width, input.height, 0);
                Graphics.Blit(input, inputRT);
            }

            RenderTexture outputHeatmapRT = outputHeatmap as RenderTexture;
            if (outputHeatmapRT == null || outputHeatmapRT.width != input.width || outputHeatmapRT.height != input.height)
            {
                if (outputHeatmapRT != null)
                {
                    outputHeatmapRT.Release();
                }
                outputHeatmapRT = new RenderTexture(input.width, input.height, 0);
                outputHeatmapRT.enableRandomWrite = true;
                outputHeatmapRT.Create();
                outputHeatmap = outputHeatmapRT;
            }

            ApplyGradient(inputRT, textureGradient, outputHeatmapRT);

            // Release the input RenderTexture if it was created in this method
            if (!(input is RenderTexture))
            {
                RenderTexture.active = null;
                inputRT.Release();
            }
        }
    }
}
