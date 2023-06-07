using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Maps
{
    /** A base class for maps that can access a NavMeshMap */
    public abstract class GeneralNavGridMap : GeneralHeatmap
    {
        /** NavMeshMap to use with this map */
        public NavMeshMap navMeshMap;

        /** Copy size and position of the NavMeshMap */
        public bool copySizeAndPos = true;

        /** Do some initialization */
        protected override void _initialize()
        {
            /* Create a PaintGrid to draw onto */
            int resX = (int)(size.x / cellSize);
            int resY = (int)(size.y / cellSize);
            mPaintGrid = new PaintGrid(resX, resY);
        }

        /** Copy cell size of the NavMeshMap */
        public bool copyCellSize = true;

        /** Returns true if pos is empty space according to
         * the NavMeshMap.
         * @param pos: Position in the world to check.
         * @returns Returns true if pos is empty space
         * according to NavMeshMap, else false.
         */
        protected virtual bool IsEmptySpace(Vector2 pos)
        {
            float navMeshValue = navMeshMap.GetValueAt(pos, null);
            bool isEmptySpace = navMeshMap.invert ^ navMeshValue > 0.5f;
            return isEmptySpace;
        }

        /** Returns a value of the map at a position in the world. */
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

        /** Unity Start method */
        public override void Start()
        {
            /* Overriding this so that the Initialize method
             * is not called in the Start method */
        }

        /** Unity Update method */
        protected override void Update()
        {
            base.Update();
            /* only initialize if the navMeshMap has been initialized */
            if (!initialized && navMeshMap.Initialized)
            {
                Initialize();
            }
        }

        /** Unity OnDrawGizmos method */
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (copySizeAndPos)
            {
                size = navMeshMap.size;
                transform.position = navMeshMap.transform.position;
            }
            if (copyCellSize)
            {
                cellSize = navMeshMap.cellSize;
            }
        }
    }
}

