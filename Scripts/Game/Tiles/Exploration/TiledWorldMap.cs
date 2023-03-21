using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldMap : FrigidMonoBehaviour
    {
        [SerializeField]
        private TiledWorldMapCell cellPrefab;
        [SerializeField]
        private TiledWorldMapCellConnector cellConnectorPrefab;
        [SerializeField]
        private RectTransform cellsTransform;
        [SerializeField]
        private bool showTokens;
        [SerializeField]
        [ShowIfBool("showTokens", true)]
        private TiledWorldMapToken tokenPrefab;
        [SerializeField]
        [ShowIfPreviouslyShown(true)]
        private RectTransform tokensTransform;
        [SerializeField]
        private float worldToMapScalingFactor;

        private List<TiledWorldMapCell> currentCells;
        private RecyclePool<TiledWorldMapCell> cellPool;
        private List<TiledWorldMapCellConnector> currentCellConnectors;
        private RecyclePool<TiledWorldMapCellConnector> cellConnectorPool;
        private List<TiledWorldMapToken> currentTokens;
        private RecyclePool<TiledWorldMapToken> tokenPool;

        public float WorldToMapScalingFactor
        {
            get
            {
                return this.worldToMapScalingFactor;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentCells = new List<TiledWorldMapCell>();
            this.cellPool = new RecyclePool<TiledWorldMapCell>(
                () => FrigidInstancing.CreateInstance<TiledWorldMapCell>(this.cellPrefab, this.cellsTransform, false),
                (TiledWorldMapCell cell) => FrigidInstancing.DestroyInstance(cell)
                );
            this.currentCellConnectors = new List<TiledWorldMapCellConnector>();
            this.cellConnectorPool = new RecyclePool<TiledWorldMapCellConnector>(
                () => FrigidInstancing.CreateInstance<TiledWorldMapCellConnector>(this.cellConnectorPrefab, this.cellsTransform, false),
                (TiledWorldMapCellConnector connector) => FrigidInstancing.DestroyInstance(connector)
                );
            if (this.showTokens)
            {
                this.currentTokens = new List<TiledWorldMapToken>();
                this.tokenPool = new RecyclePool<TiledWorldMapToken>(
                    () => FrigidInstancing.CreateInstance<TiledWorldMapToken>(this.tokenPrefab, this.tokensTransform, false),
                    (TiledWorldMapToken token) => FrigidInstancing.DestroyInstance(token)
                    );
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TiledWorldExplorer.OnExploredNewTiledArea += RefreshCells;
            TiledWorldDiscovery.OnTiledWorldDiscoveryRevealed += RefreshTokens;
            TiledWorldDiscovery.OnTiledWorldDiscoveryHidden += RefreshTokens;
            TiledLevel.OnFocusedTiledLevelChanged += RefreshCells;
            TiledLevel.OnFocusedTiledLevelChanged += RefreshTokens;
            RefreshCells();
            RefreshTokens();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TiledWorldExplorer.OnExploredNewTiledArea -= RefreshCells;
            TiledWorldDiscovery.OnTiledWorldDiscoveryRevealed -= RefreshTokens;
            TiledWorldDiscovery.OnTiledWorldDiscoveryHidden -= RefreshTokens;
            TiledLevel.OnFocusedTiledLevelChanged -= RefreshCells;
            TiledLevel.OnFocusedTiledLevelChanged -= RefreshTokens;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        public void MoveBy(Vector2 worldPositionDelta)
        {
            MoveTo((Vector2)this.cellsTransform.localPosition / this.worldToMapScalingFactor + worldPositionDelta);
        }

        public void MoveTo(Vector2 worldPosition)
        {
            if (TiledLevel.TryGetFocusedTiledLevel(out TiledLevel focusedTiledLevel)) 
            {
                float clampedX = Mathf.Clamp(
                    worldPosition.x,
                    -focusedTiledLevel.CenterPosition.x - focusedTiledLevel.WallBoundsSize.x / 2, 
                    -focusedTiledLevel.CenterPosition.x + focusedTiledLevel.WallBoundsSize.x / 2
                    );
                float clampedY = Mathf.Clamp(
                    worldPosition.y,
                    -focusedTiledLevel.CenterPosition.y - focusedTiledLevel.WallBoundsSize.y / 2,
                    -focusedTiledLevel.CenterPosition.y + focusedTiledLevel.WallBoundsSize.y / 2
                    );
                this.cellsTransform.localPosition = new Vector2(clampedX, clampedY) * this.worldToMapScalingFactor;
                if (this.showTokens) this.tokensTransform.localPosition = this.cellsTransform.localPosition;
            }
        }

        private void RefreshCells(TiledArea exploredTiledArea)
        {
            RefreshCells();
        }

        private void RefreshCells()
        {
            if (TiledLevel.TryGetFocusedTiledLevel(out TiledLevel focusedTiledLevel))
            {
                this.cellPool.Cycle(this.currentCells, focusedTiledLevel.TiledLevelPlan.Areas.Count);

                List<TiledLevelPlanArea> planAreas = focusedTiledLevel.TiledLevelPlan.Areas.ToList();
                for (int i = 0; i < planAreas.Count; i++)
                {
                    TiledArea tiledArea = focusedTiledLevel.SpawnedAreaPerPlanAreas[planAreas[i]];
                    this.currentCells[i].FillCell(tiledArea, TiledWorldExplorer.ExploredTiledAreas.Contains(tiledArea), this.worldToMapScalingFactor);
                }

                List<TiledLevelPlanConnection> areaToAreaPlanConnections = focusedTiledLevel.TiledLevelPlan.Connections.ToList().FindAll(
                    (TiledLevelPlanConnection planConnection) => !planConnection.IsSubLevelConnection
                    );
                this.cellConnectorPool.Cycle(this.currentCellConnectors, areaToAreaPlanConnections.Count);
                for (int i = 0; i < areaToAreaPlanConnections.Count; i++)
                {
                    TiledArea firstTiledArea = focusedTiledLevel.SpawnedAreaPerPlanAreas[areaToAreaPlanConnections[i].FirstEntrance.Area];
                    TiledArea secondTiledArea = focusedTiledLevel.SpawnedAreaPerPlanAreas[areaToAreaPlanConnections[i].SecondEntrance.Area];
                    this.currentCellConnectors[i].FillConnector(
                        focusedTiledLevel.SpawnedEntrancePerPlanEntrances[areaToAreaPlanConnections[i].FirstEntrance],
                        focusedTiledLevel.SpawnedEntrancePerPlanEntrances[areaToAreaPlanConnections[i].SecondEntrance],
                        TiledWorldExplorer.ExploredTiledAreas.Contains(firstTiledArea) || TiledWorldExplorer.ExploredTiledAreas.Contains(secondTiledArea),
                        this.worldToMapScalingFactor
                        );
                }
            }
        }

        private void RefreshTokens(TiledWorldDiscovery tiledWorldDiscoveryChange)
        {
            RefreshTokens();
        }

        private void RefreshTokens()
        {
            if (this.showTokens && TiledLevel.TryGetFocusedTiledLevel(out TiledLevel focusedTiledLevel))
            {
                int numberTokens = 0;
                foreach (TiledArea spawnedTiledArea in focusedTiledLevel.SpawnedAreaPerPlanAreas.Values)
                {
                    if (TiledWorldDiscovery.TryGetDiscoveriesInTiledArea(spawnedTiledArea, out HashSet<TiledWorldDiscovery> tiledWorldDiscoveries) && TiledWorldExplorer.ExploredTiledAreas.Contains(spawnedTiledArea))
                    {
                        numberTokens += tiledWorldDiscoveries.Count;
                    }
                }

                this.tokenPool.Cycle(this.currentTokens, numberTokens);

                int numTokensUsed = 0;
                foreach (TiledArea tiledArea in focusedTiledLevel.SpawnedAreaPerPlanAreas.Values)
                {
                    if (TiledWorldExplorer.ExploredTiledAreas.Contains(tiledArea) && TiledWorldDiscovery.TryGetDiscoveriesInTiledArea(tiledArea, out HashSet<TiledWorldDiscovery> tiledWorldDiscoveries))
                    {
                        int tokenIndex = 0;
                        foreach (TiledWorldDiscovery tiledWorldDiscovery in tiledWorldDiscoveries)
                        {
                            this.currentTokens[numTokensUsed].FillToken(
                                tiledArea,
                                tiledWorldDiscovery,
                                tiledWorldDiscoveries.Count,
                                tokenIndex,
                                this.worldToMapScalingFactor
                                );
                            tokenIndex++;
                            numTokensUsed++;
                        }
                    }
                }
            }
        }
    }
}
