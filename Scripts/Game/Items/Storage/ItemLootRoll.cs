namespace FrigidBlackwaters.Game
{
    public class ItemLootRoll
    {
        private ItemStorable itemStorable;
        private int quantity;

        public ItemLootRoll(ItemStorable itemStorable, int quantity)
        {
            this.itemStorable = itemStorable;
            this.quantity = quantity;
        }

        public ItemStorable ItemStorable
        {
            get
            {
                return this.itemStorable;
            }
        }

        public int Quantity
        {
            get
            {
                return this.quantity;
            }
        }
    }
}
