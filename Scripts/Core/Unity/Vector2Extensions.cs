using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class Vector2Extensions
    {
        public static bool WithinBox(this Vector2 position, Vector2 boxPosition, Vector2 dimensions)
        {
            return 
                position.x >= (boxPosition.x - dimensions.x / 2) && 
                position.x <= (boxPosition.x + dimensions.x / 2) && 
                position.y >= (boxPosition.y - dimensions.y / 2) && 
                position.y <= (boxPosition.y + dimensions.y / 2);
        }

        public static float ComponentAngle0To2PI(this Vector2 vector)
        {
            return (Mathf.Atan2(vector.y, vector.x) + Mathf.PI * 2) % (Mathf.PI * 2);
        }

        public static float ComponentAngle(this Vector2 vector)
        {
            return Mathf.Abs(vector.ComponentAngleSigned());
        }

        public static float ComponentAngleSigned(this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x);
        }

        public static int Division(this Vector2 vector, int numberDivisions)
        {
            return Mathf.RoundToInt((Mathf.Atan2(vector.y, vector.x) + (2 * Mathf.PI)) % (2 * Mathf.PI) / (2 * Mathf.PI / numberDivisions)) % numberDivisions;
        }

        public static Vector2 RotateAround(this Vector2 vector, Vector2 pivot, float angleRad)
        {
            float s = Mathf.Sin(angleRad);
            float c = Mathf.Cos(angleRad);

            vector.x -= pivot.x;
            vector.y -= pivot.y;

            float xNew = vector.x * c - vector.y * s;
            float yNew = vector.x * s + vector.y * c;

            vector.x = xNew + pivot.x;
            vector.y = yNew + pivot.y;

            return vector;
        } 
    }
}

