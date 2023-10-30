using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "ItemLootTableScriptableVariable", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Items + "ItemLootTableScriptableVariable")]
    public class ItemLootTableScriptableVariable : ScriptableVariable<ItemLootTable> { }
}
