#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FrigidBlackwaters.Game
{
    public class CartographerTile : FrigidMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private SpriteRenderer highlightSpriteRenderer;
        [SerializeField]
        private Color32 select;
        [SerializeField]
        private Color32 notSelect;

        private TerrainTile water;
        private string waterId;
        private TerrainTile land;
        private string landId;
        private WallTile wall;

        private bool mouseDown;
        private bool mouseHover;
        private FrigidMonoBehaviour preview;

        private Vector2Int position;
        private int wallFloorIndex;
        private TileTerrain terrain;
        private List<TerrainContentInformation> terrainContentInfos = new List<TerrainContentInformation>();
        private WallContentInformation wallContentInfo;
        private CartographerTiledArea terrainArea;

        public TerrainTile Water
        {
            get
            {
                return this.water;
            }
            set
            {
                this.water = value;
            }
        }

        public string WaterID
        {
            get
            {
                return this.waterId;
            }
            set
            {
                this.waterId = value;
            }
        }

        public TerrainTile Land
        {
            get
            {
                return this.land;
            }
            set
            {
                this.land = value;
            }
        }

        public WallTile WallTile
        {
            get
            {
                return this.wall;
            }
            set
            {
                this.wall = value;
            }
        }

        public string LandID
        {
            get
            {
                return this.landId;
            }
            set
            {
                this.landId = value;
            }
        }

        public int WallFloorIndex
        {
            get
            {
                return this.wallFloorIndex;
            }
            set
            {
                this.wallFloorIndex = value;
            }
        }

        public Vector2Int Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        public TileTerrain Terrain
        {
            get
            {
                return this.terrain;
            }
        }

        public List<TerrainContentInformation> TileContentInfos
        {
            get
            {
                return this.terrainContentInfos;
            }
        }

        public WallContentInformation WallContentInfo
        {
            get
            {
                return this.wallContentInfo;
            }
        }

        public CartographerTiledArea TerrainArea
        {
            set
            {
                this.terrainArea = value;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.mouseHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.mouseHover = false;
        }

        private void PositionContent(TerrainContent content)
        {
            content.transform.position = this.transform.position + new Vector3(((float)content.Dimensions.x - 1) / -2, ((float)content.Dimensions.y - 1) / 2);
        }

        private void PositionContent(WallContent content)
        {
            content.transform.position = this.transform.position + new Vector3(((float)content.Width - 1) / -2, 0);
        }

        public void ReplaceContent(TerrainContentAsset selectedAsset, TerrainContentHeight selectedAssetHeight)
        {
            int previousContentIndex = -1;
            for (int i = 0; i < this.terrainContentInfos.Count; i++)
            {
                if (this.terrainContentInfos[i].Height == selectedAssetHeight)
                {
                    previousContentIndex = i;
                }
            }
            if (previousContentIndex != -1)
            {
                FrigidInstancing.DestroyInstance(this.terrainContentInfos[previousContentIndex].AssetContent);
                this.terrainContentInfos.RemoveAt(previousContentIndex);
            }
            TerrainContentInformation newInformation = new TerrainContentInformation();
            newInformation.Height = selectedAssetHeight;
            TerrainContent tileContentPrefab = selectedAsset.TerrainContentPrefabs[0];
            foreach (TiledLevel tiledLevel in tileContentPrefab.GetComponentsInChildren<TiledLevel>())
            {
                tiledLevel.enabled = false;
            }
            newInformation.AssetContent = FrigidInstancing.CreateInstance<TerrainContent>(tileContentPrefab, this.transform);
            foreach (TiledLevel tiledLevel in tileContentPrefab.GetComponentsInChildren<TiledLevel>())
            {
                tiledLevel.enabled = true;
            }
            foreach (Collider2D collider in newInformation.AssetContent.GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
            PositionContent(newInformation.AssetContent);
            newInformation.ID = selectedAsset.BlueprintID;
            this.terrainContentInfos.Add(newInformation);
        }

        public void ReplaceContent(WallContentAsset selectedAsset)
        {
            if (this.wallContentInfo.AssetContent != null)
            {
                FrigidInstancing.DestroyInstance(this.wallContentInfo.AssetContent);
            }
            WallContentInformation newInformation = new WallContentInformation();
            WallContent wallContentPrefab = selectedAsset.WallContentPrefabs[0];
            newInformation.AssetContent = FrigidInstancing.CreateInstance<WallContent>(wallContentPrefab, this.wall.transform);
            newInformation.AssetContent.transform.rotation = this.wall.transform.rotation;
            foreach (Collider2D collider in newInformation.AssetContent.GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
            PositionContent(newInformation.AssetContent);
            newInformation.ID = selectedAsset.BlueprintID;
            this.wallContentInfo = newInformation;
        }

        public void SetTerrain(TileTerrain terrainType)
        {
            this.terrain = terrainType;
            if (this.terrain == TileTerrain.Water)
            {
                this.water.gameObject.SetActive(true);
                this.land.gameObject.SetActive(false);
            }
            else if (this.terrain == TileTerrain.Land)
            {
                this.water.gameObject.SetActive(false);
                this.land.gameObject.SetActive(true);
            }
        }

        private void DeleteAllContent()
        {
            foreach (TerrainContentInformation information in this.terrainContentInfos)
            {
                FrigidInstancing.DestroyInstance(information.AssetContent);
            }
            if (this.wallContentInfo.AssetContent != null)
            {
                FrigidInstancing.DestroyInstance(this.wallContentInfo.AssetContent);
                this.wallContentInfo.ID = "";
            }
            this.terrainContentInfos.Clear();
        }

        private void SwapTile(bool forwardIteration) 
        {
            TerrainTileAsset newTile;
            if (this.terrain == TileTerrain.Water)
            {
                newTile = IterateTile(this.terrain, this.waterId, forwardIteration);
                FrigidInstancing.DestroyInstance(this.water);
                this.waterId = newTile.BlueprintID;
                this.water = FrigidInstancing.CreateInstance<TerrainTile>(newTile.TerrainTilePrefab, this.transform.position, this.transform);
            }
            else if (this.terrain == TileTerrain.Land)
            {
                newTile = IterateTile(this.terrain, this.landId, forwardIteration);
                FrigidInstancing.DestroyInstance(this.land);
                this.landId = newTile.BlueprintID;
                this.land = FrigidInstancing.CreateInstance<TerrainTile>(newTile.TerrainTilePrefab, this.transform.position, this.transform);
            }
        }

        private TerrainTileAsset IterateTile(TileTerrain terrain, string id, bool forwardIteration)
        {
            Dictionary<TileTerrain, List<TerrainTileAsset>> terrainTileAssetMap = new Dictionary<TileTerrain, List<TerrainTileAsset>>();
            foreach (TerrainTileAsset terrainTileAsset in this.terrainArea.TerrainAssetGroup.TerrainTileAssets)
            {
                if (!terrainTileAssetMap.ContainsKey(terrainTileAsset.Terrain))
                {
                    terrainTileAssetMap.Add(terrainTileAsset.Terrain, new List<TerrainTileAsset>());
                }
                terrainTileAssetMap[terrainTileAsset.Terrain].Add(terrainTileAsset);
            }

            int index = 0;
            foreach (TerrainTileAsset terrainTileAsset in terrainTileAssetMap[terrain])
            {
                if (terrainTileAsset.BlueprintID == id) break;
                index++;
            }
            int nextIndex = (index + this.terrainArea.TerrainAssetGroup.TerrainTileAssets.Count + (forwardIteration ? 1 : -1)) % this.terrainArea.TerrainAssetGroup.TerrainTileAssets.Count;
            return this.terrainArea.TerrainAssetGroup.TerrainTileAssets[nextIndex];
        }

        protected override void Update()
        {
            base.Update();
            // Water / Land
            if (this.mouseHover)
            {
                if (this.terrainArea.DecorationImporter.SelectedSlot != null)
                {
                    if (this.preview == null)
                    {
                        if (!this.terrainArea.DecorationImporter.SelectedSlot.ForWalls &&
                            this.terrainArea.DecorationImporter.SelectedSlot.TerrainContentAsset.Terrains.Contains(this.terrain))
                        {
                            TerrainContent tileContentPrefab = this.terrainArea.DecorationImporter.SelectedSlot.TerrainContentAsset.TerrainContentPrefabs[0];
                            foreach (TiledLevel tiledLevel in tileContentPrefab.GetComponentsInChildren<TiledLevel>())
                            {
                                tiledLevel.enabled = false;
                            }
                            TerrainContent terrainContentPreview = FrigidInstancing.CreateInstance<TerrainContent>(tileContentPrefab, this.transform);
                            this.preview = terrainContentPreview;
                            foreach (TiledLevel tiledLevel in tileContentPrefab.GetComponentsInChildren<TiledLevel>())
                            {
                                tiledLevel.enabled = true;
                            }
                            PositionContent(terrainContentPreview);
                            foreach (Collider2D collider in this.preview.GetComponentsInChildren<Collider2D>())
                            {
                                collider.enabled = false;
                            }
                            foreach (TiledLevel tiledLevel in this.preview.GetComponentsInChildren<TiledLevel>())
                            {
                                tiledLevel.enabled = false;
                            }
                            SpriteRenderer renderer = terrainContentPreview.GetComponentInChildren<SpriteRenderer>();
                            renderer.color = select;
                            renderer.sortingOrder = 99;
                        }
                        else if (this.terrain == TileTerrain.None &&
                                 this.terrainArea.DecorationImporter.SelectedSlot.ForWalls &&
                                 this.terrainArea.DecorationImporter.SelectedSlot.WallContentAsset.Terrains.Contains(this.terrainArea.GetTileTerrain(this.wallFloorIndex)))
                        {
                            WallContent wallContentPrefab = this.terrainArea.DecorationImporter.SelectedSlot.WallContentAsset.WallContentPrefabs[0];
                            WallContent wallContentPreview = FrigidInstancing.CreateInstance<WallContent>(wallContentPrefab, this.transform);
                            this.preview = wallContentPreview;
                            this.preview.transform.rotation = this.wall.transform.rotation;
                            PositionContent(wallContentPreview);
                            foreach (Collider2D collider in this.preview.GetComponentsInChildren<Collider2D>())
                            {
                                collider.enabled = false;
                            }
                            foreach (TiledLevel tiledLevel in this.preview.GetComponentsInChildren<TiledLevel>())
                            {
                                tiledLevel.enabled = false;
                            }
                            SpriteRenderer renderer = preview.GetComponentInChildren<SpriteRenderer>();
                            renderer.color = select;
                            renderer.sortingOrder = 99;
                        }
                    }
                }
                else
                {
                    this.highlightSpriteRenderer.color = this.select;
                }

                if (Input.GetMouseButton(0) && !this.mouseDown)
                {
                    this.mouseDown = true;
                    if (this.terrainArea.DecorationImporter.SelectedSlot != null)
                    {
                        if (!this.terrainArea.DecorationImporter.SelectedSlot.ForWalls &&
                            this.terrainArea.DecorationImporter.SelectedSlot.TerrainContentAsset.Terrains.Contains(this.terrain))
                        {
                            ReplaceContent(
                                this.terrainArea.DecorationImporter.SelectedSlot.TerrainContentAsset,
                                this.terrainArea.DecorationImporter.SelectedSlot.TerrainContentAsset.Height
                                );
                        }
                        else if (this.terrain == TileTerrain.None &&
                                 this.terrainArea.DecorationImporter.SelectedSlot.ForWalls &&
                                 this.terrainArea.DecorationImporter.SelectedSlot.WallContentAsset.Terrains.Contains(this.terrainArea.GetTileTerrain(this.wallFloorIndex)))
                        {
                            ReplaceContent(this.terrainArea.DecorationImporter.SelectedSlot.WallContentAsset);
                        }
                    }
                    else
                    {
                        DeleteAllContent();
                        if (this.terrain == TileTerrain.Land)
                        {
                            SetTerrain(TileTerrain.Water);
                        }
                        else if (this.terrain == TileTerrain.Water)
                        {
                            SetTerrain(TileTerrain.Land);
                        }
                    }
                }
                if (Input.GetMouseButton(1))
                {
                    DeleteAllContent();
                    this.mouseDown = false;
                }
                if (Input.GetKeyDown(KeyCode.LeftBracket))
                {
                    SwapTile(false);
                }
                else if (Input.GetKeyDown(KeyCode.RightBracket))
                {
                    SwapTile(true);
                }
            }
            else
            {
                this.highlightSpriteRenderer.color = this.notSelect;
                if (this.preview != null)
                {
                    FrigidInstancing.DestroyInstance(this.preview);
                }
                this.mouseDown = false;
            }
        }

        public struct WallContentInformation
        {
            private WallContent assetContent;
            private string id;

            public WallContent AssetContent
            {
                get
                {
                    return this.assetContent;
                }
                set
                {
                    this.assetContent = value;
                }
            }

            public string ID
            {
                get
                {
                    return this.id;
                }
                set
                {
                    this.id = value;
                }
            }
        }

        public struct TerrainContentInformation
        {
            private TerrainContentHeight height;
            private TerrainContent assetContent;
            private string id;

            public TerrainContentHeight Height
            {
                get
                {
                    return this.height;
                }
                set
                {
                    this.height = value;
                }
            }

            public TerrainContent AssetContent
            {
                get
                {
                    return this.assetContent;
                }
                set
                {
                    this.assetContent = value;
                }
            }

            public string ID
            {
                get
                {
                    return this.id;
                }
                set
                {
                    this.id = value;
                }
            }
        }
    }
}
#endif
