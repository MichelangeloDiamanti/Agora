using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UrbanTerritoriality.Maps;
using Rapid.Tools;

[RequireComponent(typeof(SphereCollider))]
public class PointOfInterest : MonoBehaviour
{
    public bool drawGizmos = true;
    public bool snapToGround = true;
    public bool onceOnly = false;
    public float radius = 10f;
    [Range(0f, 1f)] public float saliency = 0.75f;
    public float waitForSecs = 60f;
    public int count;

    Vector3 _lastPos;
    Dictionary<MapWanderer, Perceptor> _perceptors = new Dictionary<MapWanderer, Perceptor>();
    BakedMap _bakedMap;

    public bool IsWandererAgent(GameObject gameObject)
    {
        return gameObject.GetComponent<MapWanderer>() != null;
    }

    void Start()
    {
        SetupSphereCollider();
        StartCoroutine(OnMappingInitialized());
    }

    IEnumerator OnMappingInitialized()
    {
        // NOTE:    I first tried using the event OnInitialized but there isn't
        //          an obvious way to register to the event before the map has
        //          has already initialized (guaranteed).

        // Grab the bake map and do something after it initializes.
        // Here we assume there is only one bakemap (global) for 
        // any simulation.
        _bakedMap = FindObjectOfType<BakedMap>();

        // Wait until the baked map is initialized.
        while (!_bakedMap.Initialized) yield return null;

        // After the baked map has initialized contribute to an additional
        // layer where saliency stacks up on top of visibilites.
        Vector2 posXZ = new Vector2(transform.position.x, transform.position.z);
        _bakedMap.AddValueAt(posXZ, saliency);
    }

    IEnumerator Logging()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Graph.Log(name, count);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Graph.Log(name, count);
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsWandererAgent(other.gameObject))
        {
            MapWanderer agent = other.gameObject.GetComponent<MapWanderer>();

            if (onceOnly && agent.IsRemembering(this.gameObject))
            {
                return;
            }

            if (!_perceptors.ContainsKey(agent))
            {
                POIPerceptor pct = other.gameObject.AddComponent<POIPerceptor>() as POIPerceptor;
                _perceptors[agent] = pct;
                pct.poi = this;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsWandererAgent(other.gameObject))
        {
            MapWanderer agent = other.gameObject.GetComponent<MapWanderer>();

            if (_perceptors.ContainsKey(agent))
            {
                Object.Destroy(_perceptors[agent]);
                _perceptors.Remove(agent);
            }
        }
    }

    void SetupSphereCollider()
    {
        SphereCollider cld = GetComponent<SphereCollider>();
        cld.isTrigger = true;
        cld.radius = radius;
        cld.center = Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        SetupSphereCollider();

        if (snapToGround && transform.position != _lastPos)
        {

            Vector3 rayStart = transform.position;
            rayStart.y = 999999999f;
            RaycastHit[] hits = Physics.RaycastAll(rayStart, Vector3.down, Mathf.Infinity, LayerMask.GetMask("World Obstacle"));
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject != transform.gameObject)
                {
                    transform.position = hits[i].point;
                    break;
                }
            }
            _lastPos = transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(2f, 0f, 2f));
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f * radius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f * radius, 1f);
        }
    }
}
