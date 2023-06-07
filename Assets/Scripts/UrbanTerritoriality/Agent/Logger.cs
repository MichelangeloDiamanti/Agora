using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rapid.Tools;

public class Logger : MonoBehaviour 
{
	void Start() 
    {
        GraphLogger.DefaultIdMode = GraphLogger.FileIdMode.None;
        GraphLogger.DefaultBufferSizeGlobal = 5000;
        Graph.Initialize();
	}

    private void OnApplicationQuit()
    {
        Graph.Dispose();
    }
}
