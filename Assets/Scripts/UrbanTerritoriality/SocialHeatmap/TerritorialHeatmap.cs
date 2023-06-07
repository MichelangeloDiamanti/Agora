using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UrbanTerritoriality.Utilities;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.Maps
{
    /**
     * A territorial heatmap for characters.
     * Can be used to see what locations in an area
     * are socially hot
     */
    public class TerritorialHeatmap : GeneralHeatmap
    {
        /** Travel cost on the heatmap per meter where there
         is no territory */
        public float ClearCostPerMeter
        {
            get { return clearCostPerMeter; }
            set { clearCostPerMeter = value; }
        }
        private float clearCostPerMeter = 0.003f;

        /** Travel cost on the heatmap per meter in a territory
         * area */
        public float TerritoryCostPerMeter
        {
            get { return territoryCostPerMeter; }
            set { territoryCostPerMeter = value; }
        }
        private float territoryCostPerMeter = 0.3f;

        /** A list of the zones (ellipses) that affect the heatmap */
        private List<HeatSpace> heatSpaces;

        /** A dictionary containing info on the territory
         * ellipses in the last rendered heatmap.
         * They keys in the dictionary are the instance ids
         * of the agents game objects that the territory ellipses
         * belong to. */
        private Dictionary<int, PaintGridTerritoryEllipse> currentlyRenderedEllipses;

        /** Get the territorial agents. */
        public List<HeatSpace> GetHeatSpaces()
        {
            return heatSpaces;
        }

        /** Implementation of applySettings.
         * Apply settings from an UTSettings object.
         * @param UTSettings Settings to be applied.
         */
        protected override void applySettings(UTSettings settings)
        {
            size = settings.heatmapSize;
            cellSize = settings.heatmapCellSize;           
        }

        /** Performs some initialization */
        protected override void _initialize()
        {
            heatSpaces = new List<HeatSpace>();
            HeatSpace[] hs = (HeatSpace[])GameObject.FindObjectsOfType(typeof(HeatSpace));
            heatSpaces.AddRange(hs);

            currentlyRenderedEllipses = new Dictionary<int, PaintGridTerritoryEllipse>();
            foreach (HeatSpace s in heatSpaces)
            {
                currentlyRenderedEllipses[s.GetInstanceID()] = new PaintGridTerritoryEllipse();
            }
            int resX = (int)(size.x / cellSize);
            int resY = (int)(size.y / cellSize);
            mPaintGrid = new PaintGrid(resX, resY);
        }
        
        /** The the heatmap value at a position in the world
         * @param position The position in the world to get the heatmap
         * value for.
         * @param heatSpace The HeatSpaces owned by the character that is asking
         * for the heatmap value. These spaces will be subtracted from the heatmap.
         * @return Returns the value in the map at the specified position in the world.
         */
        public float GetValueAt(Vector3 position, HeatSpace heatSpace)
        {
            return GetValueAt(new Vector2(position.x, position.z), heatSpace);
        }
        public float GetValueAt(Vector2 position, HeatSpace heatSpace)
        {
            HeatSpace[] spaces = new HeatSpace[1];
            spaces[0] = heatSpace;
            return GetValueAt(position, spaces);
        }

        public override float GetValueAt(Vector2 position, PathFollowingAgent agent)
        {
            HeatSpace[] spaces = null;
            if (agent != null) spaces = agent.ownHeatSpaces;
            return GetValueAt(position, spaces);
        }

        /** Get the heatmap value at a specific position in the world.
        @param position The position in the world.
        @param ownHeatSpaces: The HeatSpaces that belong to the agent
            that is asking for the value. The values of these spaces will
            be subtracted from the value if the position lies within these
            spaces.
        */
        public float GetValueAt(Vector2 position, HeatSpace[] ownHeatSpaces)
        {
            Vector2Int gridpos = WorldToGridPos(position);
            if (!mPaintGrid.IsWithinGrid(gridpos.x, gridpos.y))
            {
                return float.MaxValue;
            }
            float val = mPaintGrid.GetValueAt(gridpos.x, gridpos.y);
            if (ownHeatSpaces != null)
            {
                int n = ownHeatSpaces.Length;
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        int agentID = ownHeatSpaces[i].GetInstanceID();
                        PaintGridTerritoryEllipse el = currentlyRenderedEllipses[agentID];
                        if (PaintGrid.IsPointWithinTerritoryEllipse(
                            gridpos.x, gridpos.y, el))
                        {
                            val -= el.value;
                        }
                    }
                }
            }
            return val;
        }

        /** Unity Update */
        protected override void Update()
        {
            base.Update();
            /* Draw the agents territories onto the paint grid */

            /* The cost of a clear area */
            paintGrid.Clear(clearCostPerMeter);

            /* Draw the territories of the agents */
            foreach (HeatSpace hs in heatSpaces)
            {
                int agentID = hs.GetInstanceID();

                Vector3 pos = hs.transform.position;
                Vector2Int agentPos = WorldToGridPos(new Vector2(pos.x, pos.z));
                float terrWidth = WorldToGridDistance(hs.territoryWidth);
                float terrFront = WorldToGridDistance(hs.territoryFront);
                float terrBack = WorldToGridDistance(hs.territoryBack);
                PaintGridTerritoryEllipse el = new PaintGridTerritoryEllipse();
                el.value = territoryCostPerMeter;
                el.agentCenterX = agentPos.x;
                el.agentCenterY = agentPos.y;
                el.agentRotation
                    = Util.DegToRadian(-hs.transform.rotation.eulerAngles.y);
                el.territoryWidth = terrWidth;
                el.territoryFront = terrFront;
                el.territoryBack = terrBack;
                currentlyRenderedEllipses[agentID] = el;

                paintGrid.DrawTerritoryEllipse(
                    currentlyRenderedEllipses[agentID],
                    PaintGrid.PaintMethod.ADD);
            }
        }
    }
}
