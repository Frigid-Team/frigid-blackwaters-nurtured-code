namespace FrigidBlackwaters.Game
{
    public class TiledLevelPlanEntrance
    {
        private TiledLevelPlanArea area;
        private TiledAreaEntrance entrancePrefab;
        private TiledAreaEntrance subLevelEntrance;
        private bool isSubLevelEntrance;

        public TiledLevelPlanEntrance(TiledLevelPlanArea area, TiledAreaEntrance entrancePrefab)
        {
            this.area = area;
            this.entrancePrefab = entrancePrefab;
            this.isSubLevelEntrance = false;
        }

        public TiledLevelPlanEntrance(TiledAreaEntrance subLevelEntrance)
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

        public TiledAreaEntrance EntrancePrefab
        {
            get
            {
                return this.entrancePrefab;
            }
        }

        public TiledAreaEntrance SublevelEntrance
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
