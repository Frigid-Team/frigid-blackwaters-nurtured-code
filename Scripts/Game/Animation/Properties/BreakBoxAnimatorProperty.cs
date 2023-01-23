namespace FrigidBlackwaters.Game
{
    public class BreakBoxAnimatorProperty : DamageDealerBoxAnimatorProperty<BreakBox, ResistBox, BreakInfo>
    {
        public Resistance OffensiveResistance
        {
            get
            {
                return this.DamageDealerBox.OffensiveResistance;
            }
        }
    }
}
