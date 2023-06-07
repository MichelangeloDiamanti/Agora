using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

public class ProxemicsField : MonoBehaviour 
{
    public ProxemicsZone publicZone;
    public ProxemicsZone socialZone;
    public ProxemicsZone personalZone;
    public ProxemicsZone intimateZone;

    HeuristicWanderer _agent;

    void Awake()
    {
        Reset();
    }

    public float ProxemicsScore
    {
        get
        {
            return (float)(publicZone.Count + 2 * socialZone.Count + 3 * personalZone.Count + 5 * intimateZone.Count);
        }
    }

    void Reset()
    {
        _agent = GetComponentInParent<HeuristicWanderer>();
        if (_agent != null)
        {
            transform.localPosition = Vector3.up * (0.5f * _agent.height * _agent.agentUnitSize);
        }
    }

    void OnDrawGizmosSelected()
    {
        Reset();
    }
}
