namespace FrigidBlackwaters.Game
{
    public class MobDamageHealthOnHitReceivedBehaviour : MobBehaviour
    {
        public override bool Finished
        {
            get
            {
                return false;
            }
        }

        public override void Apply()
        {
            base.Apply();
            this.Owner.DamageReceiver.OnHitReceived += DamageHealthOnHitReceived;
        }

        public override void Unapply()
        {
            base.Unapply();
            this.Owner.DamageReceiver.OnHitReceived -= DamageHealthOnHitReceived;
        }

        private void DamageHealthOnHitReceived(HitInfo hitInfo)
        {
            this.Owner.Health.Damage(hitInfo.Damage);
        }
    }
}
