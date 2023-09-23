using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UrbanTerritoriality.Utilities;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.Maps
{
    /**
     * A grid based map showing colliders in a unity scene.
     * The maps showes colliders at a certain height in the scene.
     * The size and position of the map in the scene can be set
     * to any value.
     */
    public class ColliderMap : RayMap
    {
        /** Number of iterations when adding
         * padding around colliders. */
        public int paddingIterations = 0;

        /** Use a layer mask when raycasting */
        public bool useLayerMask = false;

        /** A layer mask to use */
        public LayerMask layerMask;

        /** If set to true then the hit point
         * of the raycast will be considered
         * to be inside a collider resulting
         * in a high value in the collider map
         * for that position. If this is set
         * to false then it will be considered
         * empty space.
         */
        public bool setHitPointAsCollider = true;

        /** The amount subtracted in each cell in a 
         * rayasting line in the collider map when
         * subtracting after doing a raycast */
        public float emptySpaceSubtractValue = 0.1f;

        /** The amount the value of a random cell increases
         * for every cell whose values i decreased in the
         * collider map.
         * */
        public float colliderIncreaseValue = 0.01f;

        /** Apply settings from a UTSettings object.
         * @param settings A object containing variouse parameters
         * that may be applied in this script.
         */
        protected override void applySettings(UTSettings settings)
        {
            size = settings.colliderMapSize;
            cellSize = settings.colliderMapCellSize;
        }

        /** The the collider map value at a position in the world
        @param position Position in the world to get the collider map
        value for.
        @return The collider map value at that position. If the position
        is inside a collider the value should be close to 1 but if it
        is not the returned value should be close to 0.
        */
        public override float GetValueAt(Vector2 position)
        {
            Vector2Int gridpos = WorldToGridPos(position);
            if (!mPaintGrid.IsWithinGrid(gridpos.x, gridpos.y))
            {
                return float.MaxValue;
            }
            float val = mPaintGrid.GetValueAt(gridpos.x, gridpos.y);
            return val;
        }

        /** Get the value of the collider map at
         * a position in the world
         * If the position is inside the agent defined
         * by agentPosition and agentRadius then 
         * 0 is returned.
         * @param position The position in the world to 
         * find the value in the collider map for.
         * @param agentPosition The position of the agent.
         * @param agentRadius The radius of the agent.
         * @return Return the value of the collider map
         * at the specified position. If the position
         * is inside the agent then 0 is returned.
         */
        public float GetValueAt(Vector2 position,
            Vector2 agentPosition, float agentRadius)
        {
            if (Vector2.Distance(position, agentPosition) < agentRadius)
            {
                return 0;
            }
            return GetValueAt(position);
        }

        public float GetValueAt(Vector3 position,
            Vector3 agentPosition, float agentRadius)
        {
            Vector2 pos = new Vector2(position.x, position.z);
            Vector2 agentPos = new Vector2(agentPosition.x, agentPosition.z);
            return GetValueAt(pos, agentPos, agentRadius);
        }

        /** Get the value of the collider map at
         * a specific location in the world.
         * @position The position in the world to
         * get the collider map value for.
         * @agent A PathFollowingAgent to get the value
         * for. If is not null and the position is inside
         * the agent then the value returned will be 0.
         * @return Returns the value of the collider map
         * at the specified position taking into account
         * the agent also if it is not null.
         */ 
        public override float GetValueAt(Vector2 position, PathFollowingAgent agent)
        {
            if (agent != null)
            {
                Vector3 agentPos3d = agent.transform.position;
                Vector2 agentPos2d =
                    new Vector2(agentPos3d.x, agentPos3d.z);
                float agentRadius = agent.moveAgent.Controller.radius;
                return GetValueAt(position, agentPos2d, agentRadius);
            }
            else
            {
                return GetValueAt(position);
            }
        }

        /** Add padding around colliders in the map. That
         * is increase the values in the cells that are
         * neighbouring cells of cells within colliders.
         */
        private void AddPadding()
        {
            Vector2Int point;
            int x = Random.Range(0, mPaintGrid.Width);
            int y = Random.Range(0, mPaintGrid.Height);
            point = new Vector2Int(x, y);

            float[] neighbors = new float[8];
            int neighI = 0;

            for (int i = x - 1; i < x + 2; i++)
            {
                for (int j = y - 1; j < y + 2; j++)
                {
                    if (i == x && j == y) continue;
                    neighbors[neighI] = mPaintGrid.GetValueAt(i, j);
                    neighI++;
                }
            }
            float maxValue = Util.Max(neighbors);


            int index = point.y * mPaintGrid.Width + point.x;
            mPaintGrid.grid[index] =
            Mathf.Clamp(mPaintGrid.grid[index] + 0.03f * maxValue, 0f, 1f);
            
        }

        /** Do a single raycast and modify the collider map
         * based on information from the raycast.
         * */
        protected override void DoSingleRay()
        {
            RaycastHit hit;
            float dirX = Random.Range(-2f, 2f);
            float dirZ = Random.Range(-2f, 2f);
            Vector3 direction = new Vector3(dirX, 0, dirZ);
            direction.Normalize();
            float distance = size.x + size.y;

            if (useLayerMask ?
            Physics.Raycast(_rayStart, direction, out hit, distance, layerMask)
            : Physics.Raycast(_rayStart, direction, out hit, distance))
            {
                float hitDistance = hit.distance;
                Vector2Int s = WorldToGridPos(new Vector2(_rayStart.x, _rayStart.z));
                float hitGridDist = WorldToGridDistance(hitDistance);
                Vector2 sf = new Vector2(s.x, s.y);

                Vector2 dir2d = new Vector2(direction.x, direction.z);
                float steps = 0;
                while (steps < hitGridDist)
                {
                    Vector2 currentCellF = sf + dir2d * steps;
                    steps += 1f;
                    Vector2Int currentCell = new Vector2Int((int)currentCellF.x, (int)currentCellF.y);
                    if (mPaintGrid.IsWithinGrid(currentCell.x, currentCell.y))
                    {
                        int index =
                            currentCell.y * mPaintGrid.Width +
                            currentCell.x;
                        mPaintGrid.grid[index] = 
                            Mathf.Clamp(mPaintGrid.grid[index] - emptySpaceSubtractValue, 0f, 1f);

                        /* Increase the value of a random point by a small amount */
                        int x = Random.Range(0, mPaintGrid.Width);
                        int y = Random.Range(0, mPaintGrid.Height);
                        index = y * mPaintGrid.Width + x;
                        mPaintGrid.grid[index] =
                            Mathf.Clamp(mPaintGrid.grid[index] + colliderIncreaseValue, 0f, 1f);
                    }
                }
                if (setHitPointAsCollider)
                {
                    Vector2Int hitPoint = WorldToGridPos(new Vector2(hit.point.x, hit.point.z));
                    if (mPaintGrid.IsWithinGrid(hitPoint.x, hitPoint.y))
                    {
                        int index = hitPoint.y * mPaintGrid.Width + hitPoint.x;
                        mPaintGrid.grid[index] = 1f;
                    }
                }
            }
            _rayStart = (_rayStart + hit.point) / 2f;
            FixRayStart();
            
            for (int i = 0; i < paddingIterations; i++)
            {
                AddPadding();
            }
        }
    }
}
