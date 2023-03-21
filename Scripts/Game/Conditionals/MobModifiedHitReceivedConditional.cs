using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobModifiedHitReceivedConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private List<HitModifier> hitModifiers;
        [SerializeField]
        private FloatSerializedReference lastingDuration;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            if(this.mob.ImmutableValue.HitsReceived.Count <= 0)
            {
                return false;
            }

            LinkedListNode<HitInfo> hit = this.mob.ImmutableValue.HitsReceived.First;
            while (Time.time - hit.Value.TimeHit < this.lastingDuration.ImmutableValue + Time.deltaTime)
            {
                if (hit.Value.TryGetHitModifier(out HitModifier hitModifierInHit))
                {
                    if (this.hitModifiers.Contains(hitModifierInHit)) return true;
                }

                if (hit.Next == null) break;
                hit = hit.Next;
            }

            return false;
        }
    }
}
