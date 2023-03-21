using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class OrbitRingSFX : SFX
    {
        [SerializeField]
        private ParticleSystem orbiterParticlePrefab;
        [SerializeField]
        private IntSerializedReference numberOrbiters;
        [SerializeField]
        private FloatSerializedReference orbitSpeed;
        [SerializeField]
        private List<Sprite> orbiterSprites;

        private FrigidCoroutine orbitRoutine;
        private RecyclePool<ParticleSystem> orbiterPool;
        private List<ParticleSystem> orbiters;

        protected override void Played(AnimatorBody animatorBody)
        {
            this.orbiters.AddRange(this.orbiterPool.Retrieve(this.numberOrbiters.MutableValue));
            foreach (ParticleSystem orbiter in this.orbiters)
            {
                orbiter.transform.SetParent(animatorBody.transform);
                orbiter.Play();
            }
            this.orbitRoutine = FrigidCoroutine.Run(Orbit(animatorBody), this.gameObject);
        }

        protected override void Stopped()
        {
            FrigidCoroutine.Kill(this.orbitRoutine);
            foreach (ParticleSystem orbiter in this.orbiters)
            {
                orbiter.transform.SetParent(this.transform);
                orbiter.Stop();
            }
            this.orbiterPool.Pool(this.orbiters);
            this.orbiters.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            this.orbiterPool = new RecyclePool<ParticleSystem>(
                () => Object.Instantiate<ParticleSystem>(this.orbiterParticlePrefab),
                (ParticleSystem particle) => Object.Destroy(particle)
                );
            this.orbiters = new List<ParticleSystem>();
            if (this.orbiterSprites.Count == 0)
            {
                Debug.LogError("No sprites for the orbiter in " + this.name + "!");
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> Orbit(AnimatorBody animatorBody)
        {
            float offsetAngleRad = 0;
            while (true)
            {
                Bounds areaOccupied = animatorBody.TotalAreaOccupied;
                float orbitRadius = areaOccupied.extents.magnitude / 2;
                offsetAngleRad = (offsetAngleRad + this.orbitSpeed.ImmutableValue / orbitRadius * FrigidCoroutine.DeltaTime) % (Mathf.PI * 2);
                Vector2 localOffset = areaOccupied.center - animatorBody.transform.position;
                localOffset.y /= 2;

                for (int i = 0; i < this.orbiters.Count; i++)
                {
                    ParticleSystem orbiter = this.orbiters[i];
                    float orbiterAngle = (offsetAngleRad + i * (Mathf.PI * 2) / this.orbiters.Count) % (Mathf.PI * 2);
                    int spriteIndex = Mathf.RoundToInt(orbiterAngle / (Mathf.PI * 2) * this.orbiterSprites.Count) % this.orbiterSprites.Count;
                    Sprite sprite = this.orbiterSprites[spriteIndex];

                    ParticleSystemRenderer particleRenderer = orbiter.GetComponent<ParticleSystemRenderer>();
                    particleRenderer.material.mainTexture = sprite.texture;

                    ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = orbiter.textureSheetAnimation;
                    textureSheetAnimation.numTilesX = Mathf.RoundToInt(sprite.texture.width / sprite.rect.width);
                    textureSheetAnimation.numTilesY = Mathf.RoundToInt(sprite.texture.height / sprite.rect.height);
                    int widthIndex = Mathf.RoundToInt(sprite.rect.position.x / sprite.rect.width);
                    int heightIndex = textureSheetAnimation.numTilesY - 1 - Mathf.RoundToInt(sprite.rect.position.y / sprite.rect.height);
                    textureSheetAnimation.startFrame = (float)(heightIndex * textureSheetAnimation.numTilesX + widthIndex) / (textureSheetAnimation.numTilesX * textureSheetAnimation.numTilesY);

                    ParticleSystem.MainModule main = orbiter.main;
                    main.startSizeX = sprite.rect.width / sprite.pixelsPerUnit;
                    main.startSizeY = sprite.rect.height / sprite.pixelsPerUnit;

                    orbiter.transform.localPosition = localOffset + new Vector2(Mathf.Cos(orbiterAngle), Mathf.Sin(orbiterAngle)) * orbitRadius;
                }
                yield return null;
            }
        }
    }
}
