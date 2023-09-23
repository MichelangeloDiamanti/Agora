using System.Collections;
using UnityEngine;

public class SignagePerceptor : Perceptor 
{
    public Signage signage;

	void Start()
    {
        agent.OnPerception.AddListener(PushSignagePosition);
	}
    
    void PushSignagePosition()
    {
        int n = Mathf.RoundToInt(((float)agent.NrOfPickedPoints * signage.saliency));
        for (int i = 0; i < n; i++)
        {
            agent.AddToPushedPoints(
                signage.transform.position + signage.WorldDirection * 2f * signage.radius);
        }
    }

    /* ************************************************************************
         
    // Update is called once per frame
    void Update () 
    {
        if (!agentIsNear)
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


    void PushPOIPosition()
    {
        int n = (int)((float)agent.NrOfPickedPoints * poi.salience);
        for (int i = 0; i < n; i++)
        {
            agent.AddToPushedPoints(poi.transform.position);
        }
    }

    IEnumerator AgentWaitForSecs()
    {
        poi.count++;
        agent.paused = true;
        yield return new WaitForSeconds(poi.waitForSecs);

        poi.count--;
        agent.paused = false;
        agent.OnPerception.RemoveListener(PushPOIPosition);
        Destroy(this);
    }

    ************************************************************************ */
}
