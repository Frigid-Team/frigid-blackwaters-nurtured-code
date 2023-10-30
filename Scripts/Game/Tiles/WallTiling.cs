using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public static class WallTiling
    {
        public static int WallIndexFromWallIndexDirection(Vector2Int wallIndexDirection)
        {
            if (IsValidWallIndexDirection(wallIndexDirection)) return Mathf.RoundToInt(wallIndexDirection.CartesianAngle() / 90);
            return -1;
        }

        public static Vector2Int WallIndexDirectionFromWallIndex(int wallIndex)
        {
            float wallAngleRad = wallIndex * Mathf.PI / 2;
            return new Vector2Int(Mathf.CeilToInt(Mathf.Cos(wallAngleRad)), Mathf.CeilToInt(Mathf.Sin(wallAngleRad)));
        }

        public static float WallAngleFromWallIndexDirection(Vector2Int wallIndexDirection)
        {
            if (wallIndexDirection == Vector2Int.right)
            {
                return 0;
            }
            else if (wallIndexDirection == Vector2Int.up)
            {
                return 90;
            }
            else if (wallIndexDirection == Vector2Int.left)
            {
                return 180;
            }
            else if (wallIndexDirection == Vector2Int.down)
            {
                return 270;
            }
            return 0;
        }

        public static bool IsValidWallIndexDirection(Vector2Int wallIndexDirection)
        {
            return wallIndexDirection == Vector2Int.right || wallIndexDirection == Vector2Int.up || wallIndexDirection == Vector2Int.left || wallIndexDirection == Vector2Int.down;
        }

        public static Vector2Int[] GetAllWallIndexDirections()
        {
            return new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        }

        public static int GetEdgeLength(Vector2Int wallIndexDirection, Vector2Int boundsDimensions)
        {
            if (wallIndexDirection == Vector2Int.right || wallIndexDirection == Vector2Int.left)
            {
                return boundsDimensions.y;
            }
            else if (wallIndexDirection == Vector2Int.up || wallIndexDirection == Vector2Int.down)
            {
                return boundsDimensions.x;
            }
            return -1;
        }

        public static Vector2Int WallIndexDirectionAndTileIndexToInnerTileIndexPosition(Vector2Int wallIndexDirection, int tileIndex, Vector2Int boundsDimensions)
        {
            if (wallIndexDirection == Vector2Int.right)
            {
                return new Vector2Int(boundsDimensions.x - 1, tileIndex);
            }
            else if (wallIndexDirection == Vector2Int.up)
            {
                return new Vector2Int(tileIndex, 0);
            }
            else if (wallIndexDirection == Vector2Int.left)
            {
                return new Vector2Int(0, boundsDimensions.y - 1 - tileIndex);
            }
            else if (wallIndexDirection == Vector2Int.down)
            {
                return new Vector2Int(boundsDimensions.x - 1 - tileIndex, boundsDimensions.y - 1);
            }
            return Vector2Int.zero;
        } 

        public static Vector2 EdgeTilePositionFromWallIndexDirectionAndTileIndex(Vector2Int wallIndexDirection, int tileIndex, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return EdgeTileLocalPositionFromWallIndexDirectionAndTileIndex(wallIndexDirection, tileIndex, boundsDimensions) + centerPosition;
        }

        public static Vector2 EdgeTileLocalPositionFromWallIndexDirectionAndTileIndex(Vector2Int wallIndexDirection, int tileIndex, Vector2Int boundsDimensions)
        {
            Vector2Int wallDimensions = Vector2Int.zero;
            if (wallIndexDirection == Vector2Int.right)
            {
                wallDimensions = new Vector2Int(boundsDimensions.x + 2, boundsDimensions.y);
            }
            else if (wallIndexDirection == Vector2Int.up)
            {
                wallDimensions = new Vector2Int(boundsDimensions.x, boundsDimensions.y + 2);
            }
            else if (wallIndexDirection == Vector2Int.left)
            {
                wallDimensions = new Vector2Int(boundsDimensions.x + 2, boundsDimensions.y);
            }
            else if (wallIndexDirection == Vector2Int.down)
            {
                wallDimensions = new Vector2Int(boundsDimensions.x, boundsDimensions.y + 2);
            }
            return AreaTiling.TileLocalPositionFromIndexPosition(WallIndexDirectionAndTileIndexToInnerTileIndexPosition(wallIndexDirection, tileIndex, wallDimensions), wallDimensions);
        }

        public static (Vector2Int, int) WallIndexDirectionAndEdgeTileIndexFromPosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return WallIndexDirectionAndEdgeTileIndexFromLocalPosition(position - centerPosition, boundsDimensions);
        }

        public static (Vector2Int, int) WallIndexDirectionAndEdgeTileIndexFromLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions)
        {
            Vector2Int extendedBoundsDimensions = boundsDimensions + new Vector2Int(2, 2);
            Vector2Int tileIndexPosition = AreaTiling.TileIndexPositionFromLocalPosition(localPosition, extendedBoundsDimensions);
            if (tileIndexPosition.x == extendedBoundsDimensions.x - 1)
            {
                return (Vector2Int.right, tileIndexPosition.y - 1);
            }
            else if (tileIndexPosition.y == 0)
            {
                return (Vector2Int.up, tileIndexPosition.x - 1);
            }
            else if (tileIndexPosition.x == 0)
            {
                return (Vector2Int.left, boundsDimensions.y - 1 - (tileIndexPosition.y - 1));
            }
            else if (tileIndexPosition.y == extendedBoundsDimensions.y - 1)
            {
                return (Vector2Int.down, boundsDimensions.x - 1 - (tileIndexPosition.x - 1));
            }
            return (Vector2Int.zero, -1);
        }

        public static bool EdgeTileIndexWithinBounds(Vector2Int wallIndexDirection, int tileIndex, Vector2Int boundsDimensions)
        {
            if (wallIndexDirection == Vector2Int.right || wallIndexDirection == Vector2Int.left)
            {
                return tileIndex >= 0 && tileIndex <= boundsDimensions.y - 1;
            }
            else if (wallIndexDirection == Vector2Int.up || wallIndexDirection == Vector2Int.down)
            {
                return tileIndex >= 0 && tileIndex <= boundsDimensions.x - 1;
            }
            return false;
        }

        public static bool EdgeTilePositionWithinBounds(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return EdgeTileLocalPositionWithinBounds(position - centerPosition, boundsDimensions);
        }

        public static bool EdgeTileLocalPositionWithinBounds(Vector2 localPosition, Vector2Int boundsDimensions)
        {
            if (localPosition.x > -boundsDimensions.x / 2f * FrigidConstants.UnitWorldSize && localPosition.x < (-boundsDimensions.x - 2) / 2f * FrigidConstants.UnitWorldSize ||
                localPosition.x > boundsDimensions.x / 2f * FrigidConstants.UnitWorldSize && localPosition.x < (boundsDimensions.x + 2) / 2f * FrigidConstants.UnitWorldSize)
            {
                return localPosition.y > (-boundsDimensions.y - 1) / 2f * FrigidConstants.UnitWorldSize && localPosition.y < (boundsDimensions.y + 1) / 2f * FrigidConstants.UnitWorldSize;
            }
            else if (localPosition.y > -boundsDimensions.y / 2f * FrigidConstants.UnitWorldSize && localPosition.y < (-boundsDimensions.y - 2) / 2f * FrigidConstants.UnitWorldSize ||
                     localPosition.y > boundsDimensions.y / 2f * FrigidConstants.UnitWorldSize && localPosition.y < (boundsDimensions.y + 2) / 2f * FrigidConstants.UnitWorldSize)
            {
                return localPosition.x > (-boundsDimensions.x - 1) / 2f * FrigidConstants.UnitWorldSize && localPosition.x < (boundsDimensions.x + 1) / 2f * FrigidConstants.UnitWorldSize;
            }
            return false;
        }

        public static bool AreEdgeExtentIndexesOverlapping(int firstExtentIndex, int firstWidth, int secondExtentIndex, int secondWidth)
        {
            int firstLeft = firstExtentIndex - firstWidth + 1;
            int firstRight = firstExtentIndex;
            int secondLeft = secondExtentIndex - secondWidth + 1;
            int secondRight = secondExtentIndex;
            return firstLeft <= secondRight && secondLeft <= firstRight;
        }

        public static Vector2 EdgeExtentPositionFromWallIndexDirectionAndExtentIndex(Vector2Int wallIndexDirection, int extentIndex, Vector2 centerPosition, Vector2Int boundsDimensions, int width)
        {
            return EdgeExtentLocalPositionFromWallIndexDirectionAndExtentIndex(wallIndexDirection, extentIndex, boundsDimensions, width) + centerPosition;
        }

        public static Vector2 EdgeExtentLocalPositionFromWallIndexDirectionAndExtentIndex(Vector2Int wallIndexDirection, int extentIndex, Vector2Int boundsDimensions, int width)
        {
            if (wallIndexDirection == Vector2Int.right)
            {
                return EdgeTileLocalPositionFromWallIndexDirectionAndTileIndex(wallIndexDirection, extentIndex, boundsDimensions) + new Vector2(0, (width - 1) / 2f) * FrigidConstants.UnitWorldSize;
            }
            else if (wallIndexDirection == Vector2Int.up)
            {
                return EdgeTileLocalPositionFromWallIndexDirectionAndTileIndex(wallIndexDirection, extentIndex, boundsDimensions) - new Vector2((width - 1) / 2f, 0) * FrigidConstants.UnitWorldSize;
            }
            else if (wallIndexDirection == Vector2Int.left)
            {
                return EdgeTileLocalPositionFromWallIndexDirectionAndTileIndex(wallIndexDirection, extentIndex, boundsDimensions) - new Vector2(0, (width - 1) / 2f) * FrigidConstants.UnitWorldSize;
            }
            else if (wallIndexDirection == Vector2Int.down)
            {
                return EdgeTileLocalPositionFromWallIndexDirectionAndTileIndex(wallIndexDirection, extentIndex, boundsDimensions) + new Vector2((width - 1) / 2f, 0) * FrigidConstants.UnitWorldSize;
            }
            return Vector2.zero;
        }

        public static (Vector2Int, int) WallIndexDirectionAndEdgeExtentIndexFromPosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions, int width)
        {
            return WallIndexDirectionAndEdgeExtentIndexFromLocalPosition(position - centerPosition, boundsDimensions, width);
        }

        public static (Vector2Int, int) WallIndexDirectionAndEdgeExtentIndexFromLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions, int width)
        {
            Vector2Int extendedBoundsDimensions = boundsDimensions + new Vector2Int(2, 2);
            Vector2Int tileIndexPosition = AreaTiling.TileIndexPositionFromLocalPosition(localPosition, extendedBoundsDimensions);
            if (tileIndexPosition.x == extendedBoundsDimensions.x - 1)
            {
                return WallIndexDirectionAndEdgeTileIndexFromLocalPosition(localPosition - new Vector2(0, (width - 1) / 2f) * FrigidConstants.UnitWorldSize, boundsDimensions);
            }
            else if (tileIndexPosition.y == 0)
            {
                return WallIndexDirectionAndEdgeTileIndexFromLocalPosition(localPosition + new Vector2((width - 1) / 2f, 0) * FrigidConstants.UnitWorldSize, boundsDimensions);
            }
            else if (tileIndexPosition.x == 0)
            {
                return WallIndexDirectionAndEdgeTileIndexFromLocalPosition(localPosition + new Vector2(0, (width - 1) / 2f) * FrigidConstants.UnitWorldSize, boundsDimensions);
            }
            else if (tileIndexPosition.y == extendedBoundsDimensions.y - 1)
            {
                return WallIndexDirectionAndEdgeTileIndexFromLocalPosition(localPosition - new Vector2((width - 1) / 2f, 0) * FrigidConstants.UnitWorldSize, boundsDimensions);
            }
            return (Vector2Int.zero, -1);
        }

        public static bool EdgeExtentIndexWithinBounds(Vector2Int wallIndexDirection, int extentIndex, Vector2Int boundsDimensions, int width)
        {
            return extentIndex >= width - 1 && extentIndex <= GetEdgeLength(wallIndexDirection, boundsDimensions) - 1;
        }

        public static bool EdgeExtentPositionWithinBounds(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions, int width)
        {
            return EdgeExtentLocalPositionWithinBounds(position - centerPosition, boundsDimensions, width);
        }

        public static bool EdgeExtentLocalPositionWithinBounds(Vector2 localPosition, Vector2Int boundsDimensions, int width)
        {
            int edgeLength;
            if (localPosition.x > -boundsDimensions.x / 2f * FrigidConstants.UnitWorldSize && localPosition.x < (-boundsDimensions.x - 2) / 2f * FrigidConstants.UnitWorldSize ||
                localPosition.x > boundsDimensions.x / 2f * FrigidConstants.UnitWorldSize && localPosition.x < (boundsDimensions.x + 2) / 2f * FrigidConstants.UnitWorldSize)
            {
                edgeLength = boundsDimensions.y;
            }
            else if (localPosition.y > -boundsDimensions.y / 2f * FrigidConstants.UnitWorldSize && localPosition.y < (-boundsDimensions.y - 2) / 2f * FrigidConstants.UnitWorldSize ||
                     localPosition.y > boundsDimensions.y / 2f * FrigidConstants.UnitWorldSize && localPosition.y < (boundsDimensions.y + 2) / 2f * FrigidConstants.UnitWorldSize)
            {
                edgeLength = boundsDimensions.x;
            }
            else
            {
                return false;
            }
            return localPosition.x > ((-edgeLength - 1) / 2f + width) * FrigidConstants.UnitWorldSize && localPosition.x < (edgeLength + 1) / 2f * FrigidConstants.UnitWorldSize;
        }

        public static Vector2 CornerTilePositionFromWallIndexDirections(Vector2Int wallIndexDirection1, Vector2Int wallIndexDirection2, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return CornerTileLocalPositionFromWallIndexDirections(wallIndexDirection1, wallIndexDirection2, boundsDimensions) + centerPosition;
        }

        public static Vector2 CornerTileLocalPositionFromWallIndexDirections(Vector2Int wallIndexDirection1, Vector2Int wallIndexDirection2, Vector2Int boundsDimensions)
        {
            Vector2Int extendedBoundsDimensions = boundsDimensions + new Vector2Int(2, 2);
            if (wallIndexDirection1 == Vector2Int.right && wallIndexDirection2 == Vector2Int.up || wallIndexDirection1 == Vector2Int.up && wallIndexDirection2 == Vector2Int.right)
            {
                return AreaTiling.TileLocalPositionFromIndexPosition(new Vector2Int(extendedBoundsDimensions.x - 1, 0), extendedBoundsDimensions);
            }
            else if (wallIndexDirection1 == Vector2Int.up && wallIndexDirection2 == Vector2Int.left || wallIndexDirection1 == Vector2Int.left && wallIndexDirection2 == Vector2Int.up)
            {
                return AreaTiling.TileLocalPositionFromIndexPosition(new Vector2Int(0, 0), extendedBoundsDimensions);
            }
            else if (wallIndexDirection1 == Vector2Int.left && wallIndexDirection2 == Vector2Int.down || wallIndexDirection1 == Vector2Int.down && wallIndexDirection2 == Vector2Int.left)
            {
                return AreaTiling.TileLocalPositionFromIndexPosition(new Vector2Int(0, extendedBoundsDimensions.y - 1), extendedBoundsDimensions);
            }
            else if (wallIndexDirection1 == Vector2Int.down && wallIndexDirection2 == Vector2Int.right || wallIndexDirection1 == Vector2Int.right && wallIndexDirection2 == Vector2Int.down)
            {
                return AreaTiling.TileLocalPositionFromIndexPosition(new Vector2Int(extendedBoundsDimensions.x - 1, extendedBoundsDimensions.y - 1), extendedBoundsDimensions);
            }
            return Vector2.zero;
        }

        public static (Vector2Int, Vector2Int) CornerWallIndexDirectionsFromPosition(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return CornerWallIndexDirectionsFromLocalPosition(position - centerPosition, boundsDimensions);
        }

        public static (Vector2Int, Vector2Int) CornerWallIndexDirectionsFromLocalPosition(Vector2 localPosition, Vector2Int boundsDimensions)
        {
            Vector2Int extendedBoundsDimensions = boundsDimensions + new Vector2Int(2, 2);
            Vector2Int tileIndexPosition = AreaTiling.TileIndexPositionFromLocalPosition(localPosition, extendedBoundsDimensions);
            if (tileIndexPosition.x == extendedBoundsDimensions.x - 1 && tileIndexPosition.y == 0)
            {
                return (Vector2Int.right, Vector2Int.up);
            }
            else if (tileIndexPosition.x == 0 && tileIndexPosition.y == 0)
            {
                return (Vector2Int.up, Vector2Int.left);
            }
            else if (tileIndexPosition.x == 0 && tileIndexPosition.y == extendedBoundsDimensions.y - 1)
            {
                return (Vector2Int.left, Vector2Int.down);
            }
            else if (tileIndexPosition.x == extendedBoundsDimensions.x - 1 && tileIndexPosition.y == extendedBoundsDimensions.y - 1)
            {
                return (Vector2Int.down, Vector2Int.right);
            }
            return (Vector2Int.zero, Vector2Int.zero);
        }

        public static bool AreValidCornerWallIndexDirections(Vector2Int wallIndexDirection1, Vector2Int wallIndexDirection2)
        {
            return !(wallIndexDirection1 == wallIndexDirection2 && wallIndexDirection1 + wallIndexDirection2 == Vector2Int.zero) && IsValidWallIndexDirection(wallIndexDirection1) && IsValidWallIndexDirection(wallIndexDirection2);
        }

        public static bool CornerTilePositionWithinBounds(Vector2 position, Vector2 centerPosition, Vector2Int boundsDimensions)
        {
            return CornerTileLocalPositionWithinBounds(position - centerPosition, boundsDimensions);
        }

        public static bool CornerTileLocalPositionWithinBounds(Vector2 localPosition, Vector2Int boundsDimensions)
        {
            bool leftEdge = localPosition.x > -boundsDimensions.x / 2f * FrigidConstants.UnitWorldSize && localPosition.x < (-boundsDimensions.x - 2) / 2f * FrigidConstants.UnitWorldSize;
            bool rightEdge = localPosition.x > boundsDimensions.x / 2f * FrigidConstants.UnitWorldSize && localPosition.x < (boundsDimensions.x + 2) / 2f * FrigidConstants.UnitWorldSize;
            bool bottomEdge = localPosition.y > -boundsDimensions.y / 2f * FrigidConstants.UnitWorldSize && localPosition.y < (-boundsDimensions.y - 2) / 2f * FrigidConstants.UnitWorldSize;
            bool topEdge = localPosition.y > boundsDimensions.y / 2f * FrigidConstants.UnitWorldSize && localPosition.y < (boundsDimensions.y + 2) / 2f * FrigidConstants.UnitWorldSize;
            return rightEdge && topEdge || topEdge && leftEdge || leftEdge && bottomEdge || bottomEdge && rightEdge;
        }
    }
}
