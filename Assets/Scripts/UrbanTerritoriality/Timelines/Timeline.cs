/* Copyright (c) Claudio Pedica 2017 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TimelineDirection { Forward, Backward };

[System.Serializable]
public class TimelineEvent
{
	public string name = "Event";
    public bool enabled = true;
    [Range(0, 1)] public float t;
    public UnityEvent e;
}

public class Timeline : MonoBehaviour {
    public bool showDebugLog;
    public TimelineEvent[] events;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public float duration = 1f;
    public bool looping = false;
    [Range(0f, 1f)] public float randomize = 0f;
    public UnityEvent update;
    public UnityEvent finished;

    bool _run = false;
	bool _pause = false;
    float _localtime = 0f;
    float _lasttime = 0f;
    TimelineDirection _direction;
    int _nextEvent;
    float _epsilon = 0.05f;
    float _initialDuration;

    public float Value
    {
        get { return curve.Evaluate(_localtime); }
    }

    public float Time
    {
        get { return _localtime * duration; }
    }

    public TimelineDirection Direction
    {
        get { return _direction; }
    }

    public bool IsPlaying
    {
        get { return _run; }
    }

	public bool IsPaused
	{
		get { return _pause; }
	}

    void Start()
    {
        Array.Sort(events, (a, b) => a.t == b.t ? 0 : a.t < b.t ? -1 : 1);
        _lasttime = curve.keys[curve.length - 1].time;
        _initialDuration = duration;
    }

    void Update()
    {
        if (_run)
        {
            if (showDebugLog)
            {
                Debug.Log(string.Format(
                    "[{0}.{1}] Time: {2}, Value: {3}, Direction: {4}, Looping: {5}",
                    gameObject.name, name, Time, Value, Direction, looping));
            }

            if (_direction == TimelineDirection.Forward)
            {
                _localtime += UnityEngine.Time.deltaTime / duration;
                update.Invoke();
                if (_localtime >= _lasttime)
                {
                    if (looping)
                    {
                        _localtime = 0f;
                        _nextEvent = events.Length > 0 ? 0 : -1;
                    }
                    else
                    {
                        finished.Invoke();
                        Stop();
                    }
                }

                if (_nextEvent >= 0 && Mathf.Abs(_localtime - events[_nextEvent].t) < _epsilon)
                {
                    if (events[_nextEvent].enabled) events[_nextEvent].e.Invoke();
                    _nextEvent = ++_nextEvent < events.Length ? _nextEvent : -1;

                }
            }
            else if (_direction == TimelineDirection.Backward)
            {
                _localtime -= UnityEngine.Time.deltaTime / duration;
                update.Invoke();
                if (_localtime <= 0f)
                {
                    if (looping)
                    {
                        _localtime = _lasttime;
                        _nextEvent = events.Length > 0 ? events.Length - 1 : -1;
                    }
                    else
                    {
                        finished.Invoke();
                        Stop();
                    }
                }

                if (_nextEvent >= 0 && Mathf.Abs(_localtime - events[_nextEvent].t) < _epsilon)
                {
                    events[_nextEvent].e.Invoke();
                    _nextEvent = --_nextEvent >= 0 ? _nextEvent : -1;

                }
            }
        }
    }

    public void Play()
    {
        _run = true;
        _direction = TimelineDirection.Forward;
        _localtime = 0f;
        _nextEvent = events.Length > 0 ? 0 : -1;
        if (randomize > 0f)
        {
            float min = _initialDuration - (_initialDuration * randomize);
            min = min < 0f ? 0f : min;
            float max =_initialDuration + (_initialDuration * randomize);
            duration = UnityEngine.Random.Range(min, max); 
        }
    }

    public void Reverse()
    {
        _run = true;
        _direction = TimelineDirection.Backward;
        _localtime = _lasttime;
        _nextEvent = events.Length > 0 ? events.Length - 1 : -1;
        if (randomize > 0f)
        {
            float min = _initialDuration - (_initialDuration * randomize);
            min = min < 0f ? 0f : min;
            float max =_initialDuration + (_initialDuration * randomize);
            duration = UnityEngine.Random.Range(min, max);
        }
    }

    public void Stop()
    {
        _run = false;
        _localtime = 0f;
    }

	public void Pause(){
		_run = false;
		_pause = true;
	}

	public void Resume(){
		_run = true;
		_pause = false;
	}
}
