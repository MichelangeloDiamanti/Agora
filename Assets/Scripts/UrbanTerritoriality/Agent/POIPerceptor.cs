using System.Collections;
using UnityEngine;

public class POIPerceptor : Perceptor 
{
    public PointOfInterest poi;

    bool agentIsNear;

	void Start() 
    {
        agent.OnPerception.AddListener(PushPOIPosition);
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (!agentIsNear)
        {
            // The agent is heading this way...
            if (agent.currentDestination == poi.transform.position)
            {
                // Agent reaches the point of interest..
                Vector3 offset = poi.transform.position - agent.transform.position;
                offset.y = 0f;
                if (offset.sqrMagnitude < 1f)
                {
                    //.. it's going to stop for a while.
                    agentIsNear = true;
                    StartCoroutine("AgentWaitForSecs");
                }
            }
        }
    }

    void PushPOIPosition()
    {
        // PoI is in front of the agent, might influence its navigation.
        int n = Mathf.RoundToInt(((float)agent.NrOfPickedPoints * poi.saliency)); 
        for (int i = 0; i < n; i++)
        {
            agent.AddToPushedPoints(poi.transform.position);
        }
    }

    IEnumerator AgentWaitForSecs()
    {
        poi.count++;
        agent.paused = true;
        yield return new WaitForSeconds(poi.waitForSecs * Random.Range(0.9f, 1.1f));

        poi.count--;
        agent.paused = false;
        agent.OnPerception.RemoveListener(PushPOIPosition);
        agent.Memorize(poi.gameObject);
        Destroy(this);
    }
}
