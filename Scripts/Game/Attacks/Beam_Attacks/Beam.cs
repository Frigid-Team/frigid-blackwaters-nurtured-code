using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class Beam : AttackBody
    {
        [SerializeField]
        private string linePointsFieldPropertyName;
        [SerializeField]
        private string pathPointsFieldPropertyName;
        [SerializeField]
        private string endPositionFieldPropertyName;
        [SerializeField]
        private string endRotationFieldPropertyName;

        [Header("Charge Up")]
        [SerializeField]
        private bool hasChargeUp;
        [SerializeField]
        [ShowIfBool("hasChargeUp", true)]
        private string chargeUpAnimationName;

        [Header("Emitting")]
        [SerializeField]
        private string emittingAnimationName;

        [Header("Charge Down")]
        [SerializeField]
        private bool hasChargeDown;
        [SerializeField]
        [ShowIfBool("hasChargeDown", true)]
        private string chargeDownAnimationName;

        private Func<float, float, Vector2[]> toGetPositions;
        private float emitDuration;
        private float emitDurationDelta;

        public Vector2[] Positions
        {
            get
            {
                return this.ToGetPositions.Invoke(this.emitDuration, this.emitDurationDelta);
            }
        }

        public Func<float, float, Vector2[]> ToGetPositions
        {
            get
            {
                return this.toGetPositions;
            }
            set
            {
                this.toGetPositions = value;
            }
        }

        public float EmitDuration
        {
            get
            {
                return this.emitDuration;
            }
        }

        public float EmitDurationDelta
        {
            get
            {
                return this.emitDurationDelta;
            }
        }

        protected override IEnumerator<FrigidCoroutine.Delay> Lifetime(TiledArea tiledArea, Func<bool> toGetForceCompleted)
        {
            this.emitDuration = 0;

            if (this.AnimatorBody.TryFindReferencedProperty<LinePointsFieldAnimatorProperty>(this.linePointsFieldPropertyName, out LinePointsFieldAnimatorProperty linePointsFieldProperty) &&
                this.AnimatorBody.TryFindReferencedProperty<PathPointsFieldAnimatorProperty>(this.pathPointsFieldPropertyName, out PathPointsFieldAnimatorProperty pathPointsFieldProperty) &&
                this.AnimatorBody.TryFindReferencedProperty<LocalPositionFieldAnimatorProperty>(this.endPositionFieldPropertyName, out LocalPositionFieldAnimatorProperty endPositionFieldProperty) &&
                this.AnimatorBody.TryFindReferencedProperty<LocalRotationFieldAnimatorProperty>(this.endRotationFieldPropertyName, out LocalRotationFieldAnimatorProperty endRotationFieldProperty))
            {
                bool UpdateBeamPath()
                {
                    Vector2[] positions = this.Positions;
                    if (positions.Length > 1)
                    {
                        Vector2 originPosition = positions[0];
                        this.transform.position = originPosition;

                        Vector2[] relativePositions = new Vector2[positions.Length];
                        for (int i = 0; i < positions.Length; i++)
                        {
                            relativePositions[i] = positions[i] - originPosition;
                        }
                        linePointsFieldProperty.LinePoints = relativePositions;
                        pathPointsFieldProperty.PathCount = 1;
                        pathPointsFieldProperty.SetPath(0, relativePositions);
                        endPositionFieldProperty.LocalPosition = relativePositions[relativePositions.Length - 1];
                        endRotationFieldProperty.LocalRotation = (positions[positions.Length - 1] - positions[positions.Length - 2]).ComponentAngleSigned();

                        return true;
                    }
                    return false;
                }

                if (UpdateBeamPath())
                {
                    if (this.hasChargeUp)
                    {
                        bool chargeUpFinished = false;
                        this.AnimatorBody.Play(this.chargeUpAnimationName, () => chargeUpFinished = true);
                        yield return new FrigidCoroutine.DelayWhile(() => !chargeUpFinished);
                    }

                    bool emitFinished = false;
                    this.AnimatorBody.Play(this.emittingAnimationName, () => emitFinished = true);
                    while (!emitFinished && !toGetForceCompleted.Invoke() && UpdateBeamPath())
                    {
                        this.emitDurationDelta = FrigidCoroutine.DeltaTime;
                        this.emitDuration += this.emitDurationDelta;
                        yield return null;
                    }

                    if (this.hasChargeDown)
                    {
                        bool chargeDownFinished = false;
                        this.AnimatorBody.Play(this.chargeDownAnimationName, () => chargeDownFinished = true);
                        yield return new FrigidCoroutine.DelayWhile(() => !chargeDownFinished);
                    }
                }
            }
            else
            {
                Debug.LogError("Beam " + this.name + " could not find a needed parameter property.");
            }
        }
    }
}
