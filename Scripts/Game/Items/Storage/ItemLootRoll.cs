namespace FrigidBlackwaters.Game
{
    public class ItemLootRoll
    {
        private ItemStorable storable;
        private int quantity;

        public ItemLootRoll(ItemStorable storable, int quantity)
        {
            this.storable = storable;
            this.quantity = quantity;
        }

        public ItemStorable Storable
        {
            get
            {
                return this.storable;
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
