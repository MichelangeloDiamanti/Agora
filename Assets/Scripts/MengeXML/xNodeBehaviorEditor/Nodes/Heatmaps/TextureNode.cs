using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Agora.Menge.Heatmap
{
    public abstract class TextureNode : Node
    {

        [Input] public Texture inputHeatmap;
        [Output] public Texture outputHeatmap;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outputHeatmap")
            {
                return GetInputValue<Texture>("inputHeatmap", this.inputHeatmap);
            }
            return null;
        }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }
    }
}
