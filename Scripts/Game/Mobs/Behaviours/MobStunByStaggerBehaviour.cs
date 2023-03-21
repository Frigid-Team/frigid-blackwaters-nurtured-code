using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobStunByStaggerBehaviour : MobBehaviour
    {
        [SerializeField]
        private FloatSerializedReference stagger;

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
            this.Owner.StunByStagger(this.stagger.MutableValue);
        }
    }
}
