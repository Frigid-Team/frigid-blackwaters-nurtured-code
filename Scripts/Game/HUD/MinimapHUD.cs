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
            TiledArea.OnFocusedTiledAreaChanged += RefreshMinimap;
            TiledLevel.OnFocusedTiledLevelChanged += RefreshMinimap;
            RefreshMinimap();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TiledArea.OnFocusedTiledAreaChanged -= RefreshMinimap;
            TiledLevel.OnFocusedTiledLevelChanged -= RefreshMinimap;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void RefreshMinimap()
        {
            if (TiledArea.TryGetFocusedTiledArea(out TiledArea focusedTiledArea))
            {
                this.tiledWorldMap.MoveTo(-focusedTiledArea.CenterPosition);
            }
        }
    }
}
