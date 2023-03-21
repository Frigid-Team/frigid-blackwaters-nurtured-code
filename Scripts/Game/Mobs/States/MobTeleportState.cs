using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobTeleportState : MobActionState
    {
        [SerializeField]
        private Targeter teleportTargeter;
        [SerializeField]
        private int teleportFrameIndex;

        private Vector2 teleportPosition;

        public override bool CanSetTiledArea
        {
            get
            {
                return false;
            }
        }

        public bool TryGetTeleportPosition(out Vector2 teleportPosition)
        {
            teleportPosition = this.teleportPosition;
            return this.EnteredSelf;
        }

        protected override void EnterSelf()
        {
            this.teleportPosition = this.teleportTargeter.Calculate(this.Owner.Position, this.SelfEnterDuration, this.SelfEnterDurationDelta);
            this.OwnerAnimatorBody.OnFrameUpdated += TeleportOnDesignatedFrame;
            base.EnterSelf();
            TeleportOnDesignatedFrame(0, this.OwnerAnimatorBody.CurrFrameIndex);
        }

        protected override void ExitSelf()
        {
            base.ExitSelf();
            this.OwnerAnimatorBody.OnFrameUpdated -= TeleportOnDesignatedFrame;
        }

        private void TeleportOnDesignatedFrame(int prevFrameIndex, int currFrameIndex)
        {
            if (currFrameIndex == this.teleportFrameIndex && this.Owner.CanMoveTo(this.teleportPosition))
            {
                this.Owner.MoveTo(this.teleportPosition);
            }
        }
    }
}
