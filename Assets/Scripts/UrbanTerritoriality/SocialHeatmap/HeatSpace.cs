using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /**
     * A class with some members representing a social
     * territory. Add this to a GameObject in a Unity
     * scene to add a social territory to the scene
     * that is shaped like an ellipse.
     */
	public class HeatSpace : MonoBehaviour {

        /** Width of the ellipse in meters. */
        public float territoryWidth = 2f;
        
        /** Length of the ellipse in front of the GameObject. */
        public float territoryFront = 1f;

        /** Length of the ellipse behind the GameObject.*/
        public float territoryBack = 1f;
	}
}