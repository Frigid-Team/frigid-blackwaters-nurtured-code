using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class MobSerializedHandle : SerializedHandle<Mob>
    {
        [SerializeField]
        private Origin origin;
        [SerializeField]
        [ShowIfInt("origin", 0, true)]
        private Mob mob;
        [SerializeField]
        [ShowIfInt("origin", 1, true)]
        private MobEquipment mobEquipment;
        [SerializeField]
        [ShowIfInt("origin", 2, true)]
        private Item item;
        [SerializeField]
        [ShowIfInt("origin", 3, true)]
        private MobQuery mobQuery;

        public override bool TryGetValue(out Mob value)
        {
            switch (this.origin)
            {
                case Origin.Direct:
                    value = this.mob;
                    return true;
                case Origin.Equipper:
                    value = this.mobEquipment.EquipPoint.Equipper;
                    return true;
                case Origin.ItemUser:
                    return this.item.Storage.TryGetUsingMob(out value);
                case Origin.FirstInQuery:
                    List<Mob> foundMobs = this.mobQuery.Execute();
                    if (foundMobs.Count == 0)
                    {
                        value = null;
                        return false;
                    }
                    value = foundMobs[0];
                    return true;
            }
            value = null;
            return false;
        }

        private enum Origin
        {
            Direct,
            Equipper,
            ItemUser,
            FirstInQuery
        }
    }
}
