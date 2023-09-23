using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UrbanTerritoriality.Agent;

[RequireComponent(typeof(HeuristicWanderer))]
public class WalkBackToStartPosition : MonoBehaviour 
{
    public bool drawGizmos = false;
    public float backByTime = 3600f;   // get back in 1h, by default.
    //public UnityEvent onCompleted;

    private HeuristicWanderer _agent;
    private Vector3 _startPosition;
    private bool _isExecuting;
    private float _elapsed;
    private float _estimatedTimeToStartPos;

    public bool IsExecuting
    {
        get { return _isExecuting; }
    }

    public Vector3 StartPosition
    {
        get { return _startPosition; }
    }

    void Start() 
    {
        _agent = GetComponent<HeuristicWanderer>();
        _startPosition = transform.position;
        StartCoroutine("Execute");
    }

    void Update()
    {
        _elapsed += Time.deltaTime;
    }

    private IEnumerator Execute()
    {
        while(_elapsed + _estimatedTimeToStartPos < backByTime)
        {
            _estimatedTimeToStartPos = (_startPosition - transform.position).magnitude / _agent.speed;
            yield return null;
        }

        _isExecuting = true;
        _agent.walkBackward = true;
        _agent.SetDestination(_startPosition);

        while(_agent.walkBackward)
        {
            yield return null;
        }

        // Behavior completed...
        _isExecuting = false;
        _agent.Dismiss();
        //onCompleted.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.red + Color.green * 0.5f;
            Gizmos.DrawCube(_startPosition, Vector3.one);
        }
    }
}
