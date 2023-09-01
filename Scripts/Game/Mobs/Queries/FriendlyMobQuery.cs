using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class FriendlyMobQuery : MobQuery
    {
        [SerializeField]
        private MobSerializedHandle friendlyToMob;
        [SerializeField]
        private bool includeDeadMobs;

        protected override List<Mob> CustomExecute()
        {
            if (!this.friendlyToMob.TryGetValue(out Mob friendlyToMob))
            {
                return new List<Mob>();
            }

            HashSet<Mob> passiveMobs = new HashSet<Mob>();
            foreach (Mob activeMob in Mob.GetActiveMobsIn(friendlyToMob.TiledArea))
            {
                if (friendlyToMob != activeMob && friendlyToMob.FriendlyAlignments.Contains(activeMob.Alignment) && (this.includeDeadMobs || activeMob.Status != MobStatus.Dead))
                {
                    passiveMobs.Add(activeMob);
                }
            }
            return passiveMobs.ToList();
        }
    }
}
