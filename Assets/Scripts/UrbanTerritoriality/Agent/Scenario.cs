using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
	public AnimationCurve unfolding;
    [Range(0.1f, 5f)] public float amplification = 1f;
	public AgentSpawner spawnerPrefab;

	// only for debug...
	private float currentHour;
	private float currentTimeInSecs;

	private int _currentIndex;
	private int _currentNofSpawners;
	private int _counter;
	private float _timeUnit = 3600f;    // 1 hour
	private float _elapsed;
	private float _timeStep;
	private bool _stopped;

	void Start()
	{
		spawnerPrefab.gameObject.SetActive(false);

		if (unfolding.keys.Length == 0)
		{
			_stopped = true;
			Debug.LogWarning(
				"Unfolding event is empty. You need to add some values ot it.");
		}
		else if (spawnerPrefab == null)
		{
			_stopped = true;
			Debug.LogWarning(
				"No spawner prefab has been set. You need one to make the scenario to work.");
		}
		else
		{
			_stopped = false;
			_currentIndex = 0;
			currentHour = unfolding.keys[_currentIndex].time;
			_currentNofSpawners = Mathf.RoundToInt(amplification * unfolding.keys[_currentIndex].value);
			_timeStep = _timeUnit / _currentNofSpawners;
			_counter = 0;
			_elapsed = 0f;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (_stopped)
		{
			return;
		}

		_elapsed += Time.deltaTime;
		currentTimeInSecs = _elapsed;

		// Every hour...
		if (_elapsed >= _timeUnit)
		{
			if (++_currentIndex >= unfolding.keys.Length)
			{
				_stopped = true;
			}
			else
			{
				currentHour = unfolding.keys[_currentIndex].time;
				_currentNofSpawners = Mathf.RoundToInt(amplification * unfolding.keys[_currentIndex].value);
            _timeStep = _timeUnit / _currentNofSpawners;
				_counter = 0;
				_elapsed = 0f;
			}

		}
		else
		{
			if (_elapsed > _timeStep * _counter)
			{
				InstatiateSpawner();
				_counter++;
			}
		}
	}

	void InstatiateSpawner()
	{
		// Debug.Log("Instantiating Spawner");
		AgentSpawner spawner = Instantiate(spawnerPrefab) as AgentSpawner;
		spawner.gameObject.SetActive(true);
		spawner.transform.parent = transform.parent;
	}
}
