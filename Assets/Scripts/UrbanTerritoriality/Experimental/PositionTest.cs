using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.Experimental
{

    public class PositionTest : MonoBehaviour
    {

        public GeneralHeatmap heatmap;

        // Use this for initialization
        void Start()
        {
        }

        bool doneLoging = false;
        // Update is called once per frame
        void Update()
        {
            if (heatmap.Initialized && !doneLoging)
            {
                doneLoging = true;
                Debug.Log("Transform position: " + transform.position);
                Vector2Int gridPos = heatmap.WorldToGridPos(new Vector2(transform.position.x,
                    transform.position.z));
                Debug.Log("grid pos: " + gridPos);
                Vector2 worldPos = heatmap.GridToWorldPosition(gridPos.x, gridPos.y);
                Debug.Log("world pos: " + worldPos);
                
            }
        }
    }
}
