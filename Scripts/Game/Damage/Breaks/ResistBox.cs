using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ResistBox : DamageReceiverBox<ResistBox, BreakBox, BreakInfo>
    {
        [SerializeField]
        private ResistanceSerializedReference defensiveResistance;

        private Action onBroken;

        public Resistance DefensiveResistance
        {
            get
            {
                return this.defensiveResistance.ImmutableValue;
            }
        }

        public Action OnBroken
        {
            get
            {
                return this.onBroken;
            }
            set
            {
                this.onBroken = value;
            }
        }
        protected override BreakInfo ProcessDamage(BreakBox breakBox, Vector2 position, Vector2 direction, Collider2D collision)
        {
            BreakInfo breakInfo = new BreakInfo(breakBox.OffensiveResistance, this.DefensiveResistance, collision);
            if (breakInfo.Broken) this.onBroken?.Invoke();
            return breakInfo;
        }
    }
}
