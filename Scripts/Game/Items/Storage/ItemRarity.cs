using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "ItemRarity", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Items + "ItemRarity")]
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
