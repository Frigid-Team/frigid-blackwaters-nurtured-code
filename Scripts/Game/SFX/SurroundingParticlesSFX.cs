using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class SurroundingParticlesSFX : SFX
    {
        [SerializeField]
        private Vector2SerializedReference offset;
        [SerializeField]
        private ParticleSystem childParticleSystem;
        private ParticleSystemRenderer childParticleSystemRenderer;

        private FrigidCoroutine resizeRoutine;

        protected override void Played(AnimatorBody animatorBody)
        {
            this.resizeRoutine = FrigidCoroutine.Run(this.ResizeParticles(animatorBody), this.gameObject);
            this.childParticleSystem.Play();
            this.childParticleSystem.transform.SetParent(animatorBody.transform);
            this.childParticleSystem.transform.localPosition = Vector3.zero;
            this.childParticleSystem.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        protected override void Stopped()
        {
            this.childParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            this.childParticleSystem.transform.SetParent(this.transform);
            this.childParticleSystem.transform.localPosition = Vector3.zero;
            FrigidCoroutine.Kill(this.resizeRoutine);
        }

        protected override void Awake()
        {
            base.Awake();
            this.childParticleSystemRenderer = this.childParticleSystem.GetComponent<ParticleSystemRenderer>();
        }

        private IEnumerator<FrigidCoroutine.Delay> ResizeParticles(AnimatorBody animatorBody)
        {
            ParticleSystem.ShapeModule shape = this.childParticleSystem.shape;
            while (true)
            {
                // This has to be here since particle systems automatically pause and do not replay when enabled -> disabled.
                Bounds areaOccupied = animatorBody.VisibleArea;
                shape.scale = new Vector3(areaOccupied.size.x / 2, areaOccupied.size.y / 2);
                shape.position = areaOccupied.center - animatorBody.transform.position + (Vector3)this.offset.ImmutableValue;
                this.childParticleSystemRenderer.sortingFudge = -shape.position.y - areaOccupied.size.y - FrigidConstants.WorldSizeEpsilon;

                yield return null;

                if (!this.childParticleSystem.isPlaying) this.childParticleSystem.Play();
            }
        }
    }
}