using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "HitModification", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.DAMAGE + "HitModification")]
    public class HitModification : FrigidScriptableObject
    {
        [SerializeField]
        private Sprite popupIcon;
        [SerializeField]
        private FloatSerializedReference damageMultiplier;
        [SerializeField]
        private IntSerializedReference priority;

        public Sprite PopupIcon
        {
            get
            {
                return this.popupIcon;
            }
        }

        public float DamageMultiplier
        {
            get
            {
                return this.damageMultiplier.ImmutableValue;
            }
        }

        public int Priority
        {
            get
            {
                return this.priority.ImmutableValue;
            }
        }
    }
}
