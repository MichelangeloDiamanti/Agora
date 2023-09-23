using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** A struct containing info on a territory
     * region around a character that is shaped 
     * like an ellipse that is to be drawn onto
     * a paint grid */
    public struct PaintGridTerritoryEllipse
    {
        /** Value inside the ellipse */
        public float value;

        /** X dimension of the center of the agent
         * in paint grid coordinates */
        public float agentCenterX;

        /** Y dimension of the center of the agent
         * in paint grid coordinates */
        public float agentCenterY;

        /** Rotation of the agent in radians */
        public float agentRotation;

        /** Width of territory in paint grid coordinates */
        public float territoryWidth;

        /** Size of territory in front of agent in paint grid coordinates */
        public float territoryFront;

        /** Size of territory behind the agent in paint grid coordinates */
        public float territoryBack;

        /** Constructor 
         * @param value Value inside the ellipse.
         * @param agentCenterX X dimension of the agents center.
         * @param agentCenterY Y dimension of the agents center.
         * @param agentRotation The rotation of the agent in radians.
         * @param territoryWidth Width of territory in paint grid coordinates.
         * @param territoryFront Size of territory in front of agent in paint grid coordinates.
         * @param territoryBack Size of territory behind the agent in paint grid coordinates.
         */ 
        public PaintGridTerritoryEllipse(
            float value,
            float agentCenterX,
            float agentCenterY,
            float agentRotation,
            float territoryWidth,
            float territoryFront,
            float territoryBack)
        {
            this.value = value;
            this.agentCenterX = agentCenterX;
            this.agentCenterY = agentCenterY;
            this.agentRotation = agentRotation;
            this.territoryWidth = territoryWidth;
            this.territoryFront = territoryFront;
            this.territoryBack = territoryBack;
        }
    }
}
