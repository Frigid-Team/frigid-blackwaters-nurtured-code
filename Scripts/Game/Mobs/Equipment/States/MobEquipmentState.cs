using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobEquipmentState : MobEquipmentStateNode
    {
        [SerializeField]
        private bool isFiring;
        [SerializeField]
        private Timer cooldown;
        [SerializeField]
        private string equipmentAnimationName;
        [SerializeField]
        private Direction facingDirection;

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

        public Timer Cooldown
        {
            get
            {
                return this.cooldown;
            }
        }

        public override void Enter()
        {
            base.Enter();
            if (!this.EquipmentPiece.EquipPoint.Equipper.CanAct) this.EquipmentAnimatorBody.Direction = this.EquipmentPiece.EquipPoint.Equipper.FacingDirection;
            else this.EquipmentAnimatorBody.Direction = this.facingDirection.Calculate(this.EquipmentPiece.FacingDirection, this.EnterDuration, this.EnterDurationDelta);
            this.equipmentAnimationFinished = false;
            this.EquipmentAnimatorBody.Play(this.equipmentAnimationName, () => this.equipmentAnimationFinished = true);
        }

        public override void Refresh()
        {
            base.Refresh();
            if (!this.EquipmentPiece.EquipPoint.Equipper.CanAct) this.EquipmentAnimatorBody.Direction = this.EquipmentPiece.EquipPoint.Equipper.FacingDirection;
            else this.EquipmentAnimatorBody.Direction = this.facingDirection.Calculate(this.EquipmentPiece.FacingDirection, this.EnterDuration, this.EnterDurationDelta);
        }
    }
}
