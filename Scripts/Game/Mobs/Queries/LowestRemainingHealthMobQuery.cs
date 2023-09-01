using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class LowestRemainingHealthMobQuery : MobQuery
    {
        [SerializeField]
        private MobQuery sortQuery;

        protected override List<Mob> CustomExecute()
        {
            List<Mob> sortMobs = this.sortQuery.Execute();
            sortMobs.Sort((Mob l, Mob r) => l.RemainingHealth - r.RemainingHealth);
            return sortMobs;
        }
    }
}
