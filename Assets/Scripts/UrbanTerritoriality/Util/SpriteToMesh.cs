using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpriteToMesh : MonoBehaviour 
{
    public SpriteRenderer sprite;
    public bool make = false;

	// Update is called once per frame
	void Update () 
    {
        if (make)
        {
            DoMake();
        }
		
	}

    void DoMake()
    {
        make = false;

        if (sprite == null)
        {
            return;
        }

        MeshFilter mesh = GetComponent<MeshFilter>();
        mesh.sharedMesh = new Mesh();
        mesh.sharedMesh.vertices = Array.ConvertAll(sprite.sprite.vertices, i => (Vector3)i);
        mesh.sharedMesh.uv = sprite.sprite.uv;
        mesh.sharedMesh.triangles = Array.ConvertAll(sprite.sprite.triangles, i => (int)i);
        mesh.sharedMesh.RecalculateBounds();
        mesh.sharedMesh.RecalculateNormals();
    }
}
