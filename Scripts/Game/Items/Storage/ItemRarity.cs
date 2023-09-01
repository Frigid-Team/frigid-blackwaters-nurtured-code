using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "ItemRarity", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.ITEMS + "ItemRarity")]
    public class ItemRarity : FrigidScriptableObject
    {
        [SerializeField]
        private Color displayColor;
        [SerializeField]
        private string displayName;

        public Color DisplayColor
        {
            get
            {
                return this.displayColor;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
        }
    }
}
