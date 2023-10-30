using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class SprayBeamAttack : BeamAttack
    {
        [SerializeField]
        private Beam beamPrefab;
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private IntSerializedReference numberSprays;
        [Space]
        [SerializeField]
        private IntSerializedReference numberBeams;
        [SerializeField]
        private FloatSerializedReference angleBetween;
        [SerializeField]
        private FloatSerializedReference distanceFromSpawnPosition;
        [SerializeField]
        private FloatSerializedReference durationBetween;
        [SerializeField]
        private bool revolveClockwise;
        [SerializeField]
        private Direction emitDirection;
        [SerializeField]
        private FloatSerializedReference maxEmitDuration;

        public override List<BeamSpawnSetting> GetSpawnSettings(TiledArea tiledArea, float elapsedDuration)
        {
            List<BeamSpawnSetting> spawnSettings = new List<BeamSpawnSetting>();

            int currNumberSprays = this.numberSprays.MutableValue;
            Vector2[] originPositions = this.originTargeter.Retrieve(new Vector2[currNumberSprays], elapsedDuration, 0);
            foreach (Vector2 originPosition in originPositions)
            {
                int currNumberBeams = this.numberBeams.MutableValue;
                float currAngleBetweenBeams = this.angleBetween.MutableValue;
                float baseAngle = this.emitDirection.Retrieve(Vector2.zero, elapsedDuration, 0).ComponentAngle0To360() + (currNumberBeams % 2 == 0 ? currAngleBetweenBeams / 2 : 0);

                float delayDuration = 0;
                for (int i = 0; i < currNumberBeams; i++)
                {
                    float beamAngle = baseAngle + (this.revolveClockwise ? (currNumberBeams / 2 * currAngleBetweenBeams - i * currAngleBetweenBeams) : (-currNumberBeams / 2 * currAngleBetweenBeams + i * currAngleBetweenBeams));
                    float beamAngleRad = beamAngle * Mathf.Deg2Rad;
                    Vector2 emitDirection = new Vector2(Mathf.Cos(beamAngleRad), Mathf.Sin(beamAngleRad));
                    Vector2 spawnPosition = originPosition + emitDirection * this.distanceFromSpawnPosition.ImmutableValue;
                    float maxEmitDuration = this.maxEmitDuration.MutableValue;
                    Vector2[] GetPositions(float emitDuration, float emitDurationDelta)
                    {
                        if (emitDuration > maxEmitDuration)
                        {
                            return new Vector2[0];
                        }

                        Vector2 startPosition = spawnPosition;
                        Vector2 endPosition = AreaTiling.EdgePositionTowardDirection(startPosition, emitDirection, tiledArea.CenterPosition, tiledArea.MainAreaDimensions);
                        return new Vector2[] { startPosition, endPosition };
                    }

                    spawnSettings.Add(new BeamSpawnSetting(spawnPosition, delayDuration, this.beamPrefab, GetPositions));
                    delayDuration += this.durationBetween.MutableValue;
                }
            }

            return spawnSettings;
        }
    }
}
