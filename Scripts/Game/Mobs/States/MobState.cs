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
        private bool showDisplays;

        public override HashSet<MobState> InitialStates
        {
            get
            {
                return new HashSet<MobState> { this };
            }
        }

        public override HashSet<MobState> MoveStates
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
    }
}
