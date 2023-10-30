using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public static class AreaTiling
    {
        public static Vector2 EdgePositionTowardDirection(Vector2 position, Vector2 direction, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return LocalEdgePositionTowardDirection(position - centerPosition, direction, boundsDimensions) + centerPosition;
        }

        public static Vector2 LocalEdgePositionTowardDirection(Vector2 localPosition, Vector2 direction, Vector2Int boundsDimensions)
        {
            Vector2[] cornerPositions = new Vector2[4]
            {
                new Vector2(-boundsDimensions.x / 2f, -boundsDimensions.y / 2f) * FrigidConstants.UnitWorldSize,
                new Vector2(boundsDimensions.x / 2f, -boundsDimensions.y / 2f) * FrigidConstants.UnitWorldSize,
                new Vector2(boundsDimensions.x / 2f, boundsDimensions.y / 2f) * FrigidConstants.UnitWorldSize,
                new Vector2(-boundsDimensions.x / 2f, boundsDimensions.y / 2f) * FrigidConstants.UnitWorldSize
            };

            Vector2 startPosition = localPosition;
            Vector2 endPosition = localPosition + direction * (boundsDimensions.x + boundsDimensions.y) * FrigidConstants.UnitWorldSize;

            for (int i = 0; i < 4; i++)
            {
                if (Geometry.LineToLineIntersection(cornerPositions[i], cornerPositions[(i + 1) % 4], startPosition, endPosition, out Vector2 edgePosition))
                {
                    return edgePosition;
                }
            }
            return localPosition;
        }
        
        public static Vector2 RandomTilePosition(Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return RandomTileLocalPosition(boundsDimensions) + centerPosition;
        }

        public static Vector2 RandomTileLocalPosition(Vector2Int boundsDimensions)
        {
            return new Vector2(
                UnityEngine.Random.Range(-boundsDimensions.x / 2f, boundsDimensions.x / 2f) * FrigidConstants.UnitWorldSize,
                UnityEngine.Random.Range(-boundsDimensions.y / 2f, boundsDimensions.y / 2f) * FrigidConstants.UnitWorldSize
                );
        }

        public static Vector2Int RandomTileIndexPosition(Vector2Int boundsDimensions)
        {
            return new Vector2Int(UnityEngine.Random.Range(0, boundsDimensions.x), UnityEngine.Random.Range(0, boundsDimensions.y));
        }

        public static Vector2 TilePositionFromIndexPosition(Vector2Int indexPosition, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return TileLocalPositionFromIndexPosition(indexPosition, boundsDimensions) + centerPosition;
        }

        public static Vector2 TileLocalPositionFromIndexPosition(Vector2Int indexPosition, Vector2Int boundsDimensions)
        {
            return new Vector2(
                -boundsDimensions.x / 2f * FrigidConstants.UnitWorldSize + FrigidConstants.UnitWorldSize / 2 + indexPosition.x * FrigidConstants.UnitWorldSize,
                boundsDimensions.y / 2f * FrigidConstants.UnitWorldSize - FrigidConstants.UnitWorldSize / 2 - indexPosition.y * FrigidConstants.UnitWorldSize
                );
        }

        public static Vector2Int TileIndexPositionFromPosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return TileIndexPositionFromLocalPosition(position - centerPosition, boundsDimensions);
        }

        public static Vector2Int TileIndexPositionFromLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions)
        {
            int x = Mathf.FloorToInt(localPosition.x / FrigidConstants.UnitWorldSize + boundsDimensions.x / 2f);
            int y = Mathf.FloorToInt(boundsDimensions.y / 2f - localPosition.y / FrigidConstants.UnitWorldSize);
            return new Vector2Int(x, y);
        }

        public static bool TileIndexPositionWithinBounds(Vector2Int indexPosition, Vector2Int boundsDimensions)
        {
            return indexPosition.x >= 0 && indexPosition.x < boundsDimensions.x && indexPosition.y >= 0 && indexPosition.y < boundsDimensions.y;
        }

        public static bool TilePositionWithinBounds(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return TileLocalPositionWithinBounds(position - centerPosition, boundsDimensions);
        }

        public static bool TileLocalPositionWithinBounds(Vector2 localPosition, Vector2Int boundsDimensions)
        {
            return
                localPosition.x > (-boundsDimensions.x - 1) / 2f * FrigidConstants.UnitWorldSize &&
                localPosition.x < (boundsDimensions.x + 1) / 2f * FrigidConstants.UnitWorldSize &&
                localPosition.y > (-boundsDimensions.y - 1) / 2f * FrigidConstants.UnitWorldSize &&
                localPosition.y < (boundsDimensions.y + 1) / 2f * FrigidConstants.UnitWorldSize;
        }

        public static Vector2Int ClampTileIndexPosition(Vector2Int indexPosition, Vector2Int boundsDimensions)
        {
            return new Vector2Int(Mathf.Clamp(indexPosition.x, 0, boundsDimensions.x - 1), Mathf.Clamp(indexPosition.y, 0, boundsDimensions.y - 1));
        }

        public static Vector2 ClampTilePosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return ClampTileLocalPosition(position - centerPosition, boundsDimensions) + centerPosition;
        }

        public static Vector2 ClampTileLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions)
        {
            return new Vector2(
                Mathf.Clamp(localPosition.x, -boundsDimensions.x / 2f * FrigidConstants.UnitWorldSize, boundsDimensions.x / 2f * FrigidConstants.UnitWorldSize),
                Mathf.Clamp(localPosition.y, -boundsDimensions.y / 2f * FrigidConstants.UnitWorldSize, boundsDimensions.y / 2f * FrigidConstants.UnitWorldSize)
                );
        }

        public static bool VisitTileIndexPositionsInTileRect(Vector2Int indexPosition, Vector2Int rectDimensions, Vector2Int boundsDimensions, Action<Vector2Int> onVisited)
        {
            bool allVisited = true;
            for (int x = indexPosition.x; x > indexPosition.x - rectDimensions.x; x--)
            {
                for (int y = indexPosition.y; y > indexPosition.y - rectDimensions.y; y--)
                {
                    Vector2Int tileIndexPosition = new Vector2Int(x, y);
                    if (TileIndexPositionWithinBounds(tileIndexPosition, boundsDimensions))
                    {
                        onVisited?.Invoke(tileIndexPosition);
                    }
                    else
                    {
                        allVisited = false;
                    }
                }
            }
            return allVisited;
        }

        public static bool VisitTileIndexPositionsInLocalRect(Vector2 localPosition, Vector2 size, Vector2Int boundsDimensions, Action<Vector2Int> onVisited)
        {
            Vector2Int topLeft = TileIndexPositionFromLocalPosition(localPosition + new Vector2(-size.x, size.y) / 2, boundsDimensions);
            Vector2Int bottomRight = TileIndexPositionFromLocalPosition(localPosition + new Vector2(size.x, -size.y) / 2, boundsDimensions);
            bool allVisited = true;
            for (int x = topLeft.x; x <= bottomRight.x; x++)
            {
                for (int y = topLeft.y; y <= bottomRight.y; y++)
                {
                    Vector2Int tileIndexPosition = new Vector2Int(x, y);
                    if (TileIndexPositionWithinBounds(tileIndexPosition, boundsDimensions))
                    {
                        onVisited?.Invoke(tileIndexPosition);
                    }
                    else
                    {
                        allVisited = false;
                    }
                }
            }
            return allVisited;
        }

        public static bool VisitTileIndexPositionsInRect(Vector2 position, Vector2 size, Vector2 centerPosition, Vector2Int boundsDimensions, Action<Vector2Int> onVisited)
        {
            return VisitTileIndexPositionsInLocalRect(position - centerPosition, size, boundsDimensions, onVisited);
        }

        public static bool AreRectIndexPositionsOverlapping(Vector2Int firstIndexPosition, Vector2Int firstRectDimensions, Vector2Int secondIndexPosition, Vector2Int secondRectDimensions)
        {
            Vector2Int firstTopLeft = firstIndexPosition - firstRectDimensions + Vector2Int.one;
            Vector2Int firstBottomRight = firstIndexPosition;
            Vector2Int secondTopLeft = secondIndexPosition - secondRectDimensions + Vector2Int.one;
            Vector2Int secondBottomRight = secondIndexPosition;
            return firstTopLeft.x <= secondBottomRight.x && firstBottomRight.x >= secondTopLeft.x && firstTopLeft.y <= secondBottomRight.y && firstBottomRight.y >= secondTopLeft.y;
        }

        public static Vector2 RectPositionFromIndexPosition(Vector2Int indexPosition, Vector2 centerPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return RectLocalPositionFromIndexPosition(indexPosition, boundsDimensions, rectDimensions) + centerPosition;
        }

        public static Vector2 RectLocalPositionFromIndexPosition(Vector2Int indexPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return TileLocalPositionFromIndexPosition(indexPosition, boundsDimensions) + new Vector2(-(rectDimensions.x - 1) * FrigidConstants.UnitWorldSize / 2, (rectDimensions.y - 1) * FrigidConstants.UnitWorldSize / 2);
        }

        public static Vector2Int RectIndexPositionFromPosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return RectIndexPositionFromLocalPosition(position - centerPosition, boundsDimensions, rectDimensions);
        }

        public static Vector2Int RectIndexPositionFromLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return TileIndexPositionFromLocalPosition(
                localPosition + new Vector2((rectDimensions.x - 1) * FrigidConstants.UnitWorldSize / 2, -(rectDimensions.y - 1) * FrigidConstants.UnitWorldSize / 2),
                boundsDimensions
                );
        }

        public static bool RectIndexPositionWithinBounds(Vector2Int indexPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return indexPosition.x >= rectDimensions.x - 1 && indexPosition.x <= boundsDimensions.x - 1 && indexPosition.y >= rectDimensions.y - 1 && indexPosition.y <= boundsDimensions.y - 1;  
        }

        public static bool RectPositionWithinBounds(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return RectLocalPositionWithinBounds(position - centerPosition, boundsDimensions, rectDimensions);
        }

        public static bool RectLocalPositionWithinBounds(Vector2 localPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return TileLocalPositionWithinBounds(localPosition, boundsDimensions - rectDimensions + Vector2Int.one);
        }

        public static Vector2Int ClampRectIndexPosition(Vector2Int indexPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return new Vector2Int(Mathf.Clamp(indexPosition.x, rectDimensions.x - 1, boundsDimensions.x - 1), Mathf.Clamp(indexPosition.y, rectDimensions.y - 1, boundsDimensions.y - 1));
        }

        public static Vector2 ClampRectPosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return ClampRectLocalPosition(position - centerPosition, boundsDimensions, rectDimensions) + centerPosition;
        }

        public static Vector2 ClampRectLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return new Vector2(
                Mathf.Clamp(localPosition.x, (-boundsDimensions.x + rectDimensions.x - 1) / 2f * FrigidConstants.UnitWorldSize, (boundsDimensions.x - rectDimensions.x + 1) / 2f * FrigidConstants.UnitWorldSize),
                Mathf.Clamp(localPosition.y, (-boundsDimensions.y + rectDimensions.y - 1) / 2f * FrigidConstants.UnitWorldSize, (boundsDimensions.y - rectDimensions.y + 1) / 2f * FrigidConstants.UnitWorldSize)
                );
        }
    }
}
