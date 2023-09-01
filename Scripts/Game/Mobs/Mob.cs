using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Mob : FrigidMonoBehaviour
    {
        private static SceneVariable<HashSet<Mob>> spawnedMobs;
        private static Action<Mob> onMobSpawned;

        private static SceneVariable<Dictionary<TiledArea, HashSet<Mob>>> activeMobsInTiledAreas;

        [SerializeField]
        private DamageAlignment alignment;
        [Space]
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private MobEquipPoint[] equipPoints;
        [SerializeField]
        private bool hasOverheadDisplay;
        [SerializeField]
        [ShowIfBool("hasOverheadDisplay", true)]
        private MobOverheadDisplay overheadDisplayPrefab;
        [SerializeField]
        private Mover mover;
        [SerializeField]
        private MobStateNode rootStateNode;
        [SerializeField]
        private IntSerializedReference baseMaxHealth;
        [SerializeField]
        private FloatSerializedReference basePoise;

        private ControlCounter deactivated;
        private Action onActiveChanged;

        private TiledArea tiledArea;
        private Action<TiledArea, TiledArea> onTiledAreaChanged;

        private (int, Action<int, int>)[][] statGrid;

        private int remainingHealth;
        private int maxHealthBonus;

        private Action<int, int> onRemainingHealthChanged;
        private Action<int, int> onMaxHealthChanged;

        private Action<int> onHealed;
        private Action<int> onDamaged;

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

        private Dictionary<MobBehaviour, (bool, Action, bool)> behaviourMap;

        private Action onSizeChanged;
        private Action onTraversableTerrainChanged;
        private Action onHeightChanged;
        private Action onShowDisplaysChanged;
        private Action onStatusChanged;

        private Dictionary<MobStatusTag, int> statusTagCounts;
        private Action<MobStatusTag> onStatusTagAdded;
        private Action<MobStatusTag> onStatusTagRemoved;

        private float requestedTimeScale;
        private Action onRequestedTimeScaleChanged;
        private Dictionary<int, int> timeScaleRequestCounts;

        private Dictionary<MobEquipContext, MobEquipPoint> contextualEquipPoints;

        static Mob()
        {
            spawnedMobs = new SceneVariable<HashSet<Mob>>(() => new HashSet<Mob>());
            activeMobsInTiledAreas = new SceneVariable<Dictionary<TiledArea, HashSet<Mob>>>(() => new Dictionary<TiledArea, HashSet<Mob>>());
        }

        public static HashSet<Mob> SpawnedMobs
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

        public DamageAlignment[] FriendlyAlignments
        {
            get
            {
                DamageAlignment[] friendlyAlignments = new DamageAlignment[(int)DamageAlignment.Count];
                int friendlyCount = 0;
                for (int i = 0; i < (int)DamageAlignment.Count; i++)
                {
                    DamageAlignment currentAlignment = (DamageAlignment)i;
                    bool isFriendly = false;
                    switch (currentAlignment)
                    {
                        case DamageAlignment.Voyagers:
                            isFriendly = this.Alignment == DamageAlignment.Voyagers || this.Alignment == DamageAlignment.Neutrals;
                            break;
                        case DamageAlignment.Labyrinth:
                            isFriendly = this.Alignment == DamageAlignment.Labyrinth || this.Alignment == DamageAlignment.Neutrals;
                            break;
                    }
                    if (isFriendly)
                    {
                        friendlyAlignments[friendlyCount] = currentAlignment;
                        friendlyCount++;
                    }
                }
                Array.Resize(ref friendlyAlignments, friendlyCount);
                return friendlyAlignments;
            }
        }

        public bool Active
        {
            get
            {
                return this.gameObject.activeSelf;
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

        public MobStatus Status
        {
            get
            {
                return this.RootStateNode.CurrentState.Status;
            }
        }

        public Action OnStatusChanged
        {
            get
            {
                return this.onStatusChanged;
            }
            set
            {
                this.onStatusChanged = value;
            }
        }

        public Action<MobStatusTag> OnStatusTagAdded
        {
            get
            {
                return this.onStatusTagAdded;
            }
            set
            {
                this.onStatusTagAdded = value;
            }
        }

        public Action<MobStatusTag> OnStatusTagRemoved
        {
            get
            {
                return this.onStatusTagRemoved;
            }
            set
            {
                this.onStatusTagRemoved = value;
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

        public bool IsActingAndNotStunned
        {
            get
            {
                return this.Status == MobStatus.Acting && !this.Stunned;
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

        public Vector2Int IndexPosition
        {
            get
            {
                return AreaTiling.RectIndexPositionFromPosition(this.Position, this.TiledArea.CenterPosition, this.TiledArea.MainAreaDimensions, this.TileSize);
            }
        }

        public int RemainingHealth
        {
            get
            {
                return this.remainingHealth;
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

        public Action<int> OnHealed
        {
            get
            {
                return this.onHealed;
            }
            set
            {
                this.onHealed = value;
            }
        }

        public Action<int> OnDamaged
        {
            get
            {
                return this.onDamaged;
            }
            set
            {
                this.onDamaged = value;
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

        public MobMovePriority CurrentMovePriority
        {
            get
            {
                return (MobMovePriority)this.mover.HighestPriority;
            }
        }

        public MobPushMode CurrentPushMode
        {
            get
            {
                return this.currentPushMode;
            }
        }

        public static HashSet<Mob> GetActiveMobsIn(TiledArea tiledArea)
        {
            if (activeMobsInTiledAreas.Current.TryGetValue(tiledArea, out HashSet<Mob> activeMobs))
            {
                return activeMobs;
            }
            return new HashSet<Mob>();
        }

        public void ToActive() => this.deactivated.Release();

        public void ToInactive() => this.deactivated.Request();

        public static bool CanTraverseAt(TiledArea tiledArea, Vector2 position, Vector2 size, TraversableTerrain traversableTerrain)
        {
            Vector2 testSize = new Vector2((Mathf.CeilToInt(size.x) - 1) * FrigidConstants.PIXELS_PER_UNIT + FrigidConstants.SMALLEST_WORLD_SIZE, (Mathf.CeilToInt(size.y) - 1) * FrigidConstants.PIXELS_PER_UNIT + FrigidConstants.SMALLEST_WORLD_SIZE);
            bool allTraversable = true;
            return AreaTiling.VisitTileIndexPositionsInRect(
                position,
                testSize,
                tiledArea.CenterPosition,
                tiledArea.MainAreaDimensions,
                (Vector2Int tileIndexPosition) => allTraversable &= tiledArea.NavigationGrid.IsTraversable(tileIndexPosition, Vector2Int.one, traversableTerrain, Resistance.None)
                ) && allTraversable;
        }

        public bool CanSpawnAt(Vector2 spawnPosition)
        {
            if (!TiledArea.TryGetAreaAtPosition(spawnPosition, out TiledArea tiledArea))
            {
                return false;
            }
            return this.RootStateNode.HasValidInitialState(tiledArea, spawnPosition);
        }

        public void Spawn(Vector2 spawnPosition, Vector2 facingDirection)
        {
            if (!this.CanSpawnAt(spawnPosition))
            {
                Debug.LogError("Mob " + this.name + " tried to be spawned on position that it can't traverse!");
                return;
            }

            if (!spawnedMobs.Current.Add(this))
            {
                Debug.LogError("Spawn called twice on Mob " + this.name + ".");
                return;
            }

            this.deactivated = new ControlCounter();
            this.deactivated.OnFirstRequest += this.OnInactive;
            this.deactivated.OnLastRelease += this.OnActive;

            this.transform.position = spawnPosition;
            TiledArea.TryGetAreaAtPosition(this.Position, out this.tiledArea);
            this.transform.SetParent(this.tiledArea.ContentsTransform);

            this.animatorBody.Direction = facingDirection;

            this.statGrid = new (int amount, Action<int, int> onAmountChanged)[(int)MobStatLayer.Count][];
            for (int i = 0; i < (int)MobStatLayer.Count; i++)
            {
                this.statGrid[i] = new (int amount, Action<int, int> onAmountChanged)[(int)MobStat.Count];
                for (int j = 0; j < (int)MobStat.Count; j++)
                {
                    this.statGrid[i][j] = (0, null);
                }
            }

            this.remainingHealth = this.baseMaxHealth.ImmutableValue;
            this.maxHealthBonus = MaxHealthBonusFromVitality(this.GetTotalStatAmount(MobStat.Vitality));
            this.SubscribeToTotalStatChange(MobStat.Vitality, this.UpdateMaxHealthBonusFromVitalityChange);

            this.accumulatedPoise = 0;
            this.stunned = false;

            this.SubscribeToTotalStatChange(MobStat.Might, this.UpdateDamageBonusFromMightChange);
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageAlignment = this.Alignment;
                hitBoxProperty.OnDealt += this.HitDealt;
                hitBoxProperty.DamageBonus += DamageBonusFromMight(this.GetTotalStatAmount(MobStat.Might));
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.DamageAlignment = this.Alignment;
                breakBoxProperty.OnDealt += this.BreakDealt;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.DamageAlignment = this.Alignment;

                threatBoxProperty.OnDealt += this.ThreatDealt;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageAlignment = this.Alignment;
                attackProperty.OnHitDealt += this.HitDealt;
                attackProperty.OnBreakDealt += this.BreakDealt;
                attackProperty.OnThreatDealt += this.ThreatDealt;
                attackProperty.DamageBonus += DamageBonusFromMight(this.GetTotalStatAmount(MobStat.Might));
            }
            this.stopDealingDamage = new ControlCounter();
            this.stopDealingDamage.OnFirstRequest += this.DisableDamageDealers;
            this.stopDealingDamage.OnLastRelease += this.EnableDamageDealers;
            this.hitsDealt = new LinkedList<HitInfo>();
            this.breaksDealt = new LinkedList<BreakInfo>();
            this.threatsDealt = new LinkedList<ThreatInfo>();
            this.EnableDamageDealers();

            this.SubscribeToTotalStatChange(MobStat.Armour, this.UpdateDamageMitigationFromArmourChange);
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.animatorBody.GetReferencedProperties<HurtBoxAnimatorProperty>())
            {
                hurtBoxProperty.DamageAlignment = this.Alignment;
                hurtBoxProperty.OnReceived += this.HitReceived;
                hurtBoxProperty.DamageMitigation += DamageMitigationFromArmour(this.GetTotalStatAmount(MobStat.Armour));
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.DamageAlignment = this.Alignment;
                resistBoxProperty.OnReceived += this.BreakReceived;
            }
            foreach (LookoutBoxAnimatorProperty lookoutBoxProperty in this.animatorBody.GetReferencedProperties<LookoutBoxAnimatorProperty>())
            {
                lookoutBoxProperty.DamageAlignment = this.Alignment;
                lookoutBoxProperty.OnReceived += this.ThreatReceived;
            }
            this.stopReceivingDamage = new ControlCounter();
            this.stopReceivingDamage.OnFirstRequest += this.DisableDamageReceivers;
            this.stopReceivingDamage.OnLastRelease += this.EnableDamageReceivers;
            this.hitsReceived = new LinkedList<HitInfo>();
            this.breaksReceived = new LinkedList<BreakInfo>();
            this.threatsReceived = new LinkedList<ThreatInfo>();
            this.EnableDamageReceivers();

            this.mover.SpeedBonus += SpeedBonusFromAgility(this.GetTotalStatAmount(MobStat.Agility));
            this.SubscribeToTotalStatChange(MobStat.Agility, this.UpdateSpeedBonusFromAgilityChange);

            this.numPushModeRequests = new int[(int)MobPushMode.Count];
            for (int i = 0; i < this.numPushModeRequests.Length; i++) this.numPushModeRequests[i] = 0;
            this.currentPushMode = MobPushMode.IgnoreNone;

            this.statusTagCounts = new Dictionary<MobStatusTag, int>();

            this.requestedTimeScale = 1f;
            this.timeScaleRequestCounts = new Dictionary<int, int>();

            this.behaviourMap = new Dictionary<MobBehaviour, (bool, Action, bool)>();

            this.VisitStateNodes(
                (MobStateNode stateNode) =>
                {
                    stateNode.Link(this, this.animatorBody);
                    stateNode.Init(); 
                }
                );

            this.contextualEquipPoints = new Dictionary<MobEquipContext, MobEquipPoint>();
            foreach (MobEquipPoint equipPoint in this.equipPoints)
            {
                equipPoint.Spawn(this, this.rootStateNode);
                if (!this.contextualEquipPoints.TryAdd(equipPoint.EquipContext, equipPoint))
                {
                    Debug.LogError("Null or duplicate equip context on MobEquipPoint " + equipPoint.name + " on " + this.name + ".");
                }
            }

            if (this.hasOverheadDisplay) CreateInstance<MobOverheadDisplay>(this.overheadDisplayPrefab).Spawn(this);

            this.RootStateNode.OnCurrentStateChanged += this.HandleNewState;
            this.UpdatePushColliders();

            FrigidCoroutine.Run(this.Refresh(), this.gameObject);

            onMobSpawned?.Invoke(this);
            this.OnActive();
        }

        public bool CanMoveTo(Vector2 movePosition, bool keepState = true)
        {
            if (!this.Active) return false;
            TiledArea tiledArea = this.TiledArea;
            if (!AreaTiling.TilePositionWithinBounds(movePosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions))
            {
                if (!TiledArea.TryGetAreaAtPosition(movePosition, out tiledArea))
                {
                    return false;
                }
            }
            if (keepState)
            {
                return this.RootStateNode.CurrentState.MovePositionSafe && (tiledArea == this.TiledArea || this.RootStateNode.CurrentState.MoveTiledAreaSafe);
            }
            else
            {
                return this.RootStateNode.HasValidMoveState(tiledArea, movePosition);
            }
        }

        public void MoveTo(Vector2 movePosition, bool keepState = true)
        {
            if (!this.CanMoveTo(movePosition, keepState))
            {
                Debug.LogError("Tried moving Mob " + this.name + " to a non existent TiledArea or a position where it can't traverse!");
                return;
            }

            TiledArea tiledArea = this.tiledArea;
            if (!AreaTiling.TilePositionWithinBounds(movePosition, this.tiledArea.CenterPosition, this.tiledArea.MainAreaDimensions))
            {
                TiledArea.TryGetAreaAtPosition(movePosition, out tiledArea);
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
            if (!keepState)
            {
                this.EndRootStateNode();
                this.VisitStateNodes((MobStateNode stateNode) => stateNode.Move());
                this.BeginRootStateNode();
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
                    if (AreaTiling.RectPositionWithinBounds(v1, this.TiledArea.CenterPosition, this.TiledArea.MainAreaDimensions, this.TileSize)) 
                    {
                        testPositions.Add(v1);
                    }
                    if (y1 != y2)
                    {
                        Vector2 v2 = this.Position + new Vector2(x * this.Size.x / 2, y2 * this.Size.y / 2);
                        if (AreaTiling.RectPositionWithinBounds(v2, this.TiledArea.CenterPosition, this.TiledArea.MainAreaDimensions, this.TileSize))
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
                        if (CanTraverseAt(this.TiledArea, testPosition, this.Size, this.TraversableTerrain) && distance < closestDistance)
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

        public bool HasStatusTag(MobStatusTag statusTag)
        {
            return this.statusTagCounts.ContainsKey(statusTag);
        }

        public void AddStatusTag(MobStatusTag statusTag)
        {
            if (!this.statusTagCounts.ContainsKey(statusTag))
            {
                this.statusTagCounts.Add(statusTag, 0);
            }
            this.statusTagCounts[statusTag]++;
            this.onStatusTagAdded?.Invoke(statusTag);
        }

        public void RemoveStatusTag(MobStatusTag statusTag)
        {
            this.statusTagCounts[statusTag]--;
            if (this.statusTagCounts[statusTag] == 0)
            {
                this.statusTagCounts.Remove(statusTag);
            }
            this.onStatusTagRemoved?.Invoke(statusTag);
        }

        public void RequestTimeScale(float timeScale)
        {
            int timeScaleIndex = Mathf.RoundToInt(timeScale / FrigidConstants.SMALLEST_TIME_INTERVAL);
            if (!this.timeScaleRequestCounts.ContainsKey(timeScaleIndex))
            {
                this.timeScaleRequestCounts.Add(timeScaleIndex, 0);
            }
            this.timeScaleRequestCounts[timeScaleIndex]++;
            this.UpdateRequestedTimeScale();
        }

        public void ReleaseTimeScale(float timeScale)
        {
            int timeScaleIndex = Mathf.RoundToInt(timeScale / FrigidConstants.SMALLEST_TIME_INTERVAL);
            this.timeScaleRequestCounts[timeScaleIndex]--;
            if (this.timeScaleRequestCounts[timeScaleIndex] == 0)
            {
                this.timeScaleRequestCounts.Remove(timeScaleIndex);
            }
            this.UpdateRequestedTimeScale();
        }

        public void Heal(int heal)
        {
            if (this.Status == MobStatus.Dead) return;

            heal = Mathf.Max(0, heal);

            int previousRemainingHealth = this.RemainingHealth;
            this.remainingHealth = Mathf.Min(this.RemainingHealth + heal, this.MaxHealth);

            this.onHealed?.Invoke(heal);
            if (previousRemainingHealth != this.RemainingHealth) this.onRemainingHealthChanged?.Invoke(previousRemainingHealth, this.RemainingHealth);
        }

        public void Damage(int damage)
        {
            if (this.Status == MobStatus.Dead) return;

            damage = Mathf.Max(0, damage);

            int previousRemainingHealth = this.RemainingHealth;
            this.remainingHealth = Mathf.Max(this.RemainingHealth - damage, 0);

            this.onDamaged?.Invoke(damage);
            if (previousRemainingHealth != this.RemainingHealth) this.onRemainingHealthChanged?.Invoke(previousRemainingHealth, this.RemainingHealth);
        }

        public void StunByStagger(float stagger)
        {
            if (stagger > 0)
            {
                this.StunForDuration(stagger / Mathf.Max(1f, this.Poise));
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
                    this.RequestTimeScale(0f);
                    this.stopDealingDamage.Request();
                    this.stunned = true;
                    this.onStunnedChanged?.Invoke();
                }
            }
        }

        public int GetTotalStatAmount(MobStat stat)
        {
            int totalStatAmount = 0;
            for (int i = 0; i < (int)MobStatLayer.Count; i++)
            {
                totalStatAmount += this.GetStatAmount((MobStatLayer)i, stat);
            }
            return totalStatAmount;
        }

        public int GetStatAmount(MobStatLayer statLayer, MobStat stat)
        {
            (int amount, Action<int, int> onAmountChanged) statCollectionEntry = this.statGrid[(int)statLayer][(int)stat];
            return statCollectionEntry.amount;
        }

        public void SetTotalStatAmount(MobStat stat, int totalAmount)
        {
            for (int i = 0; i < (int)MobStatLayer.Count; i++)
            {
                this.SetStatAmount((MobStatLayer)i, stat, totalAmount);
            }
        }

        public void SetStatAmount(MobStatLayer statLayer, MobStat stat, int amount)
        {
            (int amount, Action<int, int> onAmountChanged) statCollectionEntry = this.statGrid[(int)statLayer][(int)stat];
            if (statCollectionEntry.amount != amount)
            {
                int prevAmount = statCollectionEntry.amount;
                statCollectionEntry.amount = amount;
                this.statGrid[(int)statLayer][(int)stat] = statCollectionEntry;
                statCollectionEntry.onAmountChanged?.Invoke(prevAmount, amount);
            }
        }

        public void SubscribeToTotalStatChange(MobStat stat, Action<int, int> onAmountChanged)
        {
            for (int i = 0; i < (int)MobStatLayer.Count; i++)
            {
                this.SubscribeToStatChange((MobStatLayer)i, stat, onAmountChanged);
            }
        }

        public void SubscribeToStatChange(MobStatLayer statLayer, MobStat stat, Action<int, int> onAmountChanged)
        {
            (int amount, Action<int, int> onAmountChanged) statMappingValue = this.statGrid[(int)statLayer][(int)stat];
            statMappingValue.onAmountChanged += onAmountChanged;
            this.statGrid[(int)statLayer][(int)stat] = statMappingValue;
        }

        public void UnsubscribeToTotalStatChange(MobStat stat, Action<int, int> onAmountChanged)
        {
            for (int i = 0; i < (int)MobStatLayer.Count; i++)
            {
                this.UnsubscribeToStatChange((MobStatLayer)i, stat, onAmountChanged);
            }
        } 

        public void UnsubscribeToStatChange(MobStatLayer statLayer, MobStat stat, Action<int, int> onAmountChanged)
        {
            (int amount, Action<int, int> onAmountChanged) statMappingValue = this.statGrid[(int)statLayer][(int)stat];
            statMappingValue.onAmountChanged -= onAmountChanged;
            this.statGrid[(int)statLayer][(int)stat] = statMappingValue;
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
            return this.SetForcedMove(
                move,
                () =>
                {
                    this.ClearForcedMove(move);
                    onFinished?.Invoke();
                },
                onBeginMotion,
                onEndMotion
                );
        }

        public bool SetForcedMove(Move move, Action onFinished = null, Action onBeginMotion = null, Action onEndMotion = null)
        {
            return this.mover.HighestPriority < (int)MobMovePriority.Forced && this.mover.AddMove(move, (int)MobMovePriority.Forced, false, onFinished, onBeginMotion, onEndMotion);
        }

        public bool ClearForcedMove(Move move)
        {
            return this.mover.RemoveMove(move) && this.mover.HighestPriority < (int)MobMovePriority.Forced;
        }

        public bool DoMove(Move move, MobMovePriority priority, bool ignoreTimeScale, Action onFinished = null, Action onBeginMotion = null, Action onEndMotion = null)
        {
            if (priority == MobMovePriority.Forced)
            {
                Debug.LogWarning("Tried to do Move " + move.name + " with MobMovePriority.Forced. Forced priority is reserved for special movement.");
                return false;
            }
            return this.mover.DoMove(move, (int)priority, ignoreTimeScale, onFinished, onBeginMotion, onEndMotion);
        }

        public bool AddMove(Move move, MobMovePriority priority, bool ignoreTimeScale, Action onFinished = null, Action onBeginMotion = null, Action onEndMotion = null)
        {
            if (priority == MobMovePriority.Forced)
            {
                Debug.LogWarning("Tried to add Move " + move.name + " with MobMovePriority.Forced. Forced priority is reserved for special movement.");
                return false;
            }
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
            this.UpdateCurrentModeFromRequests();
        }

        public void ReleasePushMode(MobPushMode mode)
        {
            this.numPushModeRequests[(int)mode]--;
            this.UpdateCurrentModeFromRequests();
        }

        public bool CanSeeUnobstructed(Vector2 originPosition, Vector2 sightPosition, float blockingRadius)
        {
            return !this.SightCast(originPosition, sightPosition - originPosition, Vector2.Distance(sightPosition, originPosition), blockingRadius, out Collider2D collider) || collider.bounds.Contains(sightPosition);
        }

        public bool CanPassThrough(Vector2 originPosition, Vector2 pushPosition)
        {
            return !this.PushCast(originPosition, pushPosition - originPosition, Vector2.Distance(pushPosition, originPosition), out Collider2D collider) || collider.bounds.Contains(pushPosition);
        }

        public bool SightCast(Vector2 originPosition, Vector2 direction, float distance, float blockingRadius, out Collider2D collider)
        {
            direction = direction.normalized;
            LayerMask layerMask = FrigidLayerMask.GetCollidingMask(FrigidLayer.Sight);
            RaycastHit2D circleCastHit = Physics2D.CircleCast(originPosition + direction * blockingRadius, blockingRadius, direction, distance - blockingRadius, layerMask);
            collider = circleCastHit.collider;
            return collider != null;
        }

        public bool PushCast(Vector2 originPosition, Vector2 direction, float distance, out Collider2D collider)
        {
            direction = direction.normalized;
            if (this.currentPushMode != MobPushMode.IgnoreEverything)
            {
                float sizeMagnitude = this.Size.magnitude;
                if (sizeMagnitude > 0)
                {
                    List<PushColliderAnimatorProperty> pushColliderProperties = this.animatorBody.GetReferencedProperties<PushColliderAnimatorProperty>();
                    for (int i = 0; i < pushColliderProperties.Count; i++)
                    {
                        pushColliderProperties[i].Layer = (int)FrigidLayer.IgnoreRaycast;
                    }
                    FrigidLayer pushLayer = this.CalculatePushLayer();
                    LayerMask layerMask = FrigidLayerMask.GetCollidingMask(pushLayer);
                    RaycastHit2D boxCastHit = Physics2D.BoxCast(originPosition + direction * sizeMagnitude / 2, this.Size, 0, direction, distance - sizeMagnitude / 2, layerMask);
                    for (int i = 0; i < pushColliderProperties.Count; i++)
                    {
                        pushColliderProperties[i].Layer = (int)pushLayer;
                    }
                    collider = boxCastHit.collider;
                    return collider != null;
                }
            }
            collider = null;
            return false;
        }

        public bool DoBehaviour(MobBehaviour behaviour, bool ignoreTimeScale, Action onFinished = null)
        {
            return this.AddBehaviour(
                behaviour,
                ignoreTimeScale,
                () =>
                {
                    this.RemoveBehaviour(behaviour);
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
                this.BeginBehaviour(behaviour);
                return true;
            }
            return false;
        }

        public bool RemoveBehaviour(MobBehaviour behaviour)
        {
            if (this.behaviourMap.ContainsKey(behaviour))
            {
                this.EndBehaviour(behaviour);
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

        public bool TryGetEquipPointInContext(MobEquipContext equipContext, out MobEquipPoint equipPoint)
        {
            return this.contextualEquipPoints.TryGetValue(equipContext, out equipPoint);
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
            this.gameObject.SetActive(true);
            AddActiveMob(this, this.TiledArea);
            foreach (MobBehaviour behaviour in this.behaviourMap.Keys)
            {
                this.BeginBehaviour(behaviour);
            }
            this.BeginRootStateNode();
            this.onActiveChanged?.Invoke();
        }

        protected virtual void OnInactive()
        {
            this.EndRootStateNode();
            foreach (MobBehaviour behaviour in this.behaviourMap.Keys)
            {
                this.EndBehaviour(behaviour);
            }
            RemoveActiveMob(this, this.TiledArea);
            this.gameObject.SetActive(false);
            this.onActiveChanged?.Invoke();
        }

        private static void AddActiveMob(Mob mob, TiledArea tiledArea)
        {
            if (!mob.Active) return;
            if (!activeMobsInTiledAreas.Current.ContainsKey(tiledArea))
            {
                activeMobsInTiledAreas.Current.Add(tiledArea, new HashSet<Mob>());
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
                        this.ReleaseTimeScale(0f);
                        this.stopDealingDamage.Release();
                        this.stunned = false;
                        this.onStunnedChanged?.Invoke();
                    }
                }

                this.rootStateNode.Refresh();

                Dictionary<MobBehaviour, (bool, Action, bool)> behaviourMapCopy = new Dictionary<MobBehaviour, (bool, Action, bool)>(this.behaviourMap);
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
                float timeScaleInRequest = timeScaleRequestCount.Key * FrigidConstants.SMALLEST_TIME_INTERVAL;
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
                int previousRemainingHealth = this.RemainingHealth;
                this.remainingHealth = Mathf.Clamp(this.RemainingHealth, 0, this.MaxHealth);
                if (previousRemainingHealth != this.RemainingHealth) this.onRemainingHealthChanged?.Invoke(previousRemainingHealth, this.RemainingHealth);
                
                this.onMaxHealthChanged?.Invoke(prevMaxHealth, this.MaxHealth); 
            }
        }

        private void EnableDamageDealers()
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>()) hitBoxProperty.IsIgnoringDamage = false;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>()) breakBoxProperty.IsIgnoringDamage = false;
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>()) threatBoxProperty.IsIgnoringDamage = false;
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>()) attackProperty.ForceStop = false;
        }

        private void DisableDamageDealers()
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>()) hitBoxProperty.IsIgnoringDamage = true;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>()) breakBoxProperty.IsIgnoringDamage = true;
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>()) threatBoxProperty.IsIgnoringDamage = true;
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>()) attackProperty.ForceStop = true;
        }

        private void UpdateDamageBonusFromMightChange(int previousMight, int currentMight)
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageBonus += DamageBonusFromMight(currentMight) - DamageBonusFromMight(previousMight);
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageBonus += DamageBonusFromMight(currentMight) - DamageBonusFromMight(previousMight);
            }
        }

        private void EnableDamageReceivers()
        {
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.animatorBody.GetReferencedProperties<HurtBoxAnimatorProperty>()) hurtBoxProperty.IsIgnoringDamage = false;
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>()) resistBoxProperty.IsIgnoringDamage = false;
            foreach (LookoutBoxAnimatorProperty lookoutBoxProperty in this.animatorBody.GetReferencedProperties<LookoutBoxAnimatorProperty>()) lookoutBoxProperty.IsIgnoringDamage = false;
        }

        private void DisableDamageReceivers()
        {
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.animatorBody.GetReferencedProperties<HurtBoxAnimatorProperty>()) hurtBoxProperty.IsIgnoringDamage = true;
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>()) resistBoxProperty.IsIgnoringDamage = true;
            foreach (LookoutBoxAnimatorProperty lookoutBoxProperty in this.animatorBody.GetReferencedProperties<LookoutBoxAnimatorProperty>()) lookoutBoxProperty.IsIgnoringDamage = true;
        }

        private void UpdateDamageMitigationFromArmourChange(int previousArmour, int currentArmour)
        {
            foreach (HurtBoxAnimatorProperty hurtBoxAnimatorProperty in this.animatorBody.GetReferencedProperties<HurtBoxAnimatorProperty>())
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
                this.UpdatePushColliders();
            }
        }

        private void UpdatePushColliders()
        {
            FrigidLayer layer = this.CalculatePushLayer();
            foreach (PushColliderAnimatorProperty pushColliderProperty in this.animatorBody.GetReferencedProperties<PushColliderAnimatorProperty>())
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
                this.UpdatePushColliders();
                this.onTraversableTerrainChanged?.Invoke();
            }
            if (currentState.Height != previousState.Height) this.onHeightChanged?.Invoke();
            if (currentState.ShowDisplays != previousState.ShowDisplays) this.onShowDisplaysChanged?.Invoke();
            if (currentState.Status != previousState.Status) this.onStatusChanged?.Invoke();
        }
    }
}
