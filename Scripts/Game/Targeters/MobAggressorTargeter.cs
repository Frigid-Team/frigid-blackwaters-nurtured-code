using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobAggressorTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedReference mob;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] positions = new Vector2[currentPositions.Length];
            currentPositions.CopyTo(positions, 0);
            if (this.mob.ImmutableValue.TryGetAggressor(out Mob aggressor))
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = aggressor.Position;
                }
            }
            return positions;
        }
    }
}
