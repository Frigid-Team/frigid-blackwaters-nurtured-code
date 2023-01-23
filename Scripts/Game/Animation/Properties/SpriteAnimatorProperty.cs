using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class SpriteAnimatorProperty : SortingOrderedAnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        [HideInInspector]
        private bool useSharedMaterial;
        [SerializeField]
        [HideInInspector]
        private List<Material> sharedMaterials;
        [SerializeField]
        [HideInInspector]
        private List<MaterialTweensInAnimation> materialTweensInAnimations;
        [SerializeField]
        [HideInInspector]
        private List<float> hideChances;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<SpriteSerializedReference> sprites;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Vector2> localOffsets;

        private Dictionary<MaterialParameters.ColorParameter, (int tweenCount, Color originalColor)> originalMaterialColors;
        private Dictionary<MaterialTweenCoroutineTemplate, (int count, FrigidCoroutine routine, Action onComplete)> runningMaterialTweens;

        private int numberCompleteOrLoopedTweens;

        public override List<AnimatorProperty> ChildProperties
        {
            get
            {
                return new List<AnimatorProperty>();
            }
        }

        public bool UseSharedMaterial
        {
            get
            {
                return this.useSharedMaterial;
            }
            set
            {
                if (this.useSharedMaterial != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this);
                    this.useSharedMaterial = value;
                }
            }
        }

        public Color RendererColor
        {
            get
            {
                return this.spriteRenderer.color;
            }
        }

        public Material OriginalMaterial
        {
            get
            {
                return this.spriteRenderer.sharedMaterial;
            }
            set
            {
                if (this.spriteRenderer.sharedMaterial != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this.spriteRenderer);
                    this.spriteRenderer.sharedMaterial = value;
                }
            }
        }

        public Material GetSharedMaterial(int animationIndex)
        {
            return this.sharedMaterials[animationIndex];
        }

        public void SetSharedMaterial(int animationIndex, Material material)
        {
            if (this.sharedMaterials[animationIndex] != material)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.sharedMaterials[animationIndex] = material;
            }
        }

        public int GetNumberMaterialTweens(int animationIndex)
        {
            return this.materialTweensInAnimations[animationIndex].MaterialEffects.Count;
        }

        public void SetNumberMaterialTweens(int animationIndex, int numberMaterialTweens)
        {
            numberMaterialTweens = Mathf.Max(0, numberMaterialTweens);
            for (int frameIndex = this.materialTweensInAnimations[animationIndex].MaterialEffects.Count; frameIndex < numberMaterialTweens; frameIndex++)
            {
                AddMaterialTweenByReferenceAt(animationIndex, frameIndex);
            }
            for (int frameIndex = this.materialTweensInAnimations[animationIndex].MaterialEffects.Count - 1; frameIndex > numberMaterialTweens - 1; frameIndex--)
            {
                RemoveMaterialTweenByReferenceAt(animationIndex, frameIndex);
            }
        }

        public MaterialTweenCoroutineTemplateSerializedReference GetMaterialTweenByReferenceAt(int animationIndex, int index)
        {
            return this.materialTweensInAnimations[animationIndex].MaterialEffects[index];
        }

        public void SetMaterialTweenByReferenceAt(int animationIndex, int index, MaterialTweenCoroutineTemplateSerializedReference materialTween)
        {
            if (this.materialTweensInAnimations[animationIndex].MaterialEffects[index] != materialTween)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.materialTweensInAnimations[animationIndex].MaterialEffects[index] = materialTween;
            }
        }

        public void AddMaterialTweenByReferenceAt(int animationIndex, int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.materialTweensInAnimations[animationIndex].MaterialEffects.Insert(index, new MaterialTweenCoroutineTemplateSerializedReference());
        }

        public void RemoveMaterialTweenByReferenceAt(int animationIndex, int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.materialTweensInAnimations[animationIndex].MaterialEffects.RemoveAt(index);
        }

        public float GetHideChance(int animationIndex)
        {
            return this.hideChances[animationIndex];
        }

        public void SetHideChance(int animationIndex, float hideChance)
        {
            if (this.hideChances[animationIndex] != hideChance)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.hideChances[animationIndex] = hideChance;
            }
        }

        public SpriteSerializedReference GetSpriteByReference(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.sprites[animationIndex][frameIndex][orientationIndex];
        }

        public void SetSpriteByReference(int animationIndex, int frameIndex, int orientationIndex, SpriteSerializedReference sprite)
        {
            if (this.sprites[animationIndex][frameIndex][orientationIndex] != sprite)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.sprites[animationIndex][frameIndex][orientationIndex] = sprite;
            }
        }

        public Vector2 GetLocalOffset(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.localOffsets[animationIndex][frameIndex][orientationIndex];
        }

        public void SetLocalOffset(int animationIndex, int frameIndex, int orientationIndex, Vector2 localOffset)
        {
            if (this.localOffsets[animationIndex][frameIndex][orientationIndex] != localOffset)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.localOffsets[animationIndex][frameIndex][orientationIndex] = localOffset;
            }
        }

#if UNITY_EDITOR
        public void ImportSpriteSheet(int animationIndex, int orientationIndex, string spriteSheetPath)
        {
            if (string.IsNullOrWhiteSpace(spriteSheetPath)) return;

            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath).OfType<Sprite>().ToArray();
            for (int frameIndex = 0; frameIndex < Mathf.Min(sprites.Length, this.Body.GetFrameCount(animationIndex)); frameIndex++)
            {
                SetSpriteByReference(
                    animationIndex,
                    frameIndex,
                    orientationIndex,
                    new SpriteSerializedReference(SerializedReferenceType.Custom, sprites[frameIndex], null, new List<Sprite>(), null)
                    );
            }
        }
#endif

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.spriteRenderer = FrigidEditMode.AddComponent<SpriteRenderer>(this.gameObject);
            this.spriteRenderer.sortingLayerName = FrigidSortingLayer.World.ToString();
            this.useSharedMaterial = true;
            this.sharedMaterials = new List<Material>();
            this.materialTweensInAnimations = new List<MaterialTweensInAnimation>();
            this.hideChances = new List<float>();
            this.sprites = new Nested3DList<SpriteSerializedReference>();
            this.localOffsets = new Nested3DList<Vector2>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.sharedMaterials.Add(null);
                this.materialTweensInAnimations.Add(new MaterialTweensInAnimation());
                this.hideChances.Add(0);
                this.sprites.Add(new Nested2DList<SpriteSerializedReference>());
                this.localOffsets.Add(new Nested2DList<Vector2>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.sprites[animationIndex].Add(new Nested1DList<SpriteSerializedReference>());
                    this.localOffsets[animationIndex].Add(new Nested1DList<Vector2>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.sprites[animationIndex][frameIndex].Add(new SpriteSerializedReference());
                        this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                    }
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.sharedMaterials.Insert(animationIndex, null);
            this.materialTweensInAnimations.Insert(animationIndex, new MaterialTweensInAnimation());
            this.hideChances.Insert(animationIndex, 0);
            this.sprites.Insert(animationIndex, new Nested2DList<SpriteSerializedReference>());
            this.localOffsets.Insert(animationIndex, new Nested2DList<Vector2>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.sprites[animationIndex].Add(new Nested1DList<SpriteSerializedReference>());
                this.localOffsets[animationIndex].Add(new Nested1DList<Vector2>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.sprites[animationIndex][frameIndex].Add(new SpriteSerializedReference());
                    this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.sharedMaterials.RemoveAt(animationIndex);
            this.materialTweensInAnimations.RemoveAt(animationIndex);
            this.hideChances.RemoveAt(animationIndex);
            this.sprites.RemoveAt(animationIndex);
            this.localOffsets.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.sprites[animationIndex].Insert(frameIndex, new Nested1DList<SpriteSerializedReference>());
            this.localOffsets[animationIndex].Insert(frameIndex, new Nested1DList<Vector2>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.sprites[animationIndex][frameIndex].Add(new SpriteSerializedReference());
                this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.sprites[animationIndex].RemoveAt(frameIndex);
            this.localOffsets[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.sprites[animationIndex][frameIndex].Insert(orientationIndex, new SpriteSerializedReference());
            this.localOffsets[animationIndex][frameIndex].Insert(orientationIndex, Vector2.zero);
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.sprites[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.localOffsets[animationIndex][frameIndex].RemoveAt(orientationIndex);
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            SpriteAnimatorProperty otherSpriteProperty = otherProperty as SpriteAnimatorProperty;
            if (otherSpriteProperty)
            {
                otherSpriteProperty.SetSharedMaterial(toAnimationIndex, GetSharedMaterial(fromAnimationIndex));
                otherSpriteProperty.SetNumberMaterialTweens(toAnimationIndex, GetNumberMaterialTweens(fromAnimationIndex));
                for (int tweenIndex = 0; tweenIndex < GetNumberMaterialTweens(fromAnimationIndex); tweenIndex++)
                {
                    otherSpriteProperty.SetMaterialTweenByReferenceAt(toAnimationIndex, tweenIndex, new MaterialTweenCoroutineTemplateSerializedReference(GetMaterialTweenByReferenceAt(fromAnimationIndex, tweenIndex)));
                }
                otherSpriteProperty.SetHideChance(toAnimationIndex, GetHideChance(fromAnimationIndex));
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            SpriteAnimatorProperty otherSpriteProperty = otherProperty as SpriteAnimatorProperty;
            if (otherSpriteProperty)
            {
                otherSpriteProperty.SetSpriteByReference(toAnimationIndex, toFrameIndex, toOrientationIndex, new SpriteSerializedReference(GetSpriteByReference(fromAnimationIndex, fromFrameIndex, fromOrientationIndex)));
                otherSpriteProperty.SetLocalOffset(toAnimationIndex, toFrameIndex, toOrientationIndex, GetLocalOffset(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public void OneShotMaterialTween(MaterialTweenCoroutineTemplate materialTween)
        {
            PlayMaterialTween(materialTween, () => StopMaterialTween(materialTween));
        }

        public void PlayMaterialTween(MaterialTweenCoroutineTemplate materialTween, Action onComplete = null)
        {
            if (!this.UseSharedMaterial)
            {
                this.runningMaterialTweens.TryAdd(materialTween, (0, null, null));
                (int count, FrigidCoroutine routine, Action onComplete) runningMaterialTween = this.runningMaterialTweens[materialTween];
                FrigidCoroutine.Kill(runningMaterialTween.routine);
                Action summedOnComplete =
                    () =>
                    {
                        (int count, FrigidCoroutine routine, Action onComplete) currentRunningMaterialTween = this.runningMaterialTweens[materialTween];
                        this.runningMaterialTweens[materialTween] = (currentRunningMaterialTween.count, currentRunningMaterialTween.routine, null);
                        runningMaterialTween.onComplete?.Invoke();
                        onComplete?.Invoke();
                    };
                this.runningMaterialTweens[materialTween] = 
                    (runningMaterialTween.count + 1, 
                    FrigidCoroutine.Run(materialTween.GetRoutine(this.spriteRenderer.material, summedOnComplete), this.gameObject, this.Body.Paused),
                    summedOnComplete
                    );
                (int tweenCount, Color originalColor) originalMaterialColor = this.originalMaterialColors[materialTween.ColorParameter];
                this.originalMaterialColors[materialTween.ColorParameter] = (originalMaterialColor.tweenCount + 1, originalMaterialColor.originalColor);
            }
        }

        public void StopMaterialTween(MaterialTweenCoroutineTemplate materialTween)
        {
            if (!this.UseSharedMaterial)
            {
                if (this.runningMaterialTweens.TryGetValue(materialTween, out (int count, FrigidCoroutine routine, Action onComplete) runningMaterialTween))
                {
                    this.runningMaterialTweens[materialTween] = (runningMaterialTween.count - 1, runningMaterialTween.routine, runningMaterialTween.onComplete);
                    if (runningMaterialTween.count <= 1)
                    {
                        FrigidCoroutine.Kill(runningMaterialTween.routine);
                        this.runningMaterialTweens.Remove(materialTween);
                    }

                    (int tweenCount, Color originalColor) originalMaterialColor = this.originalMaterialColors[materialTween.ColorParameter];
                    this.originalMaterialColors[materialTween.ColorParameter] = (originalMaterialColor.tweenCount - 1, originalMaterialColor.originalColor);
                    if (originalMaterialColor.tweenCount <= 1)
                    {
                        MaterialParameters.SetColor(this.spriteRenderer.material, materialTween.ColorParameter, originalMaterialColor.originalColor);
                    }
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            if (!this.UseSharedMaterial)
            {
                this.originalMaterialColors = new Dictionary<MaterialParameters.ColorParameter, (int tweenCount, Color originalColor)>();
                for (int i = 0; i < (int)MaterialParameters.ColorParameter.Count; i++)
                {
                    MaterialParameters.ColorParameter colorParameter = (MaterialParameters.ColorParameter)i;
                    this.originalMaterialColors.Add(colorParameter, (0, MaterialParameters.GetColor(this.spriteRenderer.material, colorParameter)));
                }
                this.runningMaterialTweens = new Dictionary<MaterialTweenCoroutineTemplate, (int count, FrigidCoroutine routine, Action onComplete)>();
            }
        }

        public override void Paused()
        {
            if (!this.UseSharedMaterial)
            {
                foreach ((int count, FrigidCoroutine routine, Action onComplete) runningMaterialTween in this.runningMaterialTweens.Values)
                {
                    runningMaterialTween.routine.Paused = true;
                }
            }
            base.Paused();
        }

        public override void UnPaused()
        {
            if (!this.UseSharedMaterial)
            {
                foreach ((int count, FrigidCoroutine routine, Action onComplete) runningMaterialTween in this.runningMaterialTweens.Values)
                {
                    runningMaterialTween.routine.Paused = false;
                }
            }
            base.UnPaused();
        }

        public override void AnimationEnter(int animationIndex, float elapsedDuration)
        {
            if (this.UseSharedMaterial)
            {
                this.spriteRenderer.sharedMaterial = GetSharedMaterial(animationIndex);
            }
            else
            {
                this.numberCompleteOrLoopedTweens = 0;
                for (int i = 0; i < GetNumberMaterialTweens(animationIndex); i++)
                {
                    MaterialTweenCoroutineTemplate materialTween = GetMaterialTweenByReferenceAt(animationIndex, i).ImmutableValue;
                    PlayMaterialTween(materialTween, () => this.numberCompleteOrLoopedTweens++);
                    if (materialTween.TweenRoutine.LoopInfinitely) this.numberCompleteOrLoopedTweens++;
                }
            }
            this.spriteRenderer.enabled = GetHideChance(animationIndex) < UnityEngine.Random.Range(0f, 1f);
            base.AnimationEnter(animationIndex, elapsedDuration);
        }

        public override void AnimationExit(int animationIndex)
        {
            if (!this.UseSharedMaterial)
            {
                for (int i = 0; i < GetNumberMaterialTweens(animationIndex); i++)
                {
                    StopMaterialTween(GetMaterialTweenByReferenceAt(animationIndex, i).ImmutableValue);
                }
            }
            base.AnimationExit(animationIndex);
        }

        public override void OrientFrameEnter(int animationIndex, int frameIndex, int orientationIndex, float elapsedDuration)
        {
            this.spriteRenderer.sprite = GetSpriteByReference(animationIndex, frameIndex, orientationIndex).MutableValue;
            this.spriteRenderer.transform.localPosition = GetLocalOffset(animationIndex, frameIndex, orientationIndex);
            this.spriteRenderer.sortingOrder = GetSortingOrder(animationIndex, frameIndex, orientationIndex);
            base.OrientFrameEnter(animationIndex, frameIndex, orientationIndex, elapsedDuration);
        }

        protected override bool CanCompleteAtEndOfAnimation(int animationIndex, float elapsedDuration)
        {
            return this.UseSharedMaterial || this.numberCompleteOrLoopedTweens == GetNumberMaterialTweens(animationIndex);
        }

        [Serializable]
        private class MaterialTweensInAnimation
        {
            [SerializeField]
            private List<MaterialTweenCoroutineTemplateSerializedReference> materialEffects;

            public MaterialTweensInAnimation()
            {
                this.materialEffects = new List<MaterialTweenCoroutineTemplateSerializedReference>();
            }

            public List<MaterialTweenCoroutineTemplateSerializedReference> MaterialEffects
            {
                get
                {
                    return this.materialEffects;
                }
            }
        }

        public enum TweenValue
        {
            ColorAlpha,
            FlashAmount,
            FlashColor,
            Count
        }
    }
}
