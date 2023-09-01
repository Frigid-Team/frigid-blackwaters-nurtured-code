using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MinimapHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private TiledWorldMap tiledWorldMap;

        protected override void OnEnable()
        {
            base.OnEnable();
            TiledArea.OnFocusedAreaChanged += this.RefreshMinimap;
            TiledLevel.OnFocusedLevelChanged += this.RefreshMinimap;
            this.RefreshMinimap();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TiledArea.OnFocusedAreaChanged -= this.RefreshMinimap;
            TiledLevel.OnFocusedLevelChanged -= this.RefreshMinimap;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void RefreshMinimap()
        {
            if (TiledArea.TryGetFocusedArea(out TiledArea focusedTiledArea))
            {
                this.tiledWorldMap.MoveTo(-focusedTiledArea.CenterPosition);
            }
        }
    }
}
