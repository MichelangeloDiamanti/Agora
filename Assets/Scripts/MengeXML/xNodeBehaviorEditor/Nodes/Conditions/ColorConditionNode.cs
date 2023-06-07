using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;

[CreateNodeMenu("Conditions/Color Condition")]
public class ColorConditionNode : Node
{
    // These parameters will be shared across all the conditions
    public Texture2D conditionsHeatmap;
    public float conditionsHeatmapScale;
    public Vector2 conditionsHeatmapOffset;

    public List<Color> conditionsColors;

    [SerializeReference] public List<ColorCondition> colorConditions;

    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        base.OnCreateConnection(from, to);
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName.Contains("conditionsColors"))
        {
            string strColorConditionIndex = port.fieldName.Replace("conditionsColors ", "");
            int colorConditionIndex = int.Parse(strColorConditionIndex);

            return colorConditions[colorConditionIndex];
        }
        return null;
    }

    protected override void Init()
    {
        base.Init();
    }
}