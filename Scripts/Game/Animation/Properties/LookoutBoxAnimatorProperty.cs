namespace FrigidBlackwaters.Game
{
    public class LookoutBoxAnimatorProperty : DamageReceiverBoxAnimatorProperty<LookoutBox, ThreatBox, ThreatInfo>
    {
        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.gameObject.layer = (int)FrigidLayer.LookoutBoxes;
            base.Created();
        }
    }
}
