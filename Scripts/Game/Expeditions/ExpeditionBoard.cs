using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ExpeditionBoard : SceneAccessible<ExpeditionBoard> 
    {
        [SerializeField]
        private ExpeditionProgress expeditionProgress;

        public ExpeditionProgress ExpeditionProgress
        {
            get
            {
                return this.expeditionProgress;
            }
        }
    }
}