using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class AbilityResource : FrigidMonoBehaviourWithUpdate
    {
        private ControlCounter inUse;
        private float localTimeScale;

        public ControlCounter InUse
        {
            get
            {
                return this.inUse;
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
            if (this.inUse) this.UpdateInUse();
            else this.UpdateOutOfUse();
        }

        protected virtual void Use() { }

        protected virtual void UpdateInUse() { }

        protected virtual void Unuse() { }

        protected virtual void UpdateOutOfUse() { }
    }
}
