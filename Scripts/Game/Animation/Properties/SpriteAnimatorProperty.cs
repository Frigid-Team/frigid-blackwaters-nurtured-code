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
        private List<bool> enableOutlines;
        [SerializeField]
        [HideInInspector]
        private List<ColorSerializedReference> colors;
        [SerializeField]
        [HideInInspector]
        private List<float> hideChances;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<MaterialTweensInFrame> materialTweensInFrames;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<SpriteSerializedReference> sprites;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Vector2> localOffsets;

        private Dictionary<MaterialParameters.ColorParameter, (int tweenCount, Color originalColor)> originalMaterialColors;
        private Dictionary<MaterialTweenCoroutineTemplate, (int count, FrigidCoroutine routine, Action onComplete)> runningMaterialTweens;

        private List<MaterialTweenCoroutineTemplate> addedMaterialTweens;
        private List<MaterialTweenCoroutineTemplate> completedOrLoopedMaterialTweens;

        public Sprite CurrentSprite
        {
            get
            {
                return this.spriteRenderer.sprite;
            }
        }

        public Color RendererColor
        {
            get
            {
                return this.spriteRenderer.color;
            }
        }

        public override int CurrentSortingOrder
        {
            get
            {
                return this.spriteRenderer.sortingOrder;
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

        public int GetNumberMaterialTweensInFrame(int animationIndex, int frameIndex)
        {
            return this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects.Count;
        }

        public void SetNumberMaterialTweensInFrame(int animationIndex, int frameIndex, int numberMaterialTweens)
        {
            numberMaterialTweens = Mathf.Max(0, numberMaterialTweens);
            for (int index = this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects.Count; index < numberMaterialTweens; index++)
            {
                AddMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index);
            }
            for (int index = this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects.Count - 1; index > numberMaterialTweens - 1; index--)
            {
                RemoveMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index);
            }
        }

        public MaterialTweenCoroutineTemplateSerializedReference GetMaterialTweenInFrameByReferenceAt(int animationIndex, int frameIndex, int index)
        {
            return this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects[index];
        }

        public void SetMaterialTweenInFrameByReferenceAt(int animationIndex, int frameIndex, int index, MaterialTweenCoroutineTemplateSerializedReference materialTween)
        {
            if (this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects[index] != materialTween)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects[index] = materialTween;
            }
        }

        public void AddMaterialTweenInFrameByReferenceAt(int animationIndex, int frameIndex, int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects.Insert(index, new MaterialTweenCoroutineTemplateSerializedReference());
        }

        public void RemoveMaterialTweenInFrameByReferenceAt(int animationIndex, int frameIndex, int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects.RemoveAt(index);
        }

        public bool GetEnableOutline(int animationIndex)
        {
            return this.enableOutlines[animationIndex];
        }

        public void SetEnableOutline(int animationIndex, bool enableOutline)
        {
            if (this.enableOutlines[animationIndex] != enableOutline)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.enableOutlines[animationIndex] = enableOutline;
            }
        }

        public ColorSerializedReference GetColorByReference(int animationIndex)
        {
            return this.colors[animationIndex];
        }

        public void SetColorByReference(int animationIndex, ColorSerializedReference color)
        {
            if (this.colors[animationIndex] != color)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.colors[animationIndex] = color;
            }
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
            this.spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
            this.useSharedMaterial = true;
            this.sharedMaterials = new List<Material>();
            this.materialTweensInFrames = new Nested2DList<MaterialTweensInFrame>();
            this.hideChances = new List<float>();
            this.enableOutlines = new List<bool>();
            this.colors = new List<ColorSerializedReference>();
            this.sprites = new Nested3DList<SpriteSerializedReference>();
            this.localOffsets = new Nested3DList<Vector2>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.sharedMaterials.Add(null);
                this.materialTweensInFrames.Add(new Nested1DList<MaterialTweensInFrame>());
                this.hideChances.Add(0);
                this.enableOutlines.Add(true);
                this.colors.Add(new ColorSerializedReference());
                this.sprites.Add(new Nested2DList<SpriteSerializedReference>());
                this.localOffsets.Add(new Nested2DList<Vector2>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.materialTweensInFrames[animationIndex].Add(new MaterialTweensInFrame());
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
            this.materialTweensInFrames.Insert(animationIndex, new Nested1DList<MaterialTweensInFrame>());
            this.hideChances.Insert(animationIndex, 0);
            this.enableOutlines.Insert(animationIndex, true);
            this.colors.Insert(animationIndex, new ColorSerializedReference());
            this.sprites.Insert(animationIndex, new Nested2DList<SpriteSerializedReference>());
            this.localOffsets.Insert(animationIndex, new Nested2DList<Vector2>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.materialTweensInFrames[animationIndex].Add(new MaterialTweensInFrame());
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
            this.materialTweensInFrames.RemoveAt(animationIndex);
            this.hideChances.RemoveAt(animationIndex);
            this.enableOutlines.RemoveAt(animationIndex);
            this.colors.RemoveAt(animationIndex);
            this.sprites.RemoveAt(animationIndex);
            this.localOffsets.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.materialTweensInFrames[animationIndex].Insert(frameIndex, new MaterialTweensInFrame());
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
            this.materialTweensInFrames[animationIndex].RemoveAt(frameIndex);
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
                otherSpriteProperty.SetColorByReference(toAnimationIndex, GetColorByReference(fromAnimationIndex));
                otherSpriteProperty.SetEnableOutline(toAnimationIndex, GetEnableOutline(fromAnimationIndex));
                otherSpriteProperty.SetHideChance(toAnimationIndex, GetHideChance(fromAnimationIndex));
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            SpriteAnimatorProperty otherSpriteProperty = otherProperty as SpriteAnimatorProperty;
            if (otherSpriteProperty)
            {
                otherSpriteProperty.SetNumberMaterialTweensInFrame(toAnimationIndex, toFrameIndex, GetNumberMaterialTweensInFrame(fromAnimationIndex, fromFrameIndex));
                for (int tweenIndex = 0; tweenIndex < GetNumberMaterialTweensInFrame(fromAnimationIndex, fromFrameIndex); tweenIndex++)
                {
                    otherSpriteProperty.SetMaterialTweenInFrameByReferenceAt(toAnimationIndex, toFrameIndex, tweenIndex, new MaterialTweenCoroutineTemplateSerializedReference(GetMaterialTweenInFrameByReferenceAt(fromAnimationIndex, fromFrameIndex, tweenIndex)));
                }
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
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

        public void OneShotMaterialTween(MaterialTweenCoroutineTemplate materialTween, Action onComplete = null)
        {
            PlayMaterialTween(
                materialTween, 
                () =>
                {
                    StopMaterialTween(materialTween);
                    onComplete?.Invoke();
                }
                );
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
                    FrigidCoroutine.Run(materialTween.GetRoutine(this.spriteRenderer.material, summedOnComplete), this.gameObject),
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

                this.addedMaterialTweens = new List<MaterialTweenCoroutineTemplate>();
                this.completedOrLoopedMaterialTweens = new List<MaterialTweenCoroutineTemplate>();
            }
        }

        public override void AnimationEnter()
        {
            if (this.UseSharedMaterial)
            {
                this.spriteRenderer.sharedMaterial = GetSharedMaterial(this.Body.CurrAnimationIndex);
            }
            else
            {
                if (!FrigidEditMode.InEdit)
                {
                    this.spriteRenderer.material.SetInt(MaterialParameters.ENABLE_OUTLINE, GetEnableOutline(this.Body.CurrAnimationIndex) ? 1 : 0);
                    this.addedMaterialTweens.Clear();
                    this.completedOrLoopedMaterialTweens.Clear();
                }
            }
            this.spriteRenderer.color = GetColorByReference(this.Body.CurrAnimationIndex).MutableValue;
            this.spriteRenderer.enabled = GetHideChance(this.Body.CurrAnimationIndex) < UnityEngine.Random.Range(0f, 1f);
            base.AnimationEnter();
        }

        public override void AnimationExit()
        {
            if (!this.UseSharedMaterial)
            {
                foreach (MaterialTweenCoroutineTemplate addedMaterialTween in this.addedMaterialTweens)
                {
                    StopMaterialTween(addedMaterialTween);
                }
            }
            base.AnimationExit();
        }

        public override void FrameEnter()
        {
            if (!this.UseSharedMaterial)
            {
                for (int i = 0; i < GetNumberMaterialTweensInFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex); i++)
                {
                    MaterialTweenCoroutineTemplate materialTween = GetMaterialTweenInFrameByReferenceAt(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, i).ImmutableValue;
                    this.addedMaterialTweens.Add(materialTween);
                    PlayMaterialTween(materialTween, () => this.completedOrLoopedMaterialTweens.Add(materialTween));
                    if (materialTween.TweenRoutine.LoopInfinitely) this.completedOrLoopedMaterialTweens.Add(materialTween);
                }
            }
            base.FrameEnter();
        }

        public override void OrientationEnter()
        {
            this.spriteRenderer.sprite = GetSpriteByReference(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex).MutableValue;
            this.spriteRenderer.transform.localPosition = GetLocalOffset(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
            this.spriteRenderer.sortingOrder = GetSortingOrder(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
            base.OrientationEnter();
        }

        protected override bool CanCompleteAtEndOfAnimation()
        {
            return this.UseSharedMaterial || this.addedMaterialTweens.Count == this.completedOrLoopedMaterialTweens.Count;
        }

        protected override Bounds? CalculateAreaOccupied()
        {
            return this.spriteRenderer.bounds;
        }

        [Serializable]
        private class MaterialTweensInFrame
        {
            [SerializeField]
            private List<MaterialTweenCoroutineTemplateSerializedReference> materialEffects;

            public MaterialTweensInFrame()
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
