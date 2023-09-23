using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /**
     * Add this component to a GameObject in a unity
     * scene to display territory ellipses in the scene
     * editor.
     */
	public class TerritoryDisplay : MonoBehaviour {

        /**
         * Weather or not to draw the territory ellipses
         */
		public bool drawTerritories = true;

        /**
         * The color of the drawn ellipses.
         */
		public Color color = new Color(1, 0, 0, 0.2f);

        /**
         * Weather or not to always update the ellipses
         * from information in the scene in each
         * call to OnDrawGizmos. Turning this off might
         * increase the performance but if it is not turned
         * on changes to the territory ellipses might not
         * be shown in the scene editor.
         */
        public bool alwaysUpdate = false;

        /**
         * Mesh to use for drawing the ellipses
         * Ideally this should be the cylinder mesh
         * that comes with Unity.
         */
		public Mesh mesh;

        /** All the heat spaces in the scene */
		private HeatSpace[] heatSpaces;

        /** Get all the heat spaces in the scene
         * and put them in the heatSpaces array */
        private void UpdateAgents()
        {
            heatSpaces = (HeatSpace[])GameObject.FindObjectsOfType(typeof(HeatSpace));
        }

        /** Check if any agents are missing.
         * @return True if any agent is missing.
         */
        private bool MissingAgents()
        {
            if (heatSpaces == null) return true;
            foreach (HeatSpace hs in heatSpaces)
            {
                if (hs == null) return true;
            }
            return false;
        }

        /** Unity OnDrawGizmos method */
		void OnDrawGizmos()
		{
            if (drawTerritories)
            {
                if (alwaysUpdate || MissingAgents())
                {
                    UpdateAgents();
                }
                if (heatSpaces != null)
                {
                    foreach (HeatSpace hs in heatSpaces)
                    {
                        float len = hs.territoryFront + hs.territoryBack;
                        float dist = (hs.territoryFront - hs.territoryBack) / 2;
                        Vector3 pos = hs.transform.position + dist * hs.transform.forward;
                        Gizmos.color = color;
                        Gizmos.DrawMesh(mesh,
                        pos,
                        hs.transform.rotation,
                        new Vector3(hs.territoryWidth, 0.05f, len));
                    }

                }
            }
		}
	}
}

