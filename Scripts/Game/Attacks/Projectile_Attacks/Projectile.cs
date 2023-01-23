using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class Projectile : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private Rigidbody2D rigidbody;
        [SerializeField]
        private bool alignAlongTravelDirection;

        [Header("Wind Up")]
        [SerializeField]
        private bool hasWindUp;
        [SerializeField]
        [ShowIfBool("hasWindUp", true)]
        private string windUpAnimationName;

        [Header("Traveling")]
        [SerializeField]
        private bool hasMaxTravelDuration;
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
            Vector2 absoluteSpawnPosition,
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

            this.transform.position = absoluteSpawnPosition;
            this.launchDirection = launchDirection;

            if (TiledArea.TryGetTiledAreaAtPosition(absoluteSpawnPosition, out TiledArea tiledArea))
            {
                this.transform.SetParent(tiledArea.ContentsTransform);
                FrigidCoroutine.Run(ProjectileLifetime(damageBonus, onTeardown, onHitDealt, onBreakDealt, onWarningDealt, tiledArea), this.gameObject);
            }
            else
            {
                onTeardown?.Invoke();
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> ProjectileLifetime(
            int damageBonus,
            Action onTeardown,
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
                this.animatorBody.PlayByName(this.windUpAnimationName, () => { windUpFinished = true; });
                yield return new FrigidCoroutine.DelayWhile(() => { return !windUpFinished; });
            }

            void UpdateTravelVelocity(Vector2 calculatedVelocity)
            {
                this.rigidbody.velocity = calculatedVelocity;
            }

            this.animatorBody.PlayByName(this.travelingAnimationName);
            this.mover.OnVelocityUpdated += UpdateTravelVelocity;
            this.mover.AddMoves(this.moves);
            float elapsedTravelDuration = 0;
            float maxTravelDuration = this.hasMaxTravelDuration ? this.maxTravelDuration.ImmutableValue : float.MaxValue;
            while (!isBroken && elapsedTravelDuration < maxTravelDuration &&
                   TilePositioning.TileAbsolutePositionWithinBounds(this.transform.position, tiledArea.AbsoluteCenterPosition, tiledArea.WallAreaDimensions))
            {
                if (this.alignAlongTravelDirection)
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, this.mover.CalculatedVelocity.ComponentAngle0To360() * Mathf.Rad2Deg);
                }
                this.animatorBody.Direction = this.mover.CalculatedVelocity;
                elapsedTravelDuration += Time.deltaTime;
                yield return null;
            }
            this.mover.RemoveMoves(this.moves);
            this.mover.OnVelocityUpdated -= UpdateTravelVelocity;

            if (this.hasBreak)
            {
                bool breakFinished = false;
                this.animatorBody.PlayByName(this.breakAnimationName, () => { breakFinished = true; });
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

            onTeardown?.Invoke();
        }
    }
}
