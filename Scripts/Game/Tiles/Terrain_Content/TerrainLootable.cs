using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class TerrainLootable : TerrainObstruction
    {
        [SerializeField]
        private ItemStorage itemStorage;
        [SerializeField]
        private RelativeWeightPool<LootableTier> lootableTiers;
        [SerializeField]
        private float detectionRadius;

        private LootableTier chosenTier;
        private LootableAnimationSet chosenAnimations;
        private FrigidCoroutine detectionRoutine;

        public override void Populated(Vector2 orientationDirection, NavigationGrid navigationGrid, List<Vector2Int> allTileIndices)
        {
            base.Populated(orientationDirection, navigationGrid, allTileIndices);
            this.chosenTier = this.lootableTiers.Retrieve();
            this.chosenAnimations = this.chosenTier.LootableAnimations[UnityEngine.Random.Range(0, this.chosenTier.LootableAnimations.Length)];
            this.itemStorage.SetStorageGridsFromContainers(this.chosenTier.ItemContainers);
            foreach (ItemStorageGrid itemStorageGrid in this.itemStorage.StorageGrids)
            {
                itemStorageGrid.FillWithLootTable(this.chosenTier.ItemLootTable);
            }
            if (this.detectionRadius > 0)
            {
                this.AnimatorBody.Play(this.chosenAnimations.ClosedAnimationName);
                this.detectionRoutine = FrigidCoroutine.Run(DetectionLoop(), this.gameObject);
            }
            else
            {
                this.AnimatorBody.Play(this.chosenAnimations.DefaultAnimationName);
            }
        }

        protected override void Break()
        {
            base.Break();
            this.AnimatorBody.Play(this.chosenAnimations.BrokenAnimationName);
            FrigidCoroutine.Kill(this.detectionRoutine);
        }

        private Vector2 DetectionPosition
        {
            get
            {
                if (PlayerMob.TryGet(out PlayerMob player))
                {
                    return player.Position;
                }
                return (Vector2)this.transform.position + Vector2.one * this.detectionRadius;
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> DetectionLoop()
        {
            bool animationFinished;
            while (true)
            {
                while (Vector2.Distance(this.transform.position, this.DetectionPosition) > this.detectionRadius) yield return null;

                animationFinished = false;
                this.AnimatorBody.Play(
                    this.chosenAnimations.OpenAnimationName,
                    () =>
                    {
                        animationFinished = true;
                        this.AnimatorBody.Play(this.chosenAnimations.OpenedAnimationName);
                    }
                    );
                yield return new FrigidCoroutine.DelayUntil(() => { return animationFinished; });

                while (Vector2.Distance(this.transform.position, this.DetectionPosition) < this.detectionRadius) yield return null;

                animationFinished = false;
                this.AnimatorBody.Play(
                    this.chosenAnimations.CloseAnimationName,
                    () =>
                    {
                        animationFinished = true;
                        this.AnimatorBody.Play(this.chosenAnimations.ClosedAnimationName);
                    }
                    );
                yield return new FrigidCoroutine.DelayUntil(() => { return animationFinished; });
            }
        }

        [Serializable]
        private struct LootableTier
        {
            [SerializeField]
            private ItemLootTableSerializedReference itemLootTable;
            [SerializeField]
            private List<ItemContainer> itemContainers;
            [SerializeField]
            private LootableAnimationSet[] lootableAnimations;

            public ItemLootTable ItemLootTable
            {
                get
                {
                    return this.itemLootTable.ImmutableValue;
                }
            }

            public List<ItemContainer> ItemContainers
            {
                get
                {
                    return this.itemContainers;
                }
            }

            public LootableAnimationSet[] LootableAnimations
            {
                get
                {
                    return this.lootableAnimations;
                }
            }
        }

        [Serializable]
        private struct LootableAnimationSet
        {
            [SerializeField]
            [ShowIfFloat("detectionRadius", float.MinValue, 0, true, true)]
            private string defaultAnimationName;
            [SerializeField]
            [ShowIfFloat("detectionRadius", float.MinValue, 0, false, true)]
            private string openAnimationName;
            [SerializeField]
            [ShowIfPreviouslyShown(true)]
            private string openedAnimationName;
            [SerializeField]
            [ShowIfPreviouslyShown(true)]
            private string closeAnimationName;
            [SerializeField]
            [ShowIfPreviouslyShown(true)]
            private string closedAnimationName;
            [SerializeField]
            [ShowIfMethod("CanBreak", true, true)]
            private string brokenAnimationName;

            public string DefaultAnimationName
            {
                get
                {
                    return this.defaultAnimationName;
                }
            }

            public string OpenAnimationName
            {
                get
                {
                    return this.openAnimationName;
                }
            }

            public string OpenedAnimationName
            {
                get
                {
                    return this.openedAnimationName;
                }
            }

            public string CloseAnimationName
            {
                get
                {
                    return this.closeAnimationName;
                }
            }

            public string ClosedAnimationName
            {
                get
                {
                    return this.closedAnimationName;
                }
            }

            public string BrokenAnimationName
            {
                get
                {
                    return this.brokenAnimationName;
                }
            }
        }
    }
}
