using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TiledLevelPlannerScriptableVariable", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TiledLevelPlannerScriptableVariable")]
    public class TiledLevelPlannerScriptableVariable : ScriptableVariable<TiledLevelPlanner> { }
}
