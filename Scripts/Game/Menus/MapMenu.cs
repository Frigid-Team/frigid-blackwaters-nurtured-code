using UnityEngine;
using UnityEngine.EventSystems;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MapMenu : Menu, IDragHandler
    {
        [SerializeField]
        private TiledWorldMap tiledWorldMap;
        [SerializeField]
        private float dragSpeed;

        public void OnDrag(PointerEventData eventData)
        {
            RectTransform rectTransform = (RectTransform)this.transform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position - eventData.delta, MainCamera.Instance.Camera, out Vector2 previousLocalPosition) &&
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, MainCamera.Instance.Camera, out Vector2 currentLocalPosition))
            {
                this.tiledWorldMap.MoveBy((currentLocalPosition - previousLocalPosition) * this.dragSpeed);
            }
        }

        protected override void Opened()
        {
            this.tiledWorldMap.gameObject.SetActive(true);
            if (TiledArea.TryGetFocusedTiledArea(out TiledArea tiledArea))
            {
                this.tiledWorldMap.MoveTo(-tiledArea.CenterPosition);
            }
        }

        protected override void Closed()
        {
            this.tiledWorldMap.gameObject.SetActive(false);
        }

        protected override void Awake()
        {
            base.Awake();
            this.tiledWorldMap.gameObject.SetActive(false);
        }
    }
}
