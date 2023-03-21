using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class TiledLevelPlan
    {
        private TiledLevelPlanArea startingArea;
        private HashSet<TiledLevelPlanArea> areas;
        private HashSet<TiledLevelPlanConnection> connections;
        private bool isSubLevel;
        private TiledAreaMobGenerator mobGenerator;

        public TiledLevelPlan(TiledLevelPlanArea startingArea, TiledAreaMobGenerator mobGenerator)
        {
            this.startingArea = startingArea;
            this.areas = new HashSet<TiledLevelPlanArea>();
            this.areas.Add(this.startingArea);
            this.connections = new HashSet<TiledLevelPlanConnection>();
            this.isSubLevel = false;
            this.mobGenerator = mobGenerator;
        }

        public TiledLevelPlanArea StartingArea
        {
            get
            {
                return this.startingArea;
            }
        }

        public HashSet<TiledLevelPlanArea> Areas
        {
            get
            {
                return this.areas;
            }
        }

        public HashSet<TiledLevelPlanConnection> Connections
        {
            get
            {
                return this.connections;
            }
        }

        public bool IsSubLevel
        {
            get
            {
                return this.isSubLevel;
            }
        }

        public TiledAreaMobGenerator MobGenerator
        {
            get
            {
                return this.mobGenerator;
            }
        }

        public void AddArea(TiledLevelPlanArea tiledPlanArea)
        {
            this.areas.Add(tiledPlanArea);
        }

        public void AddConnection(TiledLevelPlanConnection tiledPlanConnection)
        {
            this.connections.Add(tiledPlanConnection);
            this.isSubLevel |= tiledPlanConnection.IsSubLevelConnection;

            if (!tiledPlanConnection.FirstEntrance.IsSubLevelEntrance)
            {
                AddArea(tiledPlanConnection.FirstEntrance.Area);
            }
            if (!tiledPlanConnection.SecondEntrance.IsSubLevelEntrance)
            {
                AddArea(tiledPlanConnection.SecondEntrance.Area);
            }
        }
    }
}
