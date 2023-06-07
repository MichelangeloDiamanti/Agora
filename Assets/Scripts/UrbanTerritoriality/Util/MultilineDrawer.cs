using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Utilities
{
    /** A class for drawing lines in play mode in
     a Unity scene. */
    public class MultilineDrawer : MonoBehaviour
    {
        /** Material to use */
        private Material mat = null;

        /** List of lines to be drawn */
        private List<Vector3[]> linesToDraw;

        /** The colors of the lines */
        private List<Color> lineColors;

        /** Weather or not this MultilineDrawer has been initialized */
        public bool Initialized { get {return initialized; } }
        private bool initialized = false;

        /** Initializes this MultilineDrawer */
        public void Init()
        {
            SetupMaterial();
            linesToDraw = new List<Vector3[]>();
            lineColors = new List<Color>();
            initialized = true;
        }

        /** Create the material that is used for
         * drawing the lines. */
        private void SetupMaterial()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
        }
        
        /** Draws a 3d path in the Unity scene
         * @param points The points in the path.
         * @param col The color of the path.
         */ 
        public void DrawMultiline(Vector3[] points, Color col)
        {
            if (!initialized) Init();
            linesToDraw.Add(points);
            lineColors.Add(col);
        }

        /** Used for rendering */
        public void OnRenderObject()
        {
            if (!initialized) Init();
            mat.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            int n = linesToDraw.Count;
            for (int i = 0; i < n; i++)
            {
                int m = linesToDraw[i].Length;
                GL.Begin(GL.LINES);
                GL.Color(lineColors[i]);
                for (int j = 0; j < m - 1; j++)
                {
                    Vector3 p0 = linesToDraw[i][j];
                    Vector3 p1 = linesToDraw[i][j + 1];
                    GL.Vertex3(p0.x, p0.y, p0.z);
                    GL.Vertex3(p1.x, p1.y, p1.z);
                }
                GL.End();
            }
            GL.PopMatrix();
            linesToDraw.Clear();
        }
    }
}

