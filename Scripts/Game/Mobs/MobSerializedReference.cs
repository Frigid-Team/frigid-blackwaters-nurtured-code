using System;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class MobSerializedReference : SerializedReference<Mob>
    {
        [SerializeField]
        [ShowIfInt("referenceType", 5, true)]
        private Origin origin;
        [SerializeField]
        [ShowIfInt("referenceType", 5, true)]
        [ShowIfInt("origin", 0, true)]
        private Mob mob;
        [SerializeField]
        [ShowIfInt("referenceType", 5, true)]
        [ShowIfInt("origin", 1, true)]
        private MobEquipmentPiece mobEquipmentPiece;
        [SerializeField]
        [ShowIfInt("referenceType", 5, true)]
        [ShowIfInt("origin", 2, true)]
        private Item item;

        public MobSerializedReference() : base() 
        {
            this.origin = Origin.DirectInstance;
            this.mob = null;
            this.mobEquipmentPiece = null;
            this.item = null;
        }

        public MobSerializedReference(MobSerializedReference other) : base(other)
        {
            this.origin = other.origin;
            this.mob = other.mob;
            this.mobEquipmentPiece = other.mobEquipmentPiece;
            this.item = other.item;
        }

        protected override Mob GetInheritedImmutableValue()
        {
            switch (this.origin)
            {
                case Origin.DirectInstance:
                    return this.mob;
                case Origin.Equipper:
                    return this.mobEquipmentPiece.EquipPoint.Equipper;
                case Origin.ItemUser:
                    this.item.Storage.TryGetUsingMob(out Mob usingMob);
                    return usingMob;
            }
            return null;
        }

        protected override Mob GetInheritedMutableValue()
        {
            return GetInheritedImmutableValue();
        }

        protected override bool InheritEquals(SerializedReference<Mob> other)
        {
            MobSerializedReference otherCasted = (MobSerializedReference)other;
            switch (this.origin)
            {
                case Origin.DirectInstance:
                    return this.mob == otherCasted.mob;
                case Origin.Equipper:
                    return this.mobEquipmentPiece == otherCasted.mobEquipmentPiece;
                case Origin.ItemUser:
                    return this.item == otherCasted.item;
            }
            return true;
        }

        protected override int GetHashCodeFromInherited()
        {
            switch (this.origin)
            {
                case Origin.DirectInstance:
                    return this.mob.GetHashCode();
                case Origin.Equipper:
                    return this.mobEquipmentPiece.GetHashCode();
                case Origin.ItemUser:
                    return this.item.GetHashCode();
            }
            return 0;
        }

        public enum Origin
        {
            DirectInstance,
            Equipper,
            ItemUser
        }
    }
}
