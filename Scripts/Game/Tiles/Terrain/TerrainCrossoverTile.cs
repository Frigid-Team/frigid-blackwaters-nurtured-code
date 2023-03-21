using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TerrainCrossoverTile : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private string[] animationNames;

        public void Populated(Vector2 direction, bool isOuter)
        {
            float crossoverAngleRad = Mathf.Atan2(direction.y, direction.x) + (isOuter ? 1 : -1) * Mathf.PI / 16;
            this.animatorBody.Play(this.animationNames[Random.Range(0, this.animationNames.Length)]);
            this.animatorBody.Direction = new Vector2(Mathf.Cos(crossoverAngleRad), Mathf.Sin(crossoverAngleRad));
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
