using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using UrbanTerritoriality.Maps;

public class RandomPointPicker : MonoBehaviour
{
    public Transform agentTransform;
    private NavMeshHit hit;
    private bool blocked = false;

    private void Start()
    {
        List<float> list = new List<float>();
        list.Add(0.7f);
        list.Add(3.5f);
        list.Add(5.4f);
        list.Add(2.1f);

        //Debug.Log(list[IndexOfHighestValueFromList(list)]);

        //Debug.Log(list[IndexOfRandomValueFromList(list)]);
        //Debug.Log(list[IndexOfRandomValueFromList(list)]);

        List<Vector3> listOfPoints = new List<Vector3>();
        listOfPoints = GetListOfRandomPoints(90f, 30f, 5, agentTransform);
    }

    //Returns the index of highest value in a list
    int IndexOfHighestValueFromList(List<float> listOfFloats)
    {
        float highestFloat = 0f;
        int highestIndex = -1;
        for (int i = 0; i < listOfFloats.Count; i++)
        {
            if(listOfFloats[i] > highestFloat)
            {
                highestFloat = listOfFloats[i];
                highestIndex = i;
            }
        }
        return highestIndex;
    }

    //Returns the index of a random value from list
    //Larger values have higher chance to be picked
    int IndexOfRandomValueFromList(List<float> listOfFloats) 
    {

        float sum = 0f;
        for(int i = 0; i < listOfFloats.Count; i++)
        {
            sum += listOfFloats[i]; //Sum everything together
        }
        List<float> list = new List<float>();
        float tempNum = 0f;
        float random = Random.value;
        for(int i = 0; i < listOfFloats.Count; i++)
        {
            tempNum += listOfFloats[i] / sum;
            if(tempNum >= random)
            {
                return i;
            }
        }

        return 0;
    }

   

    //Generates a list of points infront of agent
    //Takes in a parameter for degree and distance in front of an agent to create a cone, number of points and an agent
    List<Vector3> GetListOfRandomPoints(float Degrees, float Distance, int numberOfPoints, Transform agent)
    {
        List<Vector3> returnList = new List<Vector3>();

        for(int i = 0; i < numberOfPoints; i++)
        {
            float tempDegree = Random.value * Degrees; //Random degree 
            float tempDistance = Random.value * Distance; //Random distance

            //Create a vector out of the random degree and distance
            Vector3 tempAgentForwardVector = agent.forward; //Temp variable for where the agent is looking
            float forwardFromDegrees = Degrees / 2; //Find the center of the cone, also agent.forward
            float degreeAwayFromAgentForward = tempDegree - forwardFromDegrees; //Find how many degrees it should turn away from the agent.forward
            tempAgentForwardVector = Quaternion.Euler(0, degreeAwayFromAgentForward, 0) * tempAgentForwardVector; //Turn the vector the same number of degrees

            Vector3 endPoint = agent.position + (tempAgentForwardVector.normalized * tempDistance); //Find the point in the world
            
            returnList.Add(endPoint); //Put it in the list
            //Debug.Log(endPoint);
        }
        return returnList;
    }

    List<float> GetCollidermapValuesFromPoints(GeneralHeatmap map, List<Vector3> listOfPoints)
    {
        List<float> listOfCollidermapValues = new List<float>();
        for (int i = 0; i < listOfPoints.Count; i++)
        {
            listOfCollidermapValues.Add(map.GetValueAt(listOfPoints[i]));
        }
        return listOfCollidermapValues;
    }

    List<Vector3> CheckIfPointsAreBlocked(List<Vector3> listOfPoints, Transform agent)
    {
        for(int i = 0; i < listOfPoints.Count; i++)
        {
            blocked = NavMesh.Raycast(agent.position, listOfPoints[i], out hit, NavMesh.AllAreas);
            Debug.DrawLine(transform.position, listOfPoints[i], blocked ? Color.red : Color.green);

            if (blocked)
            {
                listOfPoints.RemoveAt(i);
            }
        }
        return listOfPoints;
    }

    Vector3 GetPoint(Transform target, Transform agent, float degrees, float distance, int numberOfPoints, GeneralHeatmap map)
    {
        List<Vector3> listOfPoints = new List<Vector3>();
        listOfPoints = GetListOfRandomPoints(degrees, distance, numberOfPoints, agent); //Get random points

        listOfPoints = CheckIfPointsAreBlocked(listOfPoints, agent); //Remove blocked points

        List<float> listOfCollidermapValues = new List<float>();
        listOfCollidermapValues = GetCollidermapValuesFromPoints(map, listOfPoints); //Get values for each point from colidermap

        return listOfPoints[IndexOfRandomValueFromList(listOfCollidermapValues)]; //Get a random value and return the point behind it
    }
}
