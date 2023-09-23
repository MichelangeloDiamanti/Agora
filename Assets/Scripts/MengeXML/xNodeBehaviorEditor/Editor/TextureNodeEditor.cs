using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;
using UnityEditor;
using XNode;

namespace Agora.Menge.Heatmap
{

    /// <summary> 
    /// NodeEditor functions similarly to the Editor class, only it is xNode specific.
    /// Custom node editors should have the CustomNodeEditor attribute that defines which node type it is an editor for.
    /// </summary>
    [CustomNodeEditor(typeof(TextureNode))]
    public class TextureNodeEditor : NodeEditor
    {

        /// <summary> Called whenever the xNode editor window is updated </summary>
        public override void OnBodyGUI()
        {

            // Draw the default GUI first, so we don't have to do all of that manually.
            base.OnBodyGUI();

            // `target` points to the node, but it is of type `Node`, so cast it.
            TextureNode textureNode = target as TextureNode;

            // preview the output heatmap
            Texture texture = textureNode.GetInputValue<Texture>("inputHeatmap", textureNode.inputHeatmap);
            if (texture != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(new GUIContent(texture), GUILayout.Width(200), GUILayout.Height(200));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        public override int GetWidth()
        {
            return 300;
        }
    }
}