using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "Expedition", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.EXPEDITIONS + "Expedition")]
    public class Expedition : ScriptableObject
    {
        [SerializeField]
        private TiledLevelPlanner tiledLevelPlanner;
        [SerializeField]
        private TiledLevelPlannerScriptableVariable dungeonTiledLevelVariable;

        public void AssignExpedition()
        {
            this.dungeonTiledLevelVariable.Value = this.tiledLevelPlanner;
        }
    }
}