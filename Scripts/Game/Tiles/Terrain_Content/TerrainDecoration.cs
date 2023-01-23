using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TerrainDecoration : TerrainContent
    {
        [SerializeField]
        private string[] animationNames;

        public override void Populated(Vector2 orientationDirection, NavigationGrid navigationGrid, List<Vector2Int> allTileIndices)
        {
            base.Populated(orientationDirection, navigationGrid, allTileIndices);

            this.AnimatorBody.PlayByName(this.animationNames[Random.Range(0, this.animationNames.Length)]);
        }
    }
}
