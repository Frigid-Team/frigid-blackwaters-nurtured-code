using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class LookoutBox : DamageReceiverBox<LookoutBox, ThreatBox, ThreatInfo>
    {
        protected override ThreatInfo ProcessDamage(ThreatBox threatBox, Vector2 position, Vector2 direction, Collider2D collision)
        {
            return new ThreatInfo(position, direction, collision);
        }
    }
}
