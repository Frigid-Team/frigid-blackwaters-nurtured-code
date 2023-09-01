using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobHealBehaviour : MobBehaviour
    {
        [SerializeField]
        private IntSerializedReference flatHeal;
        [SerializeField]
        private FloatSerializedReference healPercentOfMaxHealth;

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
            this.Owner.Heal(this.flatHeal.MutableValue + Mathf.RoundToInt(this.healPercentOfMaxHealth.MutableValue * this.Owner.MaxHealth));
        }
    }
}