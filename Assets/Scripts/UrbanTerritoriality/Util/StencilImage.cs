using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class StencilImage : MonoBehaviour
{
    public Texture image;
    [Range(0.01f, 500f)]
    public float uniformScale = 1f;
    [Range(0f, 1f)]
    public float alpha = 0.5f;

    public bool initImage = true;

    public Material Material
    {
        get { return GetComponent<Renderer>().sharedMaterial; }
    }

    // Update is called once per frame
    void Update()
    {
        if (image != null)
        {
            //SetTextureType(image);
            Vector3 newScale = Vector3.one;
            newScale.z = (float)image.height / image.width;
            transform.localScale = newScale;
            Material.SetTexture("_MainTex", image);
        }
        transform.localScale *= uniformScale;
        Color c = Material.GetColor("_Color");
        c.a = alpha;
        Material.SetColor("_Color", c);
        Material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
    }

    void SetTextureType(Texture texture)
    {
        if (texture == null)
        {
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(texture);
        TextureImporter tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (tImporter != null)
        {
            tImporter.textureType = TextureImporterType.GUI;
            AssetDatabase.ImportAsset(assetPath);
            AssetDatabase.Refresh();
        }
    }
}
