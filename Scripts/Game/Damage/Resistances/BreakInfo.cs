namespace FrigidBlackwaters.Game
{
    public class BreakInfo
    {
        private bool broken;

        public BreakInfo(Resistance offensiveResistance, Resistance defensiveResistance)
        {
            this.broken = ((int)offensiveResistance >= (int)defensiveResistance || defensiveResistance == Resistance.None) && defensiveResistance != Resistance.Unbreakable;
        }

        public bool Broken
        {
            get
            {
                return this.broken;
            }
        }
    }
}
