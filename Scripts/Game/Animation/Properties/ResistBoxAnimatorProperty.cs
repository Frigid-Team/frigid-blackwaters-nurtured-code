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

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.gameObject.layer = (int)FrigidLayer.ResistBoxes;
            base.Created();
        }
    }
}
