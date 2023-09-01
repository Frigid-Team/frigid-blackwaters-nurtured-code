using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobEquipBehaviour : MobBehaviour
    {
        [SerializeField]
        private MobEquipContext equipContext;
        [SerializeField]
        private MobEquipmentSpawnable equipmentSpawnable;

        private MobEquipment equipment;

        public override void Added()
        {
            base.Added();
            if (this.Owner.TryGetEquipPointInContext(this.equipContext, out MobEquipPoint equipPoint))
            {
                this.equipment.transform.SetParent(null);
                equipPoint.AddEquipment(this.equipment);
            }
        }

        public override void Removed()
        {
            base.Removed();
            if (this.Owner.TryGetEquipPointInContext(this.equipContext, out MobEquipPoint equipPoint))
            {
                equipPoint.RemoveEquipment(this.equipment);
                this.equipment.transform.SetParent(this.transform);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.equipment = this.equipmentSpawnable.Spawn();
            this.equipment.transform.SetParent(this.transform);
        }
    }
}
