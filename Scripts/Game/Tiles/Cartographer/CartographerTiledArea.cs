#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrigidBlackwaters.Game
{
    public class CartographerTiledArea : FrigidMonoBehaviour
    {
        [Header("Room Settings")]
        [SerializeField]
        private Vector2Int roomDimensions;
        [SerializeField]
        private TileTerrain defaultTerrain;

        [Header("Floor Imports")]
        [SerializeField]
        private TerrainTileAssetGroup terrainAssetGroup;
        [SerializeField]
        private string defaultWaterId;
        [SerializeField]
        private string defaultLandId;

        [Header("Wall Imports")]
        [SerializeField]
        private WallTileAssetGroup wallAssetGroup;
        [SerializeField]
        private string wallId;

        [Header("Level Import")]
        [SerializeField]
        private TiledAreaBlueprint importedBlueprint;

        [Header("Cartographer Prefabs")]
        [SerializeField]
        private CartographerTile cartographerTerrain;
        [SerializeField]
        private CartographerMobMark mobMark;

        [Header("Decoration Importer")]
        [SerializeField]
        private CartographerImporter decorationImporter;

        private List<CartographerTile> mainTiles;
        private List<List<CartographerTile>> wallTiles;
        private List<CartographerMobMark> mobMarks;

        private List<List<List<WallTile>>> depthWalls;
        private List<List<WallTile>> wallExpansionGroups;

        public TerrainTileAssetGroup TerrainAssetGroup
        {
            get
            {
                return this.terrainAssetGroup;
            }
        }

        public CartographerImporter DecorationImporter
        {
            get
            {
                return this.decorationImporter;
            }
        }

        public List<CartographerTile> MainTiles
        {
            get
            {
                return this.mainTiles;
            }
        }

        public TileTerrain GetTileTerrain(int index)
        {
            return this.mainTiles[index].Terrain;
        }

        private string GetTileID(int index)
        {
            if (GetTileTerrain(index) == TileTerrain.Water)
            {
                return this.mainTiles[index].WaterID;
            }
            else if (GetTileTerrain(index) == TileTerrain.Land)
            {
                return this.mainTiles[index].LandID;
            }
            else
            {
                return "";
            }
        }

        private Vector2Int IndexToPos(int index)
        {
            return new Vector2Int(index % this.roomDimensions.x, index / this.roomDimensions.x);
        }

        private int PosToIndex(Vector2Int position)
        {
            return position.y * this.roomDimensions.x + position.x;
        }

        private Vector2 GetMiddlePos()
        {
            return new Vector2((float)(this.roomDimensions.x - 1) / 2 + this.mainTiles[0].transform.position.x,
                               (float)-(this.roomDimensions.y - 1) / 2 + this.mainTiles[0].transform.position.y);
        }

        private TileTerrain DoorType(Vector2Int direction)
        {
            if (direction == Vector2Int.right
                && this.mainTiles[PosToIndex(new Vector2Int(this.roomDimensions.x - 1, this.roomDimensions.y / 2))].Terrain ==
                   this.mainTiles[PosToIndex(new Vector2Int(this.roomDimensions.x - 1, (this.roomDimensions.y + 1) / 2 - 1))].Terrain)
            {
                return this.mainTiles[PosToIndex(new Vector2Int(this.roomDimensions.x - 1, this.roomDimensions.y / 2))].Terrain;
            }
            else if (direction == Vector2Int.up
                     && this.mainTiles[PosToIndex(new Vector2Int(this.roomDimensions.x / 2, 0))].Terrain ==
                        this.mainTiles[PosToIndex(new Vector2Int((this.roomDimensions.x + 1) / 2 - 1, 0))].Terrain)
            {
                return this.mainTiles[PosToIndex(new Vector2Int(this.roomDimensions.x / 2, 0))].Terrain;
            }
            else if (direction == Vector2Int.left
                     && this.mainTiles[PosToIndex(new Vector2Int(0, this.roomDimensions.y / 2))].Terrain ==
                        this.mainTiles[PosToIndex(new Vector2Int(0, (this.roomDimensions.y + 1) / 2 - 1))].Terrain)
            {
                return this.mainTiles[PosToIndex(new Vector2Int(0, this.roomDimensions.y / 2))].Terrain;
            }
            else if (direction == Vector2Int.down
                     && this.mainTiles[PosToIndex(new Vector2Int(this.roomDimensions.x / 2, this.roomDimensions.y - 1))].Terrain ==
                        this.mainTiles[PosToIndex(new Vector2Int((this.roomDimensions.x + 1) / 2 - 1, this.roomDimensions.y - 1))].Terrain)
            {
                return this.mainTiles[PosToIndex(new Vector2Int(this.roomDimensions.x / 2, this.roomDimensions.y - 1))].Terrain;
            }
            else
            {
                return TileTerrain.None;
            }
        }

        private bool IsDuplicateMark(CartographerMobMark testMark)
        {
            for (int i = 0; i < this.mobMarks.Count; i++)
            {
                if (this.mobMarks[i].SpawnPointPreset.SpawnPoint.LocalPosition == testMark.SpawnPointPreset.SpawnPoint.LocalPosition)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveMobMark(CartographerMobMark mobMark)
        {
            this.mobMarks.Remove(mobMark);
            FrigidInstancing.DestroyInstance(mobMark);
        }

        private CartographerTile CreateMainTerrainTile(Vector2Int roomPosition, Vector3 visualPosition, TileTerrain terrainType, string targetId = "")
        {
            CartographerTile tile = FrigidInstancing.CreateInstance<CartographerTile>(this.cartographerTerrain);

            TerrainTileAsset terrainAsset;
            string chosenId;

            // Grab water sprite
            if (targetId != "" && terrainType == TileTerrain.Water)
            {
                chosenId = targetId;
            }
            else
            {
                chosenId = defaultWaterId;
            }
            if (!this.terrainAssetGroup.TryGetTerrainTileAsset(TileTerrain.Water, chosenId, out terrainAsset))
            {
                // On fail
                Debug.LogError("TerrainAssetGroup does not contain given water id");
                return null;
            }
            tile.Water = FrigidInstancing.CreateInstance<TerrainTile>(terrainAsset.TerrainTilePrefab, tile.transform);
            tile.WaterID = chosenId;

            // Grab land sprite
            if (targetId != "" && terrainType == TileTerrain.Land)
            {
                chosenId = targetId;
            }
            else
            {
                chosenId = defaultLandId;
            }
            if (!this.terrainAssetGroup.TryGetTerrainTileAsset(TileTerrain.Land, chosenId, out terrainAsset))
            {
                // On fail
                Debug.LogError("TerrainAssetGroup does not contain given land id");
                return null;
            }
            tile.Land = FrigidInstancing.CreateInstance<TerrainTile>(terrainAsset.TerrainTilePrefab, tile.transform);
            tile.LandID = chosenId;

            // Assign everything else
            tile.Position = roomPosition;
            tile.transform.position = visualPosition;
            tile.TerrainArea = this;
            if (terrainType != TileTerrain.None && terrainType != TileTerrain.Count)
            {
                tile.SetTerrain(terrainType);
            }
            else
            {
                tile.SetTerrain(TileTerrain.Water);
            }
            this.mainTiles.Insert(PosToIndex(roomPosition), tile);
            return tile;
        }

        private void DestroyMainTerrainTile(Vector2Int roomPosition)
        {
            CartographerTile tile = this.mainTiles[PosToIndex(roomPosition)];
            this.mainTiles.RemoveAt(PosToIndex(roomPosition));
            FrigidInstancing.DestroyInstance(tile);
        }

        private CartographerTile CreateWallTerrainTile(Vector2Int roomPosition, Vector3 visualPosition, int targetDepth)
        {
            if (!this.wallAssetGroup.TryGetAsset(this.wallId, out WallTileAsset wallTileAsset))
            {
                Debug.LogError("WallTileAssetGroup does not contain given wall id");
                return null;
            }

            WallTile tile = FrigidInstancing.CreateInstance<WallTile>(wallTileAsset.GetWallTilePrefab(targetDepth));
            tile.transform.position = visualPosition;

            // Rotate
            if (roomPosition.y < 0
                && roomPosition.x >= roomPosition.y
                && roomPosition.x - this.roomDimensions.x + 1 < -roomPosition.y)
            {
                tile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (roomPosition.x < 0
                     && -roomPosition.x >= roomPosition.y - this.roomDimensions.y + 1
                     && roomPosition.y > roomPosition.x)
            {
                tile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
            else if (roomPosition.y - this.roomDimensions.y + 1 > 0
                     && roomPosition.x - this.roomDimensions.x + 1 <= roomPosition.y - this.roomDimensions.y + 1
                     && -roomPosition.x < roomPosition.y - this.roomDimensions.y + 1)
            {
                tile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            }
            else
            {
                tile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
            }

            // Assign to expansion groups
            if (this.roomDimensions.x <= roomPosition.x) // East
            {
                this.wallExpansionGroups[0].Add(tile);
            }
            if (roomPosition.y < 0) // North
            {
                this.wallExpansionGroups[1].Add(tile);
            }
            if (roomPosition.x < 0) // West
            {
                this.wallExpansionGroups[2].Add(tile);
            }
            if (this.roomDimensions.y <= roomPosition.y) // South
            {
                this.wallExpansionGroups[3].Add(tile);
            }

            // Set wall type
            if (Mathf.Max(-roomPosition.x, roomPosition.x - this.roomDimensions.x + 1) == Mathf.Max(-roomPosition.y, roomPosition.y - this.roomDimensions.y + 1))
            {
                tile.Populated(false);
                return null;
            }
            else
            {
                tile.Populated(true);

                // Allow decorations to be added
                if (targetDepth == 1)
                {
                    CartographerTile cartographerTile = FrigidInstancing.CreateInstance<CartographerTile>(this.cartographerTerrain);
                    cartographerTile.WallTile = tile;
                    tile.transform.parent = cartographerTile.transform;
                    tile.transform.position = Vector3.zero;
                    cartographerTile.transform.position = visualPosition;
                    cartographerTile.Position = roomPosition;
                    cartographerTile.TerrainArea = this;
                    tile.gameObject.AddComponent<BoxCollider2D>();

                    if (this.roomDimensions.x <= roomPosition.x) // East
                    {
                        this.wallTiles[0].Insert(roomPosition.y, cartographerTile);
                        cartographerTile.WallFloorIndex = PosToIndex(new Vector2Int(roomPosition.x - 1, roomPosition.y));
                    }
                    else if (roomPosition.y < 0) // North
                    {
                        this.wallTiles[1].Insert(roomPosition.x, cartographerTile);
                        cartographerTile.WallFloorIndex = PosToIndex(new Vector2Int(roomPosition.x, roomPosition.y + 1));
                    }
                    else if (roomPosition.x < 0) // West
                    {
                        this.wallTiles[2].Insert(roomPosition.y, cartographerTile);
                        cartographerTile.WallFloorIndex = PosToIndex(new Vector2Int(roomPosition.x + 1, roomPosition.y));
                    }
                    else // South
                    {
                        this.wallTiles[3].Insert(roomPosition.x, cartographerTile);
                        cartographerTile.WallFloorIndex = PosToIndex(new Vector2Int(roomPosition.x, roomPosition.y - 1));
                    }
                    return cartographerTile;
                }
                else
                {
                    if (this.roomDimensions.x <= roomPosition.x && 0 <= roomPosition.y && roomPosition.y < roomDimensions.y) // East
                    {
                        this.depthWalls[0][roomPosition.y].Add(tile);
                    }
                    else if (roomPosition.y < 0 && 0 <= roomPosition.x && roomPosition.x < roomDimensions.x) // North
                    {
                        this.depthWalls[1][roomPosition.x].Add(tile);
                    }
                    else if (roomPosition.x < 0 && 0 <= roomPosition.y && roomPosition.y < roomDimensions.y) // West
                    {
                        this.depthWalls[2][roomPosition.y].Add(tile);
                    }
                    else if (this.roomDimensions.y <= roomPosition.y && 0 <= roomPosition.x && roomPosition.x < roomDimensions.x) // South
                    {
                        this.depthWalls[3][roomPosition.x].Add(tile);
                    }
                    return null;
                }
            }
        }

        private void DestroyWallTerrainTile(Vector2Int roomPosition)
        {
            int wallIndex;
            int wallPosition;
            if (this.roomDimensions.x <= roomPosition.x) // East
            {
                wallIndex = 0;
                wallPosition = roomPosition.y;
            }
            else if (roomPosition.y < 0) // North
            {
                wallIndex = 1;
                wallPosition = roomPosition.x;
            }
            else if (roomPosition.x < 0) // West
            {
                wallIndex = 2;
                wallPosition = roomPosition.y;
            }
            else // South
            {
                wallIndex = 3;
                wallPosition = roomPosition.x;
            }

            for (int i = this.depthWalls[wallIndex][wallPosition].Count - 1; i >= 0; i--)
            {
                WallTile wall = this.depthWalls[wallIndex][wallPosition][i];
                this.depthWalls[wallIndex][wallPosition].RemoveAt(i);
                this.wallExpansionGroups[wallIndex].Remove(wall);
                FrigidInstancing.DestroyInstance(wall);
            }
            this.depthWalls[wallIndex].RemoveAt(wallPosition);

            CartographerTile tile = this.wallTiles[wallIndex][wallPosition];
            this.wallTiles[wallIndex].RemoveAt(wallPosition);
            this.wallExpansionGroups[wallIndex].RemoveAt(wallPosition);
            FrigidInstancing.DestroyInstance(tile);
        }

        private void ExpandRoom(Vector2Int direction)
        {
            int wallIndex;
            if (direction == Vector2Int.right)
            {
                wallIndex = 0;
            }
            else if (direction == Vector2Int.up)
            {
                wallIndex = 1;
            }
            else if (direction == Vector2Int.left)
            {
                wallIndex = 2;
            }
            else if (direction == Vector2Int.down)
            {
                wallIndex = 3;
            }
            else
            {
                return;
            }

            // Update existing mobMarks
            foreach (CartographerMobMark mark in this.mobMarks)
            {
                TiledAreaMobSpawnPoint newSpawnPoint =
                    new TiledAreaMobSpawnPoint(mark.SpawnPointPreset.SpawnPoint.LocalPosition + new Vector2((float)-direction.x / 2, (float)-direction.y / 2), mark.SpawnPointPreset.SpawnPoint.Reaches);
                mark.SpawnPointPreset = new TiledAreaMobGenerationPreset.SpawnPointPreset(mark.SpawnPointPreset.StrategyID, newSpawnPoint);
            }

            // Update existing mainTiles
            if (direction == Vector2Int.up || direction == Vector2Int.left)
            {
                foreach (CartographerTile tile in this.mainTiles)
                {
                    tile.Position = tile.Position + new Vector2Int(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
                }
            }

            // Update existing wallTiles
            if (direction == Vector2Int.right)
            {
                for (int i = 0; i < this.roomDimensions.y; i++)
                {
                    this.wallTiles[0][i].Position = this.wallTiles[0][i].Position + Vector2Int.right;
                }
            }
            else if (direction == Vector2Int.up)
            {
                for (int i = 0; i < this.roomDimensions.x; i++)
                {
                    this.wallTiles[3][i].Position = this.wallTiles[3][i].Position + Vector2Int.up;
                }
                for (int i = 0; i < this.roomDimensions.y; i++)
                {
                    this.wallTiles[0][i].Position = this.wallTiles[0][i].Position + Vector2Int.up;
                    this.wallTiles[2][i].Position = this.wallTiles[2][i].Position + Vector2Int.up;
                }
            }
            else if (direction == Vector2Int.left)
            {
                for (int i = 0; i < this.roomDimensions.x; i++)
                {
                    this.wallTiles[1][i].Position = this.wallTiles[1][i].Position + Vector2Int.right;
                    this.wallTiles[3][i].Position = this.wallTiles[3][i].Position + Vector2Int.right;
                }
                for (int i = 0; i < this.roomDimensions.y; i++)
                {
                    this.wallTiles[0][i].Position = this.wallTiles[0][i].Position + Vector2Int.right;
                }
            }
            else
            {
                for (int i = 0; i < this.roomDimensions.x; i++)
                {
                    this.wallTiles[3][i].Position = this.wallTiles[3][i].Position + Vector2Int.up;
                }
            }

            // Move expansion groups
            foreach (WallTile wall in this.wallExpansionGroups[wallIndex])
            {
                wall.transform.position = wall.transform.position + new Vector3(direction.x, direction.y, 0);
            }

            // Update roomDimensions
            this.roomDimensions = this.roomDimensions + new Vector2Int(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

            // Add new depthWall entries
            if (direction == Vector2Int.right)
            {
                this.depthWalls[1].Insert(roomDimensions.x - 1, new List<WallTile>());
                this.depthWalls[3].Insert(roomDimensions.x - 1, new List<WallTile>());
            }
            else if (direction == Vector2Int.up)
            {
                this.depthWalls[0].Insert(0, new List<WallTile>());
                this.depthWalls[2].Insert(0, new List<WallTile>());
            }
            else if (direction == Vector2Int.left)
            {
                this.depthWalls[1].Insert(0, new List<WallTile>());
                this.depthWalls[3].Insert(0, new List<WallTile>());
            }
            else
            {
                this.depthWalls[0].Insert(roomDimensions.y - 1, new List<WallTile>());
                this.depthWalls[2].Insert(roomDimensions.y - 1, new List<WallTile>());
            }

            // Make new walls and floors
            if (!this.wallAssetGroup.TryGetAsset(this.wallId, out WallTileAsset wallTileAsset))
            {
                Debug.LogError("Cartographer invalid wallId!");
                return;
            }
            Vector2Int roomStart;
            Vector2Int roomDirection;
            Vector3 visualStart;
            Vector3 visualDirection;
            Vector2Int copyDirection;
            if (direction == Vector2Int.right)
            {
                roomStart = new Vector2Int(this.roomDimensions.x - 1, 0);
                roomDirection = Vector2Int.up;
                visualStart = new Vector3(mainTiles[mainTiles.Count - 1].transform.position.x + 1, mainTiles[0].transform.position.y, 0);
                visualDirection = Vector3.down;
                copyDirection = Vector2Int.left;
            }
            else if (direction == Vector2Int.up)
            {
                roomStart = new Vector2Int(0, 0);
                roomDirection = Vector2Int.right;
                visualStart = new Vector3(mainTiles[0].transform.position.x, mainTiles[0].transform.position.y + 1, 0);
                visualDirection = Vector3.right;
                copyDirection = Vector2Int.up;
            }
            else if (direction == Vector2Int.left)
            {
                roomStart = new Vector2Int(0, 0);
                roomDirection = Vector2Int.up;
                visualStart = new Vector3(mainTiles[0].transform.position.x - 1, mainTiles[0].transform.position.y, 0);
                visualDirection = Vector3.down;
                copyDirection = Vector2Int.right;
            }
            else
            {
                roomStart = new Vector2Int(0, this.roomDimensions.y - 1);
                roomDirection = Vector2Int.right;
                visualStart = new Vector3(mainTiles[0].transform.position.x, mainTiles[mainTiles.Count - 1].transform.position.y - 1, 0);
                visualDirection = Vector3.right;
                copyDirection = Vector2Int.down;
            }
            for (int i = -wallTileAsset.Depth; i < this.wallTiles[wallIndex].Count + wallTileAsset.Depth; i++)
            {
                if (0 <= i && i < this.wallTiles[wallIndex].Count)
                {
                    CreateMainTerrainTile(roomStart + roomDirection * i,
                                          visualStart + visualDirection * i,
                                          this.defaultTerrain);
                }
                else
                {
                    CreateWallTerrainTile(roomStart + roomDirection * i,
                                          visualStart + visualDirection * i,
                                          Mathf.Max(-i, i - this.wallTiles[wallIndex].Count + 1));
                }
            }
            for (int i = 0; i < this.wallTiles[wallIndex].Count; i++)
            {
                this.MainTiles[PosToIndex(roomStart + roomDirection * i)].SetTerrain(GetTileTerrain(PosToIndex(roomStart + roomDirection * i + copyDirection)));
            }

            // Update wallFloorIndexes
            for (int i = 0; i < this.wallTiles.Count; i++)
            {
                foreach (CartographerTile tile in this.wallTiles[i])
                {
                    switch (i)
                    {
                        case 0:
                            tile.WallFloorIndex = PosToIndex(tile.Position + Vector2Int.left);
                            break;
                        case 1:
                            tile.WallFloorIndex = PosToIndex(tile.Position + Vector2Int.up);
                            break;
                        case 2:
                            tile.WallFloorIndex = PosToIndex(tile.Position + Vector2Int.right);
                            break;
                        case 3:
                            tile.WallFloorIndex = PosToIndex(tile.Position + Vector2Int.down);
                            break;
                    }
                }
            }
        }

        private void ShrinkRoom(Vector2Int direction)
        {
            int wallIndex;
            if (direction == Vector2Int.right && roomDimensions.x > 1)
            {
                wallIndex = 0;
            }
            else if (direction == Vector2Int.up && roomDimensions.y > 1)
            {
                wallIndex = 1;
            }
            else if (direction == Vector2Int.left && roomDimensions.x > 1)
            {
                wallIndex = 2;
            }
            else if (direction == Vector2Int.down && roomDimensions.y > 1)
            {
                wallIndex = 3;
            }
            else
            {
                return;
            }

            // Update existing mobMarks
            foreach (CartographerMobMark mark in this.mobMarks)
            {
                TiledAreaMobSpawnPoint newSpawnPoint = 
                    new TiledAreaMobSpawnPoint(mark.SpawnPointPreset.SpawnPoint.LocalPosition - new Vector2((float)-direction.x / 2, (float)-direction.y / 2), mark.SpawnPointPreset.SpawnPoint.Reaches);
                mark.SpawnPointPreset = new TiledAreaMobGenerationPreset.SpawnPointPreset(mark.SpawnPointPreset.StrategyID, newSpawnPoint);
            }

            // Update existing mainTiles
            if (direction == Vector2Int.up || direction == Vector2Int.left)
            {
                foreach (CartographerTile tile in this.mainTiles)
                {
                    tile.Position = tile.Position - new Vector2Int(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
                }
            }

            // Update existing wallTiles
            if (direction == Vector2Int.right)
            {
                for (int i = 0; i < this.roomDimensions.y; i++)
                {
                    this.wallTiles[0][i].Position = this.wallTiles[0][i].Position - Vector2Int.right;
                }
            }
            else if (direction == Vector2Int.up)
            {
                for (int i = 0; i < this.roomDimensions.x; i++)
                {
                    this.wallTiles[3][i].Position = this.wallTiles[3][i].Position - Vector2Int.up;
                }
                for (int i = 0; i < this.roomDimensions.y; i++)
                {
                    this.wallTiles[0][i].Position = this.wallTiles[0][i].Position - Vector2Int.up;
                    this.wallTiles[2][i].Position = this.wallTiles[2][i].Position - Vector2Int.up;
                }
            }
            else if (direction == Vector2Int.left)
            {
                for (int i = 0; i < this.roomDimensions.x; i++)
                {
                    this.wallTiles[1][i].Position = this.wallTiles[1][i].Position - Vector2Int.right;
                    this.wallTiles[3][i].Position = this.wallTiles[3][i].Position - Vector2Int.right;
                }
                for (int i = 0; i < this.roomDimensions.y; i++)
                {
                    this.wallTiles[0][i].Position = this.wallTiles[0][i].Position - Vector2Int.right;
                }
            }
            else
            {
                for (int i = 0; i < this.roomDimensions.x; i++)
                {
                    this.wallTiles[3][i].Position = this.wallTiles[3][i].Position - Vector2Int.up;
                }
            }

            // Move expansion groups
            foreach (WallTile wall in this.wallExpansionGroups[wallIndex])
            {
                wall.transform.position = wall.transform.position - new Vector3(direction.x, direction.y, 0);
            }

            // Destroy walls and floors
            Vector2Int roomStart;
            Vector2Int roomDirection;
            if (direction == Vector2Int.right)
            {
                roomStart = new Vector2Int(this.roomDimensions.x - 1, this.roomDimensions.y - 1);
                roomDirection = Vector2Int.down;
            }
            else if (direction == Vector2Int.up)
            {
                roomStart = new Vector2Int(this.roomDimensions.x - 1, 0);
                roomDirection = Vector2Int.left;
            }
            else if (direction == Vector2Int.left)
            {
                roomStart = new Vector2Int(0, this.roomDimensions.y - 1);
                roomDirection = Vector2Int.down;
            }
            else
            {
                roomStart = new Vector2Int(this.roomDimensions.x - 1, this.roomDimensions.y - 1);
                roomDirection = Vector2Int.left;
            }
            for (int i = -1; i < this.wallTiles[wallIndex].Count + 1; i++)
            {
                if (0 <= i && i < this.wallTiles[wallIndex].Count)
                {
                    DestroyMainTerrainTile(roomStart + roomDirection * i);
                }
                else
                {
                    DestroyWallTerrainTile(roomStart + roomDirection * i);
                }
            }

            // Update roomDimensions
            this.roomDimensions = this.roomDimensions - new Vector2Int(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

            // Update wallFloorIndexes
            for (int i = 0; i < this.wallTiles.Count; i++)
            {
                foreach (CartographerTile tile in this.wallTiles[i])
                {
                    switch (i)
                    {
                        case 0:
                            tile.WallFloorIndex = PosToIndex(tile.Position + Vector2Int.left);
                            break;
                        case 1:
                            tile.WallFloorIndex = PosToIndex(tile.Position + Vector2Int.up);
                            break;
                        case 2:
                            tile.WallFloorIndex = PosToIndex(tile.Position + Vector2Int.right);
                            break;
                        case 3:
                            tile.WallFloorIndex = PosToIndex(tile.Position + Vector2Int.down);
                            break;
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.mainTiles = new List<CartographerTile>();
            this.wallTiles = new List<List<CartographerTile>>();
            this.mobMarks = new List<CartographerMobMark>();
            this.depthWalls = new List<List<List<WallTile>>>();
            this.wallExpansionGroups = new List<List<WallTile>>();
        }

        protected override void Start()
        {
            base.Start();
            // Initialize settings
            if (this.importedBlueprint != null)
            {
                this.wallId = this.importedBlueprint.WallTileID;
            }

            if (!this.wallAssetGroup.TryGetAsset(this.wallId, out WallTileAsset wallTileAsset))
            {
                Debug.LogError("WallAssetGroup does not contain given GUID, loading GUID 0");
                return;
            }

            if (this.importedBlueprint == null)
            {
                this.roomDimensions.x = Mathf.Max(1, this.roomDimensions.x);
                this.roomDimensions.y = Mathf.Max(1, this.roomDimensions.y);
            }
            else
            {
                this.roomDimensions.x = this.importedBlueprint.MainAreaDimensions.x;
                this.roomDimensions.y = this.importedBlueprint.MainAreaDimensions.y;
            }
            for (int i = 0; i < 4; i++)
            {
                this.wallTiles.Add(new List<CartographerTile>());
                this.wallExpansionGroups.Add(new List<WallTile>());

                this.depthWalls.Add(new List<List<WallTile>>());
                if (i % 2 == 0)
                {
                    for (int j = 0; j < roomDimensions.y; j++)
                    {
                        this.depthWalls[i].Add(new List<WallTile>());
                    }
                }
                else
                {
                    for (int j = 0; j < roomDimensions.x; j++)
                    {
                        this.depthWalls[i].Add(new List<WallTile>());
                    }
                }
            }

            // Create land/water
            for (int y = 0; y < this.roomDimensions.y; y++)
            {
                for (int x = 0; x < this.roomDimensions.x; x++)
                {
                    if (this.importedBlueprint != null)
                    {
                        CartographerTile tile = CreateMainTerrainTile(new Vector2Int(x, y), new Vector3(x, -y, 0),
                            this.importedBlueprint.GetTerrainAtTile(new Vector2Int(x, y)),
                            this.importedBlueprint.GetTerrainTileIDAtTile(new Vector2Int(x, y)));

                        for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                        {
                            TerrainContentHeight height = (TerrainContentHeight)i;
                            string contentId = this.importedBlueprint.GetTerrainContentIDAtTile(new Vector2Int(x, y), height);
                            if (contentId != "")
                            {
                                if (this.decorationImporter.TerrainContentAssetGroup.TryGetTerrainContentAsset(height, this.importedBlueprint.GetTerrainAtTile(new Vector2Int(x, y)), contentId, out TerrainContentAsset terrainContentAsset))
                                {
                                    tile.ReplaceContent(terrainContentAsset, height);
                                }
                            }
                        }
                    }
                    else
                    {
                        CartographerTile tile = CreateMainTerrainTile(new Vector2Int(x, y), new Vector3(x, -y, 0),
                            this.defaultTerrain);
                    }
                }
            }

            // Create walls
            for (int j = -wallTileAsset.Depth; j < this.roomDimensions.y + wallTileAsset.Depth; j++)
            {
                for (int i = -wallTileAsset.Depth; i < this.roomDimensions.x + wallTileAsset.Depth; i++)
                {
                    if (i < 0 || this.roomDimensions.x <= i || j < 0 || this.roomDimensions.y <= j)
                    {
                        int targetDepth = Mathf.Max(-i, i - this.roomDimensions.x + 1, -j, j - this.roomDimensions.y + 1);
                        CartographerTile tile = CreateWallTerrainTile(new Vector2Int(i, j), new Vector3(i, -j, 0), targetDepth);

                        if (tile != null) {
                            // Create decorations
                            if (this.importedBlueprint != null)
                            {
                                string decoGUID;
                                TileTerrain terrainType;
                                if (this.roomDimensions.x <= i) // East
                                {
                                    decoGUID = this.importedBlueprint.GetWallContentIDAtTile(Vector2Int.right, j);
                                    terrainType = this.importedBlueprint.GetTerrainAtTile(new Vector2Int(i - 1, j));
                                }
                                else if (j < 0) // North
                                {
                                    decoGUID = importedBlueprint.GetWallContentIDAtTile(Vector2Int.up, i);
                                    terrainType = importedBlueprint.GetTerrainAtTile(new Vector2Int(i, j + 1));
                                }
                                else if (i < 0) // West
                                {
                                    decoGUID = this.importedBlueprint.GetWallContentIDAtTile(Vector2Int.left, j);
                                    terrainType = this.importedBlueprint.GetTerrainAtTile(new Vector2Int(i + 1, j));
                                }
                                else // South
                                {
                                    decoGUID = importedBlueprint.GetWallContentIDAtTile(Vector2Int.down, i);
                                    terrainType = importedBlueprint.GetTerrainAtTile(new Vector2Int(i, j - 1));
                                }
                                if (decoGUID != "")
                                {
                                    if (this.decorationImporter.WallContentGroup.TryGetWallContentAsset(terrainType, decoGUID, out WallContentAsset wallContentAsset))
                                    {
                                        tile.ReplaceContent(wallContentAsset);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Create marks
            if (this.importedBlueprint != null)
            {
                foreach (TiledAreaMobGenerationPreset preset in this.importedBlueprint.MobGenerationPresets)
                {
                    foreach (TiledAreaMobGenerationPreset.SpawnPointPreset spawnPoint in preset.SpawnPointPresets)
                    {
                        CartographerMobMark newMark = FrigidInstancing.CreateInstance<CartographerMobMark>(this.mobMark);
                        newMark.TerrainArea = this;
                        newMark.transform.position = spawnPoint.SpawnPoint.LocalPosition + GetMiddlePos();
                        newMark.SpawnPointPreset = new TiledAreaMobGenerationPreset.SpawnPointPreset(spawnPoint.StrategyID, spawnPoint.SpawnPoint);
                        this.mobMarks.Add(newMark);
                    }
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            bool addMark = false;
            List<MobReach> reaches = null;
            string markId = "";
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                reaches = new List<MobReach>() { MobReach.Melee, MobReach.Ranged };
                addMark = true;
                markId = "dungeon_enemies";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                reaches = new List<MobReach>() { MobReach.Melee };
                addMark = true;
                markId = "dungeon_enemies";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                reaches = new List<MobReach>() { MobReach.Ranged };
                addMark = true;
                markId = "dungeon_enemies";
            } else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                reaches = new List<MobReach>() { MobReach.Melee, MobReach.Ranged };
                addMark = true;
                markId = "allies";
            } else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                reaches = new List<MobReach>() { MobReach.Melee, MobReach.Ranged };
                addMark = true;
                markId = "dungeon_shop_keepers";
            }

            if (addMark)
            {
                Vector2 offset = (Vector2)UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition) - GetMiddlePos();
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    if (this.roomDimensions.x % 2 == 0)
                    {
                        offset.x = Mathf.Floor(offset.x) + 0.5f;
                    }
                    else
                    {
                        offset.x = Mathf.Floor(offset.x + 0.5f);
                    }
                    if (this.roomDimensions.y % 2 == 0)
                    {
                        offset.y = Mathf.Floor(offset.y) + 0.5f;
                    }
                    else
                    {
                        offset.y = Mathf.Floor(offset.y + 0.5f);
                    }
                }

                CartographerMobMark newMark = FrigidInstancing.CreateInstance<CartographerMobMark>(this.mobMark);
                newMark.TerrainArea = this;
                newMark.transform.position = offset + GetMiddlePos();
                newMark.SpawnPointPreset = new TiledAreaMobGenerationPreset.SpawnPointPreset(markId, new TiledAreaMobSpawnPoint(offset, reaches));
                if (IsDuplicateMark(newMark))
                {
                    FrigidInstancing.DestroyInstance(newMark);
                }
                else
                {
                    this.mobMarks.Add(newMark);
                }
            }

            Vector2Int changeDirection = Vector2Int.zero;
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                changeDirection = Vector2Int.right;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                changeDirection = Vector2Int.up;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                changeDirection = Vector2Int.left;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                changeDirection = Vector2Int.down;
            }
            if (changeDirection != Vector2Int.zero) {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    ShrinkRoom(changeDirection);
                }
                else
                {
                    ExpandRoom(changeDirection);
                }
            }

            int exportState = 0;
            if (Input.GetKeyDown(KeyCode.X))
            {
                exportState = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                exportState = 2;
            }

            // EXPORTING
            if (exportState != 0)
            {
                if (DoorType(Vector2Int.right) == TileTerrain.None)
                {
                    Debug.LogError("East door is invalid");
                }
                else if (DoorType(Vector2Int.up) == TileTerrain.None)
                {
                    Debug.LogError("North door is invalid");
                }
                else if (DoorType(Vector2Int.left) == TileTerrain.None)
                {
                    Debug.LogError("West door is invalid");
                }
                else if (DoorType(Vector2Int.down) == TileTerrain.None)
                {
                    Debug.LogError("South door is invalid");
                }
                else {
                    TiledAreaMobGenerationPreset newMobGeneration;
                    if (exportState == 2 && this.importedBlueprint != null)
                    {
                        newMobGeneration = this.importedBlueprint.MobGenerationPresets[0]; // default to first MobGenerationPreset
                    }
                    else
                    {
                        newMobGeneration = ScriptableObject.CreateInstance<TiledAreaMobGenerationPreset>();
                    }
                    newMobGeneration.Setup();

                    TiledAreaBlueprint newBlueprint;
                    if (exportState == 2 && this.importedBlueprint != null)
                    {
                        newBlueprint = this.importedBlueprint;
                    }
                    else
                    {
                        newBlueprint = ScriptableObject.CreateInstance<TiledAreaBlueprint>();
                    }
                    newBlueprint.Setup(this.roomDimensions);

                    for (int i = 0; i < this.mainTiles.Count; i++)
                    {
                        newBlueprint.SetTerrainAtTile(IndexToPos(i), GetTileTerrain(i));
                        newBlueprint.SetTerrainTileIDAtTile(IndexToPos(i), GetTileID(i));
                    }

                    newBlueprint.WallTileID = this.wallId;

                    foreach (CartographerTile mainTile in this.mainTiles)
                    {
                        foreach (CartographerTile.TerrainContentInformation tileContentInformation in mainTile.TileContentInfos)
                        {
                            newBlueprint.SetTerrainContentIDAtTile(mainTile.Position, tileContentInformation.Height, tileContentInformation.ID);
                        }
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < wallTiles[i].Count; j++)
                        {
                            Vector2Int wallDirection;
                            if (i == 0)
                            {
                                wallDirection = Vector2Int.right;
                            }
                            else if (i == 1)
                            {
                                wallDirection = Vector2Int.up;
                            }
                            else if (i == 2)
                            {
                                wallDirection = Vector2Int.left;
                            }
                            else
                            {
                                wallDirection = Vector2Int.down;
                            }
                            newBlueprint.SetWallContentIDAtTile(wallDirection, j, this.wallTiles[i][j].WallContentInfo.ID);
                        }
                    }

                    newBlueprint.EntranceTerrains[0] = DoorType(Vector2Int.right);
                    newBlueprint.EntranceTerrains[1] = DoorType(Vector2Int.up);
                    newBlueprint.EntranceTerrains[2] = DoorType(Vector2Int.left);
                    newBlueprint.EntranceTerrains[3] = DoorType(Vector2Int.down);

                    for (int i = 0; i < this.mobMarks.Count; i++)
                    {
                        newMobGeneration.SpawnPointPresets.Add(this.mobMarks[i].SpawnPointPreset);
                    }

                    newBlueprint.MobGenerationPresets.Add(newMobGeneration);

                    if (!(exportState == 2 && this.importedBlueprint != null)) 
                    {
                        AssetDatabase.CreateAsset(
                            newBlueprint, 
                            FrigidPaths.ProjectFolder.ASSETS + 
                            FrigidPaths.ProjectFolder.SCRIPTABLE_OBJECTS + 
                            FrigidPaths.ProjectFolder.FIRST_DUNGEON_LEVEL +
                            FrigidPaths.ProjectFolder.TILES + 
                            FrigidPaths.ProjectFolder.BLUEPRINTS + 
                            "newBlueprint.asset"
                            );
                        AssetDatabase.CreateAsset(
                            newMobGeneration, 
                            FrigidPaths.ProjectFolder.ASSETS +
                            FrigidPaths.ProjectFolder.SCRIPTABLE_OBJECTS +
                            FrigidPaths.ProjectFolder.FIRST_DUNGEON_LEVEL +
                            FrigidPaths.ProjectFolder.TILES +
                            FrigidPaths.ProjectFolder.MOB_GENERATION +
                            FrigidPaths.ProjectFolder.PRESETS +
                            "newMobGenerationPreset.asset"
                            );
                    }

                    EditorUtility.SetDirty(newBlueprint);
                    EditorUtility.SetDirty(newMobGeneration);
                    AssetDatabase.SaveAssets();

                    if (exportState == 1)
                    {
                        Debug.Log("Exported!");
                    }
                    if (exportState == 2)
                    {
                        Debug.Log("Zaved!");
                    }
                }
            }
        }
    }
}
#endif