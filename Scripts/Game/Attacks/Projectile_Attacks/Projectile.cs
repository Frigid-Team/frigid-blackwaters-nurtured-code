using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : AttackBody
    {
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
        private bool alignAlongTravelDirection;
        [SerializeField]
        private string travelingAnimationName;
        [SerializeField]
        private Mover mover;
        [SerializeField]
        private List<MoveStep> moveSteps;

        [Header("Break")]
        [SerializeField]
        private bool hasBreak;
        [SerializeField]
        [ShowIfBool("hasBreak", true)]
        private string breakAnimationName;

        private Func<float, float, Vector2, Vector2, Vector2> toGetLaunchDirection;
        private float travelDuration;
        private float travelDurationDelta;

        public Vector2 LaunchDirection
        {
            get
            {
                return this.ToGetLaunchDirection.Invoke(this.travelDuration, this.travelDurationDelta, this.mover.Position, this.mover.CalculatedVelocity);
            }
        }

        public Func<float, float, Vector2, Vector2, Vector2> ToGetLaunchDirection
        {
            get
            {
                return this.toGetLaunchDirection;
            }
            set
            {
                this.toGetLaunchDirection = value;
            }
        }

        public float TravelDuration
        {
            get
            {
                return this.travelDuration;
            }
        }

        public float TravelDurationDelta
        {
            get
            {
                return this.travelDurationDelta;
            }
        }
        
        protected override IEnumerator<FrigidCoroutine.Delay> Lifetime(TiledArea tiledArea, Func<bool> toGetForceCompleted)
        {
            this.travelDuration = 0f;
            bool isBroken = false;
            Action<BreakInfo> toBreakProjectile = (BreakInfo breakInfo) => isBroken |= breakInfo.Broken;

            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.OnReceived += toBreakProjectile;
            }

            if (this.hasWindUp)
            {
                bool windUpFinished = false;
                this.AnimatorBody.Play(this.windUpAnimationName, () => windUpFinished = true);
                if (this.alignAlongLaunchDirection)
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, this.LaunchDirection.ComponentAngle0To360());
                }
                yield return new FrigidCoroutine.DelayWhile(() => !windUpFinished);
            }

            bool travelFinished = false;
            this.AnimatorBody.Play(this.travelingAnimationName, () => travelFinished = true);

            bool stopTravel = false;
            foreach (MoveStep moveStep in this.moveSteps)
            {
                foreach (Move move in moveStep.Moves) this.mover.AddMove(move, 0, false);
                float moveStepDuration = 0f;
                while (moveStep.IsOngoing(moveStepDuration, this.travelDurationDelta))
                {
                    if (this.alignAlongTravelDirection)
                    {
                        this.transform.rotation = Quaternion.Euler(0, 0, this.mover.CalculatedVelocity.ComponentAngle0To360());
                    }
                    if (travelFinished || toGetForceCompleted.Invoke() || isBroken || !AreaTiling.TilePositionWithinBounds(this.transform.position, tiledArea.CenterPosition, tiledArea.WallAreaDimensions))
                    {
                        stopTravel = true;
                        break;
                    }
                    this.AnimatorBody.Direction = this.mover.CalculatedVelocity;
                    this.travelDurationDelta = FrigidCoroutine.DeltaTime;
                    this.travelDuration += this.travelDurationDelta;
                    moveStepDuration += this.travelDurationDelta;
                    yield return null;
                }
                foreach (Move move in moveStep.Moves) this.mover.RemoveMove(move);

                if (stopTravel)
                {
                    break;
                }
            }

            if (this.hasBreak)
            {
                bool breakFinished = false;
                this.AnimatorBody.Play(this.breakAnimationName, () => breakFinished = true);
                yield return new FrigidCoroutine.DelayWhile(() => !breakFinished);
            }

            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.OnReceived -= toBreakProjectile;
            }
        }

        [Serializable]
        private class MoveStep
        {
            [SerializeField]
            public List<Move> moves;
            [SerializeField]
            public bool hasCondition;
            [SerializeField]
            [ShowIfBool("hasCondition", true)]
            public Conditional condition;

            public List<Move> Moves
            {
                get
                {
                    return this.moves;
                }
            }

            public bool IsOngoing(float elapsedDuration, float elapsedDurationDelta)
            {
                return !this.hasCondition || this.condition.Evaluate(elapsedDuration, elapsedDurationDelta);
            }
        }
    }
}
