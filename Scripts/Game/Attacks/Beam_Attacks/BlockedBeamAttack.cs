using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class BlockedBeamAttack : BeamAttack
    {
        private static SceneVariable<Dictionary<LineRenderer, RecyclePool<LineRenderer>>> forecastRendererPools;
        private static SceneVariable<Dictionary<Beam, (FrigidCoroutine, LineRenderer)>> currentForecastRenderers;

        [SerializeField]
        private BeamAttack sourceBeamAttack;
        [SerializeField]
        private float blockingWidth;
        [SerializeField]
        private bool showForecast;
        [SerializeField]
        [ShowIfBool("showForecast", true)]
        private LineRenderer forecastRendererPrefab;

        static BlockedBeamAttack()
        {
            forecastRendererPools = new SceneVariable<Dictionary<LineRenderer, RecyclePool<LineRenderer>>>(() => new Dictionary<LineRenderer, RecyclePool<LineRenderer>>());
            currentForecastRenderers = new SceneVariable<Dictionary<Beam, (FrigidCoroutine, LineRenderer)>>(() => new Dictionary<Beam, (FrigidCoroutine, LineRenderer)>());
        }

        public override List<BeamSpawnSetting> GetSpawnSettings(TiledArea tiledArea, float elapsedDuration)
        {
            List<BeamSpawnSetting> raycastedSpawnSettings = new List<BeamSpawnSetting>();

            LayerMask sightCollisionLayerMask = FrigidLayerMask.GetCollidingMask(FrigidLayer.Sight);
            foreach (BeamSpawnSetting sourceSpawnSetting in this.sourceBeamAttack.GetSpawnSettings(tiledArea, elapsedDuration))
            {
                Func<float, float, Vector2[]> toGetSourcePositions = sourceSpawnSetting.ToGetPositions;
                Vector2[] GetPositions(float emitDuration, float emitDurationDelta)
                {
                    Vector2[] sourcePositions = toGetSourcePositions.Invoke(emitDuration, emitDurationDelta);
                    if (sourcePositions.Length == 0)
                    {
                        return new Vector2[0];
                    }

                    List<Vector2> positions = new List<Vector2>();
                    positions.Capacity = sourcePositions.Length;
                    positions.Add(sourcePositions[0]);
                    for (int i = 1; i < sourcePositions.Length; i++)
                    {
                        Vector2 previousSourcePosition = sourcePositions[i - 1];
                        Vector2 currentSourcePosition = sourcePositions[i];
                        if (this.TryCastBlockingPosition(previousSourcePosition, currentSourcePosition, sightCollisionLayerMask, out Vector2 blockingPosition))
                        {
                            positions.Add(blockingPosition);
                            break;
                        }
                        positions.Add(currentSourcePosition);
                    }

                    return positions.ToArray();
                }

                BeamSpawnSetting spawnSetting = new BeamSpawnSetting(sourceSpawnSetting.SpawnPosition, sourceSpawnSetting.DelayDuration, sourceSpawnSetting.Prefab, GetPositions);
                if (this.showForecast)
                {
                    spawnSetting.OnBodyInitialized += (Beam beam) => this.AddForecast(beam, toGetSourcePositions);
                    spawnSetting.OnBodyDeinitialized += (Beam beam) => this.RemoveForecast(beam);
                }
                raycastedSpawnSettings.Add(spawnSetting);
            }

            return raycastedSpawnSettings;
        }

        protected override void Awake()
        {
            base.Awake();
            if (this.showForecast && !forecastRendererPools.Current.ContainsKey(this.forecastRendererPrefab))
            {
                forecastRendererPools.Current.Add(
                    this.forecastRendererPrefab, 
                    new RecyclePool<LineRenderer>(() => UnityEngine.Object.Instantiate(this.forecastRendererPrefab), (LineRenderer forecastRenderer) => UnityEngine.Object.Destroy(forecastRenderer))
                    );
            }
        }

        private bool TryCastBlockingPosition(Vector2 startPosition, Vector2 endPosition, LayerMask layerMask, out Vector2 blockingPosition)
        {
            float radius = this.blockingWidth / 2f;
            Vector2 displacement = endPosition - startPosition;
            Vector2 direction = displacement.normalized;
            float distance = displacement.magnitude;
            RaycastHit2D raycastHit = Physics2D.CircleCast(startPosition, radius, direction, distance, layerMask);
            Vector2 circleOffset = direction * radius;

            if (raycastHit.collider)
            {
                blockingPosition = raycastHit.centroid + circleOffset;
                return true;
            }
            blockingPosition = Vector2.zero;
            return false;
        }

        private void AddForecast(Beam beam, Func<float, float, Vector2[]> toGetSourcePositions)
        {
            RecyclePool<LineRenderer> forecastRendererPool = forecastRendererPools.Current[this.forecastRendererPrefab];
            LineRenderer forecastRenderer = forecastRendererPool.Retrieve();
            forecastRenderer.transform.SetParent(beam.transform);
            forecastRenderer.transform.localPosition = Vector2.zero;
            forecastRenderer.widthMultiplier = this.blockingWidth;
            FrigidCoroutine forecastRoutine = FrigidCoroutine.Run(this.Forecast(forecastRenderer, beam, toGetSourcePositions));
            currentForecastRenderers.Current.Add(beam, (forecastRoutine, forecastRenderer));
        }

        private void RemoveForecast(Beam beam)
        {
            (FrigidCoroutine forecastRoutine, LineRenderer forecastRenderer) = currentForecastRenderers.Current[beam];
            currentForecastRenderers.Current.Remove(beam);
            FrigidCoroutine.Kill(forecastRoutine);
            forecastRenderer.transform.SetParent(null);
            RecyclePool<LineRenderer> forecastRendererPool = forecastRendererPools.Current[this.forecastRendererPrefab];
            forecastRendererPool.Return(forecastRenderer);
        }

        private IEnumerator<FrigidCoroutine.Delay> Forecast(LineRenderer forecastRenderer, Beam beam, Func<float, float, Vector2[]> toGetSourcePositions)
        {
            forecastRenderer.positionCount = 0;
            while (true)
            {
                yield return null;

                Vector2[] sourceBeamPositions = toGetSourcePositions.Invoke(beam.EmitDuration, beam.EmitDurationDelta);
                forecastRenderer.positionCount = sourceBeamPositions.Length;
                for (int i = 0; i < sourceBeamPositions.Length; i++)
                {
                    forecastRenderer.SetPosition(i, sourceBeamPositions[i] - sourceBeamPositions[0]);
                }
            }
        }
    }
}
