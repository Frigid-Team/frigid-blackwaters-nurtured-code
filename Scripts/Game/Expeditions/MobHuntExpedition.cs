using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "MobHuntExpedition", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.EXPEDITIONS + "MobHuntExpedition")]
    public class MobHuntExpedition : SubGoalsExpedition<MobHuntExpedition.HuntSubGoal>
    {
        [Serializable]
        public class HuntSubGoal : SubGoal
        {
            [SerializeField]
            private MobSpawnable mobSpawnable;
            [SerializeField]
            private IntSerializedReference neededDeathCount;

            private List<Mob> huntedMobs;
            private int currentDeathCount;
            private Action onComplete;

            public override bool IsComplete
            {
                get
                {
                    return this.currentDeathCount >= this.neededDeathCount.ImmutableValue;
                }
            }

            public override void Start(Action onComplete)
            {
                base.Start(onComplete);
                this.huntedMobs = new List<Mob>();
                this.onComplete = onComplete;
                this.mobSpawnable.OnSpawned += this.AddHuntedMob;
                this.currentDeathCount = 0;
            }

            public override void End()
            {
                base.End();
                this.mobSpawnable.OnSpawned -= this.AddHuntedMob;
                foreach (Mob huntedMob in this.huntedMobs)
                {
                    huntedMob.OnStatusChanged -= this.CheckDeathCount;
                }
                this.huntedMobs = null;
                this.onComplete = null;
            }

            private void AddHuntedMob(Mob mob)
            {
                this.huntedMobs.Add(mob);
                mob.OnStatusChanged += this.CheckDeathCount;
                this.CheckDeathCount();
            }

            private void CheckDeathCount()
            {
                this.currentDeathCount = this.huntedMobs.Count((Mob huntedMob) => huntedMob.Status == MobStatus.Dead);
                if (this.currentDeathCount >= this.neededDeathCount.ImmutableValue)
                {
                    this.onComplete?.Invoke();
                }
            }
        }
    }
}
