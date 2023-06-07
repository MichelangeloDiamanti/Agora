using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heatmap3D : MonoBehaviour
{
    public UrbanTerritoriality.Maps.GeneralHeatmap mapping;
    public UrbanTerritoriality.ScriObj.GradientScriptableObject gradient;

    [Range(10f, 100f)]
    public float displacement = 50f;

    void Start () 
    {
        mapping.Initialize();
        DoMake();		
	}

    private void Update()
    {
        transform.localScale = new Vector3(1f, displacement, 1f);
    }

    void DoMake()
    {
        for (int i = 0; i < mapping.size.x; i++)
        {
            for (int j = 0; j < mapping.size.y; j++)
            {
                float v = mapping.paintGrid.GetValueAt(i, j);
                if (v > 0f)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = transform;
                    cube.GetComponent<MeshRenderer>().material.color = gradient.gradient.Evaluate(v);
                    cube.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    cube.GetComponent<MeshRenderer>().receiveShadows = false;
                    Vector2 xyPos = mapping.GridToWorldPosition(i, j);
                    cube.transform.localScale = new Vector3(mapping.cellSize, v, mapping.cellSize);
                    cube.transform.position = new Vector3(xyPos.x, v * 0.5f, xyPos.y);
                }
            }
        }

        transform.localScale = new Vector3(1f, displacement, 1f);
    }
}
