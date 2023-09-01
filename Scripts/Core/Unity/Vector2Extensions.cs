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

        public static float ComponentAngle0To360(this Vector2 vector)
        {
            return (vector.ComponentAngleSigned() + 360) % 360;
        }

        public static float ComponentAngle(this Vector2 vector)
        {
            return Mathf.Abs(vector.ComponentAngleSigned());
        }

        public static float ComponentAngleSigned(this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }

        public static int Division(this Vector2 vector, int numberDivisions)
        {
            return Mathf.RoundToInt((Mathf.Atan2(vector.y, vector.x) + (2 * Mathf.PI)) % (2 * Mathf.PI) / (2 * Mathf.PI / numberDivisions)) % numberDivisions;
        }
        

        public static Vector2 RotateAround(this Vector2 vector, float angle)
        {
            return vector.RotateAround(Vector2.zero, angle);
        }

        public static Vector2 RotateAround(this Vector2 vector, Vector2 pivot, float angle)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            float s = Mathf.Sin(angleRad);
            float c = Mathf.Cos(angleRad);

            Vector2 tempVector = new Vector2(vector.x, vector.y);

            tempVector.x -= pivot.x;
            tempVector.y -= pivot.y;

            float xNew = tempVector.x * c - tempVector.y * s;
            float yNew = tempVector.x * s + tempVector.y * c;

            tempVector.x = xNew + pivot.x;
            tempVector.y = yNew + pivot.y;

            return tempVector;
        } 
    }
}

