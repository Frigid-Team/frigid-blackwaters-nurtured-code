using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public static class TilePositioning
    {
        public static Vector2 RandomTilePosition(Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return RandomTileLocalPosition(boundsDimensions) + centerPosition;
        }

        public static Vector2 RandomTileLocalPosition(Vector2Int boundsDimensions)
        {
            return new Vector2(
                UnityEngine.Random.Range(-boundsDimensions.x / 2f, boundsDimensions.x / 2f) * GameConstants.UNIT_WORLD_SIZE,
                UnityEngine.Random.Range(-boundsDimensions.y / 2f, boundsDimensions.y / 2f) * GameConstants.UNIT_WORLD_SIZE
                );
        }

        public static Vector2Int RandomTileIndices(Vector2Int boundsDimensions)
        {
            return new Vector2Int(UnityEngine.Random.Range(0, boundsDimensions.x), UnityEngine.Random.Range(0, boundsDimensions.y));
        }

        public static Vector2 TilePositionFromIndices(Vector2Int indices, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return TileLocalPositionFromIndices(indices, boundsDimensions) + centerPosition;
        }

        public static Vector2 TileLocalPositionFromIndices(Vector2Int indices, Vector2Int boundsDimensions)
        {
            return new Vector2(
                -boundsDimensions.x / 2f * GameConstants.UNIT_WORLD_SIZE + GameConstants.UNIT_WORLD_SIZE / 2 + indices.x * GameConstants.UNIT_WORLD_SIZE,
                boundsDimensions.y / 2f * GameConstants.UNIT_WORLD_SIZE - GameConstants.UNIT_WORLD_SIZE / 2 - indices.y * GameConstants.UNIT_WORLD_SIZE
                );
        }

        public static Vector2Int TileIndicesFromPosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return TileIndicesFromLocalPosition(position - centerPosition, boundsDimensions);
        }

        public static Vector2Int TileIndicesFromLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions)
        {
            int x = Mathf.FloorToInt(localPosition.x / GameConstants.UNIT_WORLD_SIZE + boundsDimensions.x / 2f);
            int y = Mathf.FloorToInt(boundsDimensions.y / 2f - localPosition.y / GameConstants.UNIT_WORLD_SIZE);
            return new Vector2Int(x, y);
        }

        public static bool TileIndicesWithinBounds(Vector2Int indices, Vector2Int boundsDimensions)
        {
            return indices.x >= 0 && indices.x < boundsDimensions.x && indices.y >= 0 && indices.y < boundsDimensions.y;
        }

        public static bool TilePositionWithinBounds(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions, float tolerance = 0)
        {
            return TileLocalPositionWithinBounds(position - centerPosition, boundsDimensions, tolerance);
        }

        public static bool TileLocalPositionWithinBounds(Vector2 localPosition, Vector2Int boundsDimensions, float tolerance = 0)
        {
            return
                localPosition.x > -(boundsDimensions.x / 2f) * GameConstants.UNIT_WORLD_SIZE + tolerance &&
                localPosition.x < (boundsDimensions.x / 2f) * GameConstants.UNIT_WORLD_SIZE - tolerance &&
                localPosition.y > -(boundsDimensions.y / 2f) * GameConstants.UNIT_WORLD_SIZE + tolerance &&
                localPosition.y < (boundsDimensions.y / 2f) * GameConstants.UNIT_WORLD_SIZE - tolerance;
        }

        public static Vector2 ClampTileLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions)
        {
            return new Vector2(
                Mathf.Clamp(localPosition.x, -boundsDimensions.x / 2f * GameConstants.UNIT_WORLD_SIZE, boundsDimensions.x / 2f * GameConstants.UNIT_WORLD_SIZE),
                Mathf.Clamp(localPosition.y, -boundsDimensions.y / 2f * GameConstants.UNIT_WORLD_SIZE, boundsDimensions.y / 2f * GameConstants.UNIT_WORLD_SIZE)
                );
        }

        public static Vector2 ClampTilePosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return ClampTileLocalPosition(position - centerPosition, boundsDimensions) + centerPosition;
        }

        public static Vector2Int ClampTileIndices(Vector2Int indices, Vector2Int boundsDimensions)
        {
            return new Vector2Int(Mathf.Clamp(indices.x, 0, boundsDimensions.x - 1), Mathf.Clamp(indices.y, 0, boundsDimensions.y - 1));
        }

        public static bool VisitTileIndicesInLocalRect(Vector2 localPosition, Vector2 size, Vector2Int boundsDimensions, Action<Vector2Int> onVisited)
        {
            Vector2Int topLeft = TileIndicesFromLocalPosition(localPosition + new Vector2(-size.x, size.y) / 2, boundsDimensions);
            Vector2Int bottomRight = TileIndicesFromLocalPosition(localPosition + new Vector2(size.x, -size.y) / 2, boundsDimensions);

            for (int x = topLeft.x; x <= bottomRight.x; x++)
            {
                for (int y = topLeft.y; y <= bottomRight.y; y++)
                {
                    Vector2Int tileIndices = new Vector2Int(x, y);
                    if (TileIndicesWithinBounds(tileIndices, boundsDimensions))
                    {
                        onVisited?.Invoke(tileIndices);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool VisitTileIndicesInRect(Vector2 position, Vector2 size, Vector2 centerPosition, Vector2Int boundsDimensions, Action<Vector2Int> onVisited)
        {
            return VisitTileIndicesInLocalRect(position - centerPosition, size, boundsDimensions, onVisited);
        }

        public static bool VisitTileIndicesInTileRect(Vector2Int indices, Vector2Int rectDimensions, Vector2Int boundsDimensions, Action<Vector2Int> onVisited)
        {
            for (int x = indices.x; x > indices.x - rectDimensions.x; x--)
            {
                for (int y = indices.y; y > indices.y - rectDimensions.y; y--)
                {
                    Vector2Int tileIndices = new Vector2Int(x, y);
                    if (TileIndicesWithinBounds(tileIndices, boundsDimensions))
                    {
                        onVisited?.Invoke(tileIndices);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static Vector2 RectLocalPositionFromIndices(Vector2Int indices, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return TileLocalPositionFromIndices(indices, boundsDimensions) + new Vector2(-(rectDimensions.x - 1) * GameConstants.UNIT_WORLD_SIZE / 2, (rectDimensions.y - 1) * GameConstants.UNIT_WORLD_SIZE / 2);
        }

        public static Vector2 RectPositionFromIndices(Vector2Int indices, Vector2 centerPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return RectLocalPositionFromIndices(indices, boundsDimensions, rectDimensions) + centerPosition;
        }

        public static Vector2Int RectIndicesFromLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return TileIndicesFromLocalPosition(
                localPosition + new Vector2((rectDimensions.x - 1) * GameConstants.UNIT_WORLD_SIZE / 2, -(rectDimensions.y - 1) * GameConstants.UNIT_WORLD_SIZE / 2),
                boundsDimensions
                );
        }

        public static Vector2Int RectIndicesFromPosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return RectIndicesFromLocalPosition(position - centerPosition, boundsDimensions, rectDimensions);
        }

        public static bool RectIndicesWithinBounds(Vector2Int indices, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return indices.x >= rectDimensions.x - 1 && indices.x <= boundsDimensions.x - 1 && indices.y >= rectDimensions.y - 1 && indices.y <= boundsDimensions.y - 1;  
        }

        public static bool RectLocalPositionWithinBounds(Vector2 localPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions, float tolerance = 0)
        {
            return TileLocalPositionWithinBounds(localPosition, boundsDimensions - rectDimensions + Vector2Int.one, tolerance);
        }

        public static bool RectPositionWithinBounds(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions, float tolerance = 0)
        {
            return RectLocalPositionWithinBounds(position - centerPosition, boundsDimensions, rectDimensions, tolerance);
        }

        public static Vector2 ClampRectLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return new Vector2(
                Mathf.Clamp(localPosition.x, (-boundsDimensions.x + rectDimensions.x - 1) / 2f * GameConstants.UNIT_WORLD_SIZE, (boundsDimensions.x - rectDimensions.x + 1) / 2f * GameConstants.UNIT_WORLD_SIZE),
                Mathf.Clamp(localPosition.y, (-boundsDimensions.y + rectDimensions.y - 1) / 2f * GameConstants.UNIT_WORLD_SIZE, (boundsDimensions.y - rectDimensions.y + 1) / 2f * GameConstants.UNIT_WORLD_SIZE)
                );
        }

        public static Vector2 ClampRectPosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return ClampRectLocalPosition(position - centerPosition, boundsDimensions, rectDimensions) + centerPosition;
        }

        public static Vector2Int ClampRectIndices(Vector2Int indices, Vector2Int boundsDimensions, Vector2Int rectDimensions)
        {
            return new Vector2Int(Mathf.Clamp(indices.x, rectDimensions.x - 1, boundsDimensions.x - 1), Mathf.Clamp(indices.y, rectDimensions.y - 1, boundsDimensions.y - 1));
        }

        public static int WallArrayIndex(Vector2Int wallDirection)
        {
            if (IsValidWallDirection(wallDirection)) return Mathf.RoundToInt(wallDirection.CartesianAngle() / (Mathf.PI / 2));
            return -1;
        }

        public static bool IsValidWallDirection(Vector2Int direction)
        {
            return direction == Vector2Int.right || direction == Vector2Int.up || direction == Vector2Int.left || direction == Vector2Int.down;
        }

        public static List<Vector2Int> GetAllWallDirections()
        {
            return new List<Vector2Int>() { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        }

        public static Vector2 LocalWallCenterPosition(Vector2Int wallDirection, Vector2Int boundsDimensions)
        {
            return new Vector2(wallDirection.x * boundsDimensions.x, wallDirection.y * boundsDimensions.y) * GameConstants.UNIT_WORLD_SIZE / 2;
        }

        public static Vector2 WallCenterPosition(Vector2Int wallDirection, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return LocalWallCenterPosition(wallDirection, boundsDimensions) + centerPosition;
        }
    }
}
