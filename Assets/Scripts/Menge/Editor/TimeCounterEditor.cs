using System;
using UnityEngine;
using UnityEditor;

namespace MengeCS
{
    [CustomEditor(typeof(TimeCounter))]
    public class TimeCounterEditor : Editor
    {
        public TimeCounter TimeCounter
        {
            get { return target as TimeCounter; }
        }

        void OnSceneGUI()
        {
            Handles.BeginGUI();

            GUILayout.BeginArea(new Rect(650, 450, 300, 60));

            var rect = EditorGUILayout.BeginVertical();
            GUI.color = Color.yellow;
            GUI.Box(rect, GUIContent.none);

            GUI.color = Color.white;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(String.Format("Time of day {0}:{1}:{2}",
                TimeCounter.currentTime.Hours,
                TimeCounter.currentTime.Minutes,
                TimeCounter.currentTime.Seconds));
            TimeSpan timeLeft = TimeCounter.endTime - TimeCounter.currentTime;
            GUILayout.Label(String.Format("| Time left -{0}:{1}:{2}",
                timeLeft.Hours,
                timeLeft.Minutes,
                timeLeft.Seconds));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();

            Handles.EndGUI();
        }
    }
}