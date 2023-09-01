using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobPositionTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedHandle mob;

        protected override Vector2[] CustomRetrieve(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return currentPositions;
            }
            Vector2[] positions = new Vector2[currentPositions.Length];
            for (int i = 0; i < positions.Length; i++) positions[i] = mob.Position;
            return positions;
        }
    }
}
