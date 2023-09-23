using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLabel : MonoBehaviour 
{
    public string text = "Label";
    public float height = 70f;
    public Vector2 offset = new Vector2(25f, 0f);
    public int fontSize = 16;
    public TextAnchor textAnchor = TextAnchor.MiddleRight;
    public Color color = Color.white * 0.1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        Vector3 position = Vector3.up * height + Vector3.right * offset.x + Vector3.forward * offset.y;
        position = transform.position + position;
        GizmosUtils.DrawLabel(position, text, color, fontSize, textAnchor);
        Gizmos.DrawLine(transform.position, position + Vector3.down * 3f);
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 0.1f, 1f));
    }
}
