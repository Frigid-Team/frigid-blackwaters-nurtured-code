using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobEquipmentState : MobEquipmentStateNode
    {
        [SerializeField]
        private bool isFiring;
        [SerializeField]
        private AbilityResource activeAbilityResource;
        [SerializeField]
        private string equipmentAnimationName;
        [SerializeField]
        private Direction facingDirection;
        [Space]
        [SerializeField]
        private List<MobStatusTag> statusTags;

        private bool equipmentAnimationFinished;

        public override MobEquipmentState InitialState
        {
            get
            {
                return this;
            }
        }

        public override HashSet<MobEquipmentStateNode> ReferencedStateNodes
        {
            get
            {
                return new HashSet<MobEquipmentStateNode>();
            }
        }

        public override bool AutoExit
        {
            get
            {
                return this.equipmentAnimationFinished;
            }
        }

        public bool IsFiring
        {
            get
            {
                return this.isFiring;
            }
        }

        public AbilityResource ActiveAbilityResource
        {
            get
            {
                return this.activeAbilityResource;
            }
        }

        public override void Enter()
        {
            base.Enter();
            if (!this.Owner.EquipPoint.Equipper.IsActingAndNotStunned) this.OwnerAnimatorBody.Direction = this.Owner.EquipPoint.Equipper.FacingDirection;
            else this.OwnerAnimatorBody.Direction = this.facingDirection.Retrieve(this.Owner.FacingDirection, this.EnterDuration, this.EnterDurationDelta);
            this.equipmentAnimationFinished = false;
            this.OwnerAnimatorBody.Play(this.equipmentAnimationName, () => this.equipmentAnimationFinished = true);
            foreach (MobStatusTag statusTag in this.statusTags)
            {
                this.Owner.EquipPoint.Equipper.AddStatusTag(statusTag);
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            if (!this.Owner.EquipPoint.Equipper.IsActingAndNotStunned) this.OwnerAnimatorBody.Direction = this.Owner.EquipPoint.Equipper.FacingDirection;
            else this.OwnerAnimatorBody.Direction = this.facingDirection.Retrieve(this.Owner.FacingDirection, this.EnterDuration, this.EnterDurationDelta);
            foreach (MobStatusTag statusTag in this.statusTags)
            {
                this.Owner.EquipPoint.Equipper.RemoveStatusTag(statusTag);
            }
        }
    }
}
