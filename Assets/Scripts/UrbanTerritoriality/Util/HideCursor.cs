using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Utilities
{
    /** Adding this script to a GameObject in a Unity scene
     * will make the cursor hidden in game mode.
     */ 
    public class HideCursor : MonoBehaviour
    {
        /** Unity Start method */
        void Start()
        {
            Cursor.visible = false;
        }
    }
}