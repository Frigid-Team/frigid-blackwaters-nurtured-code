using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class HitInfo : DamageInfo
    {
        private const float SCRATCH_DAMAGE_PERCENT = 0.15f;

        private float timeHit;
        private Vector2 hitPosition;
        private Vector2 hitDirection;

        private int damage;
        private int increasedDamage;
        private int reducedDamage;
        private bool hasHitModifier;
        private HitModifier hitModifier;

        public HitInfo(int baseDamage, int bonusDamage, int mitigatedDamage, Vector2 hitPosition, Vector2 hitDirection, List<HitModifier> hitModifiers, Collider2D collision) : base(collision)
        {
            this.timeHit = Time.time;
            this.hitPosition = hitPosition;
            this.hitDirection = hitDirection;
            int incomingDamage = Mathf.Max(baseDamage + bonusDamage, 0);
            this.damage = Mathf.Max(Mathf.Max(incomingDamage - mitigatedDamage, 0), Mathf.CeilToInt(incomingDamage * SCRATCH_DAMAGE_PERCENT));
            List<HitModifier> appliedHitModifiers = new List<HitModifier>();
            foreach (HitModifier hitModifier in hitModifiers)
            {
                if (hitModifier.ShouldApplyOnHit(hitPosition, hitDirection))
                {
                    appliedHitModifiers.Add(hitModifier);
                }
            }
            appliedHitModifiers.Sort((HitModifier first, HitModifier second) => { return second.Modification.Priority - second.Modification.Priority; });
            this.hasHitModifier = appliedHitModifiers.Count > 0;
            if (this.hasHitModifier)
            {
                this.hitModifier = appliedHitModifiers[0];
                this.damage = Mathf.FloorToInt(this.hitModifier.Modification.DamageMultiplier * this.damage);
            }
            this.increasedDamage = Mathf.Max(this.damage - baseDamage, 0);
            this.reducedDamage = Mathf.Max(incomingDamage - this.damage, 0);
        }

        public override bool IsNonTrivial
        {
            get
            {
                return this.damage > 0;
            }
        }

        public int Damage
        {
            get
            {
                return this.damage;
            }
        }
        
        public int IncreasedDamage
        {
            get
            {
                return this.increasedDamage;
            }
        }

        public int ReducedDamage
        {
            get
            {
                return this.reducedDamage;
            }
        }

        public float TimeHit
        {
            get
            {
                return this.timeHit;
            }
        }

        public Vector2 HitPosition
        {
            get
            {
                return this.hitPosition;
            }
        }

        public Vector2 HitDirection
        {
            get
            {
                return this.hitDirection;
            }
        }

        public bool TryGetHitModifier(out HitModifier hitModifier)
        {
            hitModifier = this.hitModifier;
            return this.hasHitModifier;
        }
    }
}
