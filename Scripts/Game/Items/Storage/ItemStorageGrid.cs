using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemStorageGrid
    {
        private Vector2Int dimensions;
        private ItemContainer container;
        private ContainerItemStash[][] stashes;

        public Vector2Int Dimensions
        {
            get
            {
                return this.dimensions;
            }
        }

        public ItemContainer Container
        {
            get
            {
                return this.container;
            }
        }

        public ItemStorageGrid(ItemContainer container, ItemStorage storage)
        {
            this.dimensions = container.Dimensions;
            this.container = container;
            this.stashes = new ContainerItemStash[this.container.Dimensions.x][];
            for (int x = 0; x < this.stashes.Length; x++)
            {
                this.stashes[x] = new ContainerItemStash[this.container.Dimensions.y];
                for (int y = 0; y < this.stashes[x].Length; y++)
                {
                    this.stashes[x][y] = new ContainerItemStash(this.container, storage);
                }
            }
        }
        
        public bool TryGetStash(Vector2Int indexPosition, out ContainerItemStash stash)
        {
            if (indexPosition.x >= this.dimensions.x || indexPosition.x < 0 || indexPosition.y >= this.dimensions.y || indexPosition.y < 0)
            {
                stash = null;
                return false;
            }
            stash = this.stashes[indexPosition.x][indexPosition.y];
            return true;
        }
    }
}
