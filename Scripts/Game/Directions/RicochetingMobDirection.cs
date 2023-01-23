using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class RicochetingMobDirection : Direction
    {
        [SerializeField]
        private Mob mob;
        [SerializeField]
        private float detectRange;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2 ricochetDirection = currDirections[i];
                if (ricochetDirection == Vector2.zero)
                {
                    float randAngle = Mathf.PI / 2 * Random.Range(0, 4) + Mathf.PI / 4;
                    ricochetDirection = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
                }
                List<Collider2D> collisions = this.mob.Physicality.LinePushCast(this.mob.AbsolutePosition, new Vector2(0, ricochetDirection.y).normalized, this.detectRange);
                if (collisions.Count > 0) directions[i] = ricochetDirection * new Vector2(1, -1);
                collisions = this.mob.Physicality.LinePushCast(this.mob.AbsolutePosition, new Vector2(ricochetDirection.x, 0), this.detectRange);
                if (collisions.Count > 0) directions[i] = ricochetDirection * new Vector2(-1, 1);
            }
            return directions;
        }
    }
}
