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
            return (Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg + 360) % (360);
        }

        public static Vector2Int RotateAround(this Vector2Int vector, float angle)
        {
            return vector.RotateAround(Vector2Int.zero, angle);
        }

        public static Vector2Int RotateAround(this Vector2Int vector, Vector2Int pivot, float angle)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            float s = Mathf.Sin(angleRad);
            float c = Mathf.Cos(angleRad);

            Vector2Int tempVector = new Vector2Int(vector.x, vector.y);
            tempVector.x -= pivot.x;
            tempVector.y -= pivot.y;

            float xNew = tempVector.x * c - tempVector.y * s;
            float yNew = tempVector.x * s + tempVector.y * c;

            tempVector.x = Mathf.RoundToInt(xNew + pivot.x);
            tempVector.y = Mathf.RoundToInt(yNew + pivot.y);

            return tempVector;
        }
    }
}
