using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExecuteAtStart : MonoBehaviour 
{
    public UnityEvent atStartEvent;

	void Start () 
    {
		atStartEvent.Invoke();
	}
}
