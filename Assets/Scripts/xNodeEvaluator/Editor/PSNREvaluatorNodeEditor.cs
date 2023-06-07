using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace Agora.Evaluator.Metrics
{

    [CustomNodeEditor(typeof(PSNREvaluatorNode))]
    public class PSNREvaluatorNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            PSNREvaluatorNode psnrNode = target as PSNREvaluatorNode;

            GUILayout.BeginVertical();

            // Add a button to recompute the SSIM
            if (GUILayout.Button("Recompute PSNR"))
            {
                psnrNode.psnrValue = psnrNode.GetPSNR();
            }

            EditorGUIUtility.labelWidth = 120;
            string similarityScoreNormalizedOutput = string.Format("PSNR (more = better):\n{0} dB", (psnrNode.psnrValue).ToString("F2"));

            // Set label style for the highlighted similarity score
            GUIStyle scoreStyle = new GUIStyle(GUI.skin.label);
            scoreStyle.fontSize = 12;
            scoreStyle.fontStyle = FontStyle.Bold;

            scoreStyle.padding = new RectOffset(0, 0, 2, 2);

            GUILayout.Label(similarityScoreNormalizedOutput, scoreStyle);

            GUILayout.EndVertical();
        }
    }
}