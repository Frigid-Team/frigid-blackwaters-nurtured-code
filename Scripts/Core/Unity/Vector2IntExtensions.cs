using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class Vector2IntExtensions
    {
        public static bool WithinBox(this Vector2Int position, Vector2 boxPosition, Vector2 dimensions)
        {
            return
                position.x >= (boxPosition.x - dimensions.x / 2) &&
                position.x <= (boxPosition.x + dimensions.x / 2) &&
                position.y >= (boxPosition.y - dimensions.y / 2) &&
                position.y <= (boxPosition.y + dimensions.y / 2);
        }

        public static float CartesianAngle(this Vector2Int vector)
        {
            return (Mathf.Atan2(vector.y, vector.x) + Mathf.PI * 2) % (Mathf.PI * 2);
        }
    }
}
