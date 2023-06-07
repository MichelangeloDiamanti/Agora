using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSource : MonoBehaviour
{

    // Heat generation rate in W/m^2 
    // can be negative to represent heat sinks
    // values can be seen here https://www.ashrae.org/File%20Library/Technical%20Resources/Standards%20and%20Guidelines/Standards%20Addenda/55_2010_g_Final.pdf
    public float heatGenerationRate = 0f;
    public float heatGenerationRadius = 0f;

    public bool randomizeHeatGenerationRate = false;

    public float heatGenerationRateMin = 0f;
    public float heatGenerationRateMax = 0f;

    public float heatGenerationRadiusMin = 0f;
    public float heatGenerationRadiusMax = 0f;


    // Start is called before the first frame update
    void Start()
    {
        // randomize heat generation rate if needed
        if (randomizeHeatGenerationRate)
        {
            heatGenerationRate = Random.Range(heatGenerationRateMin, heatGenerationRateMax);
            heatGenerationRadius = Random.Range(heatGenerationRadiusMin, heatGenerationRadiusMax);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
