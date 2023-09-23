using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** A map showing the visibility of at a certain height in a scene.
     * The area that is shown is centered at the position of the GameObect
     * that this component is on.
     */
    public class VisibilityMap : GeneralVisibilityMap
    {
        /** How much the a cell increases in value when a single ray is drawn
         * over it */
        private float changePerRaycast = 0.1f;

        /** Weather to use a layer mask when raycasting */
        public bool useLayerMask = false;

        /** The layer mask that is used when performing raycasts
         * if useLayerMask is set to true */
        public LayerMask layerMask;

        /** Weather or not to show raycasting rays in the editor window */
        public bool showRaysInEditor = false;

        /** Color for rays that are drawn in the editor window */
        public Color raysInEditorColor = new Color(0, 1, 0, 1);

        /** This PaintGrid object is the one that is used
         * when drawing the raycasting lines. The cell values
         * in it will increase infinitelly. No normalizing is
         * done on it.
         */
        private PaintGrid workGrid;

        /** apply settings from UTSettings */
        protected override void applySettings(UTSettings settings)
        {
            //TODO apply settings
        }

        /** Do some initialization */
        protected override void _initialize()
        {
            base._initialize();
            int resX = (int)(size.x / cellSize);
            int resY = (int)(size.y / cellSize);
            workGrid = new PaintGrid(resX, resY);
            workGrid.Clear(0.0f);

            /* Start the normalizing coroutine that copies
             * and normalizes data from workGrid into mPaintGrid */
            StartCoroutine(normalizer.Normalize(workGrid, mPaintGrid));
        }

        /** Performs a raycast and raws a line in the map based on it */
        protected override void DoSingleRay()
        {
            RaycastHit hit;
            float randomAngle = Random.Range(0, Mathf.PI * 2);
            float dirX = Mathf.Cos(randomAngle);
            float dirZ = Mathf.Sin(randomAngle);
            Vector3 direction = new Vector3(dirX, 0, dirZ);
            direction.Normalize();
            float distance = size.x + size.y;

            if (useLayerMask ?
            Physics.Raycast(_rayStart, direction, out hit, distance, layerMask)
            : Physics.Raycast(_rayStart, direction, out hit, distance))
            {
                float hitDistance = hit.distance;

                if (showRaysInEditor)
                {
                    Debug.DrawLine(_rayStart, hit.point, raysInEditorColor);
                }
                Vector2Int s = WorldToGridPos(new Vector2(_rayStart.x, _rayStart.z));
                float hitGridDist = WorldToGridDistance(hitDistance);
                Vector2 sf = new Vector2(s.x, s.y);

                Vector2 dir2d = new Vector2(direction.x, direction.z);
                float steps = 0;
                float lastIndex = -1;
                while (steps < hitGridDist)
                {
                    Vector2 currentCellF = sf + dir2d * steps;
                    steps += 1f;
                    Vector2Int currentCell = new Vector2Int((int)currentCellF.x, (int)currentCellF.y);
                    if (workGrid.IsWithinGrid(currentCell.x, currentCell.y))
                    {
                        int index =
                            currentCell.y * workGrid.Width +
                            currentCell.x;
                        if (index != lastIndex)
                        {
                            workGrid.grid[index] += changePerRaycast;
                            lastIndex = index;
                        }
                    }
                }
                _rayStart = (_rayStart + hit.point) / 2f;
                FixRayStart();
            }
        }
    }
}