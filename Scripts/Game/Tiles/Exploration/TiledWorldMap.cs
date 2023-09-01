using System;
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
        
        private Action onMapActionPerformed;

        public float WorldToMapScalingFactor
        {
            get
            {
                return this.worldToMapScalingFactor;
            }
        }

        public Action OnMapActionPerformed
        {
            get
            {
                return this.onMapActionPerformed;
            }
            set
            {
                this.onMapActionPerformed = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentCells = new List<TiledWorldMapCell>();
            this.cellPool = new RecyclePool<TiledWorldMapCell>(
                () => CreateInstance<TiledWorldMapCell>(this.cellPrefab, this.cellsTransform, false),
                (TiledWorldMapCell cell) => DestroyInstance(cell)
                );
            this.currentCellConnectors = new List<TiledWorldMapCellConnector>();
            this.cellConnectorPool = new RecyclePool<TiledWorldMapCellConnector>(
                () => CreateInstance<TiledWorldMapCellConnector>(this.cellConnectorPrefab, this.cellsTransform, false),
                (TiledWorldMapCellConnector connector) => DestroyInstance(connector)
                );
            if (this.showTokens)
            {
                this.currentTokens = new List<TiledWorldMapToken>();
                this.tokenPool = new RecyclePool<TiledWorldMapToken>(
                    () => CreateInstance<TiledWorldMapToken>(this.tokenPrefab, this.tokensTransform, false),
                    (TiledWorldMapToken token) => DestroyInstance(token)
                    );
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TiledWorldExplorer.OnExploredArea += this.RefreshCells;
            TiledWorldDiscovery.OnDiscoveryRevealed += this.RefreshTokens;
            TiledWorldDiscovery.OnDiscoveryHidden += this.RefreshTokens;
            TiledLevel.OnFocusedLevelChanged += this.RefreshCells;
            TiledLevel.OnFocusedLevelChanged += this.RefreshTokens;
            this.RefreshCells();
            this.RefreshTokens();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TiledWorldExplorer.OnExploredArea -= this.RefreshCells;
            TiledWorldDiscovery.OnDiscoveryRevealed -= this.RefreshTokens;
            TiledWorldDiscovery.OnDiscoveryHidden -= this.RefreshTokens;
            TiledLevel.OnFocusedLevelChanged -= this.RefreshCells;
            TiledLevel.OnFocusedLevelChanged -= this.RefreshTokens;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        public void MoveBy(Vector2 worldPositionDelta)
        {
            this.MoveTo((Vector2)this.cellsTransform.localPosition / this.worldToMapScalingFactor + worldPositionDelta);
        }

        public void MoveTo(Vector2 worldPosition)
        {
            if (TiledLevel.TryGetFocusedLevel(out TiledLevel focusedLevel)) 
            {
                float clampedX = Mathf.Clamp(
                    worldPosition.x,
                    -focusedLevel.CenterPosition.x - focusedLevel.WallBoundsSize.x / 2, 
                    -focusedLevel.CenterPosition.x + focusedLevel.WallBoundsSize.x / 2
                    );
                float clampedY = Mathf.Clamp(
                    worldPosition.y,
                    -focusedLevel.CenterPosition.y - focusedLevel.WallBoundsSize.y / 2,
                    -focusedLevel.CenterPosition.y + focusedLevel.WallBoundsSize.y / 2
                    );
                this.cellsTransform.localPosition = new Vector2(clampedX, clampedY) * this.worldToMapScalingFactor;
                if (this.showTokens) this.tokensTransform.localPosition = this.cellsTransform.localPosition;
            }
        }

        private void MapActionPerformed() => this.onMapActionPerformed?.Invoke();

        private void RefreshCells(TiledArea exploredArea)
        {
            this.RefreshCells();
        }

        private void RefreshCells()
        {
            if (TiledLevel.TryGetFocusedLevel(out TiledLevel focusedLevel))
            {
                this.cellPool.Cycle(this.currentCells, focusedLevel.ContainingAreas.Count);

                List<TiledArea> containingAreas = focusedLevel.ContainingAreas.ToList();
                for (int i = 0; i < containingAreas.Count; i++)
                {
                    this.currentCells[i].FillCell(focusedLevel, containingAreas[i], TiledWorldExplorer.ExploredAreas.Contains(containingAreas[i]), this.worldToMapScalingFactor, this.MapActionPerformed);
                }

                List<(TiledEntrance, TiledEntrance)> containingConnections = focusedLevel.ContainingConnections.ToList();
                this.cellConnectorPool.Cycle(this.currentCellConnectors, containingConnections.Count);
                for (int i = 0; i < containingConnections.Count; i++)
                {
                    TiledEntrance firstEntrance = containingConnections[i].Item1;
                    TiledEntrance secondEntrance = containingConnections[i].Item2;
                    TiledArea firstTiledArea = firstEntrance.ContainedArea;
                    TiledArea secondTiledArea = secondEntrance.ContainedArea;
                    this.currentCellConnectors[i].FillConnector(
                        focusedLevel, 
                        firstEntrance, 
                        secondEntrance, 
                        TiledWorldExplorer.ExploredAreas.Contains(firstTiledArea) || TiledWorldExplorer.ExploredAreas.Contains(secondTiledArea), 
                        this.worldToMapScalingFactor
                        );
                }
            }
        }

        private void RefreshTokens(TiledWorldDiscovery discoveryChange)
        {
            this.RefreshTokens();
        }

        private void RefreshTokens()
        {
            if (this.showTokens && TiledLevel.TryGetFocusedLevel(out TiledLevel focusedLevel))
            {
                int numberTokens = 0;
                foreach (TiledArea containingArea in focusedLevel.ContainingAreas)
                {
                    if (TiledWorldDiscovery.TryGetDiscoveriesInTiledArea(containingArea, out HashSet<TiledWorldDiscovery> discoveries) && TiledWorldExplorer.ExploredAreas.Contains(containingArea))
                    {
                        numberTokens += discoveries.Count;
                    }
                }

                this.tokenPool.Cycle(this.currentTokens, numberTokens);

                int numTokensUsed = 0;
                foreach (TiledArea containingArea in focusedLevel.ContainingAreas)
                {
                    if (TiledWorldExplorer.ExploredAreas.Contains(containingArea) && TiledWorldDiscovery.TryGetDiscoveriesInTiledArea(containingArea, out HashSet<TiledWorldDiscovery> discoveries))
                    {
                        int tokenIndex = 0;
                        foreach (TiledWorldDiscovery discovery in discoveries)
                        {
                            this.currentTokens[numTokensUsed].FillToken(
                                containingArea,
                                discovery,
                                discoveries.Count,
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
