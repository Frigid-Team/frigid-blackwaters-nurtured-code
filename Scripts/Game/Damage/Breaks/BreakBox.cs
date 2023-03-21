using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class BreakBox : DamageDealerBox<BreakBox, ResistBox, BreakInfo> 
    {
        [SerializeField]
        private ResistanceSerializedReference offensiveResistance;

        public Resistance OffensiveResistance
        {
            get
            {
                return this.offensiveResistance.ImmutableValue;
            }
        }
    }
}
