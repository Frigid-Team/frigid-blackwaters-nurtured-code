using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class Mob : FrigidMonoBehaviour
    {
        private static SceneVariable<Dictionary<DamageAlignment, MobCollection>> mobsByAlignment;
        private static SceneVariable<Dictionary<TiledArea, MobCollection>> mobsByTiledArea;

        [SerializeField]
        private DamageAlignment alignment;
        [SerializeField]
        private MobClassification classification;
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private Rigidbody2D rigidbody;
        [SerializeField]
        private Mover mover;

        [SerializeField]
        private IntSerializedReference baseMaxHealth;

        [SerializeField]
        private MobStateNode rootStateNode;

        private FlagSemaphore active;

        private MobStats stats;
        private MobHealth health;
        private MobMovement movement;
        private MobDamageDealer damageDealer;
        private MobDamageReceiver damageReceiver;
        private MobPhysicality physicality;

        private TiledArea tiledArea;
        private Action<TiledArea, TiledArea> onTiledAreaChanged;

        private Dictionary<MobBehaviour, Action> behaviourMap;

        static Mob()
        {
            mobsByAlignment = new SceneVariable<Dictionary<DamageAlignment, MobCollection>>(
                () =>
                {
                    Dictionary<DamageAlignment, MobCollection> mobsByAlignment = new Dictionary<DamageAlignment, MobCollection>();
                    for (int i = 0; i < (int)DamageAlignment.Count; i++)
                    {
                        mobsByAlignment.Add((DamageAlignment)i, new MobCollection());
                    }
                    return mobsByAlignment;
                }
                );
            mobsByTiledArea = new SceneVariable<Dictionary<TiledArea, MobCollection>>(
                () => 
                {
                    Dictionary<TiledArea, MobCollection> mobsByTiledArea = new Dictionary<TiledArea, MobCollection>();
                    foreach (TiledArea tiledArea in TiledArea.SpawnedTiledAreas)
                    {
                        mobsByTiledArea.Add(tiledArea, new MobCollection());
                    }
                    TiledArea.OnTiledAreaSpawned += (TiledArea tiledArea) => mobsByTiledArea.Add(tiledArea, new MobCollection());
                    return mobsByTiledArea;
                }
                );
        }

        public DamageAlignment Alignment
        {
            get
            {
                return this.alignment;
            }
        }

        public MobClassification Classification
        {
            get
            {
                return this.classification;
            }
        }

        public Vector2 Size
        {
            get
            {
                return this.rootStateNode.CurrentState.Size;
            }
        }

        public Vector2 InitialSize
        {
            get
            {
                return this.rootStateNode.InitialState.Size;
            }
        }

        public Vector2Int TileSize
        {
            get
            {
                return new Vector2Int(Mathf.CeilToInt(this.Size.x), Mathf.CeilToInt(this.Size.y));
            }
        }

        public Vector2Int InitialTileSize
        {
            get
            {
                return new Vector2Int(Mathf.CeilToInt(this.InitialSize.x), Mathf.CeilToInt(this.InitialSize.y));
            }
        }

        public Vector2 FacingDirection
        {
            get
            {
                return this.animatorBody.Direction;
            }
        }

        public TraversableTerrain TraversableTerrain
        {
            get
            {
                return this.rootStateNode.CurrentState.TraversableTerrain;
            }
        }

        public TraversableTerrain InitialTraversableTerrain
        {
            get
            {
                return this.rootStateNode.InitialState.TraversableTerrain;
            }
        }

        public FlagSemaphore Active
        {
            get
            {
                return this.active;
            }
        }

        public TiledArea TiledArea
        {
            get
            {
                return this.tiledArea;
            }
        }

        public Action<TiledArea, TiledArea> OnTiledAreaChanged
        {
            get
            {
                return this.onTiledAreaChanged;
            }
            set
            {
                this.onTiledAreaChanged = value;
            }
        }

        public MobHealth Health
        {
            get
            {
                return this.health;
            }
        }

        public MobStats Stats
        {
            get
            {
                return this.stats;
            }
        }

        public MobMovement Movement
        {
            get
            {
                return this.movement;
            }
        }

        public MobDamageDealer DamageDealer
        {
            get
            {
                return this.damageDealer;
            }
        }

        public MobDamageReceiver DamageReceiver
        {
            get
            {
                return this.damageReceiver;
            }
        }

        public MobPhysicality Physicality
        {
            get
            {
                return this.physicality;
            }
        }

        public bool Dead
        {
            get
            {
                return this.rootStateNode.CurrentState.Dead;
            }
        }

        public Vector2 AbsolutePosition
        {
            get
            {
                return this.transform.position;
            }
            set
            {
                this.transform.position = value;
                if (TilePositioning.TileAbsolutePositionWithinBounds(value, this.tiledArea.AbsoluteCenterPosition, this.tiledArea.MainAreaDimensions)) return;
                if (TiledArea.TryGetTiledAreaAtPosition(value, out TiledArea newTiledArea))
                {
                    if (this.tiledArea != newTiledArea)
                    {
                        TiledArea previousTiledArea = this.tiledArea;
                        GetMobsInTiledArea(this.tiledArea).RemoveMob(this);
                        this.tiledArea = newTiledArea;
                        GetMobsInTiledArea(this.tiledArea).AddMob(this);

                        this.transform.SetParent(this.tiledArea.ContentsTransform);
                        this.onTiledAreaChanged?.Invoke(previousTiledArea, this.tiledArea);
                    }
                }
                else
                {
                    Debug.LogError("Mob " + this.name + " was not moved to an existing TiledArea.");
                }
            }
        }

        public Vector2Int PositionIndices
        {
            get
            {
                return TilePositioning.RectIndicesFromAbsolutePosition(this.AbsolutePosition, this.TiledArea.AbsoluteCenterPosition, this.TiledArea.MainAreaDimensions, this.TileSize);
            }
            set
            {
                this.AbsolutePosition = TilePositioning.RectAbsolutePositionFromIndices(value, this.TiledArea.AbsoluteCenterPosition, this.TiledArea.MainAreaDimensions, this.TileSize);
            }
        }

        public static MobCollection GetMobsOfAlignment(DamageAlignment alignment)
        {
            return mobsByAlignment.Current[alignment];
        }

        public static MobCollection GetMobsInTiledArea(TiledArea tiledArea)
        {
            return mobsByTiledArea.Current[tiledArea];
        }

        public void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        public bool DoBehaviour(MobBehaviour behaviour)
        {
            return AddBehaviour(behaviour, () => RemoveBehaviour(behaviour));
        }

        public bool AddBehaviour(MobBehaviour behaviour, Action onFinished = null)
        {
            if (this.behaviourMap.TryAdd(behaviour, onFinished))
            {
                behaviour.Assign(this, this.animatorBody);
                behaviour.Apply();
                if (behaviour.Finished) onFinished?.Invoke();
                return true;
            }
            return false;
        }

        public bool RemoveBehaviour(MobBehaviour behaviour)
        {
            if (this.behaviourMap.Remove(behaviour))
            {
                behaviour.Unapply();
                behaviour.Assign(null, null);
                return true;
            }
            return false;
        }

        public void RelocateToTraversableSpace()
        {
            Vector2Int positionIndices = TilePositioning.RectIndicesFromAbsolutePosition(this.AbsolutePosition, this.TiledArea.AbsoluteCenterPosition, this.TiledArea.MainAreaDimensions, this.TileSize);

            if (this.TiledArea.NavigationGrid.IsOnTerrain(positionIndices, this.TileSize, this.TraversableTerrain)) return;

            List<Vector2Int> closestTraversableIndices = this.TiledArea.NavigationGrid.GetClosestTraversableIndices(positionIndices, this.TileSize, this.TraversableTerrain);
            Vector2 closestAbsolutePosition = this.AbsolutePosition;
            float closestDistance = float.MaxValue;
            foreach (Vector2Int traversableIndices in closestTraversableIndices)
            {
                Vector2 absolutePosition = TilePositioning.RectAbsolutePositionFromIndices(traversableIndices, this.TiledArea.AbsoluteCenterPosition, this.TiledArea.MainAreaDimensions, this.TileSize);
                float distance = Vector2.Distance(absolutePosition, this.AbsolutePosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestAbsolutePosition = absolutePosition;
                }
            }

            this.AbsolutePosition = closestAbsolutePosition;
        }

        protected override void Awake()
        {
            base.Awake();
            this.active = new FlagSemaphore();

            this.stats = new MobStats();
            this.health = new MobHealth(this.baseMaxHealth.ImmutableValue, this.stats);
            this.movement = new MobMovement(this.rigidbody, this.mover, this.stats);
            this.damageDealer = new MobDamageDealer(this.alignment, this.animatorBody, this.stats);
            this.damageReceiver = new MobDamageReceiver(this.alignment, this.animatorBody, this.stats);
            this.physicality = new MobPhysicality(this.animatorBody, this.rootStateNode);

            this.behaviourMap = new Dictionary<MobBehaviour, Action>();

            if (TiledArea.TryGetTiledAreaAtPosition(this.AbsolutePosition, out this.tiledArea))
            {
                this.transform.SetParent(this.tiledArea.ContentsTransform);
            }
            else
            {
                Debug.LogError("Mob " + this.name + " was not spawned in an existing TiledArea.");
                return;
            }

            HashSet<MobStateNode> visitedStateNodes = new HashSet<MobStateNode>();
            Queue<MobStateNode> nextStateNodes = new Queue<MobStateNode>();
            nextStateNodes.Enqueue(this.rootStateNode);
            while (nextStateNodes.TryDequeue(out MobStateNode stateNode))
            {
                if (!visitedStateNodes.Contains(stateNode))
                {
                    visitedStateNodes.Add(stateNode);
                    stateNode.Assign(this, this.animatorBody);
                    stateNode.Setup();
                    foreach (MobStateNode referencedStateNode in stateNode.ReferencedStateNodes)
                    {
                        nextStateNodes.Enqueue(referencedStateNode);
                    }
                }
            }
            this.rootStateNode.OnCurrentStateChanged += RelocateOnNewTraversableTerrain;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.active.Set();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.active.Unset();
        }

        protected override void Start()
        {
            base.Start();

            this.rootStateNode.Enter();
            RelocateToTraversableSpace();

            GetMobsOfAlignment(this.alignment).AddMob(this);
            GetMobsInTiledArea(this.tiledArea).AddMob(this);
        }

        protected override void Update()
        {
            base.Update();
            this.rootStateNode.Refresh();
            foreach (KeyValuePair<MobBehaviour, Action> behaviourPair in this.behaviourMap)
            {
                MobBehaviour behaviour = behaviourPair.Key;
                Action onFinished = behaviourPair.Value;
                behaviour.Perform();
                if (behaviour.Finished)
                {
                    onFinished?.Invoke();
                }
            }
        }

        private void RelocateOnNewTraversableTerrain(MobState previousState, MobState currentState)
        {
            if (currentState.TraversableTerrain != previousState.TraversableTerrain) RelocateToTraversableSpace();
        }
    }
}
