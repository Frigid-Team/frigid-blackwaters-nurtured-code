using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class SnapDirection : Direction
    {
        [SerializeField]
        private Direction directionToSnap;
        [SerializeField]
        private List<Vector2> constantDirections;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            if (this.constantDirections.Count == 0)
            {
                return currDirections;
            }
            Vector2[] directions = this.directionToSnap.Retrieve(currDirections, elapsedDuration, elapsedDurationDelta);
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2 directionToSnap = directions[i];
                int closestConstantIndex = 0;
                for (int constantIndex = 1; constantIndex < this.constantDirections.Count; constantIndex++)
                {
                    Vector2 closestConstantDirection = this.constantDirections[closestConstantIndex];
                    Vector2 otherConstantDirection = this.constantDirections[constantIndex];
                    if (Vector2.Angle(directionToSnap, closestConstantDirection) > Vector2.Angle(directionToSnap, otherConstantDirection))
                    {
                        closestConstantIndex = constantIndex;
                    }
                }
                directions[i] = this.constantDirections[closestConstantIndex].normalized;
            }
            return directions;
        }
    }
}
