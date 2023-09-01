using System;
using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldMapCell : FrigidMonoBehaviour
    {
        [SerializeField]
        private Image cellImage;

        public virtual void FillCell(TiledLevel level, TiledArea area, bool isRevealed, float worldToMapScalingFactor, Action onMapActionPerformed)
        {
            this.cellImage.enabled = isRevealed && area.ContainedLevel == level;
            if (!isRevealed) return;
            this.transform.localPosition = area.CenterPosition * worldToMapScalingFactor;
            ((RectTransform)this.transform).sizeDelta = (Vector2)area.WallAreaDimensions * worldToMapScalingFactor;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
