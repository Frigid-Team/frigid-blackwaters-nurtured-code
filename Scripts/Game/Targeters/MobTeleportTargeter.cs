using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobTeleportTargeter : Targeter
    {
        [SerializeField]
        private MobTeleportState mobTeleportState;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] positions = new Vector2[currentPositions.Length];
            if (this.mobTeleportState.TryGetTeleportPosition(out Vector2 teleportPosition)) 
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = teleportPosition;
                }
            }
            else
            {
                currentPositions.CopyTo(positions, 0);
            }
            return positions;
        }
    }
}
