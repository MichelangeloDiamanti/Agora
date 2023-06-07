using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Maps
{
    /** 
     * A collection of experimental algorithms for different
     * kind of mappings.
     *    
     */
    public class VisExperimental : GeneralVisibilityMap
    {
        [System.Serializable]
        public enum VisAlgorithms {N1, N2, NH1, H1, C1};

        /** The NavMeshMap that will be used to compute
         * the visibility map */
        public NavMeshMap navMeshMap;

        /* Average persons height. This is used for heighmap algorithms */
        public float height = 2f;

        /* Choose what algorithm to run. */
        public VisAlgorithms algorithm = VisAlgorithms.N1;

        /** How much the a cell increases in value when a single ray is drawn
         * over it */
        private readonly float changePerRay = 0.1f;

        /** This PaintGrid object is the one that is used
         * when drawing the rays. The cell values
         * in it will increase infinitelly. No normalizing is
         * done on it.
         */
        private PaintGrid workGrid;

        // ADDITIONS -----------
        private int currentIndex = 0;
        private Vector2Int currentPixel = Vector2Int.zero;
        private List<Vector2Int> neighborhood = new List<Vector2Int>();
        private List<Vector2> content = new List<Vector2>();

        /** apply settings from an UTSettings object.
         * @param settings An UTSettings object with
         * some values that can potentially be used
         * in this class.
         */
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
            // StartCoroutine(normalizer.Normalize(workGrid, mPaintGrid));
        }

        /** Checks if a position in the world is empty space
         * By looking at the NavMeshMap.
         * @param pos The position in the world in.
         * */
        protected bool IsEmptySpace(Vector2 pos)
        {
            return navMeshMap.invert ^ (navMeshMap.GetValueAt(pos, null) > 0.5f);
        }

        /** Draws one line in the map where
        * there is empty space. */
        protected override void DoSingleRay()
        {
        }

        protected IEnumerator N1()
        {
            FixRayStart();
            neighborhood.Clear();

            for (int w = 0; w < workGrid.grid.Length; w++)
            {
                Vector2Int px = workGrid.GetXY(w);
                Vector2 pos2D = GridToWorldPosition(px.x, px.y);

                if (!IsEmptySpace(pos2D))
                {
                    continue;
                }

                Vector3 startPosition = new Vector3(pos2D.x, _rayStart.y, pos2D.y);

                for (int i = 0; i < workGrid.Width; i++)
                {
                    for (int j = 0; j < workGrid.Height; j++)
                    {
                        // Transform a grid pixel into a 3d position.
                        Vector2 targetPos2D = GridToWorldPosition(i, j);
                        Vector3 targetPosition = new Vector3(targetPos2D.x, _rayStart.y, targetPos2D.y);

                        // Is it walkable from starting point?
                        if (RaycastCheck(pos2D, targetPos2D))
                        {
                            // It is! Add it to the neighbohood
                            neighborhood.Add(new Vector2Int(i, j));
                        }

                    }
                }

                workGrid.SetCell(px.x, px.y, (float)neighborhood.Count + 1f);
                neighborhood.Clear();

                if (w % 10 == 0) yield return null;
            }
            Debug.Log("Done!!");
        }


        protected IEnumerator NH1()
        {
            FixRayStart();
            neighborhood.Clear();

            for (int w = 0; w < workGrid.grid.Length; w++)
            {
                Vector2Int px = workGrid.GetXY(w);
                Vector2 pos2D = GridToWorldPosition(px.x, px.y);

                if (!IsEmptySpace(pos2D))
                {
                    continue;
                }

                Vector3 startPosition = new Vector3(pos2D.x, _rayStart.y, pos2D.y);

                for (int i = 0; i < workGrid.Width; i++)
                {
                    for (int j = 0; j < workGrid.Height; j++)
                    {
                        // Transform a grid pixel into a 3d position.
                        Vector2 targetPos2D = GridToWorldPosition(i, j);
                        Vector3 targetPosition = new Vector3(targetPos2D.x, _rayStart.y, targetPos2D.y);

                        // Is it walkable from starting point?
                        //NavMeshHit hit;
                        //if (!NavMesh.Raycast(startPosition, targetPosition, out hit, NavMesh.AllAreas))
                        //if (RaycastCheck(pos2D, targetPos2D))
                        if (!RaycastCheck_EXPERIMENTAL(pos2D, targetPos2D))
                        {
                            // It is! Add it to the neighbohood
                            neighborhood.Add(new Vector2Int(i, j));
                        }

                    }
                }

                workGrid.SetCell(px.x, px.y, (float)neighborhood.Count + 1f);
                neighborhood.Clear();

                if (w % 10 == 0) yield return null;
            }
            Debug.Log("Done!!");
        }

        protected IEnumerator C1()
        {
            FixRayStart();
            neighborhood.Clear();

            for (int w = 0; w < workGrid.grid.Length; w++)
            {
                Vector2Int px = workGrid.GetXY(w);
                Vector2 pos2D = GridToWorldPosition(px.x, px.y);

                if (!IsEmptySpace(pos2D))
                {
                    continue;
                }

                Vector3 startPosition = new Vector3(pos2D.x, _rayStart.y, pos2D.y);

                for (int i = 0; i < workGrid.Width; i++)
                {
                    for (int j = 0; j < workGrid.Height; j++)
                    {
                        // Transform a grid pixel into a 3d position.
                        Vector2 targetPos2D = GridToWorldPosition(i, j);
                        Vector3 targetPosition = new Vector3(targetPos2D.x, _rayStart.y, targetPos2D.y);

                        // Is it walkable from starting point?
                        NavMeshHit hit;
                        if (!NavMesh.Raycast(startPosition, targetPosition, out hit, NavMesh.AllAreas))
                        {
                            // It is! Add it to the neighbohood
                            neighborhood.Add(new Vector2Int(i, j));
                        }

                    }
                }

                int nofEdges = 0;

                for (int v = 0; v < neighborhood.Count; v++)
                {
                    Vector2 startPos2D = GridToWorldPosition(neighborhood[v].x, neighborhood[v].y);
                    startPosition = new Vector3(startPos2D.x, _rayStart.y, startPos2D.y);

                    for (int t = v; t < neighborhood.Count; t++)
                    {
                        Vector2 targetPos2D = GridToWorldPosition(neighborhood[t].x, neighborhood[t].y);
                        Vector3 targetPosition = new Vector3(targetPos2D.x, _rayStart.y, targetPos2D.y);

                        NavMeshHit hit;
                        if (!NavMesh.Raycast(startPosition, targetPosition, out hit, NavMesh.AllAreas))
                        {
                            nofEdges++;
                        }
                    }
                }

                int k = neighborhood.Count;
                workGrid.SetCell(px.x, px.y, ((float)nofEdges) / (k * (k - 1)));
                neighborhood.Clear();

                Debug.Log(((float)w / workGrid.grid.Length) * 100f);

                yield return null;
            }
            Debug.Log("Done!!");
        }

        protected IEnumerator N2()
        {
            FixRayStart();
            neighborhood.Clear();

            for (int w = 0; w < workGrid.grid.Length; w++)
            {
                Vector2Int px = workGrid.GetXY(w);
                Vector2 pos2D = GridToWorldPosition(px.x, px.y);

                if (!IsEmptySpace(pos2D))
                {
                    continue;
                }

                Vector3 startPosition = new Vector3(pos2D.x, _rayStart.y, pos2D.y);

                for (int i = 0; i < workGrid.Width; i++)
                {
                    for (int j = 0; j < workGrid.Height; j++)
                    {
                        // Transform a grid pixel into a 3d position.
                        Vector2 targetPos2D = GridToWorldPosition(i, j);
                        Vector3 targetPosition = new Vector3(targetPos2D.x, _rayStart.y, targetPos2D.y);

                        // Is it walkable from starting point?
                        //NavMeshHit hit;
                        //if (!NavMesh.Raycast(startPosition, targetPosition, out hit, NavMesh.AllAreas))
                        if (RaycastCheck(pos2D, targetPos2D))
                        {
                            // It is! Add it to the neighbohood
                            neighborhood.Add(new Vector2Int(i, j));
                            workGrid.SetCell(i, j, (workGrid.GetValueAt(i, j) + changePerRay));
                        }
                    }
                }

                Debug.Log(((float)w / workGrid.grid.Length) * 100f);

                if (w % 10 == 0) yield return null;
            }
            Debug.Log("Done!!");
        }

        protected IEnumerator H1()
        {
            FixRayStart();

            for (int w = 0; w < workGrid.grid.Length; w++)
            {
                Vector2Int px = workGrid.GetXY(w);
                Vector2 pos2D = GridToWorldPosition(px.x, px.y);
                Vector3 startPosition = new Vector3(pos2D.x, _rayStart.y, pos2D.y);

                RaycastHit hit;
                if (Physics.Raycast(startPosition, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("World Obstacle")))
                {
                    workGrid.SetCell(px.x, px.y, hit.point.y);
                }
                else
                {
                    workGrid.SetCell(px.x, px.y, 0f);
                }

                //if (w % 100 == 0) yield return null;
            }
            yield return null;
            Debug.Log("Done");
        }

        /** Draw a point in the map if it is empty space.
         * @param point The world position of the point.
         * @returns Returns true if the position was in empty
         * space defined by the NavMeshMap.
         */
        protected virtual bool DrawPointIfEmptySpace(Vector2 point)
        {
            if (IsEmptySpace(point))
            {
                Vector2Int currentCell = WorldToGridPos(point);
                if (workGrid.IsWithinGrid(currentCell.x, currentCell.y))
                {
                    int index =
                        currentCell.y * workGrid.Width +
                        currentCell.x;
                    workGrid.grid[index] += changePerRay;
                    return true;
                }
            }
            return false;
        }

        /** Draws a ray in the map until it reaches an obstacle defined by
         * the NavMeshMap.
         * @param start The start of the ray in the world.
         * @param direction The direction of the ray.
         * @returns Retuns the last empty position in the world. That is
         * the last position of the ray before it went into an obstacle.
         */
        protected virtual Vector2? DrawRayUntilObstacle(Vector2 start, Vector2 direction)
        {
            int steps = 0;
            bool empty = true;
            Vector2? lastEmptyPos = null;
            while (empty)
            {
                Vector2 currentPos = start +
                    direction.normalized * (float)steps * cellSize * 0.9f;
                empty = DrawPointIfEmptySpace(currentPos);
                if (empty) lastEmptyPos = currentPos;
                steps++;
            }
            return lastEmptyPos;
        }

        /** Cast an imaginary ray from start to end. Returns true if the ray
         *  hit the end point, false if stops because of an obstacle.      
         */      
        protected bool RaycastCheck(Vector2 start, Vector2 end)
        {
            Vector2 p;
            int steps = 0;
            bool isEmpty = IsEmptySpace(start) && IsEmptySpace(end);
            Vector2 direction = (end - start).normalized;

            while (isEmpty)
            {
                p = start + direction * (float)steps * cellSize * 0.9f;
                if ((end - p).sqrMagnitude < cellSize * cellSize)
                {
                    return true;
                }
                isEmpty = IsEmptySpace(p);
                steps++;
            }
            return false;
        }

        protected bool RaycastCheck_EXPERIMENTAL(Vector2 start, Vector2 end)
        {
            RaycastHit hit;

            // Calculate start position in the world.
            // -- Note: it's likely to be better to raycast the navmesh
            // but I'm not aware of a reliable way to do it at the moment.
            Vector3 originPoint = new Vector3(start.x, _rayStart.y, start.y);
            if (Physics.Raycast(originPoint, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("World Obstacle")))
            {
                originPoint.y = hit.point.y;
            }
            originPoint.y += height;

            //Calculate end point in the world.
            Vector3 endPoint = new Vector3(end.x, _rayStart.y, end.y);
            if (Physics.Raycast(endPoint, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("World Obstacle")))
            {
                endPoint.y = hit.point.y;
            }
            endPoint.y += height;

            // Raycast check.
            Vector3 direction = endPoint - originPoint;
            float maxDistance = direction.magnitude;
            direction /= maxDistance;
            return Physics.Raycast(originPoint, direction, maxDistance, LayerMask.GetMask("World Obstacle"));
        }

        /** Unity Start method */
        public override void Start()
        {
            /* Overriding this so that the Initialize method
             * is not called in the Start method */
            //TODO maybe find a better way to do this
            /* Other possible ways 
             * Do initialization in Update instead of Start and
             * let _initialize return a boolean that tells weather
             * the initialization was successfull or not.
             * Then call _initialize until it returns true.
             */
        }

        /** Unity Update method */
        protected override void Update()
        {
            if (!initialized && navMeshMap.Initialized)
            {
                Initialize();
                StartCoroutine(algorithm.ToString());
            }
            else
            {
                base.Update();
                if (true || (int)Time.time % 10 == 0)
                {
                    meanChange = Utilities.Util.Normalize(workGrid, mPaintGrid);
                }
            }
        }

        /** On gizmos update, enforce navmeshmap to have same size,
         * position, cell size and gizmo color
         */
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (navMeshMap != null)
            {
                navMeshMap.size = size;
                navMeshMap.transform.position = transform.position;
                navMeshMap.cellSize = cellSize;
                navMeshMap.gizmoColor = gizmoColor;
            }
        }
    }
}

