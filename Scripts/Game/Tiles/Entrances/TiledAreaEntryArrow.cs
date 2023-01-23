using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TiledAreaEntryArrow : FrigidMonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer progressSpriteRenderer;
        [SerializeField]
        private List<Sprite> progressSprites;

        public void ShowEntryProgress(float progress01)
        {
            if (this.progressSprites.Count > 0)
            {
                float normalizedProgress = Mathf.Clamp01(progress01);
                this.progressSpriteRenderer.enabled = normalizedProgress != 0;
                this.progressSpriteRenderer.sprite = this.progressSprites[Mathf.FloorToInt(this.progressSprites.Count * normalizedProgress)];
            }
        }
    }
}
