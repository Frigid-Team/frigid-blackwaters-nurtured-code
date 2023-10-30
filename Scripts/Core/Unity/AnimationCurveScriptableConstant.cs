using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [CreateAssetMenu(fileName = "AnimationCurveScriptableConstant", menuName = FrigidPaths.CreateAssetMenu.Core + FrigidPaths.CreateAssetMenu.Unity + "AnimationCurveScriptableConstant")]
    public class AnimationCurveScriptableConstant : ScriptableConstant<AnimationCurve> { }
}
