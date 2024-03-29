using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class MobState : MobStateNode
    {
        [SerializeField]
        private Vector2 size;
        [SerializeField]
        private float height;
        [SerializeField]
        private TraversableTerrain traversableTerrain;
        [SerializeField]
        private bool showDisplays;

        private bool enteredSelf;
        private float selfEnterDuration;
        private float selfEnterDurationDelta;

        public Vector2 Size
        {
            get
            {
                return this.size;
            }
        }

        public Vector2Int TileSize
        {
            get
            {
                return new Vector2Int(Mathf.Max(1, Mathf.CeilToInt(this.size.x)), Mathf.Max(1, Mathf.CeilToInt(this.size.y)));
            }
        }

        public float Height
        {
            get
            {
                return this.height;
            }
        }

        public TraversableTerrain TraversableTerrain
        {
            get
            {
                return this.traversableTerrain;
            }
        }

        public bool ShowDisplays
        {
            get
            {
                return this.showDisplays;
            }
        }

        public abstract MobStatus Status
        {
            get;
        }

        public virtual bool MovePositionSafe
        {
            get
            {
                return true;
            }
        }

        public virtual bool MoveTiledAreaSafe
        {
            get
            {
                return true;
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
            this.selfEnterDurationDelta = Time.deltaTime * this.Owner.RequestedTimeScale;
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
