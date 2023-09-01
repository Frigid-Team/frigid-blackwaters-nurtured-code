using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TowardMobEquipPointAimDirection : Direction
    {
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private MobEquipPointSerializedHandle mobEquipPoint;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mobEquipPoint.TryGetValue(out MobEquipPoint mobEquipPoint))
            {
                return currDirections;
            }

            Vector2[] originPositions = this.originTargeter.Retrieve(new Vector2[currDirections.Length], elapsedDuration, elapsedDurationDelta);
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = (mobEquipPoint.AimPosition - originPositions[i]).normalized;
            return directions;
        }
    }
}
