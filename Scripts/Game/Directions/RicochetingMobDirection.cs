using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class RicochetingMobDirection : Direction
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private float detectRange;

        private Vector2 ricochetDirection;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = this.ricochetDirection;
            }
            return directions;
        }

        protected override void Awake()
        {
            base.Awake();
            float randAngle = Mathf.PI / 2 * Random.Range(0, 4) + Mathf.PI / 4;
            this.ricochetDirection = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
        }

        protected override void Start()
        {
            base.Start();
            FrigidCoroutine.Run(this.RicochetUpdate(), this.gameObject);
        }

        private IEnumerator<FrigidCoroutine.Delay> RicochetUpdate()
        {
            while (true)
            {
                if (this.mob.TryGetValue(out Mob mob))
                {
                    bool blocked = mob.PushCast(mob.Position, new Vector2(0, this.ricochetDirection.y).normalized, this.detectRange, out _);
                    if (blocked) this.ricochetDirection = this.ricochetDirection * new Vector2(1, -1);
                    blocked = mob.PushCast(mob.Position, new Vector2(this.ricochetDirection.x, 0), this.detectRange, out _);
                    if (blocked) this.ricochetDirection = this.ricochetDirection * new Vector2(-1, 1);
                }
                yield return null;
            }
        }
    }
}
