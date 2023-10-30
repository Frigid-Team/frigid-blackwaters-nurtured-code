using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class LeadMobTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedHandle mobToLead;
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private FloatSerializedReference predictionSpeed;
        [SerializeField]
        private FloatSerializedReference coneAngle;

        protected override Vector2[] CustomRetrieve(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mobToLead.TryGetValue(out Mob mobToLead))
            {
                return currentPositions;
            }

            Vector2[] origins = this.originTargeter.Retrieve(currentPositions, elapsedDuration, elapsedDurationDelta);
            Vector2[] positions = new Vector2[currentPositions.Length];
            currentPositions.CopyTo(positions, 0);

            for (int i = 0; i < positions.Length; i++)
            {
                Vector2 targetVelocity = mobToLead.CurrentVelocity;
                Vector2 targetPosition = mobToLead.Position;
                Vector2 originPosition = origins[i];
                Vector2 incidentDelta = targetPosition - originPosition;
                if (targetVelocity.magnitude <= FrigidConstants.WorldSizeEpsilon)
                {
                    positions[i] = targetPosition;
                    continue;
                }
                float predictionSpeed = this.predictionSpeed.ImmutableValue;

                float a = (targetVelocity.x * targetVelocity.x) + (targetVelocity.y * targetVelocity.y) - (predictionSpeed * predictionSpeed);
                float b = 2 * (targetVelocity.x * (targetPosition.x - originPosition.x) + targetVelocity.y * (targetPosition.y - originPosition.y));
                float c = Mathf.Pow(targetPosition.x - originPosition.x, 2) + Mathf.Pow(targetPosition.y - originPosition.y, 2);

                float disc = b * b - (4 * a * c);
                Vector2 leadPosition;
                if (disc > 0)
                {
                    float t = Mathf.Max((-1 * b + Mathf.Sqrt(disc)) / (2 * a), (-1 * b - Mathf.Sqrt(disc)) / (2 * a));
                    leadPosition = targetPosition + targetVelocity * t;
                }
                else
                {
                    leadPosition = targetPosition + targetVelocity;
                }
                Vector2 leadDelta = leadPosition - originPosition;

                float coneHalfAngle = this.coneAngle.ImmutableValue / 2;
                float halfAngleInterval = FrigidConstants.UnitWorldSize / incidentDelta.magnitude * Mathf.Rad2Deg;
                do
                {
                    float clampedHalfAngleRad = Mathf.Clamp(Vector2.SignedAngle(incidentDelta, leadDelta), -coneHalfAngle, coneHalfAngle) * Mathf.Deg2Rad;
                    float clampedIncidentAngleRad = incidentDelta.ComponentAngleSigned() * Mathf.Deg2Rad + clampedHalfAngleRad;
                    float predictAngleRad = Vector2.SignedAngle(targetVelocity, new Vector2(Mathf.Cos(clampedIncidentAngleRad), Mathf.Sin(clampedIncidentAngleRad))) * Mathf.Deg2Rad;
                    float predictDistance = incidentDelta.magnitude / Mathf.Sin(Mathf.Abs(predictAngleRad)) * Mathf.Sin(Mathf.Abs(clampedHalfAngleRad));
                    if (float.IsNaN(predictDistance)) predictDistance = 0;
                    positions[i] = targetPosition + targetVelocity.normalized * predictDistance;
                    coneHalfAngle = Mathf.Max(0, coneHalfAngle - halfAngleInterval);
                }
                while ((!AreaTiling.TilePositionWithinBounds(positions[i], mobToLead.TiledArea.CenterPosition, mobToLead.TiledArea.MainAreaDimensions) || 
                        !mobToLead.TiledArea.NavigationGrid.IsTraversable(AreaTiling.RectIndexPositionFromPosition(positions[i], mobToLead.TiledArea.CenterPosition, mobToLead.TiledArea.MainAreaDimensions, mobToLead.TileSize), mobToLead.TileSize, mobToLead.TraversableTerrain, Resistance.None)) &&
                        coneHalfAngle > 0);
            }
            return positions;
        }
    }
}
