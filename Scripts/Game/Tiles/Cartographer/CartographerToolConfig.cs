#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "CartographerToolConfig", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "CartographerToolConfig")]
    public class CartographerToolConfig : FrigidScriptableObject
    {
        [SerializeField]
        private float minTileDrawLength;
        [SerializeField]
        private float maxTileDrawLength;
        [SerializeField]
        private Color gridLineColor;
        [SerializeField]
        private Color highlightLineColor;
        [SerializeField]
        private float blueprintPreviewAspectRatio;
        [SerializeField]
        private Color selectedAssetTintColor;
        [SerializeField]
        private float sideMenuMaxWidth;
        [SerializeField]
        private float assetPreviewScale;
        [SerializeField]
        private float resizeButtonLength;
        [SerializeField]
        private Texture2D mobSpawnPointMarkTexture;

        public float MinTileDrawLength
        {
            get
            {
                return this.minTileDrawLength;
            }
        }

        public float MaxTileDrawLength
        {
            get
            {
                return this.maxTileDrawLength;
            }
        }

        public Color GridLineColor
        {
            get
            {
                return this.gridLineColor;
            }
        }

        public Color HighlightLineColor
        {
            get
            {
                return this.highlightLineColor;
            }
        }

        public float BlueprintPreviewAspectRatio
        {
            get
            {
                return this.blueprintPreviewAspectRatio;
            }
        }

        public Color SelectedAssetTintColor
        {
            get
            {
                return this.selectedAssetTintColor;
            }
        }

        public float SideMenuMaxWidth
        {
            get
            {
                return this.sideMenuMaxWidth;
            }
        }

        public float AssetPreviewScale
        {
            get
            {
                return this.assetPreviewScale;
            }
        }

        public float ResizeButtonLength
        {
            get
            {
                return this.resizeButtonLength;
            }
        }

        public Texture2D MobSpawnPointMarkTexture
        {
            get
            {
                return this.mobSpawnPointMarkTexture;
            }
        }
    }
}
#endif
