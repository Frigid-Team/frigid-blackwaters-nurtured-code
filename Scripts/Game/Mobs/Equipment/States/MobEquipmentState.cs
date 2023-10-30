using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class MobEquipmentState : MobEquipmentStateNode
    {
        [SerializeField]
        private bool isFiring;

        private bool enteredSelf;
        private float selfEnterDuration;
        private float selfEnterDurationDelta;

        public bool IsFiring
        {
            get
            {
                return this.isFiring;
            }
        }

        public override void Spawn()
        {
            base.Spawn();
            this.enteredSelf = false;
        }

        public virtual void EnterSelf() 
        {
            this.enteredSelf = true;
            this.selfEnterDuration = 0f;
            this.selfEnterDurationDelta = 0f;
        }

        public virtual void ExitSelf() 
        {
            this.enteredSelf = false;
        }

        public virtual void RefreshSelf() 
        {
            this.selfEnterDurationDelta = Time.deltaTime * this.Owner.EquipPoint.Equipper.RequestedTimeScale;
            this.selfEnterDuration += this.selfEnterDurationDelta;
        }

        protected bool EnteredSelf
        {
            get
            {
                return this.enteredSelf;
            }
        }

        protected float SelfEnterDuration
        {
            get
            {
                return this.selfEnterDuration;
            }
        }

        protected float SelfEnterDurationDelta
        {
            get
            {
                return this.selfEnterDurationDelta;
            }
        }
    }
}
