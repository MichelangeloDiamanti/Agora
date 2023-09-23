using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** 
     * This map does not do anything useful.
     * This is just an example to show how to create a class
     * that inherits from GeneralNavGridMap.
     * This maps simply creates random noise inside the empty area
     * defined by the NavMeshMap.
     */
    public class NoiseMap : GeneralNavGridMap
    {
        /** Number of rounds per frame in the noise algorithm */
        int roundsPerFrame = 100;

        /** Apply settings from an UTSettings object
         * @param settings Settings to apply.
         */
        protected override void applySettings(UTSettings settings)
        {
            // Apply settings here
        }

        /** Unity Update */
        protected override void Update()
        {
            /* Make sure to call Update for base */
            base.Update();

            if (initialized) /* Use initialized to check if the map is ready to be worked on */
            {
                for (int i = 0; i < roundsPerFrame; i++)
                {
                    Vector2 pos2d = new Vector2(transform.position.x, transform.position.z);
                    Vector2 halfSize = size / 2f;
                    Vector2 max = pos2d + halfSize;
                    Vector2 min = pos2d - halfSize;

                    float randX = Random.Range(min.x, max.x);
                    float randY = Random.Range(min.y, max.y);

                    Vector2 pos = new Vector2(randX, randY);

                    /* Use IsWithin(pos) to check if a position in the world is within the map */
                    /* Use IsEmptySpace(pos) to check if a position in the world is empty space according
                     * to the NavMeshMap */
                    if (IsWithin(pos) && IsEmptySpace(pos))
                    {
                        Vector2Int cell = WorldToGridPos(pos);
                        int index =
                            cell.y * mPaintGrid.Width +
                            cell.x;
                        mPaintGrid.grid[index] = Random.Range(0f, 1f);
                    }
                }
            }
        }
    }
}

