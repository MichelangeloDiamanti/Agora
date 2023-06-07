using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Activities
{
    /** A class representing a role that a character/agent
     * can take in an activity.
     */
    public class Role : MonoBehaviour
    {
        /** Triggers when a role is reserved for an agent. */
        public event System.Action roleBooked;

        /** Triggers when a role starts */
        public event System.Action roleStarted;

        /** Triggers when a role ends */
        public event System.Action roleEnded;

        /** Let the roleEnded event happen */
        public void TriggerRoleEnded()
        {
            roleEnded();
        }

        /** The activity that the role belongs to */
        public Activity activity;

        public HumanAgent currentAgent;

        public void StartRole()
        {
            if (roleStarted != null)
            {
                roleStarted();
            }
        }

        /** Weather or not this role is occupied or not. */
        public bool Occupied
        { get { return occupied; } }
        private bool occupied;

        /** Let some agent take this role
         * @return Returns this role if the role was successfully
         * taken. Returns null if it was already occupied.
         */
        public Role TakeRole(HumanAgent agent)
        {
            if (!occupied)
            {
                currentAgent = agent;
                occupied = true;
                if (roleBooked != null) roleBooked();
                return this;
            }
            return null;
        }
    }
}
