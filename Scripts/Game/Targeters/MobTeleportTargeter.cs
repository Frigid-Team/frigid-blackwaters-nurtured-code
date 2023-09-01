using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobTeleportTargeter : Targeter
    {
        [SerializeField]
        private MobTeleportState mobTeleportState;

        protected override Vector2[] CustomRetrieve(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            if (this.mobTeleportState.TryGetTeleportPosition(out Vector2 teleportPosition)) 
            {
                Vector2[] positions = new Vector2[currentPositions.Length];
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = teleportPosition;
                }
                return positions;
            }
            else
            {
                return currentPositions;
            }
        }
    }
}
