using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class ClosestMobQuery : MobQuery
    {
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private MobQuery sortQuery;

        protected override List<Mob> CustomExecute()
        {
            Vector2 originPosition = this.originTargeter.Retrieve(Vector2.zero, 0, 0);
            List<Mob> sortMobs = this.sortQuery.Execute();
            sortMobs.Sort((Mob l, Mob r) => Mathf.RoundToInt(Vector2.SqrMagnitude(l.Position - originPosition) / FrigidConstants.WorldSizeEpsilon) - Mathf.RoundToInt(Vector2.SqrMagnitude(r.Position - originPosition) / FrigidConstants.WorldSizeEpsilon));
            return sortMobs;
        }
    }
}
