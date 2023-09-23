using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Maps
{
    /** A map that is loaded from an asset on disk.
     * It is possible to save any map to disk as
     * an asset and load it at runtime using
     * this script.
     */
    public class BakedMap : GeneralHeatmap
    {
        /** The map asset to load from disk */
        public MapDataScriptableObject mapData;

        /** Unity update */
        protected override void Update()
        {
            base.Update();
            if (mapData != null)
            {
                SetValues();
            }
        }

        /** Read values from mapData and set the right
         * values of this object */
        public virtual void SetValues()
        {
            transform.position = mapData.position;
            cellSize = mapData.cellSize;
            size = new Vector2(mapData.gridSize.x, mapData.gridSize.y) * cellSize;
        }

        /** Do some initialization */
        protected override void _initialize()
        {
            if (mapData != null)
            {
                SetValues();
                mPaintGrid = new PaintGrid(mapData.gridSize.x, mapData.gridSize.y);
                mapData.data.CopyTo(mPaintGrid.grid, 0);
            }
            else
            {
                Debug.LogWarning("mapData missing in " + this.GetType().FullName);
            }
        }

        /** Implementation of GetValueAt.
         * Gets the value of the map at a specific position in the world.
         * @param position A position in the world to get the value for.
         * @param agent A PathFollowingAgent that wants to get a value from the map.
         * Not used here.
         * @return Returns the value in the map at the specified position.
         */
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

        public void AddValueAt(Vector2 position, float value)
        {
            Vector2Int gridpos = WorldToGridPos(position);

            if (!mPaintGrid.IsWithinGrid(gridpos.x, gridpos.y))
            {
                return;
            }
            
            float x = mPaintGrid.GetValueAt(gridpos.x, gridpos.y);
            mPaintGrid.SetCell(gridpos.x, gridpos.y, value + x);
        }

        /** Apply settings from a global settings object
         * @param UTSettings An object containing settings parameters.
         */
        protected override void applySettings(UTSettings settings)
        {
            //TODO apply settings
        }
    }
}

