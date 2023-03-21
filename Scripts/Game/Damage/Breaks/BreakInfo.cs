using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class BreakInfo : DamageInfo
    {
        private bool broken;

        public BreakInfo(Resistance offensiveResistance, Resistance defensiveResistance, Collider2D collision) : base(collision)
        {
            this.broken = ((int)offensiveResistance >= (int)defensiveResistance || defensiveResistance == Resistance.None) && defensiveResistance != Resistance.Unbreakable;
        }

        public override bool IsNonTrivial
        {
            get
            {
                return true;
            }
        }

        public bool Broken
        {
            get
            {
                return this.broken;
            }
        }
    }
}
