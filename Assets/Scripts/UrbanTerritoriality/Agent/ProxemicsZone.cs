using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

[RequireComponent(typeof(SphereCollider))]
public class ProxemicsZone : MonoBehaviour
{
    [Range(0.1f, 10f)] public float distance = 1f;

    HeuristicWanderer _agent;
    SphereCollider _zone;
    float _agentUnitSize = 1f;
    float _agentRadius = 0f;
    float _agentHeight = 0f;
    int _count;

    public int Count
    {
        get 
        {
            int n = _count;
            _count = 0;
            return n; 
        }
    }

    void Awake()
    {
        Reset();
    }

    void Reset()
    {
        // Identity transform.
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // Agent vars.
        _agent = GetComponentInParent<HeuristicWanderer>();
        if (_agent != null)
        {
            _agentUnitSize = _agent.agentUnitSize;
            _agentRadius = _agent.radius;
            _agentHeight = _agent.height;
        }

        // Zone distance.
        _zone = GetComponent<SphereCollider>();
        _zone.isTrigger = true;
        _zone.radius = _agentUnitSize * (_agentRadius + distance);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<MapWanderer>() != null)
        {
            _count++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<MapWanderer>() != null)
        {
            if (_count >= 0)
            {
                _count--;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Reset();
    }
}
