#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class CartographerTool : FrigidEditorWindow
    {
        [SerializeField]
        private CartographerToolConfig config;

        [SerializeField]
        private bool startupEmpty;
        [SerializeField]
        private TiledArea startupAreaPrefab;
        [SerializeField]
        private Vector2Int startupDimensions;
        [SerializeField]
        private TerrainTileAsset startupTerrainTileAsset;
        [SerializeField]
        private WallTileAsset startupWallTileAsset;
        [SerializeField]
        private TiledAreaBlueprint startupBlueprint;

        [SerializeField]
        private TiledAreaBlueprint editBlueprint;

        [SerializeField]
        private float blueprintPreviewTileDrawLength;
        [SerializeField]
        private Vector2 blueprintPreviewOffset;

        [SerializeField]
        private int selectedAssetTypeIndex;
        [SerializeField]
        private TerrainTileAsset[] availableTerrainTileAssets;
        [SerializeField]
        private int selectedTerrainTileAssetIndex;
        [SerializeField]
        private string terrainTileSearchFilter;
        [SerializeField]
        private WallTileAsset[] availableWallTileAssets;
        [SerializeField]
        private int selectedWallTileAssetIndex;
        [SerializeField]
        private string wallTileSearchFilter;
        [SerializeField]
        private TerrainContentAsset[] availableTerrainContentAssets;
        [SerializeField]
        private int selectedTerrainContentAssetIndex;
        [SerializeField]
        private string terrainContentSearchFilter;
        [SerializeField]
        private WallContentAsset[] availableWallContentAssets;
        [SerializeField]
        private int selectedWallContentAssetIndex;
        [SerializeField]
        private string wallContentSearchFilter;
        [SerializeField]
        private TiledEntranceAsset[] availableEntranceAssets;
        [SerializeField]
        private int selectedEntranceAssetIndex;
        [SerializeField]
        private string entranceSearchFilter;
        [SerializeField]
        private int selectedEntranceWidth;
        [SerializeField]
        private Vector2 assetBrowserScrollPos;

        [SerializeField]
        private int sideMenuIndex;
        [SerializeField]
        private Vector2 sideMenuScrollPos;
        [SerializeField]
        private int chosenSpawnerIndex;
        [SerializeField]
        private int chosenSpawnPointIndex;

        [SerializeField]
        private Vector2 pickedOrientationDirection;

        private Dictionary<TerrainTileAsset, Texture2D> terrainTileTextures;
        private Dictionary<(WallTileAsset, Vector2Int, bool), Texture2D> wallTileTextures;
        private Dictionary<(TerrainContentAsset, TileTerrain, int), (Texture2D, Vector2)> terrainContentTextures;
        private Dictionary<(WallContentAsset, TileTerrain, Vector2Int, int), (Texture2D, Vector2)> wallContentTextures;
        private Dictionary<(TiledEntranceAsset, TileTerrain, Vector2Int), (Texture2D, Vector2)> entranceTextures;

        protected override string Title
        {
            get
            {
                return "Cartographer";
            }
        }

        protected override void Opened()
        {
            base.Opened();
            if (!AssetDatabaseUpdater.TryFindAsset<CartographerToolConfig>(out this.config))
            {
                Debug.LogError("Could not find a CartographerToolConfig asset!");
                return;
            }

            this.startupEmpty = false;
            this.startupTerrainTileAsset = null;
            this.startupWallTileAsset = null;
            this.startupBlueprint = null;
            this.startupDimensions = Vector2Int.zero;

            this.editBlueprint = null;

            this.terrainTileSearchFilter = string.Empty;
            this.wallTileSearchFilter = string.Empty;
            this.terrainContentSearchFilter = string.Empty;
            this.wallContentSearchFilter = string.Empty;
            this.entranceSearchFilter = string.Empty;

            this.RefreshAvailableAssets();
        }

        protected override void Draw()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if (this.editBlueprint == null)
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        this.startupEmpty = GUILayout.Toggle(this.startupEmpty, "Create Empty Blueprint");
                    }
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        if (this.startupEmpty)
                        {
                            EditorGUILayout.LabelField("Empty Blueprint Customization", UtilityStyles.WordWrapAndCenter(EditorStyles.largeLabel));
                            this.startupAreaPrefab = (TiledArea)EditorGUILayout.ObjectField("Area Prefab", this.startupAreaPrefab, typeof(TiledArea), false);
                            this.startupDimensions = EditorGUILayout.Vector2IntField("Main Area Dimensions", this.startupDimensions);
                            this.startupTerrainTileAsset = (TerrainTileAsset)EditorGUILayout.ObjectField("Terrain Tile Asset", this.startupTerrainTileAsset, typeof(TerrainTileAsset), false);
                            this.startupWallTileAsset = (WallTileAsset)EditorGUILayout.ObjectField("Wall Tile Asset", this.startupWallTileAsset, typeof(WallTileAsset), false);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Existing Blueprint Customization", UtilityStyles.WordWrapAndCenter(EditorStyles.largeLabel));
                            this.startupBlueprint = (TiledAreaBlueprint)EditorGUILayout.ObjectField("Blueprint", this.startupBlueprint, typeof(TiledAreaBlueprint), false);
                        }
                        EditorGUILayout.Space();
                    }

                    bool hasInvalidStartupParameters =
                        this.startupEmpty && (this.startupAreaPrefab == null || this.startupDimensions.x <= 0 || this.startupDimensions.y <= 0 || this.startupTerrainTileAsset == null || this.startupWallTileAsset == null) ||
                        !this.startupEmpty && this.startupBlueprint == null;
                    using (new EditorGUI.DisabledScope(hasInvalidStartupParameters))
                    {
                        if (GUILayout.Button("Enter Cartographer", GUILayout.Height(UtilityGUIUtility.LargeLineHeight)))
                        {
                            if (this.startupEmpty)
                            {
                                string assetPath = FileUtility.AssetsRelativePath(EditorUtility.SaveFilePanel("Pick Empty Blueprint Path", FrigidPaths.ProjectFolder.Assets, "New_Blueprint", "asset"));
                                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                                this.editBlueprint = TiledAreaBlueprint.CreateEmpty(assetPath, this.startupAreaPrefab, this.startupDimensions, this.startupTerrainTileAsset, this.startupWallTileAsset);
                            }
                            else
                            {
                                this.editBlueprint = this.startupBlueprint;
                            }
                        }
                    }
                    return;
                }
            }

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                EditorGUILayout.LabelField(this.editBlueprint.name, EditorStyles.boldLabel);
                this.editBlueprint.HasVisibleWalls = EditorGUILayout.Toggle("Has Visible Walls", this.editBlueprint.HasVisibleWalls, GUILayout.MaxWidth(170));
                if (GUILayout.Button("Return to Launch", EditorStyles.toolbarButton))
                {
                    this.editBlueprint = null;
                    return;
                }
                if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                {
                    AssetDatabase.SaveAssets();
                }
            }

            this.DrawBlueprintPreview();
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                this.DrawAssetBrowser();
                this.DrawSideMenu();
            }
        }

        [MenuItem(FrigidPaths.MenuItem.Window + "Cartographer")]
        private static void ShowCartographerTool()
        {
            Show<CartographerTool>();
        }

        private void DrawBlueprintPreview()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                this.blueprintPreviewTileDrawLength = EditorGUILayout.Slider("Tile Draw Length", this.blueprintPreviewTileDrawLength, this.config.MinTileDrawLength, this.config.MaxTileDrawLength);
                if (GUILayout.Button("Reset Preview Offset", EditorStyles.toolbarButton)) this.blueprintPreviewOffset = Vector2.zero;
            }

            float previewWidth = EditorGUIUtility.currentViewWidth;
            float previewHeight = previewWidth / this.config.BlueprintPreviewAspectRatio;
            using (new EditorGUILayout.VerticalScope())
            {
                Rect previewRect = GUILayoutUtility.GetRect(previewWidth, previewHeight, GUILayout.Width(previewWidth), GUILayout.Height(previewHeight));
                using (new UtilityGUI.ColorScope(UtilityGUIUtility.Darken(Color.grey)))
                {
                    GUI.Box(previewRect, "");
                }

                using (new GUI.GroupScope(previewRect))
                {
                    Vector2 tileDrawSize = new Vector2(this.blueprintPreviewTileDrawLength, this.blueprintPreviewTileDrawLength);
                    Vector2 areaPreviewCenter = previewRect.size / 2 + this.blueprintPreviewOffset;

                    if (previewRect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
                        {
                            this.blueprintPreviewOffset += Event.current.delta;
                        }
                        else if (Event.current.type == EventType.ScrollWheel)
                        {
                            this.blueprintPreviewTileDrawLength += -Event.current.delta.y;
                        }
                    }

                    bool ignoreAssetPlacement = this.sideMenuIndex == 2;
                    Vector2Int[] wallIndexDirections = WallTiling.GetAllWallIndexDirections();

                    // Draw Wall Tiles
                    if (this.editBlueprint.HasVisibleWalls)
                    {
                        for (int i = 0; i < wallIndexDirections.Length; i++)
                        {
                            Vector2Int currWallIndexDirection = wallIndexDirections[i];
                            Vector2Int nextWallIndexDirection = wallIndexDirections[(i + 1) % wallIndexDirections.Length];
                            for (int tileIndex = 0; tileIndex < WallTiling.GetEdgeLength(currWallIndexDirection, this.editBlueprint.MainAreaDimensions); tileIndex++)
                            {
                                Vector2 edgeLocalPosition = WallTiling.EdgeTileLocalPositionFromWallIndexDirectionAndTileIndex(currWallIndexDirection, tileIndex, this.editBlueprint.MainAreaDimensions);
                                edgeLocalPosition *= tileDrawSize * new Vector2(1, -1);
                                Rect edgeTileRect = new Rect(areaPreviewCenter + edgeLocalPosition - tileDrawSize / 2, tileDrawSize);

                                WallTileAsset wallTileAsset = this.editBlueprint.GetWallTileAssetAt(currWallIndexDirection, tileIndex);
                                Color tintColor = Color.white;
                                if (this.selectedAssetTypeIndex == 1 && edgeTileRect.Contains(Event.current.mousePosition) && previewRect.Contains(Event.current.mousePosition) &&
                                    this.selectedWallTileAssetIndex >= 0 && this.selectedWallTileAssetIndex < this.availableWallTileAssets.Length && !ignoreAssetPlacement)
                                {
                                    wallTileAsset = this.availableWallTileAssets[this.selectedWallTileAssetIndex];
                                    tintColor = this.config.SelectedAssetTintColor;

                                    if (!WallTiling.EdgeTileIndexWithinBounds(currWallIndexDirection, tileIndex, this.editBlueprint.MainAreaDimensions))
                                    {
                                        continue;
                                    }

                                    if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) && Event.current.button == 0)
                                    {
                                        this.editBlueprint.SetWallTileAssetAt(currWallIndexDirection, tileIndex, wallTileAsset);
                                    }
                                }

                                this.GetWallTileTexture(wallTileAsset, currWallIndexDirection, true, out Texture2D edgeTexture);
                                using (new UtilityGUI.ColorScope(tintColor))
                                {
                                    GUI.DrawTexture(edgeTileRect, edgeTexture);
                                }
                            }
                            Vector2 cornerLocalPosition = WallTiling.CornerTileLocalPositionFromWallIndexDirections(currWallIndexDirection, nextWallIndexDirection, this.editBlueprint.MainAreaDimensions);
                            cornerLocalPosition *= tileDrawSize * new Vector2(1, -1);
                            Rect cornerTileRect = new Rect(areaPreviewCenter + cornerLocalPosition - tileDrawSize / 2, tileDrawSize);
                            this.GetWallTileTexture(this.editBlueprint.GetWallTileAssetAt(currWallIndexDirection, 0), currWallIndexDirection, false, out Texture2D cornerTexture);
                            GUI.DrawTexture(cornerTileRect, cornerTexture);
                        }
                    }

                    // Draw Terrain Tiles
                    for (int y = 0; y < this.editBlueprint.MainAreaDimensions.y; y++)
                    {
                        for (int x = 0; x < this.editBlueprint.MainAreaDimensions.x; x++)
                        {
                            Vector2Int tileIndexPosition = new Vector2Int(x, y);
                            Vector2 tileLocalPosition = AreaTiling.TileLocalPositionFromIndexPosition(tileIndexPosition, this.editBlueprint.MainAreaDimensions);
                            tileLocalPosition *= tileDrawSize * new Vector2(1, -1);
                            Rect tileRect = new Rect(areaPreviewCenter + tileLocalPosition - tileDrawSize / 2, tileDrawSize);

                            TerrainTileAsset terrainTileAsset = this.editBlueprint.GetTerrainTileAssetAt(tileIndexPosition);
                            Color tintColor = Color.white;
                            if (this.selectedAssetTypeIndex == 0 && tileRect.Contains(Event.current.mousePosition) && previewRect.Contains(Event.current.mousePosition) &&
                                this.selectedTerrainTileAssetIndex >= 0 && this.selectedTerrainTileAssetIndex < this.availableTerrainTileAssets.Length && !ignoreAssetPlacement)
                            {
                                terrainTileAsset = this.availableTerrainTileAssets[this.selectedTerrainTileAssetIndex];
                                tintColor = this.config.SelectedAssetTintColor;

                                if (!AreaTiling.TileIndexPositionWithinBounds(tileIndexPosition, this.editBlueprint.MainAreaDimensions))
                                {
                                    continue;
                                }

                                if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) && Event.current.button == 0)
                                {
                                    this.editBlueprint.SetTerrainTileAssetAt(tileIndexPosition, terrainTileAsset);
                                }
                            }

                            this.GetTerrainTileTexture(terrainTileAsset, out Texture2D tileTexture);
                            using (new UtilityGUI.ColorScope(tintColor))
                            {
                                GUI.DrawTexture(tileRect, tileTexture);
                            }
                        }
                    }

                    // Draw Entrances
                    for (int i = 0; i < wallIndexDirections.Length; i++)
                    {
                        Vector2Int wallIndexDirection = wallIndexDirections[i];

                        void DrawEntranceTexture(TiledEntranceAsset entranceAssetToDraw, int tileIndex, int width)
                        {
                            TileTerrain terrain = this.editBlueprint.GetTerrainTileAssetAt(WallTiling.WallIndexDirectionAndTileIndexToInnerTileIndexPosition(wallIndexDirection, tileIndex, this.editBlueprint.MainAreaDimensions)).Terrain;
                            this.GetEntranceTexture(entranceAssetToDraw, terrain, wallIndexDirection, out Texture2D entranceTexture, out Vector2 entrancePivot);

                            if (entranceTexture == null) return;

                            Vector2 contentLocalPosition = WallTiling.EdgeExtentLocalPositionFromWallIndexDirectionAndExtentIndex(wallIndexDirection, tileIndex, this.editBlueprint.MainAreaDimensions, width);
                            contentLocalPosition *= tileDrawSize * new Vector2(1, -1);
                            contentLocalPosition += entrancePivot * tileDrawSize / FrigidConstants.PixelsPerUnit;
                            Vector2 contentSize = new Vector2(entranceTexture.width, entranceTexture.height) * tileDrawSize / FrigidConstants.PixelsPerUnit;
                            Rect contentRect = new Rect(areaPreviewCenter + contentLocalPosition - contentSize / 2, contentSize);

                            GUI.DrawTexture(contentRect, entranceTexture);
                        }

                        if (this.editBlueprint.TryGetWallEntranceAssetAndIndexAndWidth(wallIndexDirection, out TiledEntranceAsset existingEntranceAsset, out int existingEntranceTileIndex, out int existingEntranceWidth))
                        {
                            DrawEntranceTexture(existingEntranceAsset, existingEntranceTileIndex, existingEntranceWidth);
                        }

                        for (int tileIndex = 0; tileIndex < WallTiling.GetEdgeLength(wallIndexDirection, this.editBlueprint.MainAreaDimensions); tileIndex++)
                        {
                            Vector2 edgeLocalPosition = WallTiling.EdgeTileLocalPositionFromWallIndexDirectionAndTileIndex(wallIndexDirection, tileIndex, this.editBlueprint.MainAreaDimensions);
                            edgeLocalPosition *= tileDrawSize * new Vector2(1, -1);
                            Rect edgeTileRect = new Rect(areaPreviewCenter + edgeLocalPosition - tileDrawSize / 2, tileDrawSize);

                            if (this.selectedAssetTypeIndex == 4 && edgeTileRect.Contains(Event.current.mousePosition) && previewRect.Contains(Event.current.mousePosition) &&
                                this.selectedEntranceAssetIndex >= 0 && this.selectedEntranceAssetIndex < this.availableEntranceAssets.Length && !ignoreAssetPlacement)
                            {
                                TiledEntranceAsset selectedEntranceAsset = this.availableEntranceAssets[this.selectedEntranceAssetIndex];

                                if (this.selectedEntranceWidth < selectedEntranceAsset.MinWidth || this.selectedEntranceWidth > selectedEntranceAsset.MaxWidth ||
                                    !WallTiling.EdgeExtentIndexWithinBounds(wallIndexDirection, tileIndex, this.editBlueprint.MainAreaDimensions, this.selectedEntranceWidth)) continue;

                                DrawEntranceTexture(selectedEntranceAsset, tileIndex, this.selectedEntranceWidth);

                                if (Event.current.type == EventType.MouseDown)
                                {
                                    if (Event.current.button == 0)
                                    {
                                        this.editBlueprint.SetWallEntranceAssetAndIndexAndWidth(wallIndexDirection, selectedEntranceAsset, tileIndex, this.selectedEntranceWidth);
                                    }
                                    else if (Event.current.button == 1 && tileIndex == existingEntranceTileIndex)
                                    {
                                        this.editBlueprint.ClearWallEntranceAsset(wallIndexDirection);
                                    }
                                }
                            }
                        }
                    }

                    // Draw Wall Content
                    if (this.editBlueprint.HasVisibleWalls)
                    {
                        for (int i = 0; i < wallIndexDirections.Length; i++)
                        {
                            Vector2Int wallIndexDirection = wallIndexDirections[i];
                            for (int tileIndex = 0; tileIndex < WallTiling.GetEdgeLength(wallIndexDirection, this.editBlueprint.MainAreaDimensions); tileIndex++)
                            {
                                Vector2 edgeLocalPosition = WallTiling.EdgeTileLocalPositionFromWallIndexDirectionAndTileIndex(wallIndexDirection, tileIndex, this.editBlueprint.MainAreaDimensions);
                                edgeLocalPosition *= tileDrawSize * new Vector2(1, -1);
                                Rect edgeTileRect = new Rect(areaPreviewCenter + edgeLocalPosition - tileDrawSize / 2, tileDrawSize);

                                void DrawWallContentTexture(WallContentAsset wallContentAssetToDraw, Vector2 orientationDirectionToDraw, Color tintColor)
                                {
                                    TileTerrain terrain = this.editBlueprint.GetTerrainTileAssetAt(WallTiling.WallIndexDirectionAndTileIndexToInnerTileIndexPosition(wallIndexDirection, tileIndex, this.editBlueprint.MainAreaDimensions)).Terrain;
                                    this.GetWallContentTexture(wallContentAssetToDraw, terrain, wallIndexDirection, orientationDirectionToDraw, out Texture2D contentTexture, out Vector2 contentPivot);

                                    if (contentTexture == null) return;

                                    Vector2 contentLocalPosition = WallTiling.EdgeExtentLocalPositionFromWallIndexDirectionAndExtentIndex(wallIndexDirection, tileIndex, this.editBlueprint.MainAreaDimensions, wallContentAssetToDraw.GetWidth(orientationDirectionToDraw));
                                    contentLocalPosition *= tileDrawSize * new Vector2(1, -1);
                                    contentLocalPosition += contentPivot * tileDrawSize / FrigidConstants.PixelsPerUnit;
                                    Vector2 contentSize = new Vector2(contentTexture.width, contentTexture.height) * tileDrawSize / FrigidConstants.PixelsPerUnit;
                                    Rect contentRect = new Rect(areaPreviewCenter + contentLocalPosition - contentSize / 2, contentSize);
                                    using (new UtilityGUI.ColorScope(tintColor))
                                    {
                                        GUI.DrawTexture(contentRect, contentTexture);
                                    }
                                }

                                if (this.editBlueprint.TryGetWallContentAssetAndOrientationAt(wallIndexDirection, tileIndex, out WallContentAsset existingWallContentAsset, out Vector2 existingOrientationDirection))
                                {
                                    DrawWallContentTexture(existingWallContentAsset, existingOrientationDirection, Color.white);
                                }

                                if (this.selectedAssetTypeIndex == 3 && edgeTileRect.Contains(Event.current.mousePosition) && previewRect.Contains(Event.current.mousePosition) &&
                                    this.selectedWallContentAssetIndex >= 0 && this.selectedWallContentAssetIndex < this.availableWallContentAssets.Length && !ignoreAssetPlacement)
                                {
                                    WallContentAsset selectedWallContentAsset = this.availableWallContentAssets[this.selectedWallContentAssetIndex];

                                    if (!WallTiling.EdgeExtentIndexWithinBounds(wallIndexDirection, tileIndex, this.editBlueprint.MainAreaDimensions, selectedWallContentAsset.GetWidth(this.pickedOrientationDirection)))
                                    {
                                        continue;
                                    }

                                    DrawWallContentTexture(selectedWallContentAsset, this.pickedOrientationDirection, this.config.SelectedAssetTintColor);

                                    if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)
                                    {
                                        if (Event.current.button == 0)
                                        {
                                            this.editBlueprint.SetWallContentAssetAndOrientationAt(wallIndexDirection, tileIndex, selectedWallContentAsset, this.pickedOrientationDirection);
                                        }
                                        else if (Event.current.button == 1)
                                        {
                                            this.editBlueprint.ClearWallContentAssetAt(wallIndexDirection, tileIndex);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Draw Terrain Content
                    for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                    {
                        TerrainContentHeight contentHeight = (TerrainContentHeight)i;
                        for (int y = 0; y < this.editBlueprint.MainAreaDimensions.y; y++)
                        {
                            for (int x = 0; x < this.editBlueprint.MainAreaDimensions.x; x++)
                            {
                                Vector2Int tileIndexPosition = new Vector2Int(x, y);
                                Vector2 tileLocalPosition = AreaTiling.TileLocalPositionFromIndexPosition(tileIndexPosition, this.editBlueprint.MainAreaDimensions);
                                tileLocalPosition *= tileDrawSize * new Vector2(1, -1);
                                Rect tileRect = new Rect(areaPreviewCenter + tileLocalPosition - tileDrawSize / 2, tileDrawSize);

                                void DrawTerrainContentTexture(TerrainContentAsset terrainContentAssetToDraw, Vector2 orientationDirectionToDraw, Color tintColor)
                                {
                                    TileTerrain terrain = this.editBlueprint.GetTerrainTileAssetAt(tileIndexPosition).Terrain;
                                    this.GetTerrainContentTexture(terrainContentAssetToDraw, terrain, orientationDirectionToDraw, out Texture2D contentTexture, out Vector2 contentPivot);

                                    if (contentTexture == null) return;

                                    Vector2 contentLocalPosition = AreaTiling.RectLocalPositionFromIndexPosition(tileIndexPosition, this.editBlueprint.MainAreaDimensions, terrainContentAssetToDraw.GetDimensions(orientationDirectionToDraw));
                                    contentLocalPosition *= tileDrawSize * new Vector2(1, -1);
                                    contentLocalPosition += contentPivot * tileDrawSize / FrigidConstants.PixelsPerUnit;
                                    Vector2 contentSize = new Vector2(contentTexture.width, contentTexture.height) * tileDrawSize / FrigidConstants.PixelsPerUnit;
                                    Rect contentRect = new Rect(areaPreviewCenter + contentLocalPosition - contentSize / 2, contentSize);
                                    using (new UtilityGUI.ColorScope(tintColor))
                                    {
                                        GUI.DrawTexture(contentRect, contentTexture);
                                    }
                                }

                                if (this.editBlueprint.TryGetTerrainContentAssetAndOrientationAt(contentHeight, tileIndexPosition, out TerrainContentAsset existingTerrainContentAsset, out Vector2 existingOrientationDirection))
                                {
                                    DrawTerrainContentTexture(existingTerrainContentAsset, existingOrientationDirection, Color.white);
                                }

                                if (this.selectedAssetTypeIndex == 2 && tileRect.Contains(Event.current.mousePosition) && previewRect.Contains(Event.current.mousePosition) &&
                                    this.selectedTerrainContentAssetIndex >= 0 && this.selectedTerrainContentAssetIndex < this.availableTerrainContentAssets.Length &&
                                    this.availableTerrainContentAssets[this.selectedTerrainContentAssetIndex].Height == contentHeight && !ignoreAssetPlacement)
                                {
                                    TerrainContentAsset selectedTerrainContentAsset = this.availableTerrainContentAssets[this.selectedTerrainContentAssetIndex];

                                    if (!AreaTiling.RectIndexPositionWithinBounds(tileIndexPosition, this.editBlueprint.MainAreaDimensions, selectedTerrainContentAsset.GetDimensions(this.pickedOrientationDirection)))
                                    {
                                        continue;
                                    }

                                    DrawTerrainContentTexture(selectedTerrainContentAsset, this.pickedOrientationDirection, this.config.SelectedAssetTintColor);

                                    if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)
                                    {
                                        if (Event.current.button == 0)
                                        {
                                            this.editBlueprint.SetTerrainContentAssetAndOrientationAt(tileIndexPosition, selectedTerrainContentAsset, this.pickedOrientationDirection);
                                        }
                                        else if (Event.current.button == 1)
                                        {
                                            this.editBlueprint.ClearTerrainContentAssetAt(contentHeight, tileIndexPosition);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Vector2Int gridDimensions = this.editBlueprint.MainAreaDimensions + Vector2Int.one * 2;
                    for (int x = 0; x < gridDimensions.x; x++)
                    {
                        for (int y = 0; y < gridDimensions.y; y++)
                        {
                            Vector2 gridCellLocalPosition = AreaTiling.TileLocalPositionFromIndexPosition(new Vector2Int(x, y), gridDimensions);
                            gridCellLocalPosition *= tileDrawSize * new Vector2(1, -1);
                            Rect cellRect = new Rect(areaPreviewCenter + gridCellLocalPosition - tileDrawSize / 2, tileDrawSize);
                            using (new UtilityGUI.ColorScope(this.config.GridLineColor))
                            {
                                UtilityGUI.DrawLineBox(cellRect);
                            }
                            if (cellRect.Contains(Event.current.mousePosition) && previewRect.Contains(Event.current.mousePosition) && !ignoreAssetPlacement)
                            {
                                using (new UtilityGUI.ColorScope(this.config.HighlightLineColor))
                                {
                                    UtilityGUI.DrawOutlineBox(cellRect, 2);
                                }
                            }
                        }
                    }

                    // Draw Mob Spawn Points
                    if (this.sideMenuIndex == 2)
                    {
                        if (this.chosenSpawnerIndex >= 0 && this.chosenSpawnerIndex < this.editBlueprint.GetNumberMobSpawners()) 
                        {
                            bool existingHovered = false;
                            for (int spawnPointIndex = this.editBlueprint.GetNumberMobSpawnPoints(this.chosenSpawnerIndex) - 1; spawnPointIndex >= 0; spawnPointIndex--)
                            {
                                Vector2 spawnPointLocalPosition = this.editBlueprint.GetMobSpawnPoint(this.chosenSpawnerIndex, spawnPointIndex).LocalPosition;
                                spawnPointLocalPosition *= tileDrawSize * new Vector2(1, -1);
                                Rect spawnPointRect = new Rect(areaPreviewCenter + spawnPointLocalPosition - tileDrawSize / 2, tileDrawSize);
                                Color tintColor = Color.grey;
                                if (!existingHovered && spawnPointRect.Contains(Event.current.mousePosition) && previewRect.Contains(Event.current.mousePosition))
                                {
                                    existingHovered = true;
                                    tintColor = Color.white;
                                    if (Event.current.type == EventType.MouseDown)
                                    {
                                        if (Event.current.button == 0)
                                        {
                                            this.chosenSpawnPointIndex = spawnPointIndex;
                                        }
                                        else if (Event.current.button == 1)
                                        {
                                            this.editBlueprint.RemoveMobSpawnPoint(this.chosenSpawnerIndex, spawnPointIndex);
                                            continue;
                                        }
                                    }
                                }
                                else if (spawnPointIndex == this.chosenSpawnPointIndex)
                                {
                                    tintColor = Color.white;
                                }

                                using (new UtilityGUI.ColorScope(tintColor))
                                {
                                    GUI.DrawTexture(spawnPointRect, this.config.MobSpawnPointMarkTexture);
                                }
                            }

                            Vector2 mouseSnapPosition = Event.current.mousePosition;
                            if (Event.current.shift)
                            {
                                Vector2 offset = Event.current.mousePosition - areaPreviewCenter;
                                Vector2 interval = offset / (tileDrawSize / 2);
                                Vector2Int roundedInterval = new Vector2Int(Mathf.RoundToInt(interval.x), Mathf.RoundToInt(interval.y));
                                mouseSnapPosition = areaPreviewCenter + roundedInterval * (tileDrawSize / 2);
                            }
                            Vector2 spawnPointLocalPlacePosition = (mouseSnapPosition - areaPreviewCenter) * new Vector2(1, -1) / tileDrawSize;

                            if (previewRect.Contains(mouseSnapPosition))
                            {
                                if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 &&
                                    this.chosenSpawnPointIndex >= 0 && this.chosenSpawnPointIndex < this.editBlueprint.GetNumberMobSpawnPoints(this.chosenSpawnerIndex))
                                {
                                    TiledAreaMobSpawnPoint oldSpawnPoint = this.editBlueprint.GetMobSpawnPoint(this.chosenSpawnerIndex, this.chosenSpawnPointIndex);
                                    this.editBlueprint.SetMobSpawnPoint(
                                        this.chosenSpawnerIndex,
                                        this.chosenSpawnPointIndex,
                                        new TiledAreaMobSpawnPoint(spawnPointLocalPlacePosition, oldSpawnPoint.IsExclusive, oldSpawnPoint.FilteredMobSpawnables, oldSpawnPoint.FilteredMobSpawnTags, oldSpawnPoint.FilteredTierSpans)
                                        );
                                }

                                if (!existingHovered)
                                {
                                    GUI.DrawTexture(new Rect(mouseSnapPosition - tileDrawSize / 2, tileDrawSize), this.config.MobSpawnPointMarkTexture);
                                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                                    {
                                        int newSpawnPointIndex = this.editBlueprint.GetNumberMobSpawnPoints(this.chosenSpawnerIndex);
                                        this.editBlueprint.AddMobSpawnPoint(this.chosenSpawnerIndex, newSpawnPointIndex);
                                        this.editBlueprint.SetMobSpawnPoint(this.chosenSpawnerIndex, newSpawnPointIndex, new TiledAreaMobSpawnPoint(spawnPointLocalPlacePosition));
                                        this.chosenSpawnPointIndex = newSpawnPointIndex;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshAvailableAssets()
        {
            this.availableTerrainTileAssets = AssetDatabaseUpdater.FindAssets<TerrainTileAsset>();
            this.availableWallTileAssets = AssetDatabaseUpdater.FindAssets<WallTileAsset>();
            this.availableTerrainContentAssets = AssetDatabaseUpdater.FindAssets<TerrainContentAsset>();
            this.availableWallContentAssets = AssetDatabaseUpdater.FindAssets<WallContentAsset>();
            this.availableEntranceAssets = AssetDatabaseUpdater.FindAssets<TiledEntranceAsset>();
        }

        private void DrawAssetBrowser()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    if (GUILayout.Button("Refresh Available Assets", EditorStyles.toolbarButton))
                    {
                        this.RefreshAvailableAssets();
                    }
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    this.selectedAssetTypeIndex = GUILayout.Toolbar(this.selectedAssetTypeIndex, new string[] { "Terrain Tiles", "Wall Tiles", "Terrain Content", "Wall Content", "Entrances" }, EditorStyles.toolbarButton);
                }

                void DrawAssets<T>(T[] assets, ref int selected, ref string searchFilter, Action onCustomDraw = null) where T : FrigidScriptableObject
                {
                    selected = Mathf.Clamp(selected, 0, assets.Length - 1);
                    using (new EditorGUILayout.VerticalScope())
                    {
                        searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField);

                        onCustomDraw?.Invoke();

                        for (int assetIndex = 0; assetIndex < assets.Length; assetIndex++)
                        {
                            T asset = assets[assetIndex];
                            if (asset.name.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) < 0)
                            {
                                continue;
                            }
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                Rect iconRect = GUILayoutUtility.GetRect(EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, GUILayout.Width(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.singleLineHeight));
                                EditorGUILayout.LabelField(asset.name, EditorStyles.helpBox);
                                Rect labelRect = GUILayoutUtility.GetLastRect();
                                Rect totalRect = Rect.MinMaxRect(Mathf.Min(iconRect.xMin, labelRect.xMin), Mathf.Min(iconRect.yMin, labelRect.yMin), Mathf.Max(iconRect.xMax, labelRect.xMax), Mathf.Max(iconRect.yMax, labelRect.yMax));
                                if (GUI.Button(totalRect, "", UtilityStyles.EmptyStyle)) selected = assetIndex;
                                using (new UtilityGUI.ColorScope(UtilityGUIUtility.Darken(Color.white, assetIndex == selected || totalRect.Contains(Event.current.mousePosition) ? 0 : 1)))
                                {
                                    GUI.DrawTexture(iconRect, AssetPreview.GetMiniThumbnail(asset));
                                }
                            }
                            EditorGUILayout.Space();
                        }
                    }
                }

                using (EditorGUILayout.ScrollViewScope assetScrollViewScope = new EditorGUILayout.ScrollViewScope(this.assetBrowserScrollPos))
                {
                    this.assetBrowserScrollPos = assetScrollViewScope.scrollPosition;
                    using (new EditorGUILayout.VerticalScope())
                    {
                        switch (this.selectedAssetTypeIndex)
                        {
                            case 0:
                                DrawAssets<TerrainTileAsset>(this.availableTerrainTileAssets, ref this.selectedTerrainTileAssetIndex, ref this.terrainTileSearchFilter);
                                break;
                            case 1:
                                DrawAssets<WallTileAsset>(this.availableWallTileAssets, ref this.selectedWallTileAssetIndex, ref this.wallTileSearchFilter);
                                break;
                            case 2:
                                DrawAssets<TerrainContentAsset>(this.availableTerrainContentAssets, ref this.selectedTerrainContentAssetIndex, ref this.terrainContentSearchFilter);
                                break;
                            case 3:
                                DrawAssets<WallContentAsset>(this.availableWallContentAssets, ref this.selectedWallContentAssetIndex, ref this.wallContentSearchFilter);
                                break;
                            case 4:
                                DrawAssets<TiledEntranceAsset>(
                                    this.availableEntranceAssets,
                                    ref this.selectedEntranceAssetIndex, 
                                    ref this.entranceSearchFilter, 
                                    () => 
                                    { 
                                        if (this.selectedEntranceAssetIndex >= 0 && this.selectedEntranceAssetIndex < this.availableEntranceAssets.Length)
                                        {
                                            TiledEntranceAsset selectedEntranceAsset = this.availableEntranceAssets[this.selectedEntranceAssetIndex];
                                            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                                            {
                                                this.selectedEntranceWidth = EditorGUILayout.IntSlider("Width", this.selectedEntranceWidth, selectedEntranceAsset.MinWidth, selectedEntranceAsset.MaxWidth);
                                            }
                                        }
                                    }
                                    );
                                break;
                        }
                    }
                }
            }
        }

        private void DrawSideMenu()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(this.config.SideMenuMaxWidth)))
            {
                this.sideMenuIndex = EditorGUILayout.Popup(this.sideMenuIndex, new string[] { "Preview", "Resize", "Modify Mob Spawn Points", "Pick Orientation" });
                using (EditorGUILayout.ScrollViewScope sideMenuScrolViewScope = new EditorGUILayout.ScrollViewScope(this.sideMenuScrollPos))
                {
                    this.sideMenuScrollPos = sideMenuScrolViewScope.scrollPosition;
                    switch (this.sideMenuIndex)
                    {
                        case 0:
                            this.DrawAssetPreview();
                            break;
                        case 1:
                            this.DrawResizeButtons();
                            break;
                        case 2:
                            this.DrawMobSpawnPointModification();
                            break;
                        case 3:
                            this.DrawOrientationPicker();
                            break;
                    }
                }
            }
        }

        private void DrawAssetPreview()
        {
            Texture2D assetTexture = null;
            switch (this.selectedAssetTypeIndex)
            {
                case 0:
                    if (this.selectedTerrainTileAssetIndex >= 0 && this.selectedTerrainTileAssetIndex < this.availableTerrainTileAssets.Length)
                    {
                        this.GetTerrainTileTexture(this.availableTerrainTileAssets[this.selectedTerrainTileAssetIndex], out assetTexture);
                    }
                    break;
                case 1:
                    if (this.selectedWallTileAssetIndex >= 0 && this.selectedWallTileAssetIndex < this.availableWallTileAssets.Length)
                    {
                        this.GetWallTileTexture(this.availableWallTileAssets[this.selectedWallTileAssetIndex], Vector2Int.up, true, out assetTexture);
                    }
                    break;
                case 2:
                    if (this.selectedTerrainContentAssetIndex >= 0 && this.selectedTerrainContentAssetIndex < this.availableTerrainContentAssets.Length)
                    {
                        for (int i = 0; i < (int)TileTerrain.Count && assetTexture == null; i++)
                        {
                            this.GetTerrainContentTexture(this.availableTerrainContentAssets[this.selectedTerrainContentAssetIndex], (TileTerrain)i, this.pickedOrientationDirection, out assetTexture, out _);
                        }
                    }
                    break;
                case 3:
                    if (this.selectedWallContentAssetIndex >= 0 && this.selectedWallContentAssetIndex < this.availableWallContentAssets.Length)
                    {
                        for (int i = 0; i < (int)TileTerrain.Count && assetTexture == null; i++)
                        {
                            this.GetWallContentTexture(this.availableWallContentAssets[this.selectedWallContentAssetIndex], (TileTerrain)i, Vector2Int.up, this.pickedOrientationDirection, out assetTexture, out _);
                        }
                    }
                    break;
                case 4:
                    if (this.selectedEntranceAssetIndex >= 0 && this.selectedEntranceAssetIndex < this.availableEntranceAssets.Length)
                    {
                        for (int i = 0; i < (int)TileTerrain.Count && assetTexture == null; i++)
                        {
                            this.GetEntranceTexture(this.availableEntranceAssets[this.selectedEntranceAssetIndex], (TileTerrain)i, Vector2Int.up, out assetTexture, out _);
                        }
                    }
                    break;
            }

            if (assetTexture == null) return;

            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.FlexibleSpace();

                float previewWidth = this.config.SideMenuMaxWidth * this.config.AssetPreviewScale;
                float previewHeight = previewWidth * assetTexture.height / assetTexture.width;
                Rect textureRect = GUILayoutUtility.GetRect(previewWidth, previewHeight, GUILayout.Width(previewWidth), GUILayout.Height(previewHeight));
                GUI.DrawTexture(textureRect, assetTexture);

                GUILayout.FlexibleSpace();
            }
        }

        private void DrawResizeButtons()
        {
            Vector2 buttonSize = new Vector2(this.config.ResizeButtonLength, this.config.ResizeButtonLength);
            Vector2 resizeAreaSize = new Vector2(buttonSize.x * 5, buttonSize.y * 5);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Resize Blueprint", UtilityStyles.WordWrapAndCenter(EditorStyles.boldLabel));
                EditorGUILayout.Space();
                Rect resizeRect = GUILayoutUtility.GetRect(resizeAreaSize.x, resizeAreaSize.y);
                Vector2 centerOffset = resizeRect.center - buttonSize / 2;
                foreach (Vector2Int wallIndexDirection in WallTiling.GetAllWallIndexDirections())
                {
                    Vector2 drawDirection = new Vector2(wallIndexDirection.x, -wallIndexDirection.y);
                    Rect expandButtonRect = new Rect(centerOffset + drawDirection * buttonSize * 2, buttonSize);
                    if (GUI.Button(expandButtonRect, "+"))
                    {
                        this.editBlueprint.Expand(wallIndexDirection);
                    }
                    Rect shrinkButtonRect = new Rect(expandButtonRect.position - drawDirection * this.config.ResizeButtonLength, expandButtonRect.size);
                    if (GUI.Button(shrinkButtonRect, "-"))
                    {
                        this.editBlueprint.Shrink(wallIndexDirection);
                    }
                }
                GUI.Box(new Rect(centerOffset, buttonSize), "", EditorStyles.helpBox);
                EditorGUILayout.Space();
            }
        }

        private void DrawMobSpawnPointModification()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                UtilityGUILayout.IndexedList(
                    "Spawners",
                    this.editBlueprint.GetNumberMobSpawners(),
                    this.editBlueprint.AddMobSpawner,
                    this.editBlueprint.RemoveMobSpawner,
                    (int spawnerIndex) =>
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            EditorGUILayout.LabelField("Spawner [" + spawnerIndex + "]", EditorStyles.boldLabel);
                            TiledAreaMobSpawnerSerializedReference spawner = this.editBlueprint.GetMobSpawnerByReference(spawnerIndex);
                            this.editBlueprint.SetMobSpawnerByReference(spawnerIndex, CoreGUILayout.ObjectSerializedReferenceField<TiledAreaMobSpawnerSerializedReference, TiledAreaMobSpawner>("", spawner));
                            if (GUILayout.Toggle(this.chosenSpawnerIndex == spawnerIndex, "Modify") && this.chosenSpawnerIndex != spawnerIndex)
                            {
                                this.chosenSpawnerIndex = spawnerIndex;
                                this.chosenSpawnPointIndex = 0;
                            }
                            EditorGUILayout.Space();
                        }
                    }
                    );
            }

            if (this.chosenSpawnerIndex >= 0 && this.chosenSpawnerIndex < this.editBlueprint.GetNumberMobSpawners() &&
                this.chosenSpawnPointIndex >= 0 && this.chosenSpawnPointIndex < this.editBlueprint.GetNumberMobSpawnPoints(this.chosenSpawnerIndex))
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("Spawn Point [" + this.chosenSpawnPointIndex + "]", EditorStyles.boldLabel);
                    TiledAreaMobSpawnPoint currentSpawnPoint = this.editBlueprint.GetMobSpawnPoint(this.chosenSpawnerIndex, this.chosenSpawnPointIndex);
                    Vector2 localPosition = EditorGUILayout.Vector2Field("Local Position", currentSpawnPoint.LocalPosition);
                    bool isExclusive = EditorGUILayout.Toggle("Is Exclusive", currentSpawnPoint.IsExclusive);
                    List<MobSpawnable> filteredMobSpawnables = new List<MobSpawnable>(currentSpawnPoint.FilteredMobSpawnables);
                    UtilityGUILayout.IndexedList(
                        isExclusive ? "Exclusive Mob Spawnables" : "Excluded Mob Spawnables",
                        filteredMobSpawnables.Count,
                        (int spawnableIndex) => filteredMobSpawnables.Insert(spawnableIndex, null),
                        filteredMobSpawnables.RemoveAt,
                        (int spawnableIndex) => filteredMobSpawnables[spawnableIndex] = (MobSpawnable)EditorGUILayout.ObjectField(filteredMobSpawnables[spawnableIndex], typeof(MobSpawnable), false)
                        );
                    List<MobSpawnTag> filteredMobSpawnTags = new List<MobSpawnTag>(currentSpawnPoint.FilteredMobSpawnTags);
                    UtilityGUILayout.IndexedList(
                        isExclusive ? "Exclusive Mob Spawn Tags" : "Excluded Mob Spawn Tags",
                        filteredMobSpawnTags.Count,
                        (int tagIndex) => filteredMobSpawnTags.Insert(tagIndex, null),
                        filteredMobSpawnTags.RemoveAt,
                        (int tagIndex) => filteredMobSpawnTags[tagIndex] = (MobSpawnTag)EditorGUILayout.ObjectField(filteredMobSpawnTags[tagIndex], typeof(MobSpawnTag), false)
                        );
                    List<Core.Span<int>> filteredTierSpans = new List<Core.Span<int>>(currentSpawnPoint.FilteredTierSpans);
                    UtilityGUILayout.IndexedList(
                        isExclusive ? "Exclusive Tier Spans" : "Excluded Tier Spans",
                        filteredTierSpans.Count,
                        (int spanIndex) => filteredTierSpans.Insert(spanIndex, new Core.Span<int>(0, 0)),
                        filteredTierSpans.RemoveAt,
                        (int spanIndex) => filteredTierSpans[spanIndex] = CoreGUILayout.IntSpanField(filteredTierSpans[spanIndex])
                        );
                    this.editBlueprint.SetMobSpawnPoint(this.chosenSpawnerIndex, this.chosenSpawnPointIndex, new TiledAreaMobSpawnPoint(localPosition, isExclusive, filteredMobSpawnables, filteredMobSpawnTags, filteredTierSpans));
                }
            }
        }

        private void DrawOrientationPicker()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                float orientationPickerLength = this.config.SideMenuMaxWidth / 2;
                Rect orientationRect = GUILayoutUtility.GetRect(orientationPickerLength, orientationPickerLength);
                using (new UtilityGUI.ColorScope(UtilityGUIUtility.Darken(Color.grey)))
                {
                    GUI.Box(orientationRect, "");
                }
                float lineRadius = Mathf.Min(orientationRect.height, orientationRect.width) / 2;
                using (new UtilityGUI.ColorScope(Color.green))
                {
                    if (this.pickedOrientationDirection == Vector2.zero)
                    {
                        UtilityGUI.DrawLineArc(orientationRect.center, 0, Mathf.PI * 2, lineRadius / 4);
                    }
                    else
                    {
                        UtilityGUI.DrawLine(orientationRect.center, orientationRect.center + this.pickedOrientationDirection * new Vector2(1, -1) * lineRadius);
                    }
                    UtilityGUI.DrawLineArc(orientationRect.center, 0, Mathf.PI * 2, lineRadius);
                }
                Vector2 mouseSnapPosition = Event.current.mousePosition;
                if (Event.current.shift)
                {
                    Vector2 mousePositionOffset = Event.current.mousePosition - orientationRect.center;
                    float snappedAngleRad = Mathf.RoundToInt(mousePositionOffset.ComponentAngle0To360() / 45f) * Mathf.PI / 4;
                    mouseSnapPosition = orientationRect.center + new Vector2(Mathf.Cos(snappedAngleRad), Mathf.Sin(snappedAngleRad)) * mousePositionOffset.magnitude;
                }

                if (orientationRect.Contains(mouseSnapPosition) && Event.current.button == 0 && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
                {
                    if (Vector2.Distance(mouseSnapPosition, orientationRect.center) < lineRadius / 2)
                    {
                        this.pickedOrientationDirection = Vector2.zero;
                    }
                    else
                    {
                        this.pickedOrientationDirection = (mouseSnapPosition - orientationRect.center).normalized * new Vector2(1, -1);
                    }
                }
            }
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                this.pickedOrientationDirection = EditorGUILayout.Vector2Field("Orientation Direction", this.pickedOrientationDirection);
            }
        }

        private void GetTerrainTileTexture(TerrainTileAsset terrainTileAsset, out Texture2D texture)
        {
            if (this.terrainTileTextures == null)
            {
                this.terrainTileTextures = new Dictionary<TerrainTileAsset, Texture2D>();
            }
            if (!this.terrainTileTextures.ContainsKey(terrainTileAsset) || 
                this.terrainTileTextures[terrainTileAsset] == null)
            {
                Snapshot.SnapPrefab<TerrainTile>(terrainTileAsset.TerrainTilePrefab, (TerrainTile terrainTile) => terrainTile.Preview(), out texture, out _);
                this.terrainTileTextures[terrainTileAsset] = texture;
                return;
            }
            texture = this.terrainTileTextures[terrainTileAsset];
        }

        private void GetWallTileTexture(WallTileAsset wallTileAsset, Vector2Int wallIndexDirection, bool isEdge, out Texture2D texture)
        {
            if (this.wallTileTextures == null)
            {
                this.wallTileTextures = new Dictionary<(WallTileAsset, Vector2Int, bool), Texture2D>();
            }
            if (!this.wallTileTextures.ContainsKey((wallTileAsset, wallIndexDirection, isEdge)) || 
                this.wallTileTextures[(wallTileAsset, wallIndexDirection, isEdge)] == null)
            {
                Snapshot.SnapPrefab<WallTile>(
                    wallTileAsset.GetWallTilePrefab(1), 
                    (WallTile wallTile) => 
                    { 
                        wallTile.Preview(isEdge); 
                        wallTile.transform.rotation = Quaternion.Euler(0, 0, wallIndexDirection.CartesianAngle() - 90); 
                    }, 
                    out texture,
                    out _
                    );
                this.wallTileTextures[(wallTileAsset, wallIndexDirection, isEdge)] = texture;
            }
            texture = this.wallTileTextures[(wallTileAsset, wallIndexDirection, isEdge)];
        }

        private void GetTerrainContentTexture(TerrainContentAsset terrainContentAsset, TileTerrain terrain, Vector2 orientationDirection, out Texture2D texture, out Vector2 pivot)
        {
            if (this.terrainContentTextures == null)
            {
                this.terrainContentTextures = new Dictionary<(TerrainContentAsset, TileTerrain, int), (Texture2D, Vector2)>();
            }
            int octantIndex = Mathf.RoundToInt(orientationDirection.ComponentAngle0To360() / 45);
            if (!this.terrainContentTextures.ContainsKey((terrainContentAsset, terrain, octantIndex)) || 
                this.terrainContentTextures[(terrainContentAsset, terrain, octantIndex)].Item1 == null)
            {
                texture = null;
                pivot = Vector2.zero;
                if (terrainContentAsset.TryGetTerrainContentPrefab(terrain, out TerrainContent terrainContentPrefab))
                {
                    Snapshot.SnapPrefab<TerrainContent>(terrainContentPrefab, (TerrainContent terrainContent) => terrainContent.Preview(orientationDirection), out texture, out pivot);
                }
                this.terrainContentTextures[(terrainContentAsset, terrain, octantIndex)] = (texture, pivot);
                return;
            }
            (texture, pivot) = this.terrainContentTextures[(terrainContentAsset, terrain, octantIndex)];
        }

        private void GetWallContentTexture(WallContentAsset wallContentAsset, TileTerrain terrain, Vector2Int wallIndexDirection, Vector2 orientationDirection, out Texture2D texture, out Vector2 pivot)
        {
            if (this.wallContentTextures == null)
            {
                this.wallContentTextures = new Dictionary<(WallContentAsset, TileTerrain, Vector2Int, int), (Texture2D, Vector2)>();
            }
            int octantIndex = Mathf.RoundToInt(orientationDirection.ComponentAngle0To360() / 45);
            if (!this.wallContentTextures.ContainsKey((wallContentAsset, terrain, wallIndexDirection, octantIndex)) ||
                this.wallContentTextures[(wallContentAsset, terrain, wallIndexDirection, octantIndex)].Item1 == null)
            {
                texture = null;
                pivot = Vector2.zero;
                if (wallContentAsset.TryGetWallContentPrefab(terrain, out WallContent wallContentPrefab))
                {
                    Snapshot.SnapPrefab<WallContent>(
                        wallContentPrefab,
                        (WallContent wallContent) =>
                        {
                            wallContent.Preview(orientationDirection);
                            wallContent.transform.rotation = Quaternion.Euler(0, 0, wallIndexDirection.CartesianAngle() - 90);
                        },
                        out texture,
                        out pivot
                        );
                }
                this.wallContentTextures[(wallContentAsset, terrain, wallIndexDirection, octantIndex)] = (texture, pivot);
                return;
            }
            (texture, pivot) = this.wallContentTextures[(wallContentAsset, terrain, wallIndexDirection, octantIndex)];
        }

        private void GetEntranceTexture(TiledEntranceAsset entranceAsset, TileTerrain terrain, Vector2Int wallIndexDirection, out Texture2D texture, out Vector2 pivot)
        {
            if (this.entranceTextures == null)
            {
                this.entranceTextures = new Dictionary<(TiledEntranceAsset, TileTerrain, Vector2Int), (Texture2D, Vector2)>();
            }
            
            if (!this.entranceTextures.ContainsKey((entranceAsset, terrain, wallIndexDirection)) || 
                this.entranceTextures[(entranceAsset, terrain, wallIndexDirection)].Item1 == null)
            {
                texture = null;
                pivot = Vector2.zero;

                if (entranceAsset.TryGetEntrancePrefab(terrain, null, null, out TiledEntrance entrancePrefab))
                {
                    Snapshot.SnapPrefab<TiledEntrance>(
                        entrancePrefab,
                        (TiledEntrance entrance) =>
                        {
                            entrance.Preview();
                            entrance.transform.rotation = Quaternion.Euler(0, 0, wallIndexDirection.CartesianAngle() - 90);
                        },
                        out texture,
                        out pivot
                        );
                }
                this.entranceTextures[(entranceAsset, terrain, wallIndexDirection)] = (texture, pivot);
            }
            (texture, pivot) = this.entranceTextures[(entranceAsset, terrain, wallIndexDirection)];
        }
    }
}
#endif
