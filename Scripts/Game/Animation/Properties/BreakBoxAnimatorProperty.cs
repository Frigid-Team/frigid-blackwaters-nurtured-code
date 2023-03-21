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

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.gameObject.layer = (int)FrigidLayer.BreakBoxes;
            base.Created();
        }
    }
}
