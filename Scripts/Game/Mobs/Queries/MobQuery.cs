using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public abstract class MobQuery : FrigidMonoBehaviour
    {
        [SerializeField]
        private bool reverseOrder;

        public List<Mob> Execute()
        {
            List<Mob> customRetrievedMobs = this.CustomExecute();
            if (this.reverseOrder)
            {
                customRetrievedMobs.Reverse();
            }
            return customRetrievedMobs;
        }

        protected abstract List<Mob> CustomExecute();
    }
}
