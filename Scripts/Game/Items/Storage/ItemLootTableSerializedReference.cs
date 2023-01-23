using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class ItemLootTableSerializedReference : SerializedReference<ItemLootTable>
    {
        public ItemLootTableSerializedReference() : base() { }

        public ItemLootTableSerializedReference(ItemLootTableSerializedReference other) : base(other) { }

        public ItemLootTableSerializedReference(SerializedReferenceType referenceType, ItemLootTable customValue, ScriptableConstant<ItemLootTable> scriptableConstant, List<ItemLootTable> selection, ScriptableVariable<ItemLootTable> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }
    }
}
