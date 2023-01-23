using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class HitBox : DamageDealerBox<HitBox, HurtBox, HitInfo>
    {
        [SerializeField]
        private IntSerializedReference baseDamage;

        private int damageBonus;

        public int BaseDamage
        {
            get
            {
                return this.baseDamage.ImmutableValue;
            }
        }

        public int DamageBonus
        {
            get
            {
                return this.damageBonus;
            }
            set
            {
                this.damageBonus = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.damageBonus = 0;
        }
    }
}
