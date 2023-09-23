using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineMap : MonoBehaviour {

	public Material matWalkable;
	public Material matNotWalkable;

	public Camera walkable;
	public Camera notWalkable;
	public RenderTexture result;


	// Use this for initialization
	void Start () {
		matNotWalkable.SetTexture("WalkableTexture", walkable.targetTexture);
	}
	
	// Update is called once per frame
	void Update () {
		Graphics.Blit(walkable.targetTexture, result, matWalkable);
		Graphics.Blit(notWalkable.targetTexture, result, matNotWalkable);
	}
}
