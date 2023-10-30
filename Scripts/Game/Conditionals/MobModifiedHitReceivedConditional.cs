using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobModifiedHitReceivedConditional : MobDamageInfoConditional<HitInfo>
    {
        [SerializeField]
        private IntSerializedReference minimumDamage;
        [SerializeField]
        private List<HitModifier> hitModifiers;

        protected override LinkedList<HitInfo> GetDamageInfos(Mob mob)
        {
            return mob.HitsReceived;
        }

        protected override int TallyDamageInfo(HitInfo hitInfo)
        {
            return hitInfo.Damage >= this.minimumDamage.ImmutableValue && hitInfo.AppliedHitModifiers.Intersect(this.hitModifiers).Count() > 0 ? 1 : 0;
        }
    }
}
