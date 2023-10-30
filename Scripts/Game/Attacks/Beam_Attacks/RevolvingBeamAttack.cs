using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RevolvingBeamAttack : BeamAttack
    {
        [SerializeField]
        private Beam beamPrefab;
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private IntSerializedReference numberRevolutions;
        [Space]
        [SerializeField]
        private IntSerializedReference numberBeams;
        [SerializeField]
        private FloatSerializedReference durationBetween;
        [SerializeField]
        private FloatSerializedReference undershootAngle;
        [SerializeField]
        private FloatSerializedReference overshootAngle;
        [SerializeField]
        private AnimationCurveSerializedReference revolutionCurve;
        [SerializeField]
        private FloatSerializedReference revolutionDuration;
        [SerializeField]
        private Direction destinationDirection;

        public override List<BeamSpawnSetting> GetSpawnSettings(TiledArea tiledArea, float elapsedDuration)
        {
            List<BeamSpawnSetting> spawnSettings = new List<BeamSpawnSetting>();

            int currNumberRevolutions = this.numberRevolutions.MutableValue;
            Vector2[] originPositions = this.originTargeter.Retrieve(new Vector2[currNumberRevolutions], elapsedDuration, 0f);
            foreach (Vector2 originPosition in originPositions)
            {
                int currNumberBeams = this.numberBeams.MutableValue;
                float delayDuration = 0;
                for (int i = 0; i < currNumberBeams; i++)
                {
                    Vector2 destinationDirection = this.destinationDirection.Retrieve(Vector2.zero, elapsedDuration + delayDuration, 0f);
                    float undershootAngle = this.undershootAngle.MutableValue;
                    float overshootAngle = this.overshootAngle.MutableValue;
                    float totalAngle = undershootAngle + overshootAngle;
                    AnimationCurve revolutionCurve = this.revolutionCurve.MutableValue;
                    float revolutionDuration = this.revolutionDuration.MutableValue;
                    Vector2[] GetPositions(float emitDuration, float emitDurationDelta)
                    {
                        if (emitDuration > revolutionDuration)
                        {
                            return new Vector2[0];
                        }

                        float totalDuration = elapsedDuration + emitDuration + delayDuration;
                        float currentAngle = (1f - revolutionCurve.Evaluate(emitDuration / revolutionDuration)) * totalAngle - overshootAngle;
                        Vector2 startPosition = originPosition;
                        Vector2 endPosition = AreaTiling.EdgePositionTowardDirection(startPosition, destinationDirection.RotateAround(currentAngle), tiledArea.CenterPosition, tiledArea.MainAreaDimensions);
                        return new Vector2[] { startPosition, endPosition };
                    }
                    spawnSettings.Add(new BeamSpawnSetting(originPosition, delayDuration, this.beamPrefab, GetPositions));

                    delayDuration += this.durationBetween.MutableValue;
                }
            }

            return spawnSettings;
        }
    }
}
