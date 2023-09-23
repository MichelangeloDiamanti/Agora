using System.Collections.Generic;
using UnityEngine;
using XNode;
using UnityEngine.Experimental.Rendering;

public abstract class HeatmapBaseCombinerNode : Node
{
    public List<Texture> inputHeatmaps;

    public Texture outputHeatmap;

    public Material CombineTexturesMaterial;

    public string CombineTexturesMaterialName;

    public bool normalize = true;

    public Vector2 offset = Vector2.zero;

    protected override void Init()
    {
        if (inputHeatmaps == null) inputHeatmaps = new List<Texture>();
        CombineTexturesMaterialInit();
        base.Init();
    }

    void CombineTexturesMaterialInit()
    {
        if (CombineTexturesMaterialName == null || CombineTexturesMaterialName == "")
        {
            CombineTexturesMaterialName = "Shader Graphs/addTexturesShaderGraph";
            Debug.LogWarning("CombineTexturesMaterialName NULL, Changed to default: " + CombineTexturesMaterialName);
        }
        if (CombineTexturesMaterial == null) CombineTexturesMaterial = new Material(Shader.Find(CombineTexturesMaterialName));
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "outputHeatmap")
        {
            updateOutputHeatmap();
            return outputHeatmap;
        }
        return null;
    }

    public void updateInputHeatmaps()
    {
        foreach (NodePort port in Ports)
        {
            if (port.fieldName.Contains("inputHeatmaps") && port.IsConnected)
            {
                string inputHeatmapIndexString = port.fieldName.Replace("inputHeatmaps ", "");
                int inputHeatmapIndex = int.Parse(inputHeatmapIndexString);
                inputHeatmaps[inputHeatmapIndex] = port.GetInputValue<Texture>();
            }
        }
    }

    public void updateOutputHeatmap()
    {
        CombineTexturesMaterialInit();
        updateInputHeatmaps();
        if (inputHeatmaps.Count == 0)
        {
            outputHeatmap = null;
        }
        else if (inputHeatmaps.Count == 1 && inputHeatmaps[0] != null)
        {
            outputHeatmap = inputHeatmaps[0];
        }
        else if (inputHeatmaps.Count > 1)
        {
            // Find the first non-null input texture
            int firstNonNullIndex = inputHeatmaps.FindIndex(tex => tex != null);
            if (firstNonNullIndex == -1)
            {
                // All input textures are null
                outputHeatmap = null;
                return;
            }

            // Initialize output heatmap with the first non-null input heatmap
            Texture inputTex = inputHeatmaps[firstNonNullIndex];
            RenderTexture rt = RenderTexture.GetTemporary(inputTex.width, inputTex.height, 0, inputTex.graphicsFormat);
            Graphics.Blit(inputTex, rt);

            CombineTexturesMaterial.SetInt(Shader.PropertyToID("_normalize"), normalize ? 1 : 0);
            CombineTexturesMaterial.SetVector(Shader.PropertyToID("_offset"), offset);

            // Iterate through the input textures and blit them one by one to the output heatmap, starting from the second non-null input texture
            for (int i = firstNonNullIndex + 1; i < inputHeatmaps.Count; i++)
            {
                if (inputHeatmaps[i] == null) continue; // skip null textures

                // temp is needed so that the output heatmap is not used as the input texture (overwriting the output heatmap)
                RenderTexture temp = RenderTexture.GetTemporary(inputHeatmaps[i].width, inputHeatmaps[i].height, 0, inputTex.graphicsFormat);
                RenderTexture.active = temp;

                ApplyTextureOperation(rt, temp, i);

                RenderTexture.active = rt;
                Graphics.Blit(temp, rt);

                RenderTexture.ReleaseTemporary(temp);
            }

            // Update or create the output RenderTexture
            outputHeatmap = UpdateOrCreateRenderTexture(outputHeatmap as RenderTexture, rt.width, rt.height, inputTex.graphicsFormat);

            Graphics.Blit(rt, outputHeatmap as RenderTexture);

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
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


    public virtual void ApplyTextureOperation(Texture src, RenderTexture dest, int operationIndex)
    {
        CombineTexturesMaterial.SetTexture(Shader.PropertyToID("_tex1"), src);
        CombineTexturesMaterial.SetTexture(Shader.PropertyToID("_tex2"), inputHeatmaps[operationIndex]);
        Graphics.Blit(src, dest, CombineTexturesMaterial);
    }

    protected Texture2D UpdateOrCreateTexture2D(Texture2D tex, int width, int height, TextureFormat format, bool mipChain)
    {
        if (tex == null || tex.width != width || tex.height != height)
        {
            if (tex != null)
            {
                Destroy(tex);
            }
            tex = new Texture2D(width, height, format, mipChain);
        }
        return tex;
    }


}