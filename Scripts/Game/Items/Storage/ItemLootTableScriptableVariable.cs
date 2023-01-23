using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "ItemLootTableScriptableVariable", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.ITEMS + "ItemLootTableScriptableVariable")]
    public class ItemLootTableScriptableVariable : ScriptableVariable<ItemLootTable> { }
}
