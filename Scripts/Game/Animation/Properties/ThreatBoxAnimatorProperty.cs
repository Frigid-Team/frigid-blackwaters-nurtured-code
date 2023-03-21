namespace FrigidBlackwaters.Game
{
    public class ThreatBoxAnimatorProperty : DamageDealerBoxAnimatorProperty<ThreatBox, LookoutBox, ThreatInfo>
    {
        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.gameObject.layer = (int)FrigidLayer.ThreatBoxes;
            base.Created();
        }
    }
}
