using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TransformTargeter : Targeter
    {
        [SerializeField]
        private Transform targetTransform;

        protected override Vector2[] CustomRetrieve(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] positions = new Vector2[currentPositions.Length];
            for (int i = 0; i < positions.Length; i++) positions[i] = this.targetTransform.position;
            return positions;
        }
    }
}
