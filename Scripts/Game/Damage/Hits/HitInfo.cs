using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class HitInfo : DamageInfo
    {
        private const float ScratchDamagePercent = 0.15f;

        private Vector2 hitPosition;
        private Vector2 hitDirection;

        private int damage;
        private int increasedDamage;
        private int reducedDamage;
        private List<HitModifier> appliedHitModifiers;

        public HitInfo(int baseDamage, int bonusDamage, int mitigatedDamage, Vector2 hitPosition, Vector2 hitDirection, List<HitModifier> hitModifiers, Collider2D collision) : base(collision)
        {
            this.hitPosition = hitPosition;
            this.hitDirection = hitDirection;
            int incomingDamage = Mathf.Max(baseDamage + bonusDamage, 0);
            this.damage = Mathf.Max(Mathf.Max(incomingDamage - mitigatedDamage, 0), Mathf.CeilToInt(incomingDamage * ScratchDamagePercent));
            this.appliedHitModifiers = new List<HitModifier>();
            foreach (HitModifier hitModifier in hitModifiers)
            {
                if (hitModifier.TryApplyOnHit(hitPosition, hitDirection, ref this.damage, out bool evaluateFollowingModifiers))
                {
                    this.appliedHitModifiers.Add(hitModifier);
                    if (!evaluateFollowingModifiers)
                    {
                        break;
                    }
                }
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

        public List<HitModifier> AppliedHitModifiers
        {
            get
            {
                return this.appliedHitModifiers;
            }
        }
    }
}
