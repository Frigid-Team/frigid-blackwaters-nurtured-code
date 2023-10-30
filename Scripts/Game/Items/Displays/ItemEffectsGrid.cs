using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ItemEffectsGrid : FrigidMonoBehaviour
    {
        [SerializeField]
        private ItemEffectsGridSlot slotPrefab;
        [SerializeField]
        private List<ItemClassification> classificationOrder;

        private HashSet<ItemStorage> storages;
        private RecyclePool<ItemEffectsGridSlot> slotsPool;
        private Dictionary<(ItemStorable, Item), ItemEffectsGridSlot> currentSlots;

        public void ShowStorage(ItemStorage storage)
        {
            if (this.storages.Add(storage))
            {
                storage.OnItemUsed += this.AddSlot;
                storage.OnItemUnused += this.RemoveSlot;
                foreach ((ItemStorable storable, Item item) in storage.ItemUsages)
                {
                    this.AddSlot(storable, item);
                }
            }
        }

        public void ClearStorage(ItemStorage storage)
        {
            if (this.storages.Remove(storage))
            {
                storage.OnItemUsed -= this.AddSlot;
                storage.OnItemUnused -= this.RemoveSlot;
                foreach ((ItemStorable storable, Item item) in storage.ItemUsages)
                {
                    this.RemoveSlot(storable, item);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.storages = new HashSet<ItemStorage>();
            this.slotsPool = new RecyclePool<ItemEffectsGridSlot>(
                () => CreateInstance<ItemEffectsGridSlot>(this.slotPrefab, this.transform, false), 
                (ItemEffectsGridSlot slot) => DestroyInstance(slot)
                );
            this.currentSlots = new Dictionary<(ItemStorable, Item), ItemEffectsGridSlot>();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void AddSlot(ItemStorable storable, Item item)
        {
            ItemEffectsGridSlot slot = this.slotsPool.Retrieve();
            this.currentSlots.Add((storable, item), slot);
            slot.Populate(storable, item);
            this.SortCurrentSlots();
        }

        private void RemoveSlot(ItemStorable storable, Item item)
        {
            this.slotsPool.Return(this.currentSlots[(storable, item)]);
            this.currentSlots.Remove((storable, item));
            this.SortCurrentSlots();
        }

        private void SortCurrentSlots()
        {
            List<(ItemStorable, Item)> itemDisplays = this.currentSlots.Keys.ToList();
            itemDisplays.Sort(((ItemStorable storable, Item item) l, (ItemStorable storable, Item item) r) => this.classificationOrder.IndexOf(l.storable.Classification) - this.classificationOrder.IndexOf(r.storable.Classification));
            for (int i = 0; i < itemDisplays.Count; i++)
            {
                this.currentSlots[itemDisplays[i]].transform.SetSiblingIndex(i);
            }
        }
    }
}
