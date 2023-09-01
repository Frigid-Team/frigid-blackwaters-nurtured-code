using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobDamageBehaviour : MobBehaviour
    {
        [SerializeField]
        private IntSerializedReference flatDamage;
        [SerializeField]
        private FloatSerializedReference damagePercentOfMaxHealth;

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
            this.Owner.Damage(this.flatDamage.MutableValue + Mathf.RoundToInt(this.damagePercentOfMaxHealth.MutableValue * this.Owner.MaxHealth));
        }
    }
}