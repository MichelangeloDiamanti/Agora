using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.Maps
{
    /** A map that is computed by using rays.
     */ 
    public abstract class RayMap : GeneralHeatmap
    {
        /** A transform with the x and z position coordinates of the
         * first ray. Note that the height of the ray will be
         * determined by the y coordinate of the GameObject
         * this script is attached to.*/
        public Transform rayStart;

        /** The ray starting position.
         * This is constantly updated as
         * time progresses. */
        protected Vector3 _rayStart;

        /** Maximum time allowed for certain computations */
        public float maxTime = 0.001f;

        /** If set to true there will be no more than one ray drawn per frame */
        public bool slowMode = false;

        /** Time between rays in slow mode */
        public float slowModeTimeBetween = 1f;

        /** THe last time a ray was drawn */
        protected float lastRayTime = 0f;

        /** Applies settings
         * @param settings An UTSettings object object with
         * some parameters that might be used in this class */
        protected override void applySettings(UTSettings settings)
        {
            //TODO apply settings, perhaps in child classes
        }

        /** Do some initialization */
        protected override void _initialize()
        {
            int resX = (int)(size.x / cellSize);
            int resY = (int)(size.y / cellSize);
            mPaintGrid = new PaintGrid(resX, resY);

            /* Log a warning if raycastStart is missing */
            if (rayStart == null)
            {
                Debug.LogWarning(
                    "Please specify the raycasting start position for "
                    + gameObject.name + " (" + this.GetType().FullName + ")");
                rayStart = transform;
            }

            /* Set the first raycasting start position */
            _rayStart = rayStart.position;
            FixRayStart();
        }

        /** Correct the starting position of the ray. */
        protected virtual void FixRayStart()
        {
            /** It is important to fix the y coordinate of
             * the position vector because it may change
             * slightly over time when doing multiple
             * raycasts */
            _rayStart.y = transform.position.y;
        }

        /** Returns a value of the map at a position */
        public override float GetValueAt(Vector2 position, PathFollowingAgent agent)
        {
            Vector2Int gridpos = WorldToGridPos(position);
            if (!mPaintGrid.IsWithinGrid(gridpos.x, gridpos.y))
            {
                return mPaintGrid.defaultValue;
            }
            float val = mPaintGrid.GetValueAt(gridpos.x, gridpos.y);
            return val;
        }

        /** Unity OnDrawGizmos method */
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (rayStart == null) return;
            Vector3 rayStartPosition = rayStart.position;
            rayStartPosition.y = transform.position.y;
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(rayStart.position, rayStartPosition);
            Gizmos.DrawIcon(rayStartPosition, "RayStartIcon.png", false);
        }

        /** Perform the drawing of a single ray and do everything
         * needed for drawing that ray. Child classes should
         * implement this method. */
        protected abstract void DoSingleRay();

        /** Draws multiple rays */
        protected virtual void DoManyRays()
        {
            float startTime = Time.realtimeSinceStartup;
            float endTime = Time.realtimeSinceStartup;

            while ((float)(endTime - startTime) < maxTime)
            {
                DoSingleRay();
                endTime = Time.realtimeSinceStartup;
            }
        }

        /** Draw rays, either multiple or single, depending
         * on wether slowMode is true or false. */
        protected virtual void DoRays()
        {
            if (slowMode)
            {
                float curTime = Time.time;
                if ((curTime - lastRayTime) > slowModeTimeBetween)
                {
                    DoSingleRay();
                    lastRayTime = curTime;
                }
            }
            else
            {
                DoManyRays();
            }
        }

        /** Unity Update method */
        protected override void Update()
        {
            base.Update();
            if (initialized)
            {
                DoRays();
            }
        }
    }
}

