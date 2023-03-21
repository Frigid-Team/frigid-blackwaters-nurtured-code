namespace FrigidBlackwaters.Game
{
    public class ContainerItemStash : ItemStash
    {
        private ItemContainer container;

        public ContainerItemStash(ItemContainer container, ItemStorage itemStorage) : base(itemStorage)
        {
            this.container = container;
        }

        protected override int CalculateMaxCapacity(ItemStorable storable)
        {
            return this.container.CalculateMaxCapacityFromStorable(storable);
        }
    }
}
