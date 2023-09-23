using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UrbanTerritoriality.Agent
{
    /** A static class with some methods useful for agent behavior */
    public static class AgentUtil
    {
        //Returns the index of highest value in a list
        public static int IndexOfHighestValueFromList(List<float> listOfFloats)
        {
            float highestFloat = float.NegativeInfinity;
            int highestIndex = -1;
            if (listOfFloats.Count > 0) highestIndex = 0;
            for (int i = 0; i < listOfFloats.Count; i++)
            {
                if (listOfFloats[i] > highestFloat)
                {
                    highestFloat = listOfFloats[i];
                    highestIndex = i;
                }
            }
            return highestIndex;
        }

        //Generates a list of points infront of agent
        //Takes in a parameter for degree and distance in front of an agent to create a cone, number of points and an agent
        public static List<Vector3> GetListOfRandomPoints(float Degrees, float Distance, int numberOfPoints, Transform agent)
        {
            List<Vector3> returnList = new List<Vector3>();

            for (int i = 0; i < numberOfPoints; i++)
            {
                Vector3 endPoint = Random.insideUnitSphere;
                endPoint.y = 0f;
                endPoint.z = Mathf.Abs(endPoint.z);
                endPoint *= Distance;
                endPoint = agent.TransformPoint(endPoint);

                NavMeshHit hit;
                if (!NavMesh.SamplePosition(endPoint, out hit, Distance, NavMesh.AllAreas))
                {
                    // try again...
                    i -= 1;
                    continue;

                    // RECONSIDER - 
                    // The code works in practice but it might loop
                    // under unattentive modification of the surronding code.
                    // Better is to think of either a check of forced ultimation
                    // or find a more accurate way to sample positions onto the
                    // the navmesh.
                }
                endPoint = hit.position;
                returnList.Add(endPoint); //Put it in the list
                                          //Debug.Log(endPoint);
            }
            return returnList;
        }
    }
}

