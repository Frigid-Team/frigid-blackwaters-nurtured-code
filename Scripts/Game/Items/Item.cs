using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public abstract class Item : FrigidMonoBehaviour
    {
        public abstract bool IsUsable { get; }

        public abstract bool IsInEffect { get; }
        
        public virtual bool Used(List<Mob> usingMobs, ItemPowerBudget itemPowerBudget) { return false; }

        public virtual void Stashed(List<Mob> usingMobs, ItemPowerBudget itemPowerBudget) { }

        public virtual void Unstashed(List<Mob> usingMobs, ItemPowerBudget itemPowerBudget) { }

        protected void ApplyItemRule(List<Mob> usingMobs, ItemRule itemRule)
        {
            foreach (Mob usingMob in usingMobs)
            {
                itemRule.ApplyToMob(usingMob);
            }
        }

        protected void UnapplyItemRule(List<Mob> usingMobs, ItemRule itemRule)
        {
            foreach (Mob usingMob in usingMobs)
            {
                itemRule.UnapplyToMob(usingMob);
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
