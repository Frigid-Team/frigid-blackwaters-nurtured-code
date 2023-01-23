using UnityEngine;

namespace FrigidBlackwaters.Game 
{
    [CreateAssetMenu(fileName = "ItemClassification", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.ITEMS + "ItemClassification")]
    public class ItemClassification : FrigidScriptableObject
    {
        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private string displayName;

        public Sprite Icon
        {
            get
            {
                return this.icon;
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
