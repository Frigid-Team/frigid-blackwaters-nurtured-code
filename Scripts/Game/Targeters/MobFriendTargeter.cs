using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobFriendTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedReference mob;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] positions = new Vector2[currentPositions.Length];
            currentPositions.CopyTo(positions, 0);
            if (this.mob.ImmutableValue.TryGetFriend(out Mob friend))
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = friend.Position;
                }
            }
            return positions;
        }
    }
}
