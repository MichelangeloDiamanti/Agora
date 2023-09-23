using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.MeshToolkit
{
    /** A structure containing an integer id
     * and a 3D vector */
    [System.Serializable]
    public struct Vector3Info
    {
        /** An integer */
        public int id;

        /** A 3D vector */
        public Vector3 value;

        /** Constructor
         * @param id The integer id
         * @param value The 3D vector
         */
        public Vector3Info(int id, Vector3 value)
        {
            this.id = id;
            this.value = value;
        }
    }
}

