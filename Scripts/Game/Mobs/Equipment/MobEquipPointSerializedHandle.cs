using System;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class MobEquipPointSerializedHandle : SerializedHandle<MobEquipPoint>
    {
        [SerializeField]
        private Origin origin;
        [SerializeField]
        [ShowIfInt("origin", 0, true)]
        private MobEquipPoint mobEquipPoint;
        [SerializeField]
        [ShowIfInt("origin", 1, true)]
        private MobEquipment mobEquipment;

        public override bool TryGetValue(out MobEquipPoint value)
        {
            switch (this.origin)
            {
                case Origin.Direct:
                    value = this.mobEquipPoint;
                    return true;
                case Origin.Equipment:
                    value = this.mobEquipment.EquipPoint;
                    return true;
            }
            value = null;
            return false;
        }

        private enum Origin
        {
            Direct,
            Equipment
        }
    }
}
