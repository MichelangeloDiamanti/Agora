using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UrbanTerritoriality.GenAlg;

namespace UrbanTerritoriality.Agent
{
    /** A script for controlling the behavior of an
     * agent that can engage in many different activities.
     */
    public class HumanAgent : MonoBehaviour
    {
        /** Used for navigation and path following */
        public GPNMAgent navigatingAgent;

        /** An activity manager used for getting roles for
         * the agent */
        public Activities.ActivityManager activityMan;

        /** A SimpleMover component used for fine tweaking
         * the position and rotation of the agent */
        public SimpleMover simpleMove;

        /** The current activity that the agent is engaged in */
        private Activities.Activity currentActivity = null;

        /** The instance id of the last activity the agent
         * was engaged in.
         * */
        private int lastActivityInstanceID = -1;
        
        /** A PlayableDirector used for playing a timeline
         * for every activity that the agent is engaged in.
         * */
        public PlayableDirector director;

        /** Animator used for animating the agent. */
        public Animator animator;

        /** A method that gets called when a position
         * for an activity role is reached. */
        private void rolePositionReached()
        {
            currentActivity.finished += activityFinished;
            currentActivity.StartActivity();
        }

        /** A method that gets called when an activity is finished
         * @param act This should be the activity that got finished.
         */
        private void activityFinished(Activities.Activity act)
        {
            if (currentActivity.gameObject.GetInstanceID() ==
                act.gameObject.GetInstanceID())
            {
                currentActivity.finished -= activityFinished;
                currentActivity.Release();
                currentActivity = null;
                FindActivity();
            }
        }

        /**
         * Gets called when the destinationReached event is
         * fired on the navigationAgent object.
         * Makes the agent move more precisily to the role
         * position if the agent has an activity.
         */
        public void navDestReached()
        {
            if (currentActivity != null)
            {
                navigatingAgent.pathAgent.moveAgent.running = false;
                Vector3 rolePos = currentActivity.transform.position;
                rolePos.y = simpleMove.transform.position.y;
                simpleMove.TurnOn();
                simpleMove.destinationReached += rolePositionReached;
                simpleMove.MoveTo(rolePos, currentActivity.transform.rotation, true);
            }
        }

        /** Unity Start */
        private void Start()
        {
            navigatingAgent.destinationReached += navDestReached;
        }

        /** Makes a request to activityMan to get an available
         * activity */
        private void FindActivity()
        {
            currentActivity = activityMan.RequestRandomActivity(this);
            if (currentActivity != null)
            {
                int instanceID = currentActivity.gameObject.GetInstanceID();
                if (instanceID != lastActivityInstanceID)
                {
                    lastActivityInstanceID = instanceID;
                    navigatingAgent.Destination = currentActivity.transform;
                    return;
                }
            }
        }

        /** Unity Update */
        private void Update()
        {
            if (currentActivity == null)
            {
                FindActivity();
            }
        }
    }
}
