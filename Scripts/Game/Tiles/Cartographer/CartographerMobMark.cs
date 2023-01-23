#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.EventSystems;

namespace FrigidBlackwaters.Game
{
    public class CartographerMobMark : FrigidMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private SpriteRenderer meleeIconSpriteRenderer;
        [SerializeField]
        private SpriteRenderer rangedIconSpriteRenderer;

        private TiledAreaMobGenerationPreset.SpawnPointPreset spawnPointPreset;
        private CartographerTiledArea terrainArea;
        private bool mouseHover;

        public TiledAreaMobGenerationPreset.SpawnPointPreset SpawnPointPreset
        {
            get
            {
                return this.spawnPointPreset;
            }
            set
            {
                this.spawnPointPreset = value;
            }
        }

        public CartographerTiledArea TerrainArea
        {
            set
            {
                this.terrainArea = value;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.mouseHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.mouseHover = false;
        }

        protected override void Update()
        {
            base.Update();
            this.meleeIconSpriteRenderer.enabled = this.spawnPointPreset.SpawnPoint.Reaches.Contains(MobReach.Melee);
            this.rangedIconSpriteRenderer.enabled = this.spawnPointPreset.SpawnPoint.Reaches.Contains(MobReach.Ranged);
            this.meleeIconSpriteRenderer.color = GetMarkColor(this.spawnPointPreset.StrategyID);
            this.rangedIconSpriteRenderer.color = GetMarkColor(this.spawnPointPreset.StrategyID);

            if (Input.GetKey(KeyCode.Alpha0) && this.mouseHover)
            {
                this.terrainArea.RemoveMobMark(this);
            }
        }

        private Color GetMarkColor(string strategyId)
        {
            if (strategyId == "dungeon_enemies")
            {
                return Color.cyan;
            }
            else if (strategyId == "allies") 
            {
                return Color.red;
            }
            else if (strategyId == "dungeon_shop_keepers")
            {
                return Color.yellow;
            }
            else
            {
                return Color.white;
            }
        }
    }
}
#endif