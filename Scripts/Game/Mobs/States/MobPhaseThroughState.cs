using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobPhaseThroughState : MobActionState
    {
        [SerializeField]
        private MoveToTarget moveToTarget;

        private bool phasing;

        public override bool MovePositionSafe
        {
            get
            {
                return false;
            }
        }

        public override bool MoveTiledAreaSafe
        {
            get
            {
                return false;
            }
        }

        public override void EnterSelf()
        {
            base.EnterSelf();
            this.StartPhasing();
        }

        public override void ExitSelf()
        {
            base.ExitSelf();
            this.CancelPhasing();
        }

        private void BeginPhase()
        {
            if (this.CanPhaseThrough())
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
            if (!this.Owner.StopVelocities && this.Owner.SetForcedMove(this.moveToTarget, null, this.BeginPhase, this.EndPhase))
            {
                this.Owner.StopVelocities.OnFirstRequest += this.CancelPhasing;
            }
        }

        private void CancelPhasing()
        {
            this.EndPhase();
            if (this.Owner.ClearForcedMove(this.moveToTarget))
            {
                if (this.phasing) this.Owner.RelocateToTraversableSpace();
                this.Owner.StopVelocities.OnFirstRequest -= this.CancelPhasing;
            }
        }

        private bool CanPhaseThrough()
        {
            return Mob.CanTraverseAt(this.Owner.TiledArea, this.moveToTarget.DestinationPosition, this.Owner.Size, this.Owner.TraversableTerrain);
        }
    }
}
