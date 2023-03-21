namespace FrigidBlackwaters.Game
{
    public class MobDamageHealthOnHitReceivedBehaviour : MobBehaviour
    {
        public override void Enter()
        {
            base.Enter();
            this.Owner.OnHitReceived += DamageHealthOnHitReceived;
        }

        public override void Exit()
        {
            base.Exit();
            this.Owner.OnHitReceived -= DamageHealthOnHitReceived;
        }

        private void DamageHealthOnHitReceived(HitInfo hitInfo)
        {
            this.Owner.RemainingHealth -= hitInfo.Damage;
        }
    }
}
