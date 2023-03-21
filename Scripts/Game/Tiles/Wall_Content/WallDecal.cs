using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class WallDecal : WallContent
    {
        [SerializeField]
        private string[] animationNames;

        protected override void Start()
        {
            base.Start();
            this.AnimatorBody.Play(this.animationNames[Random.Range(0, this.animationNames.Length)]);
        }
    }
}
