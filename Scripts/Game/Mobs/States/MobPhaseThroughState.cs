using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobPhaseThroughState : MobActionState
    {
        [SerializeField]
        private MoveToTarget moveByDashingToTarget;

        private bool phasing;

        public override bool CanSetPosition
        {
            get
            {
                return false;
            }
        }

        public override bool CanSetTiledArea
        {
            get
            {
                return false;
            }
        }

        protected override void EnterSelf()
        {
            base.EnterSelf();
            StartPhasing();
        }

        protected override void ExitSelf()
        {
            base.ExitSelf();
            CancelPhasing();
        }

        private void BeginPhase()
        {
            if (CanPhaseThrough())
            {
                this.phasing = true;
                this.Owner.RequestPushMode(MobPushMode.IgnoreMobsTerrainAndObstacles);
            }
        }

        private void EndPhase()
        {
            if (this.phasing)
            {
                this.phasing = false;
                this.Owner.ReleasePushMode(MobPushMode.IgnoreMobsTerrainAndObstacles);
            }
        }

        private void StartPhasing()
        {
            this.phasing = false;
            if (!this.Owner.StopVelocities && this.Owner.SetForcedMove(this.moveByDashingToTarget, null, BeginPhase, EndPhase))
            {
                this.Owner.StopVelocities.OnFirstRequest += CancelPhasing;
            }
        }

        private void CancelPhasing()
        {
            EndPhase();
            if (this.Owner.ClearForcedMove(this.moveByDashingToTarget))
            {
                if (this.phasing) this.Owner.RelocateToTraversableSpace();
                this.Owner.StopVelocities.OnFirstRequest -= CancelPhasing;
            }
        }

        private bool CanPhaseThrough()
        {
            return Mob.CanFitAt(this.Owner.TiledArea, this.moveByDashingToTarget.DestinationPosition, this.Owner.Size, this.Owner.TraversableTerrain);
        }
    }
}
