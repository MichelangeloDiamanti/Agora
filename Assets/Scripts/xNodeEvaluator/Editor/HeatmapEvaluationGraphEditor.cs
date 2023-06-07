using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;
using System;
using XNode;

[CustomNodeGraphEditor(typeof(HeatmapEvaluationGraph))]
public class HeatmapEvaluationGraphEditor : NodeGraphEditor
{

    private HeatmapEvaluationGraph m_HeatmapEvaluationGraph;

    public override string GetNodeMenuName(System.Type type)
    {
        if (type.Namespace == "Agora.Menge.Heatmap")
        {
            // return base.GetNodeMenuName(type).Replace("X Node/Examples/Logic Toy/", "");
            return base.GetNodeMenuName(type).Replace("Agora/Menge/Heatmap/", "Heatmaps/");
        }
        else if(type.Namespace == "Agora.Evaluator.Metrics")
        {
            return base.GetNodeMenuName(type).Replace("Agora/Evaluator/Metrics/", "Metrics/");
        }
        else return null;
    }
}
