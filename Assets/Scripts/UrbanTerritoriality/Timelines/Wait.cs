using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Wait : MonoBehaviour 
{
	public float delay = 1f;
	public UnityEvent delayed_execute;

	public void Doit()
	{
		Invoke("Done", delay);
	}
	
	void Done()
	{
		delayed_execute.Invoke();
	}
}
