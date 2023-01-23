namespace FrigidBlackwaters.Game
{
    public class ResistBoxAnimatorProperty : DamageReceiverBoxAnimatorProperty<ResistBox, BreakBox, BreakInfo>
    {
        public Resistance DefensiveResistance
        {
            get
            {
                return this.DamageReceiverBox.DefensiveResistance;
            }
        }
    }
}
