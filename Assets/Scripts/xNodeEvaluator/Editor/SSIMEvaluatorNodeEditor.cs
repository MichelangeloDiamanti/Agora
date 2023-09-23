using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace Agora.Evaluator.Metrics
{

    [CustomNodeEditor(typeof(SSIMEvaluatorNode))]
    public class SSIMEvaluatorNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            SSIMEvaluatorNode ssimNode = target as SSIMEvaluatorNode;

            GUILayout.BeginVertical();

            // Add a button to recompute the SSIM
            if (GUILayout.Button("Recompute SSIM"))
            {
                ssimNode.similarityScore = ssimNode.GetMSSIM();
            }

            EditorGUIUtility.labelWidth = 120;
            string similarityScoreNormalizedOutput = string.Format("Similarity Score: {0}/100", (ssimNode.similarityScore).ToString("F2"));

            // Set label style for the highlighted similarity score
            GUIStyle scoreStyle = new GUIStyle(GUI.skin.label);
            scoreStyle.fontSize = 12;
            scoreStyle.fontStyle = FontStyle.Bold;

            // Interpolate color between red and green based on the similarity score
            float scoreNormalized = (float)ssimNode.similarityScore / 100.0f;
            Color textColor = Color.Lerp(Color.red, Color.green, scoreNormalized);
            scoreStyle.normal.textColor = textColor;

            scoreStyle.padding = new RectOffset(0, 0, 2, 2);

            GUILayout.Label(similarityScoreNormalizedOutput, scoreStyle);

            GUILayout.EndVertical();
        }
    }
}