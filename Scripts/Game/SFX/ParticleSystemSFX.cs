using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class ParticleSystemSFX : SFX
    {
        [SerializeField]
        private ParticleSystem particleSystem;

        private FrigidCoroutine resizeRoutine;

        protected override void Played(AnimatorBody animatorBody)
        {
            this.particleSystem.Play();
            this.particleSystem.transform.SetParent(animatorBody.transform);
            this.particleSystem.transform.localPosition = Vector2.down * GameConstants.SMALLEST_WORLD_SIZE;
            this.resizeRoutine = FrigidCoroutine.Run(ResizeParticles(animatorBody), this.gameObject);
        }

        protected override void Stopped()
        {
            this.particleSystem.Stop();
            FrigidCoroutine.Kill(this.resizeRoutine);
        }

        private IEnumerator<FrigidCoroutine.Delay> ResizeParticles(AnimatorBody animatorBody)
        {
            ParticleSystem.ShapeModule shape = this.particleSystem.shape;
            while (true)
            {
                Bounds areaOccupied = animatorBody.TotalAreaOccupied;
                shape.scale = new Vector3(areaOccupied.size.x / 2, areaOccupied.size.y / 2);
                shape.position = areaOccupied.center - animatorBody.transform.position;
                yield return null;
            }
        }
    }
}