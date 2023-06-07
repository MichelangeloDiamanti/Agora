using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UnityEngine.Events;

namespace UrbanTerritoriality.Maps
{
    // ThermalConductivityMap is a heatmap representing the thermal conductivity values
    // across a 2D space. It differentiates between air and concrete areas and assigns
    // different thermal conductivity values accordingly.
    public class PositionConversionMap : GeneralHeatmap
    {

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

        public override float GetValueAt(Vector2 position)
        {
            return GetValueAt(position, null);
        }

        public float GetValueAt(int x, int y)
        {
            return GetValueAt(new Vector2(x, y), null);
        }


        public override void InitializeFromAsset(MapDataScriptableObject data)
        {
            throw new System.NotImplementedException();
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

            convergenceTimer = 0;
            base.currentTime = 0;
        }

        public override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {

        }


    }
}

