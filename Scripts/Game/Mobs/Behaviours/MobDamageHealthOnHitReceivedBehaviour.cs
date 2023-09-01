namespace FrigidBlackwaters.Game
{
    public class MobDamageHealthOnHitReceivedBehaviour : MobBehaviour
    {
        public override void Enter()
        {
            base.Enter();
            this.Owner.OnHitReceived += this.DamageHealthOnHitReceived;
        }

        public override void Exit()
        {
            base.Exit();
            this.Owner.OnHitReceived -= this.DamageHealthOnHitReceived;
        }

        private void DamageHealthOnHitReceived(HitInfo hitInfo)
        {
            this.Owner.Damage(hitInfo.Damage);
        }
    }
}
