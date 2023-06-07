using XNode;
using XNodeEditor;
using UnityEngine;
using UrbanTerritoriality.Maps;

namespace Agora.Menge.Heatmap
{
    public class GPUHeatmapNode : Node
    {
        [SerializeField] public GPUHeatmap gpuHeatmap;
        [Output] public Texture outputHeatmap;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outputHeatmap")
            {
                return gpuHeatmap.paintGrid;
            }
            return null;
        }

        protected override void Init()
        {
            base.Init();
        }

    }
}
