using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class OrbitalRingEffect : SpecialEffect
    {
        [SerializeField]
        private FloatSerializedReference orbitHeight;
        [SerializeField]
        private FloatSerializedReference orbitRadius;
        [SerializeField]
        private FloatSerializedReference orbitRotationSpeedDeg;

        private FrigidCoroutine orbitRoutine;

        protected override void Played(float effectsTimeScaling = 1)
        {
            throw new System.NotImplementedException();
        }

        protected override void Stopped()
        {
            throw new System.NotImplementedException();
        }

        /*
        protected override void Played(float effectsTimeScaling = 1)
        {
            this.orbitRoutine = FrigidCoroutine.Run(Orbit(), this.gameObject);
            foreach (DirectionalGraphic2D orbitalGraphic in this.orbitalGraphics) orbitalGraphic.Enabled = true;
        }

        protected override void Stopped()
        {
            if (this.orbitRoutine != null)
            {
                FrigidCoroutine.Kill(this.orbitRoutine);
                this.orbitRoutine = null;
            }
            foreach (DirectionalGraphic2D orbitalGraphic in this.orbitalGraphics) orbitalGraphic.Enabled = false;
        }

        protected override void Awake()
        {
            base.Awake();
            foreach (DirectionalGraphic2D orbitalGraphic in this.orbitalGraphics) orbitalGraphic.Enabled = false;
        }

        private IEnumerator<FrigidCoroutine.Delay> Orbit()
        {
            int numberGraphics = this.orbitalGraphics.Count;
            float baseAngleRad = Random.Range(0, Mathf.PI * 2);
            while (true)
            {
                baseAngleRad += Time.deltaTime * this.orbitRotationSpeedDeg.ImmutableValue * Mathf.Deg2Rad;
                for (int i = 0; i < numberGraphics; i++)
                {
                    DirectionalGraphic2D orbitalGraphic = this.orbitalGraphics[i];
                    float offsetRad = i * 2 * Mathf.PI / numberGraphics;
                    Vector2 direction = new Vector2(Mathf.Cos(baseAngleRad + offsetRad), Mathf.Sin(baseAngleRad + offsetRad));
                    orbitalGraphic.Transform.localPosition = direction * this.orbitRadius.ImmutableValue + new Vector2(0, this.orbitHeight.ImmutableValue);
                    orbitalGraphic.Play(direction);
                }
                yield return null;
            }
        }
        */
    }
}
