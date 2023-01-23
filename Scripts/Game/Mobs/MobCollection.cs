using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobCollection
    {
        private LinkedList<Mob> activeMobs;
        private LinkedList<Mob> inactiveMobs;
        private Dictionary<Mob, Tuple<Action, Action>> callbacks;
        private Action<Mob> onAddedToActiveMobs;
        private Action<Mob> onRemovedFromActiveMobs;
        private Action onOrderChanged;
        private Action<bool, Mob, bool, Mob> onRecentlyActiveMobChanged;
        private Action<Mob> onMobAdded;
        private Action<Mob> onMobRemoved;

        public MobCollection()
        {
            this.activeMobs = new LinkedList<Mob>();
            this.inactiveMobs = new LinkedList<Mob>();
            this.callbacks = new Dictionary<Mob, Tuple<Action, Action>>();
        }

        public LinkedList<Mob> ActiveMobs
        {
            get
            {
                return this.activeMobs;
            }
        }

        public LinkedList<Mob> ActiveOrder
        {
            get
            {
                LinkedList<Mob> activationOrder = new LinkedList<Mob>();
                foreach (Mob activeMob in this.activeMobs)
                {
                    activationOrder.AddLast(activeMob);
                }
                foreach (Mob inactiveMob in this.inactiveMobs)
                {
                    activationOrder.AddLast(inactiveMob);
                }
                return activationOrder;
            }
        }

        public int Count
        {
            get
            {
                return this.activeMobs.Count + this.inactiveMobs.Count;
            }
        }

        public Action<Mob> OnAddedToActiveMobs
        {
            get
            {
                return this.onAddedToActiveMobs;
            }
            set
            {
                this.onAddedToActiveMobs = value;
            }
        }

        public Action<Mob> OnRemovedFromActiveMobs
        {
            get
            {
                return this.onRemovedFromActiveMobs;
            }
            set
            {
                this.onRemovedFromActiveMobs = value;
            }
        }

        public Action OnOrderChanged
        {
            get
            {
                return this.onOrderChanged;
            }
            set
            {
                this.onOrderChanged = value;
            }
        }

        public Action<bool, Mob, bool, Mob> OnRecentlyActivatedMobChanged
        {
            get
            {
                return this.onRecentlyActiveMobChanged;
            }
            set
            {
                this.onRecentlyActiveMobChanged = value;
            }
        }

        public Action<Mob> OnMobAdded
        {
            get
            {
                return this.onMobAdded;
            }
            set
            {
                this.onMobAdded = value;
            }
        }

        public Action<Mob> OnMobRemoved
        {
            get
            {
                return this.onMobRemoved;
            }
            set
            {
                this.onMobRemoved = value;
            }
        }

        public bool TryGetRecentlyActiveMob(out Mob recentlyActivatedMob)
        {
            if (this.activeMobs.Count > 0)
            {
                recentlyActivatedMob = this.activeMobs.First.Value;
                return true;
            }
            if (this.inactiveMobs.Count > 0)
            {
                recentlyActivatedMob = this.inactiveMobs.First.Value;
                return true;
            }
            recentlyActivatedMob = null;
            return false;
        }

        public bool AddMob(Mob mob)
        {
            if (!this.callbacks.ContainsKey(mob))
            {
                bool firstChanged;
                if (mob.Active)
                {
                    firstChanged = this.activeMobs.Count == 0;
                    this.activeMobs.AddLast(mob);
                    this.onAddedToActiveMobs?.Invoke(mob);
                }
                else
                {
                    firstChanged = this.activeMobs.Count == 0 && this.inactiveMobs.Count == 0;
                    this.inactiveMobs.AddLast(mob);
                }
                this.callbacks.Add(mob, new Tuple<Action, Action>(() => MobActive(mob), () => MobInactive(mob)));
                mob.Active.OnSet += this.callbacks[mob].Item1;
                mob.Active.OnUnset += this.callbacks[mob].Item2;
                this.onOrderChanged?.Invoke();
                if (firstChanged) this.onRecentlyActiveMobChanged?.Invoke(false, null, true, mob);
                this.onMobAdded?.Invoke(mob);
                return true;
            }
            return false;
        }

        public bool RemoveMob(Mob mob)
        {
            if (this.callbacks.ContainsKey(mob))
            {
                bool firstChanged;
                if (mob.Active)
                {
                    firstChanged = this.activeMobs.First.Value == mob;
                    this.activeMobs.Remove(mob);
                    this.onRemovedFromActiveMobs?.Invoke(mob);
                }
                else
                {
                    firstChanged = this.activeMobs.Count == 0 && this.inactiveMobs.First.Value == mob;
                    this.inactiveMobs.Remove(mob);
                }
                mob.Active.OnSet -= this.callbacks[mob].Item1;
                mob.Active.OnUnset -= this.callbacks[mob].Item2;
                this.callbacks.Remove(mob);
                this.onOrderChanged?.Invoke();
                if (firstChanged) this.onRecentlyActiveMobChanged?.Invoke(true, mob, TryGetRecentlyActiveMob(out Mob mostRecentlyActiveMob), mostRecentlyActiveMob);
                this.onMobRemoved?.Invoke(mob);
                return true;
            }
            return false;
        }

        private void MobActive(Mob mob)
        {
            bool wasFirst = this.activeMobs.Count == 0 && this.inactiveMobs.Count > 0 && this.inactiveMobs.First.Value == mob;
            bool firstChanged = this.inactiveMobs.Count > 0 && this.inactiveMobs.First.Value == mob && this.activeMobs.Count > 0;
            this.inactiveMobs.Remove(mob);
            this.activeMobs.AddFirst(mob);

            this.onAddedToActiveMobs?.Invoke(mob);
            if (!wasFirst) this.onOrderChanged?.Invoke();
            if (firstChanged) this.onRecentlyActiveMobChanged?.Invoke(true, this.activeMobs.First.Next.Value, true, mob);
        }

        private void MobInactive(Mob mob)
        {
            bool wasLast = this.activeMobs.Count > 0 && this.activeMobs.Last.Value == mob;
            bool firstChanged = this.activeMobs.Count > 1 && this.activeMobs.First.Value == mob;
            this.activeMobs.Remove(mob);
            this.inactiveMobs.AddFirst(mob);

            this.onRemovedFromActiveMobs?.Invoke(mob);
            if (!wasLast) this.onOrderChanged?.Invoke();
            if (firstChanged) this.onRecentlyActiveMobChanged?.Invoke(true, mob, true, this.activeMobs.First.Value);
        }
    }
}
