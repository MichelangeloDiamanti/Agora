using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Agent
{
    public class MengeTrackedAgent : TrackedAgent
    {
        private Vector3 _lastPosition;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            _lastPosition = transform.position;
        }

        protected override void Update()
        {
            base.Update();

            ChangeInPositionEventDataStructure positionData = new ChangeInPositionEventDataStructure();
            positionData.AgentID = _ID;
            positionData.OldPosition = _lastPosition;
            positionData.NewPosition = transform.position;
            base.OnNewPosition.Invoke(positionData);

            _lastPosition = transform.position;
        }
    }
}