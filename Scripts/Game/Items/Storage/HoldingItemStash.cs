namespace FrigidBlackwaters.Game
{
    public class HoldingItemStash : ItemStash
    {
        public HoldingItemStash(ItemStorage storage) : base(storage) { }

        protected override int CalculateMaxCapacity(ItemStorable storable)
        {
            return int.MaxValue;
        }
    }
}
