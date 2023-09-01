using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class HurtBox : DamageReceiverBox<HurtBox, HitBox, HitInfo>
    {
        private int damageMitigation;
        private List<HitModifier> hitModifiers;

        public int DamageMitigation
        {
            get
            {
                return this.damageMitigation;
            }
            set
            {
                this.damageMitigation = value;
            }
        }

        public void AddHitModifier(HitModifier hitModifier)
        {
            this.hitModifiers.Add(hitModifier);
        }

        public void RemoveHitModifier(HitModifier hitModifier)
        {
            this.hitModifiers.Remove(hitModifier);
        }

        protected override HitInfo ProcessDamage(HitBox hitBox, Vector2 position, Vector2 direction, Collider2D collision)
        {
            return new HitInfo(
                hitBox.BaseDamageByReference.MutableValue,
                Mathf.FloorToInt(hitBox.DamageBonus * hitBox.BonusToDamageMultiplierByReference.MutableValue),
                Mathf.FloorToInt(this.damageMitigation * hitBox.MitigationToDamageMultiplierByReference.MutableValue),
                position,
                direction,
                this.hitModifiers,
                collision
                );
        }

        protected override void Awake()
        {
            base.Awake();
            this.damageMitigation = 0;
            this.hitModifiers = new List<HitModifier>();
        }
    }
}
