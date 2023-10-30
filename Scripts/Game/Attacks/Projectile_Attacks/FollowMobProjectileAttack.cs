using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class FollowMobProjectileAttack : ProjectileAttack
    {
        [SerializeField]
        private ProjectileAttack sourceProjectileAttack;
        [SerializeField]
        private MobSerializedHandle mobToFollow;
        [Space]
        [SerializeField]
        private ProximityTriggeredBehaviour proximityTriggeredBehaviour;
        [SerializeField]
        [ShowIfInt("proximityTriggeredBehaviour", 0, false)]
        private FloatSerializedReference proximityDistance;
        [SerializeField]
        [ShowIfInt("proximityTriggeredBehaviour", 0, false)]
        private FloatSerializedReference triggerDuration;

        public override List<ProjectileSpawnSetting> GetSpawnSettings(TiledArea tiledArea, float elapsedDuration)
        {
            List<ProjectileSpawnSetting> followMobSpawnSettings = new List<ProjectileSpawnSetting>();
            if (!this.mobToFollow.TryGetValue(out Mob mobToFollow))
            {
                return followMobSpawnSettings;
            }

            foreach (ProjectileSpawnSetting sourceSpawnSetting in this.sourceProjectileAttack.GetSpawnSettings(tiledArea, elapsedDuration)) 
            {
                float triggerTime = float.NaN;
                float triggerDuration = this.triggerDuration.MutableValue;
                float proximitySqrDistance = this.proximityDistance.MutableValue;
                proximitySqrDistance *= proximitySqrDistance;
                Vector2 initialDirection = (mobToFollow.Position - sourceSpawnSetting.SpawnPosition).normalized;
                Vector2 GetLaunchDirection(float travelDuration, float travelDurationDelta, Vector2 position, Vector2 velocity)
                {
                    Vector2 followOffset = mobToFollow.Position - position;
                    Vector2 followDirection = followOffset.normalized;
                    if (float.IsNaN(triggerTime))
                    {
                        if (this.proximityTriggeredBehaviour == ProximityTriggeredBehaviour.StartFollowing)
                        {
                            followDirection = initialDirection;
                        }
                        if (followOffset.sqrMagnitude < proximitySqrDistance)
                        {
                            triggerTime = travelDuration;
                        }
                    }
                    else if (travelDuration >= triggerTime + triggerDuration)
                    {
                        if (this.proximityTriggeredBehaviour == ProximityTriggeredBehaviour.StopFollowing)
                        {
                            followDirection = velocity.normalized;
                        }
                    }
                    else
                    {
                        if (this.proximityTriggeredBehaviour == ProximityTriggeredBehaviour.StartFollowing)
                        {
                            followDirection = initialDirection;
                        }
                    }

                    if (followDirection.sqrMagnitude < Mathf.Epsilon)
                    {
                        return velocity.normalized;
                    }
                    return followDirection;
                }

                followMobSpawnSettings.Add(new ProjectileSpawnSetting(sourceSpawnSetting.SpawnPosition, sourceSpawnSetting.DelayDuration, sourceSpawnSetting.Prefab, GetLaunchDirection));
            }
            return followMobSpawnSettings;
        }

        private enum ProximityTriggeredBehaviour
        {
            None,
            StartFollowing,
            StopFollowing
        }
    }
}
