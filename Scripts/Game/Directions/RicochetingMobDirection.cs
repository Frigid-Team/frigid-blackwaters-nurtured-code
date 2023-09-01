using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class RicochetingMobDirection : Direction
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private float detectRange;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return currDirections;
            }

            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2 ricochetDirection = currDirections[i].normalized;
                if (ricochetDirection == Vector2.zero)
                {
                    float randAngle = Mathf.PI / 2 * Random.Range(0, 4) + Mathf.PI / 4;
                    ricochetDirection = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
                }
                bool blocked = mob.PushCast(mob.Position, new Vector2(0, ricochetDirection.y).normalized, this.detectRange, out _);
                if (blocked) ricochetDirection = ricochetDirection * new Vector2(1, -1);
                blocked = mob.PushCast(mob.Position, new Vector2(ricochetDirection.x, 0), this.detectRange, out _);
                if (blocked) ricochetDirection = ricochetDirection * new Vector2(-1, 1);
                directions[i] = ricochetDirection;
            }
            return directions;
        }
    }
}
