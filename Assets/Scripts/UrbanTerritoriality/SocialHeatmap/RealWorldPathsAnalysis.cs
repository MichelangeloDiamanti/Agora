using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using PathCreation;

namespace UrbanTerritoriality.Maps
{

    public class RealWorldPathsAnalysis : GeneralHeatmap
    {

        /** 
		 * How much the a cell increases in value when 
         * a single agent goes over it 
		*/
        private const float changePerNewPosition = 0.1f;

        /** 
            this gameobject should contain childs, each one with a path
            specifically, we're assuming that they have a component of type
            PathCreator, which contains the path both in bezier and vertex form
         */
        public GameObject realWorldPathsRoot;

        /** This PaintGrid object is the one that is used
         * when drawing agents position. The cell values
         * in it will increase infinitelly. No normalizing is
         * done on it.
         */
        private PaintGrid workGrid;

        private List<PathCreator> realWorldPaths;

        private bool logIfAgentsOutsideMap = false;
        private bool agentsOutsideMap;

        public override float GetValueAt(Vector2 position, PathFollowingAgent agent)
        {
            Vector2Int gridpos = WorldToGridPos(position);
            if (!mPaintGrid.IsWithinGrid(gridpos.x, gridpos.y))
            {
                return mPaintGrid.defaultValue;
            }
            float val = mPaintGrid.GetValueAt(gridpos.x, gridpos.y);
            return val;
        }

        protected override void applySettings(UTSettings settings)
        {
            throw new System.NotImplementedException();
        }

        protected override void _initialize()
        {
            int resX = (int)(size.x / cellSize);
            int resY = (int)(size.y / cellSize);
            mPaintGrid = new PaintGrid(resX, resY);

            workGrid = new PaintGrid(resX, resY);
            workGrid.Clear(0.0f);

            convergenceTimer = 0;
            base.currentTime = 0;

            logIfAgentsOutsideMap = true;

            if (logIfAgentsOutsideMap)
                StartCoroutine(logAgentsOutsideMap());
        }

        // If some agent is outside of the map boundaries log a message
        // The flag is set in the UpdateCellPosition function
        // Doing in a centralized manner to preserve performances
        private IEnumerator logAgentsOutsideMap()
        {
            while (true)
            {
                if (agentsOutsideMap)
                {
                    agentsOutsideMap = false;
                }
                yield return new WaitForSeconds(1 * Time.timeScale);
            }
        }

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            
            realWorldPaths = new List<PathCreator>(realWorldPathsRoot.GetComponentsInChildren<PathCreator>(includeInactive: true));
            Debug.Log("Found " + realWorldPaths.Count + " paths");

            // now iterate all the paths and add their vertices to the grid
            foreach (PathCreator path in realWorldPaths)
            {
                Vector3[] vertices = path.path.localPoints;
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 vertex = vertices[i];
                    if (IsWithin(new Vector2(vertex.x, vertex.z)) == false)
                    {
                        agentsOutsideMap = true;    // Used for logging
                        continue;
                    }
                    else
                    {
                        Vector2Int gridpos = WorldToGridPos(new Vector2(vertex.x, vertex.z));
                        float newPositionValue = workGrid.GetValueAt(gridpos.x, gridpos.y);
                        newPositionValue += changePerNewPosition;
                        workGrid.SetCell(gridpos.x, gridpos.y, newPositionValue);
                    }
                }
            }

            // Debug.Log("Normalizing...");
            meanChange = Utilities.Util.MinMaxNormalization(workGrid, mPaintGrid);

        }

        // Update is called once per frame
        protected override void Update()
        {

        }
    }
}