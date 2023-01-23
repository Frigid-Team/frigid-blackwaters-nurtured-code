using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ThreatInfo
    {
        private float timeThreatened;
        private Vector2 threatPosition;
        private Vector2 threatDirection;

        public ThreatInfo(Vector2 threatPosition, Vector2 threatDirection)
        {
            this.timeThreatened = Time.time;
            this.threatPosition = threatPosition;
            this.threatDirection = threatDirection;
        }

        public float TimeWarned
        {
            get
            {
                return this.timeThreatened;
            }
        }

        public Vector2 ThreatPosition
        {
            get
            {
                return this.threatPosition;
            }
        }

        public Vector2 ThreatDirection
        {
            get
            {
                return this.threatDirection;
            }
        }
    }
}
