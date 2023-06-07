using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Maps;
public class GPUHeatmapToRenderTexture : MonoBehaviour
{
    public GPUHeatmap heatmap;
    public RenderTexture renderTexture;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // blit the heatmap to the render texture
        if(heatmap != null && renderTexture != null) Graphics.Blit(heatmap.paintGrid, renderTexture);
    }
}
