using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MoveBySyncingToMover : Move
    {
        [SerializeField]
        private Mover otherMover;
        [SerializeField]
        private FloatSerializedReference speedMultiplier;

        public override Vector2 Velocity
        {
            get
            {
                return this.otherMover.CalculatedVelocity * this.speedMultiplier.ImmutableValue;
            }
        }
    }
}
