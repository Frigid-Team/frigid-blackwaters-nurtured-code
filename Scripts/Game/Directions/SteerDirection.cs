using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class SteerDirection : Direction
    {
        [SerializeField]
        private Direction startDirection;
        [SerializeField]
        private Direction targetDirection;
        [SerializeField]
        private FloatSerializedReference steerDuration;
        [SerializeField]
        private float steerSpeedDegrees;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] startDirections = this.startDirection.Calculate(currDirections, elapsedDuration, elapsedDurationDelta);
            Vector2[] targetDirections = this.targetDirection.Calculate(currDirections, elapsedDuration, elapsedDurationDelta);
            float angleStep = this.steerSpeedDegrees * Mathf.Deg2Rad * elapsedDurationDelta;
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = currDirections[i];
                if (elapsedDuration < this.steerDuration.ImmutableValue)
                {
                    Vector2 steeredDirection = currDirections[i];
                    if (steeredDirection == Vector2.zero)
                    {
                        steeredDirection = startDirections[i];
                    }
                    float steeredAngleRad = steeredDirection.ComponentAngle0To360();
                    steeredAngleRad = Vector2.SignedAngle(steeredDirection, targetDirections[i]) > 0 ? (steeredAngleRad + angleStep) : (steeredAngleRad - angleStep);
                    directions[i] = new Vector2(Mathf.Cos(steeredAngleRad), Mathf.Sin(steeredAngleRad));
                }
            }
            return directions;
        }
    }
}
