using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldMapCellConnector : FrigidMonoBehaviour
    {
        [SerializeField]
        private Image cellConnectorImage;
        [SerializeField]
        private float paddedLength;

        public void FillConnector(TiledAreaEntrance firstEntrance, TiledAreaEntrance secondEntrance, bool isRevealed, float worldToMapScalingFactor)
        {
            this.cellConnectorImage.enabled = isRevealed;
            if (!isRevealed) return;
            RectTransform rectTransform = (RectTransform)this.transform;
            float distanceBetweenEntrances = Vector2.Distance(firstEntrance.EntryPosition, secondEntrance.EntryPosition);
            rectTransform.sizeDelta = new Vector2(distanceBetweenEntrances * worldToMapScalingFactor + this.paddedLength, rectTransform.sizeDelta.y);
            float angle = Mathf.Atan2(secondEntrance.EntryPosition.y - firstEntrance.EntryPosition.y, secondEntrance.EntryPosition.x - firstEntrance.EntryPosition.x);
            rectTransform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            rectTransform.localPosition = (firstEntrance.EntryPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distanceBetweenEntrances / 2) * worldToMapScalingFactor;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
