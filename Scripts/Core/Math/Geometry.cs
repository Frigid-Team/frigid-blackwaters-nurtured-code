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

        public static Vector2 GetPolygonCentroid(Vector2[] polygonPoints)
        {
            Vector2 centroid = Vector2.zero;
            foreach (Vector2 polygonPoint in polygonPoints)
            {
                centroid += polygonPoint;
            }
            centroid /= polygonPoints.Length;
            return centroid;
        }

        public static Vector2[] GetRectAlongLine(Vector2 a, Vector2 b, float width)
        {
            float m = (b.y - a.y) / (b.x - a.x);
            float deltaX;
            float deltaY;
            if (float.IsInfinity(m))
            {
                deltaX = (width / 2f);
                deltaY = 0f;
            }
            else
            {
                deltaX = (width / 2f) * (m / Mathf.Sqrt(m * m + 1));
                deltaY = (width / 2f) * (1 / Mathf.Sqrt(1 + m * m));
            }
            return new Vector2[]
            {
                a + new Vector2(-deltaX, deltaY),
                b + new Vector2(-deltaX, deltaY),
                b + new Vector2(deltaX, -deltaY),
                a + new Vector2(deltaX, -deltaY)
            };
        }

        public static bool LineToLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.zero;

            Vector2 b = a2 - a1;
            Vector2 d = b2 - b1;
            float bDotDPerp = b.x * d.y - b.y * d.x;

            if (bDotDPerp == 0)
            {
                return false;
            }

            Vector2 c = b1 - a1;
            float t = (c.x * d.y - c.y * d.x) / bDotDPerp;
            if (t < 0 || t > 1)
            {
                return false;
            }

            float u = (c.x * b.y - c.y * b.x) / bDotDPerp;
            if (u < 0 || u > 1)
            {
                return false;
            }

            intersectionPoint = a1 + t * b;
            return true;
        }
    }
}
