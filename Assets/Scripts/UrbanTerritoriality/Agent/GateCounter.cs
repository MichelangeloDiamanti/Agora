using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rapid.Tools;

public class GateCounter : MonoBehaviour
{
    public bool snapToGround;
    public float timeUnit = 3600f;
    public bool cumulative = false;     // when cumulative it counts totals regardless of time

    private int count;
    private int countIN;
    private int countOUT;
    private float rate;
    private float rateIN;
    private float rateOUT;
    private Vector3 lastPos;
    private bool isFirstTime = true;

    private string LogName
    {
        get { return name + " x " + timeUnit + " secs"; }
    }

    // Use this for initialization
    void Start()
    {
        if (!cumulative)
        {
            StartCoroutine("CountPerSecs");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Graph.Log<float, float, float>(LogName, 
            isFirstTime ? count : rate,
            isFirstTime ? countIN : rateIN,
            isFirstTime ? countOUT : rateOUT);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<UrbanTerritoriality.Agent.MapWanderer>() != null)
        {
            if (Vector3.Dot((other.transform.position - transform.position), transform.forward) > 0)
            {
                countIN++;
            }
            else
            {
                countOUT++;
            }
            count++;
        }
    }

    private IEnumerator CountPerSecs()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(timeUnit);
            if (isFirstTime)
            {
                Graph.LogEvent(LogName, "at regime", Color.green);
            }
            else
            {
                if ((float)count > rate)
                {
                    Graph.LogEvent(LogName, "increased", Color.red + Color.green * 0.5f);
                }
                else if ((float)count < rate)
                {
                    Graph.LogEvent(LogName, "decreased", Color.blue);
                }
                else
                {
                    Graph.LogEvent(LogName, "constant", Color.grey);
                }
            }
            rate = (float)count;
            rateIN = (float)countIN;
            rateOUT = (float)countOUT;
            count = countIN = countOUT = 0;
            isFirstTime = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (snapToGround && transform.position != lastPos)
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
            lastPos = transform.position;
        }

        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    private void OnDrawGizmos()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        Vector3 inPos = transform.position + transform.up * col.bounds.extents.y + transform.forward * col.bounds.extents.z * 0.5f;
        Vector3 outPos = transform.position + transform.up * col.bounds.extents.y - transform.forward * col.bounds.extents.z * 0.5f;
        GizmosUtils.DrawLabel(inPos, "IN", Color.white, fontSize: 10);
        GizmosUtils.DrawLabel(outPos, "OUT", Color.white, fontSize: 10);
        if (cumulative)
        {
            timeUnit = 0f;
        }
    }
}
