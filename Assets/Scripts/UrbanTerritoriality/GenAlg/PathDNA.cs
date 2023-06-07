using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.GenAlg
{
    /** DNA representing a path to be created by
     * a GeneticPathGenerator */
    public class PathDNA : System.IComparable
    {
        /**
         * Constructor
         */
		public PathDNA()
		{
			inbetweenPoints = new List<Vector2>();
			cost = 0;
		}

        /** A list of points between the start and end.
         * The start and end points should not be included
         * */
        public List<Vector2> inbetweenPoints;

        /** The cost of this path */
        public float cost;

        /** Defines weather this object is less, greater
         * or equal to another PathDNA object
         * @param other The object to compare to this.
         * @return Returns -1 if this object is less,
         * 1 if this object is greater and 0 if
         * they are equal.
         */
        public int CompareTo(object other)
        {
            PathDNA otherDNA = other as PathDNA;
            if (cost < otherDNA.cost)
            {
                return -1;
            }
            if (cost > otherDNA.cost)
            {
                return 1;
            }
            return 0;
        }

        /**
         * Create a clone of this object
         * @return Returns a new PathDNA that is exactly
         * like this one but does not use the same data
         * as this one.
         */
        public PathDNA MakeCopy()
        {
            PathDNA p = new PathDNA();
            p.cost = cost;
            p.inbetweenPoints = new List<Vector2>();
            p.inbetweenPoints.AddRange(inbetweenPoints);
            return p;
        }
    }
}

