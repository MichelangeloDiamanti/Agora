using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

[RequireComponent(typeof(SphereCollider))]
public class Signage : MonoBehaviour
{
    public bool drawGizmos = true;
    public bool snapToGround = true;
    public float radius = 10f;
    [Range(0f, 360f)]
    public float direction = 0f;
    [Range(0f, 1f)]
    public float saliency = 0.5f;

    Vector3 _worldDir;
    Vector3 _lastPos;
    Dictionary<MapWanderer, Perceptor> _perceptors = new Dictionary<MapWanderer, Perceptor>();

    public Vector3 WorldDirection
    {
        get { return _worldDir; }
    }

    public bool IsWandererAgent(GameObject gameObject)
    {
        return gameObject.GetComponent<MapWanderer>() != null;
    }

    void Start()
    {
        ComputeWorldDirection();
        SetupSphereCollider();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsWandererAgent(other.gameObject))
        {
            SignagePerceptor pct = other.gameObject.AddComponent<SignagePerceptor>() as SignagePerceptor;
            _perceptors[other.gameObject.GetComponent<MapWanderer>()] = pct;
            pct.signage = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsWandererAgent(other.gameObject))
        {
            Object.Destroy(_perceptors[other.gameObject.GetComponent<MapWanderer>()]);
        }
    }

    void ComputeWorldDirection()
    {
        _worldDir = Quaternion.AngleAxis(direction, Vector3.up) * transform.forward;
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
            ComputeWorldDirection();
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(2f, 0f, 2f));
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2f * radius, new Vector3(1f, 3f, 0.5f));
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f * radius);
            Vector3 start = transform.position + Vector3.up * .1f * radius;
            Vector3 end = start + _worldDir * 2f * radius;
            Gizmos.DrawLine(start, end);
        }
    }
}
