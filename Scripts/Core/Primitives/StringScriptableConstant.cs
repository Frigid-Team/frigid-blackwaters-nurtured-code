using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [CreateAssetMenu(fileName = "StringScriptableConstant", menuName = FrigidPaths.CreateAssetMenu.Core + FrigidPaths.CreateAssetMenu.Primitives + "StringScriptableConstant")]
    public class StringScriptableConstant : ScriptableConstant<string> { }
}
