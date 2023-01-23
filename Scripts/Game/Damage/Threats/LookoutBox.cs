using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class LookoutBox : DamageReceiverBox<LookoutBox, ThreatBox, ThreatInfo>
    {
        protected override ThreatInfo ProcessDamage(ThreatBox threatBox, Vector2 position, Vector2 direction)
        {
            return new ThreatInfo(position, direction);
        }
    }
}
