using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Utilities;

namespace UrbanTerritoriality.Agent
{
    /** A script that sets some animator paramters
     * so that the right animation clips are played
     * when the character moves around in a scene.
     */
    public class HumanLocomotion : MonoBehaviour
    {
        /** A ObjectChaser component to get
         * the speed and rotation from */
        public ObjectChaser agent;

        /** The animmator component that will be used */
        public Animator animator;

        /* Hash for speed in the z direction in the animator. */
        private int animHash_SpeedZ;

        /** Hash for a speed paramater in the animator */
        private int animHash_SpeedX;

        /** Hash for an angular speed paramters in the animator */
        private int animHash_AngularSpeed;

        /** Unity Start method */
        void Start()
        {
            animHash_SpeedZ = Animator.StringToHash("ZSpeed");
            animHash_SpeedX = Animator.StringToHash("XSpeed");
            animHash_AngularSpeed = Animator.StringToHash("AngularSpeed");
        }

        /** Unity Update method */
        void Update()
        {
            /* Set some animator values */
            animator.SetFloat(animHash_SpeedZ, agent.SpeedZ);
            animator.SetFloat(animHash_SpeedX, agent.SpeedX);
            animator.SetFloat(animHash_AngularSpeed,
                Util.DegToRadian(agent.AngularSpeed));
        }
    }
}

