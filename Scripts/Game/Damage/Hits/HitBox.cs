using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class HitBox : DamageDealerBox<HitBox, HurtBox, HitInfo>
    {
        [SerializeField]
        private IntSerializedReference baseDamage;
        [SerializeField]
        private FloatSerializedReference bonusToDamageMultiplier;
        [SerializeField]
        private FloatSerializedReference mitigationToDamageMultiplier;

        private int damageBonus;

        public IntSerializedReference BaseDamageByReference
        {
            get
            {
                return this.baseDamage;
            }
        }

        public FloatSerializedReference BonusToDamageMultiplierByReference
        {
            get
            {
                return this.bonusToDamageMultiplier;
            }
        }

        public FloatSerializedReference MitigationToDamageMultiplierByReference
        {
            get
            {
                return this.mitigationToDamageMultiplier;
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
