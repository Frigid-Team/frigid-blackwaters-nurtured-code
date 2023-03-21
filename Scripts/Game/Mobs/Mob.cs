using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Mob : FrigidMonoBehaviour
    {
        private static SceneVariable<MobSet> spawnedMobs;
        private static Action<Mob> onMobSpawned;

        private static SceneVariable<Dictionary<TiledArea, MobSet>> activeMobsInTiledAreas;

        [SerializeField]
        private DamageAlignment alignment;
        [Space]
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private MobEquipPoint[] equipPoints;
        [SerializeField]
        private MobOverheadDisplay overheadDisplayPrefab;
        [SerializeField]
        private Mover mover;
        [SerializeField]
        private MobStateNode rootStateNode;
        [SerializeField]
        private IntSerializedReference baseMaxHealth;
        [SerializeField]
        private FloatSerializedReference basePoise;

        private Action onActiveChanged;

        private TiledArea tiledArea;
        private Action<TiledArea, TiledArea> onTiledAreaChanged;

        private Dictionary<MobStat, (int amount, Action<int, int> onAmountChanged)> statMap;

        private int remainingHealth;
        private int maxHealthBonus;
        private Action<int, int> onRemainingHealthChanged;
        private Action<int, int> onMaxHealthChanged;

        private float accumulatedPoise;
        private bool stunned;
        private Action onStunnedChanged;
        private float elapsedStunDuration;

        private ControlCounter stopDealingDamage;
        private Action<HitInfo> onHitDealt;
        private Action<BreakInfo> onBreakDealt;
        private Action<ThreatInfo> onThreatDealt;
        private LinkedList<HitInfo> hitsDealt;
        private LinkedList<BreakInfo> breaksDealt;
        private LinkedList<ThreatInfo> threatsDealt;

        private ControlCounter stopReceivingDamage;
        private Action<HitInfo> onHitReceived;
        private Action<BreakInfo> onBreakReceived;
        private Action<ThreatInfo> onThreatReceived;
        private LinkedList<HitInfo> hitsReceived;
        private LinkedList<BreakInfo> breaksReceived;
        private LinkedList<ThreatInfo> threatsReceived;

        private int[] numPushModeRequests;
        private MobPushMode currentPushMode;

        private Dictionary<MobBehaviour, (bool ignoreTimeScale, Action onFinished, bool currentlyFinished)> behaviourMap;

        private Action onSizeChanged;
        private Action onTraversableTerrainChanged;
        private Action onClassificationChanged;
        private Action onHeightChanged;
        private Action onShowDisplaysChanged;

        private Action onDeadChange;
        private Action onWaitChange;

        private float requestedTimeScale;
        private Action onRequestedTimeScaleChanged;
        private Dictionary<int, int> timeScaleRequestCounts; 

        static Mob()
        {
            spawnedMobs = new SceneVariable<MobSet>(() => new MobSet());
            activeMobsInTiledAreas = new SceneVariable<Dictionary<TiledArea, MobSet>>(() => new Dictionary<TiledArea, MobSet>());
        }

        public static MobSet SpawnedMobs
        {
            get
            {
                return spawnedMobs.Current;
            }
        }

        public static Action<Mob> OnMobSpawned
        {
            get
            {
                return onMobSpawned;
            }
            set
            {
                onMobSpawned = value;
            }
        }

        public DamageAlignment Alignment
        {
            get
            {
                return this.alignment;
            }
        }

        public DamageAlignment[] HostileAlignments
        {
            get
            {
                DamageAlignment[] hostileAlignments = new DamageAlignment[(int)DamageAlignment.Count];
                int hostileCount = 0;
                for (int i = 0; i < (int)DamageAlignment.Count; i++)
                {
                    DamageAlignment currentAlignment = (DamageAlignment)i;
                    bool isHostile = false;
                    switch (currentAlignment)
                    {
                        case DamageAlignment.Voyagers: 
                            isHostile = this.Alignment == DamageAlignment.Labyrinth; 
                            break;
                        case DamageAlignment.Labyrinth: 
                            isHostile = this.Alignment == DamageAlignment.Voyagers; 
                            break;
                    }
                    if (isHostile)
                    {
                        hostileAlignments[hostileCount] = currentAlignment;
                        hostileCount++;
                    }
                }
                Array.Resize(ref hostileAlignments, hostileCount);
                return hostileAlignments;
            }
        }

        public DamageAlignment[] PassiveAlignments
        {
            get
            {
                DamageAlignment[] passiveAlignments = new DamageAlignment[(int)DamageAlignment.Count];
                int passiveCount = 0;
                for (int i = 0; i < (int)DamageAlignment.Count; i++)
                {
                    DamageAlignment currentAlignment = (DamageAlignment)i;
                    bool isPassive = false;
                    switch (currentAlignment)
                    {
                        case DamageAlignment.Voyagers:
                            isPassive = this.Alignment == DamageAlignment.Voyagers || this.Alignment == DamageAlignment.Neutrals;
                            break;
                        case DamageAlignment.Labyrinth:
                            isPassive = this.Alignment == DamageAlignment.Labyrinth || this.Alignment == DamageAlignment.Neutrals;
                            break;
                    }
                    if (isPassive)
                    {
                        passiveAlignments[passiveCount] = currentAlignment;
                        passiveCount++;
                    }
                }
                Array.Resize(ref passiveAlignments, passiveCount);
                return passiveAlignments;
            }
        }

        public bool Active
        {
            get
            {
                return this.gameObject.activeSelf;
            }
            set
            {
                if (value != this.gameObject.activeSelf)
                {
                    if (value)
                    {
                        this.gameObject.SetActive(true);
                        OnActive();
                        foreach (MobBehaviour behaviour in this.behaviourMap.Keys)
                        {
                            BeginBehaviour(behaviour);
                        }
                        BeginRootStateNode();
                        this.onActiveChanged?.Invoke();
                    }
                    else
                    {
                        EndRootStateNode();
                        foreach (MobBehaviour behaviour in this.behaviourMap.Keys)
                        {
                            EndBehaviour(behaviour);
                        }
                        OnInactive();
                        this.gameObject.SetActive(false);
                        this.onActiveChanged?.Invoke();
                    }
                }
            }
        }

        public Action OnActiveChanged
        {
            get
            {
                return this.onActiveChanged;
            }
            set
            {
                this.onActiveChanged = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                return this.RootStateNode.CurrentState.Size;
            }
        }

        public Vector2Int TileSize
        {
            get
            {
                return this.RootStateNode.CurrentState.TileSize;
            }
        }

        public Action OnSizeChanged
        {
            get
            {
                return this.onSizeChanged;
            }
            set
            {
                this.onSizeChanged = value;
            }
        }

        public float Height
        {
            get
            {
                return this.RootStateNode.CurrentState.Height;
            }
        }

        public Action OnHeightChanged
        {
            get
            {
                return this.onHeightChanged;
            }
            set
            {
                this.onHeightChanged = value;
            }
        }

        public TraversableTerrain TraversableTerrain
        {
            get
            {
                return this.RootStateNode.CurrentState.TraversableTerrain;
            }
        }

        public Action OnTraversableTerrainChanged
        {
            get
            {
                return this.onTraversableTerrainChanged;
            }
            set
            {
                this.onTraversableTerrainChanged = value;
            }
        }

        public MobClassification Classification
        {
            get
            {
                return this.RootStateNode.CurrentState.Classification;
            }
        }

        public Action OnClassificationChanged
        {
            get
            {
                return this.onClassificationChanged;
            }
            set
            {
                this.onClassificationChanged = value;
            }
        }

        public bool ShowDisplays
        {
            get
            {
                return this.RootStateNode.CurrentState.ShowDisplays;
            }
        }

        public Action OnShowDisplaysChanged
        {
            get
            {
                return this.onShowDisplaysChanged;
            }
            set
            {
                this.onShowDisplaysChanged = value;
            }
        }

        public Vector2 DisplayPosition
        {
            get
            {
                return this.RootStateNode.CurrentState.DisplayPosition;
            }
        }

        public float RequestedTimeScale
        {
            get
            {
                return this.requestedTimeScale;
            }
        }

        public Action OnRequestedTimeScaleChanged
        {
            get
            {
                return this.onRequestedTimeScaleChanged;
            }
            set
            {
                this.onRequestedTimeScaleChanged = value;
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

        public bool Dead
        {
            get
            {
                return this.RootStateNode.CurrentState.Dead;
            }
        }

        public Action OnDeadChange
        {
            get
            {
                return this.onDeadChange;
            }
            set
            {
                this.onDeadChange = value;
            }
        }

        public bool Waiting
        {
            get
            {
                return this.RootStateNode.CurrentState.Waiting;
            }
        }

        public Action OnWaitChange
        {
            get
            {
                return this.onWaitChange;
            }
            set
            {
                this.onWaitChange = value;
            }
        }

        public bool CanAct
        {
            get
            {
                return !this.Dead && !this.Waiting && !this.Stunned;
            }
        }

        public Vector2 FacingDirection
        {
            get
            {
                return this.animatorBody.Direction;
            }
        }

        public Vector2 Position
        {
            get
            {
                return this.transform.position;
            }
        }

        public Vector2Int PositionIndices
        {
            get
            {
                return TilePositioning.RectIndicesFromPosition(this.Position, this.TiledArea.CenterPosition, this.TiledArea.MainAreaDimensions, this.TileSize);
            }
        }

        public int RemainingHealth
        {
            get
            {
                return this.remainingHealth;
            }
            set
            {
                if (this.Dead) return;
                int previousRemainingHealth = this.RemainingHealth;
                this.remainingHealth = Mathf.Clamp(value, 0, this.MaxHealth);
                if (previousRemainingHealth != this.RemainingHealth) this.onRemainingHealthChanged?.Invoke(previousRemainingHealth, this.RemainingHealth);
            }
        }

        public int MaxHealth
        {
            get
            {
                return Mathf.Max(1, this.baseMaxHealth.ImmutableValue + this.maxHealthBonus);
            }
        }

        public Action<int, int> OnRemainingHealthChanged
        {
            get
            {
                return this.onRemainingHealthChanged;
            }
            set
            {
                this.onRemainingHealthChanged = value;
            }
        }

        public Action<int, int> OnMaxHealthChanged
        {
            get
            {
                return this.onMaxHealthChanged;
            }
            set
            {
                this.onMaxHealthChanged = value;
            }
        }

        public float Poise
        {
            get
            {
                return this.basePoise.ImmutableValue + this.accumulatedPoise;
            }
        }

        public bool Stunned
        {
            get
            {
                return this.stunned;
            }
        }

        public Action OnStunnedChanged
        {
            get
            {
                return this.onStunnedChanged;
            }
            set
            {
                this.onStunnedChanged = value;
            }
        }

        public ControlCounter StopDealingDamage
        {
            get
            {
                return this.stopDealingDamage;
            }
        }

        public Action<HitInfo> OnHitDealt
        {
            get
            {
                return this.onHitDealt;
            }
            set
            {
                this.onHitDealt = value;
            }
        }

        public Action<BreakInfo> OnBreakDealt
        {
            get
            {
                return this.onBreakDealt;
            }
            set
            {
                this.onBreakDealt = value;
            }
        }

        public Action<ThreatInfo> OnThreatDealt
        {
            get
            {
                return this.onThreatDealt;
            }
            set
            {
                this.onThreatDealt = value;
            }
        }

        public LinkedList<HitInfo> HitsDealt
        {
            get
            {
                return this.hitsDealt;
            }
        }

        public LinkedList<BreakInfo> BreaksDealt
        {
            get
            {
                return this.breaksDealt;
            }
        }

        public LinkedList<ThreatInfo> ThreatsDealt
        {
            get
            {
                return this.threatsDealt;
            }
        }

        public ControlCounter StopReceivingDamage
        {
            get
            {
                return this.stopReceivingDamage;
            }
        }

        public Action<HitInfo> OnHitReceived
        {
            get
            {
                return this.onHitReceived;
            }
            set
            {
                this.onHitReceived = value;
            }
        }

        public Action<BreakInfo> OnBreakReceived
        {
            get
            {
                return this.onBreakReceived;
            }
            set
            {
                this.onBreakReceived = value;
            }
        }

        public Action<ThreatInfo> OnThreatReceived
        {
            get
            {
                return this.onThreatReceived;
            }
            set
            {
                this.onThreatReceived = value;
            }
        }

        public LinkedList<HitInfo> HitsReceived
        {
            get
            {
                return this.hitsReceived;
            }
        }

        public LinkedList<BreakInfo> BreaksReceived
        {
            get
            {
                return this.breaksReceived;
            }
        }

        public LinkedList<ThreatInfo> ThreatsReceived
        {
            get
            {
                return this.threatsReceived;
            }
        }

        public ControlCounter StopVelocities
        {
            get
            {
                return this.mover.StopVelocities;
            }
        }

        public Vector2 DesiredVelocity
        {
            get
            {
                return this.mover.CalculatedVelocity;
            }
        }

        public Vector2 CurrentVelocity
        {
            get
            {
                return this.mover.ActualVelocity;
            }
        }

        public MobPushMode CurrentPushMode
        {
            get
            {
                return this.currentPushMode;
            }
        }

        public static MobSet GetActiveMobsIn(TiledArea tiledArea)
        {
            if (activeMobsInTiledAreas.Current.TryGetValue(tiledArea, out MobSet activeMobs))
            {
                return activeMobs;
            }
            return new MobSet();
        }

        public static bool CanFitAt(TiledArea tiledArea, Vector2 position, Vector2 size, TraversableTerrain traversableTerrain)
        {
            Vector2 testSize = new Vector2(
                (Mathf.CeilToInt(size.x) - 1) * GameConstants.PIXELS_PER_UNIT + GameConstants.SMALLEST_WORLD_SIZE,
                (Mathf.CeilToInt(size.y) - 1) * GameConstants.PIXELS_PER_UNIT + GameConstants.SMALLEST_WORLD_SIZE
                );
            bool allTraversable = true;
            return TilePositioning.VisitTileIndicesInRect(
                position,
                testSize,
                tiledArea.CenterPosition,
                tiledArea.MainAreaDimensions,
                (Vector2Int tileIndices) => allTraversable &= tiledArea.NavigationGrid.IsTraversable(tileIndices, Vector2Int.one, traversableTerrain)
                ) && allTraversable;
        }

        public bool CanSpawnAt(Vector2 spawnPosition)
        {
            if (!TiledArea.TryGetTiledAreaAtPosition(spawnPosition, out TiledArea tiledArea))
            {
                return false;
            }
            return this.RootStateNode.HasValidInitialState(tiledArea, spawnPosition);
        }

        public void Spawn(Vector2 spawnPosition, Vector2 facingDirection)
        {
            if (!CanSpawnAt(spawnPosition))
            {
                Debug.LogError("Mob " + this.name + " tried to be spawned as active on position " + spawnPosition + ".");
                return;
            }

            if (!spawnedMobs.Current.Add(this))
            {
                Debug.LogError("Spawn called twice on Mob " + this.name + ".");
                return;
            }

            this.transform.position = spawnPosition;
            TiledArea.TryGetTiledAreaAtPosition(this.Position, out this.tiledArea);
            this.transform.SetParent(this.tiledArea.ContentsTransform);

            this.animatorBody.Direction = facingDirection;

            this.statMap = new Dictionary<MobStat, (int amount, Action<int, int> onAmountChanged)>();
            for (int i = 0; i < (int)MobStat.Count; i++)
            {
                this.statMap.Add((MobStat)i, (0, null));
            }

            this.remainingHealth = this.baseMaxHealth.ImmutableValue;
            this.maxHealthBonus = MaxHealthBonusFromVitality(GetStatAmount(MobStat.Vitality));
            SubscribeToStatChange(MobStat.Vitality, UpdateMaxHealthBonusFromVitalityChange);

            this.accumulatedPoise = 0;
            this.stunned = false;

            SubscribeToStatChange(MobStat.Might, UpdateDamageBonusFromMightChange);
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageAlignment = this.Alignment;
                hitBoxProperty.OnDealt += HitDealt;
                hitBoxProperty.DamageBonus += DamageBonusFromMight(GetStatAmount(MobStat.Might));
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.DamageAlignment = this.Alignment;
                breakBoxProperty.OnDealt += BreakDealt;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.DamageAlignment = this.Alignment;

                threatBoxProperty.OnDealt += ThreatDealt;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageAlignment = this.Alignment;
                attackProperty.OnHitDealt += HitDealt;
                attackProperty.OnBreakDealt += BreakDealt;
                attackProperty.OnThreatDealt += ThreatDealt;
                attackProperty.DamageBonus += DamageBonusFromMight(GetStatAmount(MobStat.Might));
            }
            this.stopDealingDamage = new ControlCounter();
            this.stopDealingDamage.OnFirstRequest += DisableDamageDealers;
            this.stopDealingDamage.OnLastRelease += EnableDamageDealers;
            this.hitsDealt = new LinkedList<HitInfo>();
            this.breaksDealt = new LinkedList<BreakInfo>();
            this.threatsDealt = new LinkedList<ThreatInfo>();
            EnableDamageDealers();

            SubscribeToStatChange(MobStat.Armour, UpdateDamageMitigationFromArmourChange);
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.animatorBody.GetProperties<HurtBoxAnimatorProperty>())
            {
                hurtBoxProperty.DamageAlignment = this.Alignment;
                hurtBoxProperty.OnReceived += HitReceived;
                hurtBoxProperty.DamageMitigation += DamageMitigationFromArmour(GetStatAmount(MobStat.Armour));
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.DamageAlignment = this.Alignment;
                resistBoxProperty.OnReceived += BreakReceived;
            }
            foreach (LookoutBoxAnimatorProperty lookoutBoxProperty in this.animatorBody.GetProperties<LookoutBoxAnimatorProperty>())
            {
                lookoutBoxProperty.DamageAlignment = this.Alignment;
                lookoutBoxProperty.OnReceived += ThreatReceived;
            }
            this.stopReceivingDamage = new ControlCounter();
            this.stopReceivingDamage.OnFirstRequest += DisableDamageReceivers;
            this.stopReceivingDamage.OnLastRelease += EnableDamageReceivers;
            this.hitsReceived = new LinkedList<HitInfo>();
            this.breaksReceived = new LinkedList<BreakInfo>();
            this.threatsReceived = new LinkedList<ThreatInfo>();
            EnableDamageReceivers();

            this.mover.SpeedBonus += SpeedBonusFromAgility(GetStatAmount(MobStat.Agility));
            SubscribeToStatChange(MobStat.Agility, UpdateSpeedBonusFromAgilityChange);

            this.numPushModeRequests = new int[(int)MobPushMode.Count];
            for (int i = 0; i < this.numPushModeRequests.Length; i++) this.numPushModeRequests[i] = 0;
            this.currentPushMode = MobPushMode.IgnoreNone;

            this.requestedTimeScale = 1f;
            this.timeScaleRequestCounts = new Dictionary<int, int>();

            this.behaviourMap = new Dictionary<MobBehaviour, (bool ignoreTimeScale, Action onFinished, bool currentlyFinished)>();

            VisitStateNodes(
                (MobStateNode stateNode) =>
                {
                    stateNode.Link(this, this.animatorBody);
                    stateNode.Init(); 
                }
                );

            foreach (MobEquipPoint equipPoint in this.equipPoints)
            {
                equipPoint.Spawn(this);
            }

            FrigidInstancing.CreateInstance<MobOverheadDisplay>(this.overheadDisplayPrefab, this.transform).Spawn(this);

            this.RootStateNode.OnCurrentStateChanged += HandleNewState;
            BeginRootStateNode();
            UpdatePushColliders();

            FrigidCoroutine.Run(Refresh(), this.gameObject);

            onMobSpawned?.Invoke(this);
            OnActive();
        }

        public bool CanMoveTo(Vector2 movePosition)
        {
            if (!this.RootStateNode.CurrentState.CanSetPosition) return false;
            TiledArea tiledArea = this.TiledArea;
            if (!TilePositioning.TilePositionWithinBounds(movePosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions))
            {
                if (!this.RootStateNode.CurrentState.CanSetTiledArea) return false;

                if (!TiledArea.TryGetTiledAreaAtPosition(movePosition, out tiledArea))
                {
                    return false;
                }
            }
            return CanFitAt(tiledArea, movePosition, this.Size, this.TraversableTerrain) || this.RootStateNode.HasValidSwitchState(tiledArea, movePosition);
        }

        public void MoveTo(Vector2 movePosition)
        {
            if (!CanMoveTo(movePosition))
            {
                Debug.LogError("Tried moving Mob " + this.name + " to a non existent TiledArea or a position where it doesn't fit!");
                return;
            }

            TiledArea tiledArea = this.tiledArea;
            if (!TilePositioning.TilePositionWithinBounds(movePosition, this.tiledArea.CenterPosition, this.tiledArea.MainAreaDimensions))
            {
                TiledArea.TryGetTiledAreaAtPosition(movePosition, out tiledArea);
            }

            this.transform.position = movePosition;
            if (this.tiledArea != tiledArea)
            {
                TiledArea previousTiledArea = this.tiledArea;
                this.tiledArea = tiledArea;
                this.transform.SetParent(this.tiledArea.ContentsTransform);
                RemoveActiveMob(this, previousTiledArea);
                AddActiveMob(this, this.tiledArea);
                this.onTiledAreaChanged?.Invoke(previousTiledArea, this.tiledArea);
            }
            if (!CanFitAt(this.TiledArea, this.Position, this.Size, this.TraversableTerrain))
            {
                EndRootStateNode();
                VisitStateNodes((MobStateNode stateNode) => stateNode.Switch());
                BeginRootStateNode();
            }
        }

        public void RelocateToTraversableSpace()
        {
            List<Vector2> testPositions = new List<Vector2>();
            int range = 0;
            while (true)
            {
                for (int x = -range; x <= range; x++)
                {
                    int y1 = range - Mathf.Abs(x);
                    int y2 = -y1;
                    Vector2 v1 = this.Position + new Vector2(x * this.Size.x / 2, y1 * this.Size.y / 2);
                    if (TilePositioning.RectPositionWithinBounds(v1, this.TiledArea.CenterPosition, this.TiledArea.MainAreaDimensions, this.TileSize)) 
                    {
                        testPositions.Add(v1);
                    }
                    if (y1 != y2)
                    {
                        Vector2 v2 = this.Position + new Vector2(x * this.Size.x / 2, y2 * this.Size.y / 2);
                        if (TilePositioning.RectPositionWithinBounds(v2, this.TiledArea.CenterPosition, this.TiledArea.MainAreaDimensions, this.TileSize))
                        {
                            testPositions.Add(v2);
                        }
                    }
                }

                if (testPositions.Count > 0)
                {
                    bool foundPosition = false;
                    Vector2 closestTestPosition = Vector2.zero;
                    float closestDistance = float.MaxValue;
                    foreach (Vector2 testPosition in testPositions)
                    {
                        float distance = Vector2.Distance(this.Position, testPosition);
                        if (CanFitAt(this.TiledArea, testPosition, this.Size, this.TraversableTerrain) && distance < closestDistance)
                        {
                            foundPosition = true;
                            closestTestPosition = testPosition;
                            closestDistance = distance;
                        }
                    }

                    if (foundPosition)
                    {
                        this.transform.position = closestTestPosition;
                        return;
                    }
                }
                else
                {
                    break;
                }

                range++;
            }
        }

        public void RequestTimeScale(float timeScale)
        {
            int timeScaleIndex = Mathf.RoundToInt(timeScale / GameConstants.SMALLEST_TIME_INTERVAL);
            if (!timeScaleRequestCounts.ContainsKey(timeScaleIndex))
            {
                timeScaleRequestCounts.Add(timeScaleIndex, 0);
            }
            timeScaleRequestCounts[timeScaleIndex]++;
            UpdateRequestedTimeScale();
        }

        public void ReleaseTimeScale(float timeScale)
        {
            int timeScaleIndex = Mathf.RoundToInt(timeScale / GameConstants.SMALLEST_TIME_INTERVAL);
            timeScaleRequestCounts[timeScaleIndex]--;
            if (timeScaleRequestCounts[timeScaleIndex] == 0)
            {
                timeScaleRequestCounts.Remove(timeScaleIndex);
            }
            UpdateRequestedTimeScale();
        }

        public bool TryGetAggressor(out Mob aggressor)
        {
            MobSet hostileMobs = GetActiveMobsIn(this.TiledArea).ThatAreNotDead().ThatAreOfAlignments(this.HostileAlignments).ThatDoNotInclude(this);
            aggressor = null;
            float closestDistance = float.MaxValue;
            foreach (Mob hostileMob in hostileMobs)
            {
                float distance = Vector2.Distance(hostileMob.Position, this.Position);
                if (distance < closestDistance)
                {
                    aggressor = hostileMob;
                    closestDistance = distance;
                }
            }
            return aggressor != null;
        }

        public bool TryGetFriend(out Mob friend)
        {
            MobSet passiveMobs = GetActiveMobsIn(this.TiledArea).ThatAreNotDead().ThatAreOfAlignments(this.PassiveAlignments).ThatDoNotInclude(this);
            friend = null;
            float closestDistance = float.MaxValue;
            foreach (Mob passiveMob in passiveMobs)
            {
                float distance = Vector2.Distance(passiveMob.Position, this.Position);
                if (distance < closestDistance)
                {
                    friend = passiveMob;
                    closestDistance = distance;
                }
            }
            return friend != null;
        }

        public void StunByStagger(float stagger)
        {
            if (stagger > 0)
            {
                StunForDuration(stagger / Mathf.Max(1f, this.Poise));
                this.accumulatedPoise += stagger + this.accumulatedPoise;
            }
        }

        public void StunForDuration(float duration)
        {
            const float MINIMUM_STUN_DURATION = 0.1f;

            if (duration >= MINIMUM_STUN_DURATION)
            {
                this.elapsedStunDuration = Mathf.Max(this.elapsedStunDuration, duration);
                if (!this.stunned)
                {
                    RequestTimeScale(0f);
                    this.stopDealingDamage.Request();
                    this.stunned = true;
                    this.onStunnedChanged?.Invoke();
                }
            }
        }

        public int GetStatAmount(MobStat stat)
        {
            return this.statMap[stat].amount;
        }

        public void SetStatAmount(MobStat stat, int amount)
        {
            (int amount, Action<int, int> onAmountChanged) statMappingValue = this.statMap[stat];
            if (statMappingValue.amount != amount)
            {
                int prevAmount = statMappingValue.amount;
                statMappingValue.amount = amount;
                this.statMap[stat] = statMappingValue;
                statMappingValue.onAmountChanged?.Invoke(prevAmount, amount);
            }
        }

        public void SubscribeToStatChange(MobStat stat, Action<int, int> onAmountChanged)
        {
            (int amount, Action<int, int> onAmountChanged) statMappingValue = this.statMap[stat];
            statMappingValue.onAmountChanged += onAmountChanged;
            this.statMap[stat] = statMappingValue;
        }

        public void UnsubscribeToStatChange(MobStat stat, Action<int, int> onAmountChanged)
        {
            (int amount, Action<int, int> onAmountChanged) statMappingValue = this.statMap[stat];
            statMappingValue.onAmountChanged -= onAmountChanged;
            this.statMap[stat] = statMappingValue;
        }

        public static int MaxHealthBonusFromVitality(int vitality)
        {
            return vitality;
        }

        public void HitDealt(HitInfo hitInfo)
        {
            this.hitsDealt.AddFirst(hitInfo);
            this.onHitDealt?.Invoke(hitInfo);
        }

        public void BreakDealt(BreakInfo breakInfo)
        {
            this.breaksDealt.AddFirst(breakInfo);
            this.onBreakDealt?.Invoke(breakInfo);
        }

        public static int DamageBonusFromMight(int might)
        {
            return might;
        }

        public void ThreatDealt(ThreatInfo threatInfo)
        {
            this.threatsDealt.AddFirst(threatInfo);
            this.onThreatDealt?.Invoke(threatInfo);
        }

        public void HitReceived(HitInfo hitInfo)
        {
            this.hitsReceived.AddFirst(hitInfo);
            this.onHitReceived?.Invoke(hitInfo);
        }

        public void BreakReceived(BreakInfo breakInfo)
        {
            this.breaksReceived.AddFirst(breakInfo);
            this.onBreakReceived?.Invoke(breakInfo);
        }

        public void ThreatReceived(ThreatInfo threatInfo)
        {
            this.threatsReceived.AddFirst(threatInfo);
            this.onThreatReceived?.Invoke(threatInfo);
        }

        public static int DamageMitigationFromArmour(int armour)
        {
            return armour;
        }

        public bool DoForcedMove(Move move, Action onFinished = null, Action onBeginMotion = null, Action onEndMotion = null)
        {
            return SetForcedMove(
                move,
                () =>
                {
                    ClearForcedMove(move);
                    onFinished?.Invoke();
                },
                onBeginMotion,
                onEndMotion
                );
        }

        public bool SetForcedMove(Move move, Action onFinished = null, Action onBeginMotion = null, Action onEndMotion = null)
        {
            return this.mover.HighestPriority < (int)MobMovePriority.Count && this.mover.AddMove(move, (int)MobMovePriority.Count, false, onFinished, onBeginMotion, onEndMotion);
        }

        public bool ClearForcedMove(Move move)
        {
            return this.mover.RemoveMove(move) && this.mover.HighestPriority < (int)MobMovePriority.Count;
        }

        public bool DoMove(Move move, MobMovePriority priority, bool ignoreTimeScale, Action onFinished = null, Action onBeginMotion = null, Action onEndMotion = null)
        {
            return this.mover.DoMove(move, (int)priority, ignoreTimeScale, onFinished, onBeginMotion, onEndMotion);
        }

        public bool AddMove(Move move, MobMovePriority priority, bool ignoreTimeScale, Action onFinished = null, Action onBeginMotion = null, Action onEndMotion = null)
        {
            return this.mover.AddMove(move, (int)priority, ignoreTimeScale, onFinished, onBeginMotion, onEndMotion);
        }

        public void RemoveMove(Move move)
        {
            this.mover.RemoveMove(move);
        }

        public static float SpeedBonusFromAgility(int agility)
        {
            return agility * 0.1f;
        }

        public void RequestPushMode(MobPushMode mode)
        {
            this.numPushModeRequests[(int)mode]++;
            UpdateCurrentModeFromRequests();
        }

        public void ReleasePushMode(MobPushMode mode)
        {
            this.numPushModeRequests[(int)mode]--;
            UpdateCurrentModeFromRequests();
        }

        public bool CanSeeThrough(Vector2 originPosition, Vector2 sightPosition, float blockingRadius)
        {
            List<Collider2D> sightColliders = LineSightCast(originPosition, sightPosition - originPosition, Vector2.Distance(sightPosition, originPosition), blockingRadius);
            foreach (Collider2D sightCollider in sightColliders)
            {
                if (!sightCollider.bounds.Contains(sightPosition))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanPassThrough(Vector2 originPosition, Vector2 pushPosition)
        {
            List<Collider2D> pushColliders = LinePushCast(originPosition, pushPosition - originPosition, Vector2.Distance(pushPosition, originPosition));
            foreach (Collider2D sightCollider in pushColliders)
            {
                if (!sightCollider.bounds.Contains(pushPosition))
                {
                    return false;
                }
            }
            return true;
        }

        public List<Collider2D> LineSightCast(Vector2 originPosition, Vector2 direction, float distance, float blockingRadius)
        {
            List<Collider2D> otherColliders = new List<Collider2D>();
            LayerMask layerMask = FrigidLayerMask.GetCollidingMask(FrigidLayer.Sight);
            RaycastHit2D[] circleCastHits = Physics2D.CircleCastAll(originPosition, blockingRadius, direction, distance, layerMask);
            foreach (RaycastHit2D circleCastHit in circleCastHits)
            {
                otherColliders.Add(circleCastHit.collider);
            }
            return otherColliders;
        }

        public List<Collider2D> LinePushCast(Vector2 originPosition, Vector2 direction, float distance)
        {
            List<Collider2D> otherColliders = new List<Collider2D>();
            if (this.currentPushMode != MobPushMode.IgnoreEverything)
            {
                FrigidLayer layer = CalculatePushLayer();
                LayerMask layerMask = FrigidLayerMask.GetCollidingMask(layer);
                if (this.Size.magnitude > 0)
                {
                    RaycastHit2D[] boxCastHits = Physics2D.BoxCastAll(originPosition, this.Size, 0, direction, distance, layerMask);
                    foreach (RaycastHit2D boxCastHit in boxCastHits)
                    {
                        if (!IsColliderPartOfPushColliders(boxCastHit.collider)) otherColliders.Add(boxCastHit.collider);
                    }
                }
            }
            return otherColliders;
        }

        public List<Collider2D> OverlapSight(Vector2 originPosition, float blockingRadius)
        {
            List<Collider2D> otherColliders = new List<Collider2D>();
            LayerMask layerMask = FrigidLayerMask.GetCollidingMask(FrigidLayer.Sight);
            Collider2D[] overlapCircleColliders = Physics2D.OverlapCircleAll(originPosition, blockingRadius, layerMask);
            foreach (Collider2D overlapCircleCollider in overlapCircleColliders)
            {
                otherColliders.Add(overlapCircleCollider);
            }
            return otherColliders;
        }

        public List<Collider2D> OverlapPush(Vector2 originPosition)
        {
            List<Collider2D> otherColliders = new List<Collider2D>();
            if (this.currentPushMode != MobPushMode.IgnoreEverything)
            {
                FrigidLayer layer = CalculatePushLayer();
                LayerMask layerMask = FrigidLayerMask.GetCollidingMask(layer);
                if (this.Size.magnitude > 0)
                {
                    Collider2D[] overlapBoxColliders = Physics2D.OverlapBoxAll(originPosition, this.Size, 0, layerMask);
                    foreach (Collider2D overlapBoxCollider in overlapBoxColliders)
                    {
                        if (!IsColliderPartOfPushColliders(overlapBoxCollider)) otherColliders.Add(overlapBoxCollider);
                    }
                }
            }
            return otherColliders;
        }

        public bool DoBehaviour(MobBehaviour behaviour, bool ignoreTimeScale, Action onFinished = null)
        {
            return AddBehaviour(
                behaviour,
                ignoreTimeScale,
                () =>
                {
                    RemoveBehaviour(behaviour);
                    onFinished?.Invoke();
                }
                );
        }

        public bool AddBehaviour(MobBehaviour behaviour, bool ignoreTimeScale, Action onFinished = null)
        {
            if (!this.behaviourMap.ContainsKey(behaviour))
            {
                this.behaviourMap.Add(behaviour, (ignoreTimeScale, onFinished, false));
                behaviour.Assign(this, this.animatorBody);
                behaviour.Added();
                BeginBehaviour(behaviour);
                return true;
            }
            return false;
        }

        public bool RemoveBehaviour(MobBehaviour behaviour)
        {
            if (this.behaviourMap.ContainsKey(behaviour))
            {
                EndBehaviour(behaviour);
                behaviour.Removed();
                behaviour.Unassign();
                this.behaviourMap.Remove(behaviour);
                return true;
            }
            return false;
        }

        public bool GetIsIgnoringTimeScale(MobBehaviour behaviour)
        {
            if (this.behaviourMap.TryGetValue(behaviour, out (bool ignoreTimeScale, Action onFinished, bool currentlyFinished) behaviourMappingValue))
            {
                return behaviourMappingValue.ignoreTimeScale;
            }
            return false;
        }

        protected MobStateNode RootStateNode
        {
            get
            {
                return this.rootStateNode;
            }
        }

        protected virtual void OnActive()
        {
            AddActiveMob(this, this.TiledArea);
        }

        protected virtual void OnInactive()
        {
            RemoveActiveMob(this, this.TiledArea);
        }

        private static void AddActiveMob(Mob mob, TiledArea tiledArea)
        {
            if (!mob.Active) return;
            if (!activeMobsInTiledAreas.Current.ContainsKey(tiledArea))
            {
                activeMobsInTiledAreas.Current.Add(tiledArea, new MobSet());
            }
            activeMobsInTiledAreas.Current[tiledArea].Add(mob);
        }

        private static void RemoveActiveMob(Mob mob, TiledArea tiledArea)
        {
            if (!mob.Active) return;
            activeMobsInTiledAreas.Current[tiledArea].Remove(mob);
            if (activeMobsInTiledAreas.Current[tiledArea].Count == 0)
            {
                activeMobsInTiledAreas.Current.Remove(tiledArea);
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> Refresh()
        {
            while (true)
            {
                this.accumulatedPoise = Mathf.Max(0, this.accumulatedPoise - Time.deltaTime * Mathf.Max(1f, this.accumulatedPoise));
                if (this.elapsedStunDuration > 0f)
                {
                    this.elapsedStunDuration -= Time.deltaTime;
                }
                else
                {
                    if (this.RequestedTimeScale == 0f && this.stunned)
                    {
                        ReleaseTimeScale(0f);
                        this.stopDealingDamage.Release();
                        this.stunned = false;
                        this.onStunnedChanged?.Invoke();
                    }
                }

                this.rootStateNode.Refresh();

                Dictionary<MobBehaviour, (bool ignoreTimeScale, Action onFinished, bool currentlyFinished)> behaviourMapCopy =
                    new Dictionary<MobBehaviour, (bool ignoreTimeScale, Action onFinished, bool currentlyFinished)>(this.behaviourMap);
                foreach (KeyValuePair<MobBehaviour, (bool ignoreTimeScale, Action onFinished, bool currentlyFinished)> behaviourMapping in behaviourMapCopy)
                {
                    MobBehaviour behaviour = behaviourMapping.Key;
                    (bool ignoreTimeScale, Action onFinished, bool currentlyFinished) behaviourMappingValue = behaviourMapping.Value;

                    if (!this.behaviourMap.ContainsKey(behaviour)) continue;

                    behaviour.Refresh();
                    if (!behaviourMappingValue.currentlyFinished && behaviour.IsFinished)
                    {
                        behaviourMappingValue.currentlyFinished = true;
                        this.behaviourMap[behaviour] = behaviourMappingValue;

                        behaviourMappingValue.onFinished?.Invoke();
                    }
                }
                yield return null;
            }
        }

        private void UpdateRequestedTimeScale()
        {
            float newTimeScale = 1f;
            foreach (KeyValuePair<int, int> timeScaleRequestCount in this.timeScaleRequestCounts)
            {
                float timeScaleInRequest = timeScaleRequestCount.Key * GameConstants.SMALLEST_TIME_INTERVAL;
                int numRequests = timeScaleRequestCount.Value;
                newTimeScale *= Mathf.Pow(timeScaleInRequest, numRequests);
            }
            if (this.requestedTimeScale != newTimeScale)
            {
                this.requestedTimeScale = newTimeScale;
                this.animatorBody.TimeScale = this.requestedTimeScale;
                this.mover.TimeScale = this.requestedTimeScale;
                this.onRequestedTimeScaleChanged?.Invoke();
            }
        }

        private void UpdateMaxHealthBonusFromVitalityChange(int previousVitality, int currentVitality)
        {
            int prevMaxHealth = this.MaxHealth;
            this.maxHealthBonus += MaxHealthBonusFromVitality(currentVitality) - MaxHealthBonusFromVitality(previousVitality);
            if (this.MaxHealth != prevMaxHealth) 
            {
                this.RemainingHealth = this.RemainingHealth;
                this.onMaxHealthChanged?.Invoke(prevMaxHealth, this.MaxHealth); 
            }
        }

        private void EnableDamageDealers()
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>()) hitBoxProperty.IsIgnoringDamage = false;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>()) breakBoxProperty.IsIgnoringDamage = false;
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetProperties<ThreatBoxAnimatorProperty>()) threatBoxProperty.IsIgnoringDamage = false;
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>()) attackProperty.ForceStop = false;
        }

        private void DisableDamageDealers()
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>()) hitBoxProperty.IsIgnoringDamage = true;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>()) breakBoxProperty.IsIgnoringDamage = true;
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetProperties<ThreatBoxAnimatorProperty>()) threatBoxProperty.IsIgnoringDamage = true;
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>()) attackProperty.ForceStop = true;
        }

        private void UpdateDamageBonusFromMightChange(int previousMight, int currentMight)
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageBonus += DamageBonusFromMight(currentMight) - DamageBonusFromMight(previousMight);
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageBonus += DamageBonusFromMight(currentMight) - DamageBonusFromMight(previousMight);
            }
        }

        private void EnableDamageReceivers()
        {
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.animatorBody.GetProperties<HurtBoxAnimatorProperty>()) hurtBoxProperty.IsIgnoringDamage = false;
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetProperties<ResistBoxAnimatorProperty>()) resistBoxProperty.IsIgnoringDamage = false;
            foreach (LookoutBoxAnimatorProperty lookoutBoxProperty in this.animatorBody.GetProperties<LookoutBoxAnimatorProperty>()) lookoutBoxProperty.IsIgnoringDamage = false;
        }

        private void DisableDamageReceivers()
        {
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.animatorBody.GetProperties<HurtBoxAnimatorProperty>()) hurtBoxProperty.IsIgnoringDamage = true;
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetProperties<ResistBoxAnimatorProperty>()) resistBoxProperty.IsIgnoringDamage = true;
            foreach (LookoutBoxAnimatorProperty lookoutBoxProperty in this.animatorBody.GetProperties<LookoutBoxAnimatorProperty>()) lookoutBoxProperty.IsIgnoringDamage = true;
        }

        private void UpdateDamageMitigationFromArmourChange(int previousArmour, int currentArmour)
        {
            foreach (HurtBoxAnimatorProperty hurtBoxAnimatorProperty in this.animatorBody.GetProperties<HurtBoxAnimatorProperty>())
            {
                hurtBoxAnimatorProperty.DamageMitigation += DamageMitigationFromArmour(currentArmour) - DamageMitigationFromArmour(previousArmour);
            }
        }

        private void UpdateSpeedBonusFromAgilityChange(int previousAgility, int currentAgility)
        {
            this.mover.SpeedBonus += SpeedBonusFromAgility(currentAgility) - SpeedBonusFromAgility(previousAgility);
        }

        private FrigidLayer CalculatePushLayer()
        {
            FrigidLayer pushLayer = FrigidLayer.IgnoreRaycast;
            switch (this.currentPushMode)
            {
                case MobPushMode.IgnoreNone:
                    if (this.TraversableTerrain.Includes(TileTerrain.Land) && this.TraversableTerrain.Includes(TileTerrain.Water)) pushLayer = FrigidLayer.FloatingMob;
                    else if (this.TraversableTerrain.Includes(TileTerrain.Land)) pushLayer = FrigidLayer.LandMob;
                    else if (this.TraversableTerrain.Includes(TileTerrain.Water)) pushLayer = FrigidLayer.WaterMob;
                    break;
                case MobPushMode.IgnoreMobs:
                    if (this.TraversableTerrain.Includes(TileTerrain.Land) && this.TraversableTerrain.Includes(TileTerrain.Water)) pushLayer = FrigidLayer.IgnoringMobsFloatingMob;
                    else if (this.TraversableTerrain.Includes(TileTerrain.Land)) pushLayer = FrigidLayer.IgnoringMobsLandMob;
                    else if (this.TraversableTerrain.Includes(TileTerrain.Water)) pushLayer = FrigidLayer.IgnoringMobsWaterMob;
                    break;
                case MobPushMode.IgnoreMobsAndTerrain:
                    pushLayer = FrigidLayer.IgnoringMobsAndTerrainMob;
                    break;
                case MobPushMode.IgnoreMobsTerrainAndObstacles:
                    pushLayer = FrigidLayer.IgnoringMobsTerrainAndObstaclesMob;
                    break;
            }
            return pushLayer;
        }

        private bool IsColliderPartOfPushColliders(Collider2D collider)
        {
            foreach (PushColliderAnimatorProperty pushColliderProperty in this.animatorBody.GetProperties<PushColliderAnimatorProperty>())
            {
                if (pushColliderProperty.Collider == collider)
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateCurrentModeFromRequests()
        {
            MobPushMode newMode = MobPushMode.IgnoreNone;
            for (int i = this.numPushModeRequests.Length - 1; i >= 0; i--)
            {
                if (this.numPushModeRequests[i] > 0)
                {
                    newMode = (MobPushMode)i;
                    break;
                }
            }
            if (newMode != this.currentPushMode)
            {
                this.currentPushMode = newMode;
                UpdatePushColliders();
            }
        }

        private void UpdatePushColliders()
        {
            FrigidLayer layer = CalculatePushLayer();
            foreach (PushColliderAnimatorProperty pushColliderProperty in this.animatorBody.GetProperties<PushColliderAnimatorProperty>())
            {
                pushColliderProperty.Layer = (int)layer;
            }
        }

        private void BeginBehaviour(MobBehaviour behaviour)
        {
            if (!this.Active) return;

            behaviour.Enter();
            if (behaviour.IsFinished)
            {
                (bool ignoreTimeScale, Action onFinished, bool currentlyFinished) behaviourMappingValue = this.behaviourMap[behaviour];
                behaviourMappingValue.currentlyFinished = true;
                this.behaviourMap[behaviour] = behaviourMappingValue;
                behaviourMappingValue.onFinished?.Invoke();
            }
        }

        private void EndBehaviour(MobBehaviour behaviour)
        {
            if (!this.Active) return;

            behaviour.Exit();
        }

        private void BeginRootStateNode()
        {
            if (!this.Active) return;
            this.RootStateNode.Enter();
        }

        private void EndRootStateNode()
        {
            if (!this.Active) return;
            this.RootStateNode.Exit();
        }

        private void VisitStateNodes(Action<MobStateNode> onVisited)
        {
            HashSet<MobStateNode> visitedStateNodes = new HashSet<MobStateNode>();
            Queue<MobStateNode> nextStateNodes = new Queue<MobStateNode>();
            nextStateNodes.Enqueue(this.rootStateNode);
            while (nextStateNodes.TryDequeue(out MobStateNode stateNode))
            {
                if (!visitedStateNodes.Contains(stateNode))
                {
                    visitedStateNodes.Add(stateNode);
                    foreach (MobStateNode referencedStateNode in stateNode.ReferencedStateNodes)
                    {
                        nextStateNodes.Enqueue(referencedStateNode);
                    }
                    onVisited.Invoke(stateNode);
                }
            }
        }

        private void HandleNewState(MobState previousState, MobState currentState)
        {
            if (currentState.Size != previousState.Size) this.onSizeChanged?.Invoke();
            if (currentState.TraversableTerrain != previousState.TraversableTerrain)
            {
                UpdatePushColliders();
                this.onTraversableTerrainChanged?.Invoke();
            }
            if (currentState.Height != previousState.Height) this.onHeightChanged?.Invoke();
            if (currentState.Classification != previousState.Classification) this.onClassificationChanged?.Invoke();
            if (currentState.ShowDisplays != previousState.ShowDisplays) this.onShowDisplaysChanged?.Invoke();
            if (currentState.Dead != previousState.Dead) this.onDeadChange?.Invoke();
            if (currentState.Waiting != previousState.Waiting) this.onWaitChange?.Invoke();
        }
    }
}
