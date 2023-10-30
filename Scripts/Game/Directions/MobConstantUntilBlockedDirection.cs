using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobConstantUntilBlockedDirection : Direction
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private FloatSerializedReference detectionDistance;
        [SerializeField]
        private List<Vector2> constantDirections;

        private Vector2 chosenDirection;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = this.chosenDirection;
            }
            return directions;
        }

        protected override void Awake()
        {
            base.Awake();
            if (this.constantDirections.Count > 0)
            {
                this.chosenDirection = this.constantDirections[Random.Range(0, this.constantDirections.Count)].normalized;
            }
        }

        protected override void Start()
        {
            base.Start();
            FrigidCoroutine.Run(this.UpdateChosenDirection(), this.gameObject);
        }

        private IEnumerator<FrigidCoroutine.Delay> UpdateChosenDirection()
        {
            while (true)
            {
                if (this.mob.TryGetValue(out Mob mob))
                {
                    if (this.chosenDirection == Vector2.zero || mob.PushCast(mob.Position, this.chosenDirection, this.detectionDistance.ImmutableValue, out _))
                    {
                        List<Vector2> unblockedDirections = new List<Vector2>();
                        foreach (Vector2 constantDirection in this.constantDirections)
                        {
                            constantDirection.Normalize();
                            if (!mob.PushCast(mob.Position, constantDirection, this.detectionDistance.ImmutableValue, out _))
                            {
                                unblockedDirections.Add(constantDirection);
                            }
                        }
                        this.chosenDirection = Vector2.zero;
                        if (unblockedDirections.Count == 0)
                        {
                            if (this.constantDirections.Count != 0)
                            {
                                this.chosenDirection = this.constantDirections[Random.Range(0, this.constantDirections.Count)];
                            }
                        }
                        else
                        {
                            this.chosenDirection = unblockedDirections[Random.Range(0, unblockedDirections.Count)];
                        }
                    }
                }
                yield return null;
            }
        }
    }
}
