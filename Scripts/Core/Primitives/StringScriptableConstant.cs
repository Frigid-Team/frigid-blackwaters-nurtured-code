using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [CreateAssetMenu(fileName = "StringScriptableConstant", menuName = FrigidPaths.CreateAssetMenu.CORE + FrigidPaths.CreateAssetMenu.PRIMITIVES + "StringScriptableConstant")]
    public class StringScriptableConstant : ScriptableConstant<string> { }
}
