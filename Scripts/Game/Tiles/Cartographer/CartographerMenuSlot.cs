#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FrigidBlackwaters.Game
{
    public class CartographerMenuSlot : FrigidMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private Image image;

        [Header("Colours")]
        [SerializeField]
        private Color32 select;
        [SerializeField]
        private Color32 notSelect;

        private TerrainContentAsset terrainContentAsset;
        private WallContentAsset wallContentAsset;
        private CartographerImporter importer;

        private bool mouseDown;
        private bool mouseHover;

        public Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
            }
        }

        public TerrainContentAsset TerrainContentAsset
        {
            get
            {
                return this.terrainContentAsset;
            }
            set
            {
                this.terrainContentAsset = value;
            }
        }

        public WallContentAsset WallContentAsset
        {
            get
            {
                return this.wallContentAsset;
            }
            set
            {
                this.wallContentAsset = value;
            }
        }

        public CartographerImporter Importer
        {
            set
            {
                this.importer = value;
            }
        }

        public bool ForWalls
        {
            get
            {
                return this.wallContentAsset != null;
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

        protected override void Update()
        {
            base.Update();
            if (this.mouseHover)
            {
                this.image.color = this.select;
                if (Input.GetMouseButton(0))
                {
                    if (this.mouseDown == false) {
                        if (this.importer.SelectedSlot != this)
                        {
                            this.importer.SelectedSlot = this;
                        }
                        else
                        {
                            this.importer.SelectedSlot = null;
                        }
                    }
                    this.mouseDown = true;
                }
                else
                {
                    this.mouseDown = false;
                }
            }
            else
            {
                this.image.color = this.notSelect;
            }
            if (this.importer.SelectedSlot == this)
            {
                this.image.color = this.select;
            }
        }
    }
}
#endif
