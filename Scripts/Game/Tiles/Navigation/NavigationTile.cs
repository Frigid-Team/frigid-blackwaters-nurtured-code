namespace FrigidBlackwaters.Game
{
    public class NavigationTile
    {
        private TileTerrain terrain;
        private int[] obstructions;
        private Resistance highestObstructiveResistance;
        
        public NavigationTile(TileTerrain terrain)
        {
            this.terrain = terrain;
            this.obstructions = new int[(int)Resistance.Count];
            for (int i = 0; i < this.obstructions.Length; i++) this.obstructions[i] = 0;
            this.highestObstructiveResistance = Resistance.None;
        }

        public TileTerrain Terrain
        {
            get
            {
                return this.terrain;
            }
            set
            {
                this.terrain = value;
            }
        }

        public bool Unobstructed
        {
            get
            {
                return this.highestObstructiveResistance == Resistance.None;
            }
        }

        public Resistance HighestObstructiveResistance
        {
            get
            {
                return this.highestObstructiveResistance;
            }
        }

        public void AddObstruction(Resistance resistance)
        {
            this.obstructions[(int)resistance]++;
            if (this.obstructions[(int)resistance] == 1) UpdateHighestObstructiveResistance();
        }

        public void RemoveObstruction(Resistance resistance)
        {
            this.obstructions[(int)resistance]--;
            if (this.obstructions[(int)resistance] == 0) UpdateHighestObstructiveResistance();
        }

        private void UpdateHighestObstructiveResistance()
        {
            this.highestObstructiveResistance = Resistance.None;
            for (int i = this.obstructions.Length - 1; i >= 0; i--)
            {
                if (this.obstructions[i] > 0)
                {
                    this.highestObstructiveResistance = (Resistance)i;
                    break;
                }
            }
        }
    }
}
