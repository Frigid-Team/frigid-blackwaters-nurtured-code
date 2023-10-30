using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [CreateAssetMenu(fileName = "IntScriptableVariable", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Expeditions + "IntScriptableVariable")]
    public class IntScriptableVariable : ScriptableVariable<int> { }
}