using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.ScriObj
{
    /** Some paramters for a wandering agent */
    [System.Serializable]
    [CreateAssetMenu(fileName = "WanderingAgentParameters",
        menuName = "UrbanTerritoriality/WanderingAgentParameters",
        order = 2)]
    public class WanderingAgentParameters : UnityEngine.ScriptableObject
    {
        /** Weight for map when calculating goodness of points. */
        [Tooltip("Default weight for map when " +
            "calculating the goodness of a destination.")]
        public float defaultMapWeight = 1f;

        /** Weight for angle when calculating goodness of points. */
        [Tooltip("Default weight for angle between agent " +
            "forward vector and direction to destination " +
            "when calculating goodness of a destination")]
        public float defaultAngleWeight = 2f;

        /** Weight for newness when calculating goodness of points. */
        [Tooltip("Default weight for newness of a destination " +
            "when calculating the goodness of it.")]
        public float defaultNewnessWeight = 3f;

        /** The perceptual distance of the agents */
        [Tooltip("The perceptual distance of the agent.")]
        public float perceptualDistance = 15f;

        /** Number of points picked when
         * choosing the next destination */
        [Tooltip("Number of new destination points to consider " +
            "each time the agent picks a new destination.")]
        public int nrOfPickedPoints = 10;

        /** Time between picking a new destination to move to. */
        [Tooltip("Maximum time between picking a new destination.")]
        public float maxTimeBetweenPicks = 10f;

        /** Weather or not to show the gizmo
         * in the scene editor */
        [Tooltip("Weather or not to show the gizmo for " +
            "the agent in the scene window.")]
        public bool showGizmo = true;

        /** The color of the gizmo */
        [Tooltip("The main color of the gizmo")]
        public Color gizmoColor = new Color(1, 0, 1, 1);

        /** Position offset for gizmo */
        [Tooltip("A position offsett for the gizmo")]
        public Vector3 gizmoPositionOffset = new Vector3(0, 1, 0);
    }
}

