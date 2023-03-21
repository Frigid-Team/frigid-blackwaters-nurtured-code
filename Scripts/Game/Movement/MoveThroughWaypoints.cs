using UnityEngine;
using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MoveThroughWaypoints : Move
    {
        [SerializeField]
        private List<WaypointEntry> waypointEntries;
        [SerializeField]
        private Move subMove;

        private List<Vector2> waypoints;
        private int currentWaypointIndex;

        public override bool IsFinished
        {
            get
            {
                return this.currentWaypointIndex >= this.waypoints.Count;
            }
        }

        public override bool IsInMotion
        {
            get
            {
                return this.currentWaypointIndex < this.waypoints.Count && this.subMove.IsInMotion;
            }
        }

        public override Vector2 Velocity
        {
            get
            {
                return Vector2.zero;
            }
        }

        public bool TryGetNextWaypoint(out Vector2 waypoint)
        {
            if (this.currentWaypointIndex < this.waypoints.Count)
            {
                waypoint = this.waypoints[this.currentWaypointIndex];
                return true;
            }
            waypoint = Vector2.zero;
            return false;
        }

        public override void StartMoving()
        {
            base.StartMoving();
            this.Mover.AddMove(this.subMove, this.Mover.GetPriority(this), this.Mover.GetIsIgnoringTimeScale(this));
            foreach (WaypointEntry waypointEntry in this.waypointEntries)
            {
                this.waypoints.AddRange(waypointEntry.GetWaypoints(this.Mover.Position));
            }
            this.currentWaypointIndex = 0;
        }

        public override void ContinueMovement()
        {
            base.ContinueMovement();
            if (this.currentWaypointIndex < this.waypoints.Count && Vector2.Distance(this.waypoints[this.currentWaypointIndex], this.Mover.Position) <= this.subMove.Velocity.magnitude * this.MovingDurationDelta)
            {
                this.currentWaypointIndex++;
                if (this.currentWaypointIndex >= this.waypoints.Count)
                {
                    this.Mover.RemoveMove(this.subMove);
                }
            }
        }

        public override void StopMoving()
        {
            base.StopMoving();
            this.Mover.RemoveMove(this.subMove);
            this.waypoints.Clear();
            this.currentWaypointIndex = 0;
        }

        protected override void Awake()
        {
            base.Awake();
            this.waypoints = new List<Vector2>();
        }

        [Serializable]
        private class WaypointEntry
        {
            [SerializeField]
            private Targeter waypointTargeter;
            [SerializeField]
            private int numberWaypoints;

            public Vector2[] GetWaypoints(Vector2 currentPosition)
            {
                Vector2[] currentPositions = new Vector2[this.numberWaypoints];
                for (int i = 0; i < this.numberWaypoints; i++) currentPositions[i] = currentPosition;
                return this.waypointTargeter.Calculate(currentPositions, 0, 0);
            }
        }
    }
}
