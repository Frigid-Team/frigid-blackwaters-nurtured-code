using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class Explosion : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;

        [Header("Exploding")]
        [SerializeField]
        private bool alignToSummonRotation;
        [SerializeField]
        private string explodeAnimationName;

        public void SummonExplosion(
            int damageBonus,
            DamageAlignment damageAlignment,
            Action onTeardown,
            Vector2 spawnPosition,
            float summonRotationDeg,
            Action<HitInfo> onHitDealt,
            Action<BreakInfo> onBreakDealt,
            Action<ThreatInfo> onThreatDealt
            )
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageAlignment = damageAlignment;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.DamageAlignment = damageAlignment;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.DamageAlignment = damageAlignment;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageAlignment = damageAlignment;
            }

            this.transform.position = spawnPosition;
            if (this.alignToSummonRotation) this.transform.rotation = Quaternion.Euler(0, 0, summonRotationDeg);

            if (TiledArea.TryGetAreaAtPosition(spawnPosition, out TiledArea tiledArea))
            {
                this.transform.SetParent(tiledArea.ContentsTransform);

                FrigidCoroutine.Run(
                    this.ExplosionLifetime(
                        damageBonus, 
                        () =>
                        {
                            this.transform.SetParent(null);
                            onTeardown?.Invoke();
                        }, 
                        onHitDealt, 
                        onBreakDealt, 
                        onThreatDealt
                        ), 
                    this.gameObject
                    );
            }
            else
            {
                onTeardown?.Invoke();
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> ExplosionLifetime(
            int damageBonus,
            Action onComplete,
            Action<HitInfo> onHitDealt,
            Action<BreakInfo> onBreakDealt,
            Action<ThreatInfo> onThreatDealt
            )
        {
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

            bool explosionFinished = false;
            this.animatorBody.Play(this.explodeAnimationName, () => { explosionFinished = true; });
            yield return new FrigidCoroutine.DelayWhile(() => { return !explosionFinished; });

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

            onComplete?.Invoke();
        }
    }
}
