using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Experimental
{
    public class DotProd : MonoBehaviour
    {
        public GameObject agent;

        public GameObject point;

        public Color forwardColor = new Color(1, 0, 0, 1);

        public Color rightColor = new Color(0, 0, 1, 1);

        public Color toPointColor = new Color(0, 1, 0, 1);

        public Color forwardProjectColor = new Color(1, 1, 0, 1);

        public Color rightProjectColor = new Color(1, 0, 1, 1);

        private void Update()
        {
            Vector3 agentPos = agent.transform.position;
            Vector3 agentForward = agent.transform.forward.normalized;
            Vector3 agentRight = agent.transform.right.normalized;

            Vector3 pointPos = point.transform.position;
            Vector3 toPoint = pointPos - agentPos;

            float forwardProjectMagn = Vector3.Dot(agentForward, toPoint);
            Vector3 forwardProject = agentForward * forwardProjectMagn;

            float rightProjectMagn = Vector3.Dot(agentRight, toPoint);
            Vector3 rightProject = agentRight * rightProjectMagn;

            Debug.DrawLine(
                agentPos,
                agentPos + agentForward,
                forwardColor);

            Debug.DrawLine(
                agentPos,
                agentPos + agentRight,
                rightColor);

            Debug.DrawLine(
                agentPos,
                agentPos + toPoint,
                toPointColor);

            Debug.DrawLine(
                agentPos,
                agentPos + forwardProject,
                forwardProjectColor);

            Debug.DrawLine(
                agentPos,
                agentPos + rightProject,
                rightProjectColor);
        }
    }
}
