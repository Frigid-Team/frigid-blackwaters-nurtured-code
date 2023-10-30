using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "ExploreToFindExpedition", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Expeditions + "ExploreToFindExpedition")]
    public class ExploreToFindExpedition : SubGoalsExpedition<ExploreToFindExpedition.ExploreSubGoal>
    {
        [Serializable]
        public class ExploreSubGoal : SubGoal
        {
            [SerializeField]
            private TiledAreaBlueprintGroup tiledAreaBlueprintGroup;
            [SerializeField]
            private IntSerializedReference neededExploreCount;

            private List<TiledArea> tiledAreasToFind;
            private int currExploreCount;
            private Action onComplete;

            public override bool IsComplete
            {
                get
                {
                    return this.currExploreCount >= this.neededExploreCount.ImmutableValue;
                }
            }

            public override void Start(Action onComplete)
            {
                this.tiledAreasToFind = new List<TiledArea>();
                this.onComplete = onComplete;

                TiledArea.OnAreaSpawned += this.AddTiledAreaToFind;
                TiledWorldExplorer.OnExploredArea += this.CheckExploreCount;
            }

            public override void End()
            {
                TiledArea.OnAreaSpawned -= this.AddTiledAreaToFind;
                TiledWorldExplorer.OnExploredArea -= this.CheckExploreCount;

                this.tiledAreasToFind = null;
                this.onComplete = null;
            }

            private void AddTiledAreaToFind(TiledArea tiledArea, TiledAreaBlueprint tiledAreaBlueprint)
            {
                if (this.tiledAreaBlueprintGroup.Includes(tiledAreaBlueprint))
                {
                    this.tiledAreasToFind.Add(tiledArea);
                    this.CheckExploreCount();
                }
            }

            private void CheckExploreCount(TiledArea exploredTiledArea) => this.CheckExploreCount();

            private void CheckExploreCount()
            {
                this.currExploreCount = TiledWorldExplorer.ExploredAreas.Count((TiledArea exploredTiledArea) => this.tiledAreasToFind.Contains(exploredTiledArea));
                if (this.currExploreCount >= this.neededExploreCount.ImmutableValue)
                {
                    this.onComplete?.Invoke();
                }
            }
        }
    }
}
