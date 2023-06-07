using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockIntoSceneView : MonoBehaviour 
{
	// Use this for initialization
	void Awake() 
    {
        #if UNITY_EDITOR
        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        #endif
    }
}
