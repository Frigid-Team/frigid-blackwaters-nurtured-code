using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldMapCell : FrigidMonoBehaviour
    {
        [SerializeField]
        private Image cellImage;

        public void FillCell(TiledArea tiledArea, bool isRevealed, float worldToMapScalingFactor)
        {
            this.cellImage.enabled = isRevealed;
            if (!isRevealed) return;
            this.transform.localPosition = tiledArea.CenterPosition * worldToMapScalingFactor;
            ((RectTransform)this.transform).sizeDelta = (Vector2)tiledArea.WallAreaDimensions * worldToMapScalingFactor;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
