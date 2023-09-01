namespace FrigidBlackwaters.Game
{
    public class TiledLevelPlanEntrance
    {
        private TiledLevelPlanArea area;
        private TiledEntrance subLevelEntrance;
        private bool isSubLevelEntrance;

        public TiledLevelPlanEntrance(TiledLevelPlanArea area)
        {
            this.area = area;
            this.isSubLevelEntrance = false;
        }

        public TiledLevelPlanEntrance(TiledEntrance subLevelEntrance)
        {
            this.subLevelEntrance = subLevelEntrance;
            this.isSubLevelEntrance = true;
        }

        public TiledLevelPlanArea Area
        {
            get
            {
                return this.area;
            }
        }

        public TiledEntrance SublevelEntrance
        {
            get
            {
                return this.subLevelEntrance;
            }
        }

        public bool IsSubLevelEntrance
        {
            get
            {
                return this.isSubLevelEntrance;
            }
        }
    }
}
