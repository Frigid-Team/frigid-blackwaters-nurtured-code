using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class Explosion : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private bool alignToSummonRotation;

        [Header("Exploding")]
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
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageAlignment = damageAlignment;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.DamageAlignment = damageAlignment;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.DamageAlignment = damageAlignment;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageAlignment = damageAlignment;
            }

            this.transform.position = spawnPosition;
            if (this.alignToSummonRotation) this.transform.rotation = Quaternion.Euler(0, 0, summonRotationDeg);

            if (TiledArea.TryGetTiledAreaAtPosition(spawnPosition, out TiledArea tiledArea))
            {
                this.transform.SetParent(tiledArea.ContentsTransform);
                FrigidCoroutine.Run(
                    ExplosionLifetime(
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
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.OnDealt += onHitDealt;
                hitBoxProperty.DamageBonus += damageBonus;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.OnDealt += onBreakDealt;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.OnDealt += onThreatDealt;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageBonus += damageBonus;
                attackProperty.OnHitDealt += onHitDealt;
                attackProperty.OnBreakDealt += onBreakDealt;
                attackProperty.OnThreatDealt += onThreatDealt;
            }

            bool explosionFinished = false;
            this.animatorBody.Play(this.explodeAnimationName, () => { explosionFinished = true; });
            yield return new FrigidCoroutine.DelayWhile(() => { return !explosionFinished; });

            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.OnDealt -= onHitDealt;
                hitBoxProperty.DamageBonus -= damageBonus;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.OnDealt -= onBreakDealt;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.OnDealt -= onThreatDealt;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>())
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
