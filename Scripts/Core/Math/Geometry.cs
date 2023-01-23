using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class Geometry
    {
        public static Vector2 FindNearestPointOnCircle(Vector2 center, float radius, Vector2 point)
        {
            float angleRad = Mathf.Atan2(point.y - center.y, point.x - center.x);
            return center + new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius;
        }

        public static Vector2 FindNearestPointOnLine(Vector2 a, Vector2 b, Vector2 point)
        {
            Vector2 heading = (b - a);
            float magnitudeMax = heading.magnitude;
            heading.Normalize();
            Vector2 lhs = point - a;
            float dotP = Vector2.Dot(lhs, heading);
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
            return a + heading * dotP;
        }

        public static List<Vector2Int> GetAllSquaresAlongLine(Vector2Int origin, Vector2Int destination)
        {
            List<Vector2Int> allSquares = new List<Vector2Int>();

            int x = origin.x;
            int y = origin.y;

            int dx = Mathf.Abs(destination.x - origin.x);
            int dy = Mathf.Abs(destination.y - origin.y);
            int ddx = 2 * dx;
            int ddy = 2 * dy;

            int sx = destination.x > origin.x ? 1 : -1;
            int sy = destination.y > origin.y ? 1 : -1;

            allSquares.Add(origin);
            if (ddx >= ddy)
            {
                int errorPrev = dx;
                int error = dx;
                for (int i = 0; i < dx; i++)
                {
                    x += sx;
                    error += ddy;
                    if (error > ddx)
                    {
                        y += sy;
                        error -= ddx;
                        if (error + errorPrev > ddx)
                        {
                            allSquares.Add(new Vector2Int(x, y - sy));
                        }
                        else if (error + errorPrev > ddx)
                        {
                            allSquares.Add(new Vector2Int(x - sx, y));
                        }
                        else
                        {
                            allSquares.Add(new Vector2Int(x, y - sy));
                            allSquares.Add(new Vector2Int(x - sx, y));
                        }
                    }
                    allSquares.Add(new Vector2Int(x, y));
                    errorPrev = error;
                }
            }
            else
            {
                int errorPrev = dy;
                int error = dy;
                for (int i = 0; i < dy; i++)
                {
                    y += sy;
                    error += ddx;
                    if (error > ddy)
                    {
                        x += sx;
                        error -= ddy;

                        if (error + errorPrev > ddy)
                        {
                            allSquares.Add(new Vector2Int(x - sx, y));
                        }
                        else if (error + errorPrev > ddy)
                        {
                            allSquares.Add(new Vector2Int(x, y - sy));
                        }
                        else
                        {
                            allSquares.Add(new Vector2Int(x - sx, y));
                            allSquares.Add(new Vector2Int(x, y - sy));
                        }
                    }
                    allSquares.Add(new Vector2Int(x, y));
                    errorPrev = error;
                }
            }
            return allSquares;
        } 

        public static List<Vector2Int> GetAllSquaresOverlappingCircle(Vector2Int origin, float radius)
        {
            List<Vector2Int> overlapping = new List<Vector2Int>();
            int ceilRadius = Mathf.CeilToInt(radius);
            for (int x = 0; x < ceilRadius; x++)
            {
                for (int y = 0; y < ceilRadius; y++)
                {
                    Vector2Int relative = new Vector2Int(x, y);
                    if (Vector2.Distance(Vector2Int.zero, relative) <= radius)
                    {
                        overlapping.Add(relative + origin);
                    }
                }
            }
            return overlapping;
        }
    }
}
