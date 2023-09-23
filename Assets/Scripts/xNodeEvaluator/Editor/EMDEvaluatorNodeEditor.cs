using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace Agora.Evaluator.Metrics
{
    [CustomNodeEditor(typeof(EMDEvaluatorNode))]
    public class EMDEvaluatorNodeEditor : NodeEditor
    {
        private bool showHelp;
        private float headerHeight;

        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            // Calculate the height of the header content
            GUIContent headerContent = new GUIContent(target.name);
            GUIStyle headerStyle = NodeEditorResources.styles.nodeHeader;
            headerHeight = headerStyle.CalcHeight(headerContent, 100);
        }

        public override void OnBodyGUI()
        {
            EMDEvaluatorNode emdNode = target as EMDEvaluatorNode;

            GUILayout.BeginVertical();

            // Position the "?" button in the header based on the headerHeight
            // GUILayout.Space(headerHeight - 40); // Adjust this value to position the button correctly in the title area

            // Add a "?" button to the header
            if (GUILayout.Button("?", GUILayout.Width(18), GUILayout.Height(18)))
            {
                showHelp = !showHelp;
            }

            GUILayout.Space(5); // Adjust this value to create space between the button and the following contents

            // Display help if the "?" button is clicked
            if (showHelp)
            {
                EditorGUILayout.HelpBox("Earth Mover's Distance (EMD) is a measure of dissimilarity between two probability distributions or histograms. A smaller EMD indicates that the two images (or heatmaps, in your case) are more similar, while a larger EMD suggests greater dissimilarity between them.", MessageType.Info);
            }

            base.OnBodyGUI();

            GUILayout.BeginVertical();

            // Add a button to recompute the EMD
            if (GUILayout.Button("Recompute EMD"))
            {
                emdNode.emd = emdNode.CalculateEMD();
            }

            EditorGUIUtility.labelWidth = 120;
            string emdOutput = string.Format("EMD (lower = better):\n {0}", (emdNode.emd).ToString("F2"));

            // Set label style for the highlighted similarity score
            GUIStyle scoreStyle = new GUIStyle(GUI.skin.label);
            scoreStyle.fontSize = 12;
            scoreStyle.fontStyle = FontStyle.Bold;

            scoreStyle.padding = new RectOffset(0, 0, 2, 2);

            GUILayout.Label(emdOutput, scoreStyle);

            GUILayout.EndVertical();
        }
    }
}
