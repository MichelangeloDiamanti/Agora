using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Activities
{
    /** A class representing an activity that one
     * or more characters can engage in */
    public class Activity : MonoBehaviour
    {
        public delegate void ActivityEvent(Activity act);
        public event ActivityEvent finished;

        protected void TriggerFinished()
        {
            if (finished != null) finished(this);
        }

        /** Weather or not this activity is occupied or not. */
        public bool Occupied
        { get { return occupied; } }
        private bool occupied = false;

        public HumanAgent currentAgent;

        public virtual bool RequestActivity(HumanAgent agent)
        {
            if (!occupied)
            {
                occupied = true;
                currentAgent = agent;
                return true;
            }
            return false;
        }

        public virtual void Release()
        {
            occupied = false;
            currentAgent = null;
        }

        protected virtual void startActivity(HumanAgent agent)
        {

        }

        public void StartActivity()
        {
            if (occupied && currentAgent != null)
            {
                startActivity(currentAgent);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            Color col1 = new Color(0, 1, 0, 0.2f);
            Color col2 = new Color(0, 1, 0, 1);
            if (occupied)
            {
                col1 = new Color(1, 0, 0, 0.2f);
                col2 = new Color(1, 0, 0, 1);
            }
            Vector3 scale = new Vector3(0.25f, 0.1f, 0.25f);
            Vector3 pos = transform.position;
            Gizmos.color = col1;
            Gizmos.DrawCube(pos, scale);
            Gizmos.color = col2;
            Gizmos.DrawWireCube(pos, scale);
            Gizmos.DrawLine(pos, pos + transform.forward/2f);
        }
    }
}
