using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobSet : HashSet<Mob>
    {
        public MobSet() : base() { }

        public MobSet(MobSet other) : base(other) { }

        public MobSet ThatAreOfAlignments(params DamageAlignment[] alignments)
        {
            MobSet mobsOfAlignment = new MobSet();
            foreach (Mob mob in this)
            {
                foreach (DamageAlignment alignment in alignments)
                {
                    if (alignment == mob.Alignment)
                    {
                        mobsOfAlignment.Add(mob);
                        break;
                    }
                }
            }
            return mobsOfAlignment;
        }

        public MobSet ThatAreNotDead()
        {
            MobSet notDeadMobs = new MobSet();
            foreach (Mob mob in this)
            {
                if (!mob.Dead) notDeadMobs.Add(mob);
            }
            return notDeadMobs;
        }

        public MobSet ThatDoNotInclude(params Mob[] mobs)
        {
            MobSet notIncludedMobs = new MobSet(this);
            notIncludedMobs.ExceptWith(mobs);
            return notIncludedMobs;
        }
    }
}
