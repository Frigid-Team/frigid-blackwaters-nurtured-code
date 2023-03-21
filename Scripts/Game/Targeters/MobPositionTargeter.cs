using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobPositionTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedReference mob;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] positions = new Vector2[currentPositions.Length];
            for (int i = 0; i < positions.Length; i++) positions[i] = this.mob.ImmutableValue.Position;
            return positions;
        }
    }
}
