using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobEquipmentActionState : MobEquipmentState
    {
        [SerializeField]
        private string equipmentAnimationName;
        [SerializeField]
        private Direction facingDirection;

        private bool equipmentAnimationFinished;

        public override bool AutoEnter
        {
            get
            {
                return false;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return this.equipmentAnimationFinished;
            }
        }

        public override void EnterSelf()
        {
            base.EnterSelf();
            if (!this.Owner.EquipPoint.Equipper.IsActingAndNotStunned) this.OwnerAnimatorBody.Direction = this.Owner.EquipPoint.Equipper.FacingDirection;
            else this.OwnerAnimatorBody.Direction = this.facingDirection.Retrieve(this.Owner.FacingDirection, this.SelfEnterDuration, this.SelfEnterDurationDelta);
            this.equipmentAnimationFinished = false;
            this.OwnerAnimatorBody.Play(this.equipmentAnimationName, () => this.equipmentAnimationFinished = true);
        }

        public override void RefreshSelf()
        {
            base.RefreshSelf();
            if (!this.Owner.EquipPoint.Equipper.IsActingAndNotStunned) this.OwnerAnimatorBody.Direction = this.Owner.EquipPoint.Equipper.FacingDirection;
            else this.OwnerAnimatorBody.Direction = this.facingDirection.Retrieve(this.Owner.FacingDirection, this.SelfEnterDuration, this.SelfEnterDurationDelta);
        }

        protected override MobEquipmentStateNode SpawnStateNode
        {
            get
            {
                return this;
            }
        }

        protected override HashSet<MobEquipmentStateNode> ChildStateNodes
        {
            get
            {
                return new HashSet<MobEquipmentStateNode>();
            }
        }
    }
}
