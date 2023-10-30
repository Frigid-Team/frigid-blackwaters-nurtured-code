using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ThreatInfo : DamageInfo
    {
        private Vector2 threatPosition;
        private Vector2 threatDirection;

        public ThreatInfo(Vector2 threatPosition, Vector2 threatDirection, Collider2D collision) : base(collision)
        {
            this.threatPosition = threatPosition;
            this.threatDirection = threatDirection;
        }

        public override bool IsNonTrivial
        {
            get
            {
                return true;
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
