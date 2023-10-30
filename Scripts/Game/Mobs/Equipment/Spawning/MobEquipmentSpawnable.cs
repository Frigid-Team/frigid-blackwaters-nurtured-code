using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "MobEquipmentSpawnable", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Mobs + "MobEquipmentSpawnable")]
    public class MobEquipmentSpawnable : FrigidScriptableObject
    {
        // Much like MobSpawnables, this class will eventually be filled out with information that help determine what kind of equipment to spawn.
        // For now, it's empty. But it will also likely include information regarding equipment upgrades, tier, etc.

        [SerializeField]
        private MobEquipment equipmentPrefab;

        public MobEquipment Spawn()
        {
            MobEquipment equipment = FrigidMonoBehaviour.CreateInstance<MobEquipment>(this.equipmentPrefab);
            equipment.Spawn();
            return equipment;
        }
    }
}
