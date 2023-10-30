using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class AttackBody : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;

        public void DoLifetime(
            DamageAlignment damageAlignment,
            bool isIgnoringDamage,
            int damageBonus,
            TiledArea tiledArea,
            Vector2 spawnPosition,
            Action<HitInfo> onHitDealt,
            Action<BreakInfo> onBreakDealt,
            Action<ThreatInfo> onThreatDealt,
            ref Action toForceComplete,
            Action onComplete
            )
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageAlignment = damageAlignment;
                hitBoxProperty.IsIgnoringDamage = isIgnoringDamage;
            }
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.animatorBody.GetReferencedProperties<HurtBoxAnimatorProperty>())
            {
                hurtBoxProperty.DamageAlignment = damageAlignment;
                hurtBoxProperty.IsIgnoringDamage = isIgnoringDamage;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.DamageAlignment = damageAlignment;
                breakBoxProperty.IsIgnoringDamage = isIgnoringDamage;
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.DamageAlignment = damageAlignment;
                resistBoxProperty.IsIgnoringDamage = isIgnoringDamage;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.DamageAlignment = damageAlignment;
                threatBoxProperty.IsIgnoringDamage = isIgnoringDamage;
            }
            foreach (LookoutBoxAnimatorProperty lookoutBoxProperty in this.animatorBody.GetReferencedProperties<LookoutBoxAnimatorProperty>())
            {
                lookoutBoxProperty.DamageAlignment = damageAlignment;
                lookoutBoxProperty.IsIgnoringDamage = isIgnoringDamage;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageAlignment = damageAlignment;
                attackProperty.IsIgnoringDamage = isIgnoringDamage;
            }

            bool forceCompleted = false;
            toForceComplete += () => forceCompleted = true;

            this.transform.position = spawnPosition;
            this.transform.SetParent(tiledArea.ContentsTransform);

            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.OnDealt += onHitDealt;
                hitBoxProperty.DamageBonus += damageBonus;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.OnDealt += onBreakDealt;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.OnDealt += onThreatDealt;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageBonus += damageBonus;
                attackProperty.OnHitDealt += onHitDealt;
                attackProperty.OnBreakDealt += onBreakDealt;
                attackProperty.OnThreatDealt += onThreatDealt;
            }

            FrigidCoroutine.Run(
                this.CheckLifetimeCompletion(
                    tiledArea,
                    () => forceCompleted,
                    () =>
                    {
                        foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>())
                        {
                            hitBoxProperty.OnDealt -= onHitDealt;
                            hitBoxProperty.DamageBonus -= damageBonus;
                        }
                        foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>())
                        {
                            breakBoxProperty.OnDealt -= onBreakDealt;
                        }
                        foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>())
                        {
                            threatBoxProperty.OnDealt -= onThreatDealt;
                        }
                        foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>())
                        {
                            attackProperty.DamageBonus -= damageBonus;
                            attackProperty.OnHitDealt -= onHitDealt;
                            attackProperty.OnBreakDealt -= onBreakDealt;
                            attackProperty.OnThreatDealt -= onThreatDealt;
                        }

                        this.transform.SetParent(null);

                        onComplete?.Invoke();
                    }
                    ),
                this.gameObject
                );
        }

        protected AnimatorBody AnimatorBody
        {
            get
            {
                return this.animatorBody;
            }
        }

        protected abstract IEnumerator<FrigidCoroutine.Delay> Lifetime(TiledArea tiledArea, Func<bool> toGetForceCompleted);

#if UNITY_EDITOR
        protected override bool OwnsGameObject()
        {
            return true;
        }
#endif

        private IEnumerator<FrigidCoroutine.Delay> CheckLifetimeCompletion(TiledArea tiledArea, Func<bool> toGetForceCompleted, Action onComplete)
        {
            IEnumerator<FrigidCoroutine.Delay> lifetimeEnumerator = this.Lifetime(tiledArea, toGetForceCompleted);
            while (lifetimeEnumerator.MoveNext())
            {
                yield return lifetimeEnumerator.Current;
            }

            onComplete?.Invoke();
        }
    }
}
