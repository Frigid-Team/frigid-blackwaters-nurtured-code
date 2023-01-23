using System.Collections.Generic;
using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class AfterImage : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<SpriteRenderer> afterImageSegmentRenderers;
        [SerializeField]
        private FloatSerializedReference afterImageDuration;

        public void PlayAfterImage(Vector2 absoluteSpawnPosition, List<SpriteRenderer> parentSegmentRenderers, Action onComplete)
        {
            if (parentSegmentRenderers.Count != this.afterImageSegmentRenderers.Count)
            {
                Debug.LogError("After image " + this.name + " does not have the same number of segment renderers as its parent.");
                return;
            }

            FrigidCoroutine[] materialEffectRoutines = new FrigidCoroutine[parentSegmentRenderers.Count];
            for (int i = 0; i < parentSegmentRenderers.Count; i++)
            {
                this.afterImageSegmentRenderers[i].sprite = parentSegmentRenderers[i].sprite;
                this.afterImageSegmentRenderers[i].flipX = parentSegmentRenderers[i].flipX;
                this.afterImageSegmentRenderers[i].sortingOrder = parentSegmentRenderers[i].sortingOrder;
                this.afterImageSegmentRenderers[i].transform.position = parentSegmentRenderers[i].transform.position;
                // materialEffectRoutines[i] = FrigidCoroutine.Run(this.materialEffects.GetRoutine(this.afterImageSegmentRenderers[i].material), this.gameObject);
            }

            this.transform.position = absoluteSpawnPosition;

            FrigidCoroutine.Run(
                TweenCoroutine.DelayedCall(
                    this.afterImageDuration.ImmutableValue, 
                    () => 
                    { 
                        onComplete?.Invoke(); 
                        foreach (FrigidCoroutine materialEffectRoutine in materialEffectRoutines)
                        {
                            FrigidCoroutine.Kill(materialEffectRoutine);
                        }
                    }
                    ), 
                this.gameObject
                );
        }
    }
}
