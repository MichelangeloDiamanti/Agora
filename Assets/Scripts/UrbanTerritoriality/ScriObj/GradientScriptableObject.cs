using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.ScriObj
{
    /** A scriptable object containing
     * nothing but a single gradient object.
     * Useful for storing gradients as assets.
     */
    [CreateAssetMenu(fileName = "Gradient",
        menuName = "UrbanTerritoriality/GradientScriptableObject",
        order = 1)]
    public class GradientScriptableObject : UnityEngine.ScriptableObject
    {
        /** The gradient */
        public Gradient gradient;
    }
}

