using UnityEngine;
using System.Collections.Generic;
using System;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;

        [Header("Wind Up")]
        [SerializeField]
        private bool hasWindUp;
        [SerializeField]
        [ShowIfBool("hasWindUp", true)]
        private bool alignAlongLaunchDirection;
        [SerializeField]
        [ShowIfBool("hasWindUp", true)]
        private string windUpAnimationName;

        [Header("Traveling")]
        [SerializeField]
        private bool hasMaxTravelDuration;
        [SerializeField]
        private bool alignAlongTravelDirection;
        [SerializeField]
        [ShowIfBool("hasMaxTravelDuration", true)]
        private FloatSerializedReference maxTravelDuration;
        [SerializeField]
        private string travelingAnimationName;
        [SerializeField]
        private Mover mover;
        [SerializeField]
        private List<Move> moves;

        [Header("Break")]
        [SerializeField]
        private bool hasBreak;
        [SerializeField]
        [ShowIfBool("hasBreak", true)]
        private string breakAnimationName;

        private Vector2 launchDirection;

        public Vector2 LaunchDirection
        {
            get
            {
                return this.launchDirection;
            }
        }

        public void LaunchProjectile(
            int damageBonus,
            DamageAlignment damageAlignment,
            Action onTeardown,
            Vector2 spawnPosition,
            Vector2 launchDirection,
            Action<HitInfo> onHitDealt,
            Action<BreakInfo> onBreakDealt,
            Action<ThreatInfo> onWarningDealt
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
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.DamageAlignment = damageAlignment;
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
            this.launchDirection = launchDirection;

            if (TiledArea.TryGetTiledAreaAtPosition(spawnPosition, out TiledArea tiledArea))
            {
                this.transform.SetParent(tiledArea.ContentsTransform);
                FrigidCoroutine.Run(
                    ProjectileLifetime(
                        damageBonus, 
                        () =>
                        {
                            this.transform.SetParent(null);
                            onTeardown?.Invoke();
                        },
                        onHitDealt,
                        onBreakDealt, 
                        onWarningDealt, 
                        tiledArea
                        ), 
                    this.gameObject
                    );
            }
            else
            {
                onTeardown?.Invoke();
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> ProjectileLifetime(
            int damageBonus,
            Action onComplete,
            Action<HitInfo> onHitDealt,
            Action<BreakInfo> onBreakDealt,
            Action<ThreatInfo> onThreatDealt,
            TiledArea tiledArea
            )
        {
            bool isBroken = false;
            Action<BreakInfo> toBreakProjectile = (BreakInfo breakInfo) => { isBroken |= breakInfo.Broken; };
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.OnDealt += onHitDealt;
                hitBoxProperty.DamageBonus += damageBonus;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.OnDealt += onBreakDealt;
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.OnReceived += toBreakProjectile;
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

            if (this.hasWindUp)
            {
                bool windUpFinished = false;
                this.animatorBody.Play(this.windUpAnimationName, () => { windUpFinished = true; });
                if (this.alignAlongLaunchDirection)
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, this.launchDirection.ComponentAngle0To2PI() * Mathf.Rad2Deg);
                }
                yield return new FrigidCoroutine.DelayWhile(() => { return !windUpFinished; });
            }

            this.animatorBody.Play(this.travelingAnimationName);
            foreach (Move move in this.moves) this.mover.AddMove(move, 0, false);
            float elapsedTravelDuration = 0;
            float maxTravelDuration = this.hasMaxTravelDuration ? this.maxTravelDuration.MutableValue : float.MaxValue;
            while (!isBroken && elapsedTravelDuration < maxTravelDuration &&
                   TilePositioning.TilePositionWithinBounds(this.transform.position, tiledArea.CenterPosition, tiledArea.WallAreaDimensions))
            {
                if (this.alignAlongTravelDirection)
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, this.mover.CalculatedVelocity.ComponentAngle0To2PI() * Mathf.Rad2Deg);
                }
                this.animatorBody.Direction = this.mover.CalculatedVelocity;
                elapsedTravelDuration += FrigidCoroutine.DeltaTime;
                yield return null;
            }
            foreach (Move move in this.moves) this.mover.RemoveMove(move);

            if (this.hasBreak)
            {
                bool breakFinished = false;
                this.animatorBody.Play(this.breakAnimationName, () => { breakFinished = true; });
                yield return new FrigidCoroutine.DelayWhile(() => { return !breakFinished; });
            }

            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.OnDealt -= onHitDealt;
                hitBoxProperty.DamageBonus -= damageBonus;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.OnDealt -= onBreakDealt;
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.OnReceived -= toBreakProjectile;
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
