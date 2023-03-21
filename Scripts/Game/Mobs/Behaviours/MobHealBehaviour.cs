using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobHealBehaviour : MobBehaviour
    {
        [SerializeField]
        private IntSerializedReference heal;

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
            this.Owner.RemainingHealth += this.heal.ImmutableValue;
        }
    }
}