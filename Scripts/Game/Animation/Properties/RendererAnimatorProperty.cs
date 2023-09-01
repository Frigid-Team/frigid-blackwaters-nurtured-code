using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class RendererAnimatorProperty : SortingOrderedAnimatorProperty
    {
        [SerializeField]
        [HideInInspector]
        private List<bool> enableOutlines;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<MaterialTweensInFrame> materialTweensInFrames;

        private Dictionary<MaterialProperties.ColorProperty, (int tweenCount, Color originalColor)> originalMaterialColors;
        private Dictionary<MaterialTweenOptionSet, (int count, FrigidCoroutine routine, Action onComplete)> runningMaterialTweens;

        private List<MaterialTweenOptionSet> addedMaterialTweens;
        private float materialTweenEndDuration;

        public sealed override int SortingOrder
        {
            get
            {
                return this.Renderer.sortingOrder;
            }
            protected set
            {
                this.Renderer.sortingOrder = value;
            }
        }

        public Material OriginalMaterial
        {
            get
            {
                return this.Renderer.sharedMaterial;
            }
            set
            {
                if (this.Renderer.sharedMaterial != value)
                {
                    FrigidEdit.RecordChanges(this.Renderer);
                    this.Renderer.sharedMaterial = value;
                }
            }
        }

        public bool GetEnableOutline(int animationIndex)
        {
            return this.enableOutlines[animationIndex];
        }

        public void SetEnableOutline(int animationIndex, bool enableOutline)
        {
            if (this.enableOutlines[animationIndex] != enableOutline)
            {
                FrigidEdit.RecordChanges(this);
                this.enableOutlines[animationIndex] = enableOutline;
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
                this.AddMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index);
            }
            for (int index = this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects.Count - 1; index > numberMaterialTweens - 1; index--)
            {
                this.RemoveMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index);
            }
        }

        public MaterialTweenOptionSetSerializedReference GetMaterialTweenInFrameByReferenceAt(int animationIndex, int frameIndex, int index)
        {
            return this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects[index];
        }

        public void SetMaterialTweenInFrameByReferenceAt(int animationIndex, int frameIndex, int index, MaterialTweenOptionSetSerializedReference materialTween)
        {
            if (this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects[index] != materialTween)
            {
                FrigidEdit.RecordChanges(this);
                this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects[index] = materialTween;
            }
        }

        public void AddMaterialTweenInFrameByReferenceAt(int animationIndex, int frameIndex, int index)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects.Insert(index, new MaterialTweenOptionSetSerializedReference());
        }

        public void RemoveMaterialTweenInFrameByReferenceAt(int animationIndex, int frameIndex, int index)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweensInFrames[animationIndex][frameIndex].MaterialEffects.RemoveAt(index);
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.Renderer.sortingLayerName = FrigidSortingLayer.World.ToString();
            this.enableOutlines = new List<bool>();
            this.materialTweensInFrames = new Nested2DList<MaterialTweensInFrame>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.enableOutlines.Add(true);
                this.materialTweensInFrames.Add(new Nested1DList<MaterialTweensInFrame>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.materialTweensInFrames[animationIndex].Add(new MaterialTweensInFrame());
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.enableOutlines.Insert(animationIndex, false);
            this.materialTweensInFrames.Insert(animationIndex, new Nested1DList<MaterialTweensInFrame>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.materialTweensInFrames[animationIndex].Add(new MaterialTweensInFrame());
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.enableOutlines.RemoveAt(animationIndex);
            this.materialTweensInFrames.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweensInFrames[animationIndex].Insert(frameIndex, new MaterialTweensInFrame());
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweensInFrames[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            RendererAnimatorProperty otherRendererProperty = otherProperty as RendererAnimatorProperty;
            if (otherRendererProperty)
            {
                otherRendererProperty.SetEnableOutline(toAnimationIndex, this.GetEnableOutline(fromAnimationIndex));
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            RendererAnimatorProperty otherRendererProperty = otherProperty as RendererAnimatorProperty;
            if (otherRendererProperty)
            {
                otherRendererProperty.SetNumberMaterialTweensInFrame(toAnimationIndex, toFrameIndex, this.GetNumberMaterialTweensInFrame(fromAnimationIndex, fromFrameIndex));
                for (int tweenIndex = 0; tweenIndex < this.GetNumberMaterialTweensInFrame(fromAnimationIndex, fromFrameIndex); tweenIndex++)
                {
                    otherRendererProperty.SetMaterialTweenInFrameByReferenceAt(toAnimationIndex, toFrameIndex, tweenIndex, new MaterialTweenOptionSetSerializedReference(this.GetMaterialTweenInFrameByReferenceAt(fromAnimationIndex, fromFrameIndex, tweenIndex)));
                }
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public void PlayMaterialTween(MaterialTweenOptionSet materialTween, Action onComplete = null)
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
                FrigidCoroutine.Run(materialTween.MakeRoutine(this.Renderer, summedOnComplete), this.gameObject),
                summedOnComplete
                );
            (int tweenCount, Color originalColor) originalMaterialColor = this.originalMaterialColors[materialTween.ColorProperty];
            this.originalMaterialColors[materialTween.ColorProperty] = (originalMaterialColor.tweenCount + 1, originalMaterialColor.originalColor);
        }

        public void StopMaterialTween(MaterialTweenOptionSet materialTween)
        {
            if (this.runningMaterialTweens.TryGetValue(materialTween, out (int count, FrigidCoroutine routine, Action onComplete) runningMaterialTween))
            {
                this.runningMaterialTweens[materialTween] = (runningMaterialTween.count - 1, runningMaterialTween.routine, runningMaterialTween.onComplete);
                if (runningMaterialTween.count <= 1)
                {
                    FrigidCoroutine.Kill(runningMaterialTween.routine);
                    this.runningMaterialTweens.Remove(materialTween);
                }

                (int tweenCount, Color originalColor) originalMaterialColor = this.originalMaterialColors[materialTween.ColorProperty];
                this.originalMaterialColors[materialTween.ColorProperty] = (originalMaterialColor.tweenCount - 1, originalMaterialColor.originalColor);
                if (originalMaterialColor.tweenCount <= 1)
                {
                    MaterialProperties.SetColor(this.Renderer, materialTween.ColorProperty, originalMaterialColor.originalColor);
                }
            }
        }

        public void OneShotMaterialTween(MaterialTweenOptionSet materialTween, Action onComplete = null)
        {
            this.PlayMaterialTween(
                materialTween,
                () =>
                {
                    this.StopMaterialTween(materialTween);
                    onComplete?.Invoke();
                }
                );
        }

        public override void Initialize()
        {
            this.originalMaterialColors = new Dictionary<MaterialProperties.ColorProperty, (int tweenCount, Color originalColor)>();
            for (int i = 0; i < (int)MaterialProperties.ColorProperty.Count; i++)
            {
                MaterialProperties.ColorProperty colorProperty = (MaterialProperties.ColorProperty)i;
                this.originalMaterialColors.Add(colorProperty, (0, MaterialProperties.GetColor(this.Renderer, colorProperty)));
            }
            this.runningMaterialTweens = new Dictionary<MaterialTweenOptionSet, (int count, FrigidCoroutine routine, Action onComplete)>();
            this.addedMaterialTweens = new List<MaterialTweenOptionSet>();
            base.Initialize();
        }

        public override void AnimationEnter()
        {
            MaterialProperties.SetEnableOutline(this.Renderer, this.GetEnableOutline(this.Body.CurrAnimationIndex));
            if (!this.Body.Previewing)
            {
                this.addedMaterialTweens.Clear();
                this.materialTweenEndDuration = 0;
            }
            base.AnimationEnter();
        }

        public override void AnimationExit()
        {
            if (!this.Body.Previewing)
            {
                foreach (MaterialTweenOptionSet addedMaterialTween in this.addedMaterialTweens)
                {
                    this.StopMaterialTween(addedMaterialTween);
                }
                this.materialTweenEndDuration = 0;
            }
            base.AnimationExit();
        }

        public override void FrameEnter()
        {
            if (!this.Body.Previewing)
            {
                for (int i = 0; i < this.GetNumberMaterialTweensInFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex); i++)
                {
                    MaterialTweenOptionSet materialTween = this.GetMaterialTweenInFrameByReferenceAt(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, i).ImmutableValue;
                    this.addedMaterialTweens.Add(materialTween);
                    if (materialTween.TweenRoutine.TryCalculateTotalDuration(out float materialTweenTotalDuration))
                    {
                        this.materialTweenEndDuration = Mathf.Max(this.Body.ElapsedDuration + materialTweenTotalDuration, this.materialTweenEndDuration);
                    }
                    this.PlayMaterialTween(materialTween);
                }
            }
            base.FrameEnter();
        }

        public override float GetDuration()
        {
            return Mathf.Max(this.materialTweenEndDuration, base.GetDuration());
        }

        protected abstract Renderer Renderer
        {
            get;
        }

        [Serializable]
        private class MaterialTweensInFrame
        {
            [SerializeField]
            private List<MaterialTweenOptionSetSerializedReference> materialEffects;

            public MaterialTweensInFrame()
            {
                this.materialEffects = new List<MaterialTweenOptionSetSerializedReference>();
            }

            public List<MaterialTweenOptionSetSerializedReference> MaterialEffects
            {
                get
                {
                    return this.materialEffects;
                }
            }
        }
    }
}
