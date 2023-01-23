using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldMapToken : FrigidMonoBehaviour
    {
        [SerializeField]
        private Image tokenImage;
        [SerializeField]
        private Vector2 tokenPadding;

        public void FillToken(TiledArea tiledArea, TiledWorldDiscovery tiledWorldDiscovery, int numberTokens, int tokenIndex, float worldToMapScalingFactor)
        {
            int squareWidth = Mathf.CeilToInt(Mathf.Sqrt(numberTokens));
            int squareHeight = Mathf.CeilToInt((float)numberTokens / squareWidth);
            Vector2 topLeftPosition = tiledArea.AbsoluteCenterPosition * worldToMapScalingFactor + new Vector2(-(squareWidth - 1) / 2f, (squareHeight - 1) / 2f) * this.tokenPadding;
            this.tokenImage.sprite = tiledWorldDiscovery.MapTokenSprite;
            this.transform.localPosition = topLeftPosition + new Vector2(tokenIndex % squareWidth, -tokenIndex / squareWidth) * this.tokenPadding;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
