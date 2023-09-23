using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

public class Perceptor : MonoBehaviour 
{
    HeuristicWanderer _agent;

    public HeuristicWanderer agent
    {
        get { return _agent; }
    }

    private void Awake()
    {
        _agent = GetComponent<HeuristicWanderer>();
    }
}
