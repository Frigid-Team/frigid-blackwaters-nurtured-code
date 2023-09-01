using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [CreateAssetMenu(fileName = "IntScriptableVariable", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.EXPEDITIONS + "IntScriptableVariable")]
    public class IntScriptableVariable : ScriptableVariable<int> { }
}