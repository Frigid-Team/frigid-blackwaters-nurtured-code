using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class AbilityResource : FrigidMonoBehaviourWithUpdate
    {
        private ControlCounter inUse;
        private float localTimeScale;

        private float localDeltaTime;

        public ControlCounter InUse
        {
            get
            {
                return this.inUse;
            }
        }

        public float LocalTimeScale
        {
            get
            {
                return this.localTimeScale;
            }
            set
            {
                this.localTimeScale = value;
            }
        }

        public abstract int Quantity
        {
            get;
        }

        public abstract float Progress
        {
            get;
        }

        public abstract bool Available
        {
            get;
        }

        protected override void Awake()
        {
            base.Awake();
            this.inUse = new ControlCounter();
            this.inUse.OnFirstRequest += this.Use;
            this.inUse.OnLastRelease += this.Unuse;
            this.localTimeScale = 1f;
        }

        protected override void Update()
        {
            base.Update();
            this.localDeltaTime = Time.deltaTime * this.localTimeScale;
        }

        protected float LocalDeltaTime
        {
            get
            {
                return this.localDeltaTime;
            }
        }

        protected virtual void Use() { }

        protected virtual void Unuse() { }
    }
}
