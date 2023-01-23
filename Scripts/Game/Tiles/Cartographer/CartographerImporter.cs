#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class CartographerImporter : FrigidMonoBehaviour
    {
        [SerializeField]
        private TerrainContentAssetGroup terrainContentAssetGroup;
        [SerializeField]
        private WallContentAssetGroup wallContentAssetGroup;
        [SerializeField]
        private CartographerMenuSlot menuSlot;

        private CartographerMenuSlot selectedSlot;

        public TerrainContentAssetGroup TerrainContentAssetGroup
        {
            get
            {
                return this.terrainContentAssetGroup;
            }
        }

        public WallContentAssetGroup WallContentGroup
        {
            get
            {
                return this.wallContentAssetGroup;
            }
        }

        public CartographerMenuSlot SelectedSlot
        {
            get
            {
                return this.selectedSlot;
            }
            set
            {
                this.selectedSlot = value;
            }
        }

        protected override void Start()
        {
            base.Start();
            foreach (TerrainContentAsset asset in this.terrainContentAssetGroup.TerrainContentAssets)
            {
                CartographerMenuSlot slot = FrigidInstancing.CreateInstance<CartographerMenuSlot>(this.menuSlot);
                slot.transform.SetParent(this.transform, false);
                if (asset.TerrainContentPrefabs.Count > 0)
                {
                    SpriteRenderer renderer = asset.TerrainContentPrefabs[0].GetComponentInChildren<SpriteRenderer>();
                    slot.Image.sprite = renderer.sprite;
                }
                slot.TerrainContentAsset = asset;
                slot.Importer = this;
            }

            foreach (WallContentAsset asset in this.wallContentAssetGroup.WallContentAssets)
            {
                CartographerMenuSlot slot = FrigidInstancing.CreateInstance<CartographerMenuSlot>(this.menuSlot);
                slot.transform.SetParent(this.transform, false);
                if (asset.WallContentPrefabs.Count > 0)
                {
                    SpriteRenderer renderer = asset.WallContentPrefabs[0].GetComponentInChildren<SpriteRenderer>();
                    slot.Image.sprite = renderer.sprite;
                }
                slot.WallContentAsset = asset;
                slot.Importer = this;
            }
        }
    }
}
#endif
