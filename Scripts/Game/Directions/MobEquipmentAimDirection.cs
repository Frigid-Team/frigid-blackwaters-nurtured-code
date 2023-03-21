using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobEquipmentAimDirection : Direction
    {
        [SerializeField]
        private MobEquipmentPiece mobEquipmentPiece;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = this.mobEquipmentPiece.EquipPoint.AimDirection;
            return directions;
        }
    }
}
