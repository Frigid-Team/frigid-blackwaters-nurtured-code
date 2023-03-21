using System.Collections.Generic;
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
        private MobClassification classification;
        [SerializeField]
        private bool showDisplays;

        public override HashSet<MobState> InitialStates
        {
            get
            {
                return new HashSet<MobState> { this };
            }
        }

        public override HashSet<MobState> SwitchableStates
        {
            get
            {
                return new HashSet<MobState> { this };
            }
        }

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

        public MobClassification Classification
        {
            get
            {
                return this.classification;
            }
        }

        public bool ShowDisplays
        {
            get
            {
                return this.showDisplays;
            }
        }

        public virtual Vector2 DisplayPosition
        {
            get
            {
                return this.Owner.Position;
            }
        }

        public virtual bool CanSetPosition
        {
            get
            {
                return true;
            }
        }

        public virtual bool CanSetTiledArea
        {
            get
            {
                return true;
            }
        }

        public abstract bool Dead
        {
            get;
        }

        public abstract bool Waiting
        {
            get;
        }
    }
}
