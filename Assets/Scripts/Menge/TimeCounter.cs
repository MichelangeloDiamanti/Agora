using System;
using UnityEngine;
using UnityEngine.UI;
using UrbanTerritoriality.Agent;

namespace MengeCS
{
    public class TimeCounter : MonoBehaviour
    {
        public GameObject simulationRootGameObject;
        public GameObject HUDGameObject;
        public GameObject timeOfDayUIGameObject;
        public GameObject timeLeftUIGameObject;

        private float _startTimeInSecs;
        private float _endTimeInSecs;
        private float _currentTimeinSecs;
        private float _elapsed;
        private MengeScenarioAgentGenerator[] _scenarios;
        private bool _usingGUI;
        private bool _isRunning;
        private Text timeOfDayUI;
        private Text timeLeftUI;

        public TimeSpan startTime
        {
            get { return TimeSpan.FromSeconds(_startTimeInSecs); }
        }

        public TimeSpan endTime
        {
            get { return TimeSpan.FromSeconds(_endTimeInSecs); }
        }

        public TimeSpan currentTime
        {
            get { return TimeSpan.FromSeconds(_currentTimeinSecs); }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        // Use this for initialization
        void Start()
        {
            float secs;

            _scenarios = simulationRootGameObject.GetComponentsInChildren<MengeScenarioAgentGenerator>();
            foreach (MengeScenarioAgentGenerator s in _scenarios)
            {
                // starting time in secs
                secs = HoursToSecs(s.unfolding.keys[0].time);
                if (_startTimeInSecs - 0f <= Mathf.Epsilon || secs < _startTimeInSecs)
                {
                    _startTimeInSecs = secs;
                }

                // ending time in secs
                secs = HoursToSecs(s.unfolding.keys[s.unfolding.keys.Length - 1].time);
                if (secs > _endTimeInSecs)
                {
                    _endTimeInSecs = secs;
                }
            }

            _isRunning = !(_startTimeInSecs == 0f && _startTimeInSecs == _endTimeInSecs);

            if (HUDGameObject != null)
            {
                _usingGUI = true;
                timeOfDayUI = timeOfDayUIGameObject.transform.Find("Value").GetComponentInChildren<Text>();
                timeLeftUI = timeLeftUIGameObject.transform.Find("Value").GetComponentInChildren<Text>();

            }
            else
            {
                _usingGUI = false;
            }
        }

        void Update()
        {
            if (_isRunning)
            {
                _elapsed += Time.deltaTime;
                _currentTimeinSecs = _startTimeInSecs + _elapsed;

                float remainingSeconds = (_endTimeInSecs - _currentTimeinSecs);

                if (currentTime > endTime)
                {
                    SimulationManager.Instance.StopSimulation();
                    _isRunning = false;
                }
            }
        }

        float HoursToSecs(float h)
        {
            h = Mathf.Clamp(h, 0f, 24f);
            return h * 60f * 60f;
        }

        void OnGUI()
        {
            // GUI.Label(new Rect(0,0, 300, 100), currentTime.ToString());
            // GUI.Label(Rect.MinMaxRect(0f, 0f, 1f, 1f), currentTime.ToString());
            if (_usingGUI)
            {
                String timeOfDayText = String.Format("{0,2:00}:{1,2:00}:{2,2:00}",
                            currentTime.Hours,
                            currentTime.Minutes,
                            currentTime.Seconds);

                TimeSpan timeLeft = endTime - currentTime;
                String timeLeftText = String.Format("{0,2:00}:{1,2:00}:{2,2:00}",
                    timeLeft.Hours,
                    timeLeft.Minutes,
                    timeLeft.Seconds);

                timeOfDayUI.text = timeOfDayText;
                timeLeftUI.text = timeLeftText;
            }
        }
    }
}