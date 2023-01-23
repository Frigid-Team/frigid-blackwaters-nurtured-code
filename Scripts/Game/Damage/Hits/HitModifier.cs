using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class HitModifier : FrigidMonoBehaviour
    {
        [SerializeField]
        private HitModification modification;

        public HitModification Modification
        {
            get
            {
                return this.modification;
            }
        }

        public abstract bool ShouldApplyOnHit(Vector2 hitPosition, Vector2 hitDirection);
    }
}
