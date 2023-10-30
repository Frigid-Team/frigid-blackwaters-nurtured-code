using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class BeamAttack : AttackBodyAttack<BeamAttack.BeamSpawnSetting, Beam>
    {
        public class BeamSpawnSetting : SpawnSetting
        {
            private Func<float, float, Vector2[]> toGetPositions;

            public BeamSpawnSetting(Vector2 spawnPosition, float delayDuration, Beam beamPrefab, Func<float, float, Vector2[]> toGetPositions) : base(spawnPosition, delayDuration, beamPrefab)
            {
                this.toGetPositions = toGetPositions;
                this.OnBodyInitialized += (Beam beam) => beam.ToGetPositions = toGetPositions;
            }

            public Func<float, float, Vector2[]> ToGetPositions
            {
                get
                {
                    return this.toGetPositions;
                }
            }
        }
    }
}
