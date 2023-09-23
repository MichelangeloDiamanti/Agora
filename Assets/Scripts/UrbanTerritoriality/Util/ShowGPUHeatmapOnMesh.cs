using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Maps;

public class ShowGPUHeatmapOnMesh : MonoBehaviour
{
    public GPUHeatmap heatmap;

    /** The gradient to use for displaying the map
     * if a gradient is to be used.
     * */
    public Gradient mapColorGradient;

    const int textureGradientResolution = 4096;

    private int _oldGradientHashCode;

    private Material heatmapMaterial;

    private Texture2D textureGradient;
    private RenderTexture outputHeatmap;
    private ComputeShader computeShader;
    private int gradientKernel;

    bool _initialized = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    private int UpdateTextureWithComputeShader()
    {
        if (null == computeShader || null == outputHeatmap)
        {
            Debug.Log("Shader or input texture missing.");
            return -1;
        }

        ApplyGradient(heatmap.paintGrid, textureGradient, outputHeatmap);

        return 0;
    }

    private Texture2D gradientToTexture(Gradient gradient, int texResolution)
    {
        Texture2D tex = new Texture2D(texResolution, 1);
        for (int i = 0; i < texResolution; i++)
            tex.SetPixel(i, 0, gradient.Evaluate((float)i / texResolution));
        tex.Apply();
        return tex;
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


    // Update is called once per frame
    void Update()
    {
        // initialization
        if (!_initialized)
        {
            if (heatmap.Initialized)
            {
                if (heatmap == null) return;

                // Load the compute shader from resources
                computeShader = Resources.Load<ComputeShader>("Shaders/Compute/GeneralHeatMapVisualizer");

                if (computeShader == null)
                    Debug.LogError("computeShader not found in the specified path");
                else
                {
                    gradientKernel = computeShader.FindKernel("ApplyGradient");

                    outputHeatmap = new RenderTexture(heatmap.paintGrid.width, heatmap.paintGrid.height, 0);
                    outputHeatmap.enableRandomWrite = true;
                    outputHeatmap.Create();


                    if (mapColorGradient != null)
                    {
                        // Convert the gradient into a texture that can be used by the compute shader
                        textureGradient = gradientToTexture(mapColorGradient, textureGradientResolution);
                        _oldGradientHashCode = UrbanTerritoriality.Utilities.Util.GradientHashCode(mapColorGradient);
                    }
                    else
                        Debug.LogError("Missing Gradient");


                    // Create a new material with the Unlit/Texture shader
                    heatmapMaterial = new Material(Shader.Find("Unlit/Texture"));

                    // Assign the paintGrid RenderTexture to the material's texture property
                    heatmapMaterial.SetTexture("_MainTex", heatmap.paintGrid);

                    // Get the MeshRenderer component of the GameObject
                    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

                    // Check if the MeshRenderer component exists
                    if (meshRenderer != null)
                    {
                        // Assign the heatmap material to the MeshRenderer's material
                        meshRenderer.material = heatmapMaterial;
                    }
                    else
                    {
                        Debug.LogWarning("No MeshRenderer found on the GameObject. Make sure the GameObject has a MeshRenderer component.");
                    }
                }
            }
            heatmapMaterial.SetTexture("_MainTex", outputHeatmap);
            _initialized = true;
        }


        if (UrbanTerritoriality.Utilities.Util.GradientHashCode(mapColorGradient) != _oldGradientHashCode)
        {
            textureGradient = gradientToTexture(mapColorGradient, textureGradientResolution);
            _oldGradientHashCode = UrbanTerritoriality.Utilities.Util.GradientHashCode(mapColorGradient);
        }

        UpdateTextureWithComputeShader();
    }
}
