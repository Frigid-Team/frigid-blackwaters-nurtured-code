using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class Explosion : AttackBody
    {
        [Header("Exploding")]
        [SerializeField]
        private bool alignToSummonRotation;
        [SerializeField]
        private string explodeAnimationName;

        private float summonAngle;

        public float SummonAngle
        {
            get
            {
                return this.summonAngle;
            }
            set
            {
                this.summonAngle = value;
            }
        }

        protected override IEnumerator<FrigidCoroutine.Delay> Lifetime(TiledArea tiledArea, Func<bool> toGetForceCompleted)
        {
            if (this.alignToSummonRotation)
            {
                this.transform.rotation = Quaternion.Euler(0, 0, this.SummonAngle);
            }
            bool explosionFinished = false;
            this.AnimatorBody.Play(this.explodeAnimationName, () => explosionFinished = true);
            yield return new FrigidCoroutine.DelayWhile(() => !explosionFinished);
        }
    }
}
