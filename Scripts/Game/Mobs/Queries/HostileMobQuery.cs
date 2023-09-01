using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class HostileMobQuery : MobQuery
    {
        [SerializeField]
        private MobSerializedHandle hostileToMob;
        [SerializeField]
        private bool includeDeadMobs;

        protected override List<Mob> CustomExecute()
        {
            if (!this.hostileToMob.TryGetValue(out Mob hostileToMob))
            {
                return new List<Mob>();
            }

            HashSet<Mob> hostileMobs = new HashSet<Mob>();
            foreach (Mob activeMob in Mob.GetActiveMobsIn(hostileToMob.TiledArea))
            {
                if (hostileToMob != activeMob && hostileToMob.HostileAlignments.Contains(activeMob.Alignment) && (this.includeDeadMobs || activeMob.Status != MobStatus.Dead))
                {
                    hostileMobs.Add(activeMob);
                }
            }
            return hostileMobs.ToList();
        }
    }
}
