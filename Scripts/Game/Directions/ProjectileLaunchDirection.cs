using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ProjectileLaunchDirection : Direction
    {
        [SerializeField]
        private Projectile projectile;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = this.projectile.LaunchDirection;
            return directions;
        }
    }
}
