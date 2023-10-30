using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldMapCellConnector : FrigidMonoBehaviour
    {
        [SerializeField]
        private Image firstConnectorImage;
        [SerializeField]
        private Image intermediateConnectorImage;
        [SerializeField]
        private Image secondConnectorImage;
        [SerializeField]
        private float paddedLength;

        public void FillConnector(TiledLevel level, TiledEntrance firstEntrance, TiledEntrance secondEntrance, bool isRevealed, float worldToMapScalingFactor)
        {
            this.firstConnectorImage.enabled = isRevealed;
            this.intermediateConnectorImage.enabled = isRevealed;
            this.secondConnectorImage.enabled = isRevealed;
            if (!isRevealed) return;

            Vector2 firstPosition = firstEntrance.ContainedArea.ContainedLevel == level ? firstEntrance.EntryPosition : (secondEntrance.EntryPosition + (Vector2)secondEntrance.EntryIndexDirection * FrigidConstants.UnitWorldSize * TiledArea.MaxWallDepth * 2);
            Vector2 secondPosition = secondEntrance.ContainedArea.ContainedLevel == level ? secondEntrance.EntryPosition : (firstEntrance.EntryPosition + (Vector2)firstEntrance.EntryIndexDirection * FrigidConstants.UnitWorldSize * TiledArea.MaxWallDepth * 2);
            Vector2 halfwayPosition = (firstPosition + secondPosition) / 2;
            Vector2 firstIntermediatePosition;
            Vector2 secondIntermediatePosition;
            if (firstEntrance.EntryIndexDirection == Vector2Int.left || firstEntrance.EntryIndexDirection == Vector2Int.right)
            {
                firstIntermediatePosition = new Vector2(halfwayPosition.x, firstPosition.y);
                secondIntermediatePosition = new Vector2(halfwayPosition.x, secondPosition.y);
            }
            else
            {
                firstIntermediatePosition = new Vector2(firstPosition.x, halfwayPosition.y);
                secondIntermediatePosition = new Vector2(secondPosition.x, halfwayPosition.y);
            }

            this.transform.localPosition = halfwayPosition * worldToMapScalingFactor;

            void PositionConnector(Image connectorImage, Vector2 startPosition, Vector2 endPosition)
            {
                float distanceBetween = Vector2.Distance(startPosition, endPosition);
                if (distanceBetween < FrigidConstants.WorldSizeEpsilon)
                {
                    connectorImage.enabled = false;
                    return;
                }
                connectorImage.rectTransform.sizeDelta = new Vector2(distanceBetween * worldToMapScalingFactor + this.paddedLength, connectorImage.rectTransform.sizeDelta.y);
                connectorImage.rectTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(endPosition.y - startPosition.y, endPosition.x - startPosition.x) * Mathf.Rad2Deg);
                connectorImage.rectTransform.localPosition = ((endPosition + startPosition) / 2  - halfwayPosition) * worldToMapScalingFactor;
            }

            PositionConnector(this.firstConnectorImage, firstPosition, firstIntermediatePosition);
            PositionConnector(this.intermediateConnectorImage, firstIntermediatePosition, secondIntermediatePosition);
            PositionConnector(this.secondConnectorImage, secondIntermediatePosition, secondPosition);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
