using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class TiledAreaAtmosphere : FrigidMonoBehaviour
    {
        public abstract void StartAtmosphere(Vector2Int mainAreaDimensions, Transform contentsTransform);

        public abstract void StopAtmosphere();
    }
}
