using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobStunForDurationBehaviour : MobBehaviour
    {
        [SerializeField]
        private FloatSerializedReference stunDuration;

        public override bool IsFinished
        {
            get
            {
                return true;
            }
        }

        public override void Enter()
        {
            base.Enter();
            this.Owner.StunForDuration(this.stunDuration.MutableValue);
        }
    }
}
