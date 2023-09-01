using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TerrainDecoration : TerrainContent
    {
        [SerializeField]
        private string[] animationNames;

        public override void Preview(Vector2 orientationDirection)
        {
            base.Preview(orientationDirection);
            this.AnimatorBody.Preview(this.animationNames[0], 0, orientationDirection);
        }

        public override void Populate(Vector2 orientationDirection, NavigationGrid navigationGrid, List<Vector2Int> tileIndexPositions)
        {
            base.Populate(orientationDirection, navigationGrid, tileIndexPositions);
            this.AnimatorBody.Play(this.animationNames[Random.Range(0, this.animationNames.Length)]);
        }
    }
}
