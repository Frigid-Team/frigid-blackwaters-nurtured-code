namespace FrigidBlackwaters.Game
{
    public class Valuable : Item
    {
        public override bool IsUsable
        {
            get
            {
                return false;
            }
        }

        public override bool IsInEffect
        {
            get
            {
                return false;
            }
        }
    }
}
