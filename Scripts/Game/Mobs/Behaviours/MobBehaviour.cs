using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public abstract class MobBehaviour : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<SFX> sfxs;

        private Mob owner;
        private AnimatorBody ownerAnimatorBody;

        private float enterDuration;
        private float enterDurationDelta;

        public virtual bool IsFinished 
        {
            get
            {
                return false;
            } 
        }

        public void Assign(Mob actor, AnimatorBody ownerAnimatorBody)
        {
            this.owner = actor;
            this.ownerAnimatorBody = ownerAnimatorBody;
        }

        public void Unassign()
        {
            this.owner = null;
            this.ownerAnimatorBody = null;
        }

        public virtual void Added() { }

        public virtual void Removed() { }

        public virtual void Enter() 
        {
            foreach (SFX sfx in this.sfxs) sfx.Play(this.OwnerAnimatorBody);
            this.enterDuration = 0;
            this.enterDurationDelta = 0;
        }

        public virtual void Exit() 
        {
            foreach (SFX sfx in this.sfxs) sfx.Stop();
        }

        public virtual void Refresh() 
        {
            this.enterDurationDelta = Time.deltaTime * (this.Owner.GetIsIgnoringTimeScale(this) ? 1f : this.Owner.RequestedTimeScale);
            this.enterDuration += this.enterDurationDelta;
        }

        protected Mob Owner
        {
            get
            {
                return this.owner;
            }
        }

        protected AnimatorBody OwnerAnimatorBody
        {
            get
            {
                return this.ownerAnimatorBody;
            }
        }

        protected float EnterDuration
        {
            get
            {
                return this.enterDuration;
            }
        }

        protected float EnterDurationDelta
        {
            get
            {
                return this.enterDurationDelta;
            }
        }
    }
}
