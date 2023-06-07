using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** A script for normalizing an array over multiple
     * frames.
     */
    public class MapNormalizer {

        /** Maximum time spent on normalizing
         * during each frame */
        public float maxTime;

        /** The average change in values in the array
         * during each iteration. This can be used
         * to check for e.g. a visibility map has
         * reached a certain quality.
         */
        public float MeanChange { get { return meanChange; } }
        protected float meanChange;

        /** Constructor
         * @param maxTime The maximum time to spend
         * on normalizing during each frame.
         * */
        public MapNormalizer(float maxTime)
        {
            this.maxTime = maxTime;
            meanChange = float.MaxValue;
        }

        /** Puts a normalized version of workGrid into mPaintGrid.
         * @param workGrid A PaintGrid with unnormalized values.
         * @param mPaintGrid A PaintGrid to put the normalized values into.
         * */
        public virtual IEnumerator Normalize(
            PaintGrid workGrid, PaintGrid mPaintGrid)
        {
            while (true)
            {
                int n = workGrid.grid.Length;
                float start = Time.realtimeSinceStartup;
                float end = start;
                bool resetTime = true;
                float max = float.MinValue;
                for (int i = 0; i < n; i++)
                {
                    if (resetTime)
                    {
                        start = Time.realtimeSinceStartup;
                        resetTime = false;
                    }
                    float val = workGrid.grid[i];
                    if (val > max) max = val;
                    end = Time.realtimeSinceStartup;
                    if ((end - start) > maxTime)
                    {
                        resetTime = true;
                        yield return null;
                    }
                }
                yield return null;

                resetTime = true;
                float changeSum = 0;
                for (int i = 0; i < n; i++)
                {
                    if (resetTime)
                    {
                        start = Time.realtimeSinceStartup;
                        resetTime = false;
                    }
                    float newVal = workGrid.grid[i] / max;
                    changeSum += Mathf.Abs(newVal - mPaintGrid.grid[i]);
                    mPaintGrid.grid[i] = newVal;
                    end = Time.realtimeSinceStartup;
                    if ((end - start) > maxTime)
                    {
                        resetTime = true;
                        yield return null;
                    }
                }
                meanChange = changeSum / (float)n;

                yield return null;
            }
        }
    }
}

