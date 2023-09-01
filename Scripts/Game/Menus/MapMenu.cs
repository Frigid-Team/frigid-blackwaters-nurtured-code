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
        private bool mapActionPerformed;

        public void OnDrag(PointerEventData eventData)
        {
            RectTransform rectTransform = (RectTransform)this.transform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position - eventData.delta, MainCamera.Instance.Camera, out Vector2 previousLocalPosition) &&
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, MainCamera.Instance.Camera, out Vector2 currentLocalPosition))
            {
                this.tiledWorldMap.MoveBy((currentLocalPosition - previousLocalPosition) * this.dragSpeed);
            }
        }

        public override bool WantsToOpen()
        {
            return InterfaceInput.ExpandPerformedThisFrame;
        }

        public override bool WantsToClose()
        {
            return this.mapActionPerformed || InterfaceInput.ExpandPerformedThisFrame || InterfaceInput.ReturnPerformedThisFrame;
        }

        public override void Opened()
        {
            base.Opened();
            this.tiledWorldMap.gameObject.SetActive(true);
            if (TiledArea.TryGetFocusedArea(out TiledArea tiledArea))
            {
                this.tiledWorldMap.MoveTo(-tiledArea.CenterPosition);
            }
            this.mapActionPerformed = false;
            this.tiledWorldMap.OnMapActionPerformed += this.FlagAsClosedOnMapAction;
        }

        public override void Closed()
        {
            base.Closed();
            this.tiledWorldMap.gameObject.SetActive(false);
            this.tiledWorldMap.OnMapActionPerformed -= this.FlagAsClosedOnMapAction;
        }

        protected override void Awake()
        {
            base.Awake();
            this.tiledWorldMap.gameObject.SetActive(false);
        }

        private void FlagAsClosedOnMapAction() => this.mapActionPerformed = true;
    }
}
