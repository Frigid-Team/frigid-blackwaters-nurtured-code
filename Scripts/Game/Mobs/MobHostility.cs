using System.Collections.Generic;
using System.Linq;

namespace FrigidBlackwaters.Game
{
    public static class MobHostility
    {
        public static HashSet<DamageAlignment> GetHostileAlignments(DamageAlignment alignment)
        {
            HashSet<DamageAlignment> hostileMobGroups = new HashSet<DamageAlignment>();
            for (int i = 0; i < (int)DamageAlignment.Count; i++)
            {
                DamageAlignment currentAlignment = (DamageAlignment)i;
                bool isHostile = false;
                switch (currentAlignment)
                {
                    case DamageAlignment.Voyagers:
                        isHostile = alignment == DamageAlignment.Labyrinth;
                        break;
                    case DamageAlignment.Labyrinth:
                        isHostile = alignment == DamageAlignment.Voyagers;
                        break;
                }
                if (isHostile)
                {
                    hostileMobGroups.Add(currentAlignment);
                }
            }
            return hostileMobGroups;
        }

        public static List<Mob> GetHostileMobsOf(Mob mob)
        {
            IEnumerable<Mob> hostileMobs = Mob.GetMobsInTiledArea(mob.TiledArea).ActiveMobs;
            foreach (DamageAlignment hostileAlignment in GetHostileAlignments(mob.Alignment))
            {
                hostileMobs = hostileMobs.Intersect(Mob.GetMobsOfAlignment(hostileAlignment).ActiveMobs);
            }
            return hostileMobs.ToList();
        }
    }
}
