using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class TerrainObstacle : TerrainObstruction
    {
        [SerializeField]
        private ObstacleAnimationSet[] obstacleAnimations;

        private ObstacleAnimationSet chosenAnimations;

        public override void Populated(Vector2 orientationDirection, NavigationGrid navigationGrid, List<Vector2Int> allTileIndices)
        {
            base.Populated(orientationDirection, navigationGrid, allTileIndices);
            this.chosenAnimations = this.obstacleAnimations[UnityEngine.Random.Range(0, this.obstacleAnimations.Length)];
            this.AnimatorBody.PlayByName(this.chosenAnimations.DefaultAnimationName);
        }

        protected override void Break()
        {
            base.Break();
            this.AnimatorBody.PlayByName(this.chosenAnimations.BrokenAnimationName);
        }

        [Serializable]
        private struct ObstacleAnimationSet
        {
            [SerializeField]
            private string defaultAnimationName;
            [SerializeField]
            [ShowIfMethod("CanBreak", true, true)]
            private string brokenAnimationName;

            public string DefaultAnimationName
            {
                get
                {
                    return this.defaultAnimationName;
                }
            }

            public string BrokenAnimationName
            {
                get
                {
                    return this.brokenAnimationName;
                }
            }
        }
    }
}
