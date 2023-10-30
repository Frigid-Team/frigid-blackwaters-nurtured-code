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
        private bool isConsideredVisibleArea;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<Nested1DList<MaterialTweenOptionSetSerializedReference>> materialTweens;
        [SerializeField]
        [HideInInspector]
        private List<MaterialPropertyBinding<ColorSerializedReference>> colorBindings;
        [SerializeField]
        [HideInInspector]
        private List<MaterialPropertyBinding<BoolSerializedReference>> boolBindings;
        [SerializeField]
        [HideInInspector]
        private List<MaterialPropertyBinding<FloatSerializedReference>> floatBindings;

        private Dictionary<(MaterialProperties.Type, string), (int, object)> originalMaterialValues;
        private Dictionary<MaterialTweenOptionSet, (int, FrigidCoroutine, Action)> runningMaterialTweens;

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

        public bool IsConsideredVisibleArea
        {
            get
            {
                return this.isConsideredVisibleArea;
            }
            set
            {
                if (this.isConsideredVisibleArea != value)
                {
                    FrigidEdit.RecordChanges(this);
                    this.isConsideredVisibleArea = value;
                }
            }
        }

        public int GetNumberBindings(MaterialProperties.Type propertyType)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    return this.colorBindings.Count;
                case MaterialProperties.Type.Boolean:
                    return this.boolBindings.Count;
                case MaterialProperties.Type.Float:
                    return this.floatBindings.Count;
            }
            return 0;
        }

        public void AddBindingAt(MaterialProperties.Type propertyType, int bindingIndex)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    MaterialPropertyBinding<ColorSerializedReference>.AddAt(this, ref this.colorBindings, bindingIndex);
                    break;
                case MaterialProperties.Type.Boolean:
                    MaterialPropertyBinding<BoolSerializedReference>.AddAt(this, ref this.boolBindings, bindingIndex);
                    break;
                case MaterialProperties.Type.Float:
                    MaterialPropertyBinding<FloatSerializedReference>.AddAt(this, ref this.floatBindings, bindingIndex);
                    break;
            }
        }

        public void RemoveBindingAt(MaterialProperties.Type propertyType, int bindingIndex)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    MaterialPropertyBinding<ColorSerializedReference>.RemoveAt(this, ref this.colorBindings, bindingIndex);
                    break;
                case MaterialProperties.Type.Boolean:
                    MaterialPropertyBinding<BoolSerializedReference>.RemoveAt(this, ref this.boolBindings, bindingIndex);
                    break;
                case MaterialProperties.Type.Float:
                    MaterialPropertyBinding<FloatSerializedReference>.RemoveAt(this, ref this.floatBindings, bindingIndex);
                    break;
            }
        }

        public string GetBindingPropertyId(MaterialProperties.Type propertyType, int bindingIndex)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    return this.colorBindings[bindingIndex].GetPropertyId();
                case MaterialProperties.Type.Boolean:
                    return this.boolBindings[bindingIndex].GetPropertyId();
                case MaterialProperties.Type.Float:
                    return this.floatBindings[bindingIndex].GetPropertyId();
            }
            return string.Empty;
        }

        public void SetBindingPropertyId(MaterialProperties.Type propertyType, int bindingIndex, string propertyId)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    this.colorBindings[bindingIndex].SetPropertyId(this, propertyId);
                    break;
                case MaterialProperties.Type.Boolean:
                    this.boolBindings[bindingIndex].SetPropertyId(this, propertyId);
                    break;
                case MaterialProperties.Type.Float:
                    this.floatBindings[bindingIndex].SetPropertyId(this, propertyId);
                    break;
            }
        }

        public short GetBindingCadence(MaterialProperties.Type propertyType, int bindingIndex)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    return this.colorBindings[bindingIndex].GetCadence();
                case MaterialProperties.Type.Boolean:
                    return this.boolBindings[bindingIndex].GetCadence();
                case MaterialProperties.Type.Float:
                    return this.floatBindings[bindingIndex].GetCadence();
            }
            return 0;
        }

        public void SetBindingCadence(MaterialProperties.Type propertyType, int bindingIndex, short cadence)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    this.colorBindings[bindingIndex].SetCadence(this, cadence);
                    break;
                case MaterialProperties.Type.Boolean:
                    this.boolBindings[bindingIndex].SetCadence(this, cadence);
                    break;
                case MaterialProperties.Type.Float:
                    this.floatBindings[bindingIndex].SetCadence(this, cadence);
                    break;
            }
        }

        public object GetBindingValueByReference(MaterialProperties.Type propertyType, int bindingIndex, int animationIndex)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    return this.colorBindings[bindingIndex].GetValue(animationIndex);
                case MaterialProperties.Type.Boolean:
                    return this.boolBindings[bindingIndex].GetValue(animationIndex);
                case MaterialProperties.Type.Float:
                    return this.floatBindings[bindingIndex].GetValue(animationIndex);
            }
            return default;
        }

        public object SetBindingValueByReference(MaterialProperties.Type propertyType, int bindingIndex, int animationIndex, object value)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    this.colorBindings[bindingIndex].SetValue(this, animationIndex, (ColorSerializedReference)value);
                    break;
                case MaterialProperties.Type.Boolean:
                    this.boolBindings[bindingIndex].SetValue(this, animationIndex, (BoolSerializedReference)value);
                    break;
                case MaterialProperties.Type.Float:
                    this.floatBindings[bindingIndex].SetValue(this, animationIndex, (FloatSerializedReference)value);
                    break;
            }
            return default;
        }

        public object GetBindingValueByReference(MaterialProperties.Type propertyType, int bindingIndex, int animationIndex, int frameIndex)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    return this.colorBindings[bindingIndex].GetValue(animationIndex, frameIndex);
                case MaterialProperties.Type.Boolean:
                    return this.boolBindings[bindingIndex].GetValue(animationIndex, frameIndex);
                case MaterialProperties.Type.Float:
                    return this.floatBindings[bindingIndex].GetValue(animationIndex, frameIndex);
            }
            return default;
        }

        public object SetBindingValueByReference(MaterialProperties.Type propertyType, int bindingIndex, int animationIndex, int frameIndex, object value)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    this.colorBindings[bindingIndex].SetValue(this, animationIndex, frameIndex, (ColorSerializedReference)value);
                    break;
                case MaterialProperties.Type.Boolean:
                    this.boolBindings[bindingIndex].SetValue(this, animationIndex, frameIndex, (BoolSerializedReference)value);
                    break;
                case MaterialProperties.Type.Float:
                    this.floatBindings[bindingIndex].SetValue(this, animationIndex, frameIndex, (FloatSerializedReference)value);
                    break;
            }
            return default;
        }

        public object GetBindingValueByReference(MaterialProperties.Type propertyType, int bindingIndex, int animationIndex, int frameIndex, int orientationIndex)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    return this.colorBindings[bindingIndex].GetValue(animationIndex, frameIndex, orientationIndex);
                case MaterialProperties.Type.Boolean:
                    return this.boolBindings[bindingIndex].GetValue(animationIndex, frameIndex, orientationIndex);
                case MaterialProperties.Type.Float:
                    return this.floatBindings[bindingIndex].GetValue(animationIndex, frameIndex, orientationIndex);
            }
            return default;
        }

        public object SetBindingValueByReference(MaterialProperties.Type propertyType, int bindingIndex, int animationIndex, int frameIndex, int orientationIndex, object value)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    this.colorBindings[bindingIndex].SetValue(this, animationIndex, frameIndex, orientationIndex, (ColorSerializedReference)value);
                    break;
                case MaterialProperties.Type.Boolean:
                    this.boolBindings[bindingIndex].SetValue(this, animationIndex, frameIndex, orientationIndex, (BoolSerializedReference)value);
                    break;
                case MaterialProperties.Type.Float:
                    this.floatBindings[bindingIndex].SetValue(this, animationIndex, frameIndex, orientationIndex, (FloatSerializedReference)value);
                    break;
            }
            return default;
        }

        public int GetNumberMaterialTweens(int animationIndex, int frameIndex)
        {
            return this.materialTweens[animationIndex][frameIndex].Count;
        }

        public MaterialTweenOptionSetSerializedReference GetMaterialTweenByReferenceAt(int animationIndex, int frameIndex, int index)
        {
            return this.materialTweens[animationIndex][frameIndex][index];
        }

        public void SetMaterialTweenByReferenceAt(int animationIndex, int frameIndex, int index, MaterialTweenOptionSetSerializedReference materialTween)
        {
            if (this.materialTweens[animationIndex][frameIndex][index] != materialTween)
            {
                FrigidEdit.RecordChanges(this);
                this.materialTweens[animationIndex][frameIndex][index] = materialTween;
            }
        }

        public void AddMaterialTweenByReferenceAt(int animationIndex, int frameIndex, int index)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweens[animationIndex][frameIndex].Insert(index, new MaterialTweenOptionSetSerializedReference());
        }

        public void RemoveMaterialTweenByReferenceAt(int animationIndex, int frameIndex, int index)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweens[animationIndex][frameIndex].RemoveAt(index);
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.Renderer.sortingLayerName = FrigidSortingLayer.World.ToString();
            this.materialTweens = new Nested2DList<Nested1DList<MaterialTweenOptionSetSerializedReference>>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.materialTweens.Add(new Nested1DList<Nested1DList<MaterialTweenOptionSetSerializedReference>>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.materialTweens[animationIndex].Add(new Nested1DList<MaterialTweenOptionSetSerializedReference>());
                }
            }
            this.colorBindings = new List<MaterialPropertyBinding<ColorSerializedReference>>();
            this.boolBindings = new List<MaterialPropertyBinding<BoolSerializedReference>>();
            this.floatBindings = new List<MaterialPropertyBinding<FloatSerializedReference>>();
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweens.Insert(animationIndex, new Nested1DList<Nested1DList<MaterialTweenOptionSetSerializedReference>>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.materialTweens[animationIndex].Add(new Nested1DList<MaterialTweenOptionSetSerializedReference>());
            }
            foreach (MaterialPropertyBinding<ColorSerializedReference> colorBinding in this.colorBindings)
            {
                colorBinding.AnimationAddedAt(this, animationIndex);
            }
            foreach (MaterialPropertyBinding<BoolSerializedReference> boolBinding in this.boolBindings)
            {
                boolBinding.AnimationAddedAt(this, animationIndex);
            }
            foreach (MaterialPropertyBinding<FloatSerializedReference> floatBinding in this.floatBindings)
            {
                floatBinding.AnimationAddedAt(this, animationIndex);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweens.RemoveAt(animationIndex);
            foreach (MaterialPropertyBinding<ColorSerializedReference> colorBinding in this.colorBindings)
            {
                colorBinding.AnimationRemovedAt(this, animationIndex);
            }
            foreach (MaterialPropertyBinding<BoolSerializedReference> boolBinding in this.boolBindings)
            {
                boolBinding.AnimationRemovedAt(this, animationIndex);
            }
            foreach (MaterialPropertyBinding<FloatSerializedReference> floatBinding in this.floatBindings)
            {
                floatBinding.AnimationRemovedAt(this, animationIndex);
            }
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweens[animationIndex].Insert(frameIndex, new Nested1DList<MaterialTweenOptionSetSerializedReference>());
            foreach (MaterialPropertyBinding<ColorSerializedReference> colorBinding in this.colorBindings)
            {
                colorBinding.FrameAddedAt(this, animationIndex, frameIndex);
            }
            foreach (MaterialPropertyBinding<BoolSerializedReference> boolBinding in this.boolBindings)
            {
                boolBinding.FrameAddedAt(this, animationIndex, frameIndex);
            }
            foreach (MaterialPropertyBinding<FloatSerializedReference> floatBinding in this.floatBindings)
            {
                floatBinding.FrameAddedAt(this, animationIndex, frameIndex);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweens[animationIndex].RemoveAt(frameIndex);
            foreach (MaterialPropertyBinding<ColorSerializedReference> colorBinding in this.colorBindings)
            {
                colorBinding.FrameRemovedAt(this, animationIndex, frameIndex);
            }
            foreach (MaterialPropertyBinding<BoolSerializedReference> boolBinding in this.boolBindings)
            {
                boolBinding.FrameRemovedAt(this, animationIndex, frameIndex);
            }
            foreach (MaterialPropertyBinding<FloatSerializedReference> floatBinding in this.floatBindings)
            {
                floatBinding.FrameRemovedAt(this, animationIndex, frameIndex);
            }
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            foreach (MaterialPropertyBinding<ColorSerializedReference> colorBinding in this.colorBindings)
            {
                colorBinding.OrientationAddedAt(this, animationIndex, frameIndex, orientationIndex);
            }
            foreach (MaterialPropertyBinding<BoolSerializedReference> boolBinding in this.boolBindings)
            {
                boolBinding.OrientationAddedAt(this, animationIndex, frameIndex, orientationIndex);
            }
            foreach (MaterialPropertyBinding<FloatSerializedReference> floatBinding in this.floatBindings)
            {
                floatBinding.OrientationAddedAt(this, animationIndex, frameIndex, orientationIndex);
            }
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            foreach (MaterialPropertyBinding<ColorSerializedReference> colorBinding in this.colorBindings)
            {
                colorBinding.OrientationRemovedAt(this, animationIndex, frameIndex, orientationIndex);
            }
            foreach (MaterialPropertyBinding<BoolSerializedReference> boolBinding in this.boolBindings)
            {
                boolBinding.OrientationRemovedAt(this, animationIndex, frameIndex, orientationIndex);
            }
            foreach (MaterialPropertyBinding<FloatSerializedReference> floatBinding in this.floatBindings)
            {
                floatBinding.OrientationRemovedAt(this, animationIndex, frameIndex, orientationIndex);
            }
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            RendererAnimatorProperty otherRendererProperty = otherProperty as RendererAnimatorProperty;
            if (otherRendererProperty)
            {
                if (otherRendererProperty == this) 
                {
                    for (int i = 0; i < (int)MaterialProperties.Type.Count; i++)
                    {
                        MaterialProperties.Type propertyType = (MaterialProperties.Type)i;
                        for (int bindingIndex = 0; bindingIndex < this.GetNumberBindings(propertyType); bindingIndex++)
                        {
                            this.SetBindingValueByReference(propertyType, bindingIndex, toAnimationIndex, this.GetBindingValueByReference(propertyType, bindingIndex, fromAnimationIndex));
                        }
                    }
                }
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            RendererAnimatorProperty otherRendererProperty = otherProperty as RendererAnimatorProperty;
            if (otherRendererProperty)
            {
                for (int tweenIndex = otherRendererProperty.GetNumberMaterialTweens(toAnimationIndex, toFrameIndex);
                    tweenIndex < this.GetNumberMaterialTweens(fromAnimationIndex, fromFrameIndex);
                    tweenIndex++)
                {
                    otherRendererProperty.AddMaterialTweenByReferenceAt(toAnimationIndex, toFrameIndex, tweenIndex);
                }
                for (int tweenIndex = otherRendererProperty.GetNumberMaterialTweens(toAnimationIndex, toFrameIndex) - 1;
                    tweenIndex >= this.GetNumberMaterialTweens(fromAnimationIndex, fromFrameIndex);
                    tweenIndex--)
                {
                    otherRendererProperty.RemoveMaterialTweenByReferenceAt(toAnimationIndex, toFrameIndex, tweenIndex);
                }
                for (int tweenIndex = 0;
                    tweenIndex < otherRendererProperty.GetNumberMaterialTweens(toAnimationIndex, toFrameIndex);
                    tweenIndex++)
                {
                    otherRendererProperty.SetMaterialTweenByReferenceAt(toAnimationIndex, toFrameIndex, tweenIndex, this.GetMaterialTweenByReferenceAt(fromAnimationIndex, fromFrameIndex, tweenIndex));
                }
                if (otherRendererProperty == this)
                {
                    for (int i = 0; i < (int)MaterialProperties.Type.Count; i++)
                    {
                        MaterialProperties.Type propertyType = (MaterialProperties.Type)i;
                        for (int bindingIndex = 0; bindingIndex < this.GetNumberBindings(propertyType); bindingIndex++)
                        {
                            this.SetBindingValueByReference(propertyType, bindingIndex, toAnimationIndex, toFrameIndex, this.GetBindingValueByReference(propertyType, bindingIndex, fromAnimationIndex, fromFrameIndex));
                        }
                    }
                }
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            RendererAnimatorProperty otherRendererProperty = otherProperty as RendererAnimatorProperty;
            if (otherRendererProperty)
            {
                if (otherRendererProperty == this)
                {
                    for (int i = 0; i < (int)MaterialProperties.Type.Count; i++)
                    {
                        MaterialProperties.Type propertyType = (MaterialProperties.Type)i;
                        for (int bindingIndex = 0; bindingIndex < this.GetNumberBindings(propertyType); bindingIndex++)
                        {
                            this.SetBindingValueByReference(propertyType, bindingIndex, toAnimationIndex, toFrameIndex, toOrientationIndex, this.GetBindingValueByReference(propertyType, bindingIndex, fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
                        }
                    }
                }
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public void PlayMaterialTween(MaterialTweenOptionSet materialTween, Action onComplete = null)
        {
            this.originalMaterialValues.TryAdd((materialTween.PropertyType, materialTween.PropertyId), (0, MaterialProperties.GetProperty(this.Renderer, materialTween.PropertyType, materialTween.PropertyId)));
            (int tweenCount, object originalValue) = this.originalMaterialValues[(materialTween.PropertyType, materialTween.PropertyId)];
            this.originalMaterialValues[(materialTween.PropertyType, materialTween.PropertyId)] = (tweenCount + 1, originalValue);

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

                (int tweenCount, object originalValue) = this.originalMaterialValues[(materialTween.PropertyType, materialTween.PropertyId)];
                this.originalMaterialValues[(materialTween.PropertyType, materialTween.PropertyId)] = (tweenCount - 1, originalValue);
                if (tweenCount <= 1)
                {
                    MaterialProperties.SetProperty(this.Renderer, materialTween.PropertyType, materialTween.PropertyId, originalValue);
                    this.originalMaterialValues.Remove((materialTween.PropertyType, materialTween.PropertyId));
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
            this.originalMaterialValues = new Dictionary<(MaterialProperties.Type, string), (int, object)>();
            this.runningMaterialTweens = new Dictionary<MaterialTweenOptionSet, (int count, FrigidCoroutine routine, Action onComplete)>();
            this.addedMaterialTweens = new List<MaterialTweenOptionSet>();
            base.Initialize();
        }

        public override void AnimationEnter()
        {
            for (int i = 0; i < (int)MaterialProperties.Type.Count; i++)
            {
                MaterialProperties.Type propertyType = (MaterialProperties.Type)i;
                for (int bindingIndex = 0; bindingIndex < this.GetNumberBindings(propertyType); bindingIndex++)
                {
                    if (this.GetBindingCadence(propertyType, bindingIndex) == 0)
                    {
                        string propertyId = this.GetBindingPropertyId(propertyType, bindingIndex);
                        object newValue = this.GetBindingValueByReference(propertyType, bindingIndex, this.Body.CurrAnimationIndex);
                        if (!this.Body.Previewing)
                        {
                            if (this.originalMaterialValues.ContainsKey((propertyType, propertyId)) && !this.Body.Previewing)
                            {
                                (int tweenCount, object _) = this.originalMaterialValues[(propertyType, propertyId)];
                                this.originalMaterialValues[(propertyType, propertyId)] = (tweenCount, newValue);
                                continue;
                            }
                        }
                        MaterialProperties.SetPropertyByReference(this.Renderer, propertyType, propertyId, newValue);
                    }
                }
            }
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
            for (int i = 0; i < (int)MaterialProperties.Type.Count; i++)
            {
                MaterialProperties.Type propertyType = (MaterialProperties.Type)i;
                for (int bindingIndex = 0; bindingIndex < this.GetNumberBindings(propertyType); bindingIndex++)
                {
                    if (this.GetBindingCadence(propertyType, bindingIndex) == 1)
                    {
                        string propertyId = this.GetBindingPropertyId(propertyType, bindingIndex);
                        object newValue = this.GetBindingValueByReference(propertyType, bindingIndex, this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex);
                        if (!this.Body.Previewing)
                        {
                            if (this.originalMaterialValues.ContainsKey((propertyType, propertyId)) && !this.Body.Previewing)
                            {
                                (int tweenCount, object _) = this.originalMaterialValues[(propertyType, propertyId)];
                                this.originalMaterialValues[(propertyType, propertyId)] = (tweenCount, newValue);
                                continue;
                            }
                        }
                        MaterialProperties.SetPropertyByReference(this.Renderer, propertyType, propertyId, newValue);
                    }
                }
            }
            if (!this.Body.Previewing)
            {
                for (int i = 0; i < this.GetNumberMaterialTweens(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex); i++)
                {
                    MaterialTweenOptionSet materialTween = this.GetMaterialTweenByReferenceAt(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, i).ImmutableValue;
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

        public override void OrientationEnter()
        {
            for (int i = 0; i < (int)MaterialProperties.Type.Count; i++)
            {
                MaterialProperties.Type propertyType = (MaterialProperties.Type)i;
                for (int bindingIndex = 0; bindingIndex < this.GetNumberBindings(propertyType); bindingIndex++)
                {
                    if (this.GetBindingCadence(propertyType, bindingIndex) == 2)
                    {
                        string propertyId = this.GetBindingPropertyId(propertyType, bindingIndex);
                        object newValue = this.GetBindingValueByReference(propertyType, bindingIndex, this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                        if (!this.Body.Previewing)
                        {
                            if (this.originalMaterialValues.ContainsKey((propertyType, propertyId)) && !this.Body.Previewing)
                            {
                                (int tweenCount, object _) = this.originalMaterialValues[(propertyType, propertyId)];
                                this.originalMaterialValues[(propertyType, propertyId)] = (tweenCount, newValue);
                                continue;
                            }
                        }
                        MaterialProperties.SetPropertyByReference(this.Renderer, propertyType, propertyId, newValue);
                    }
                }
            }
            base.OrientationEnter();
        }

        public override float GetDuration()
        {
            return Mathf.Max(this.materialTweenEndDuration, base.GetDuration());
        }

        public override Bounds? GetVisibleArea()
        {
            Bounds? visibleArea = base.GetVisibleArea();
            Bounds rendererBounds = this.Renderer.bounds;
            if (!this.IsConsideredVisibleArea ||
                rendererBounds.size.x < FrigidConstants.WorldSizeEpsilon ||
                rendererBounds.size.y < FrigidConstants.WorldSizeEpsilon)
            {
                return visibleArea;
            }
            if (visibleArea.HasValue)
            {
                visibleArea.Value.Encapsulate(rendererBounds);
                return visibleArea;
            }
            return rendererBounds;
        }

        protected abstract Renderer Renderer
        {
            get;
        }

        [Serializable]
        private class MaterialPropertyBinding<T> where T : new()
        {
            [SerializeField]
            private string propertyId;
            [SerializeField]
            private short cadence;
            [SerializeField]
            private Nested1DList<T> animationValues;
            [SerializeField]
            private Nested2DList<T> frameValues;
            [SerializeField]
            private Nested3DList<T> orientationValues;

            public MaterialPropertyBinding(RendererAnimatorProperty rendererProperty)
            {
                this.propertyId = string.Empty;
                this.cadence = 0;
                this.animationValues = new Nested1DList<T>();
                this.frameValues = new Nested2DList<T>();
                this.orientationValues = new Nested3DList<T>();
                for (int animationIndex = 0; animationIndex < rendererProperty.Body.GetAnimationCount(); animationIndex++)
                {
                    this.animationValues.Add(new T());
                }
            }

            public static void AddAt(RendererAnimatorProperty rendererProperty, ref List<MaterialPropertyBinding<T>> bindings, int bindingIndex)
            {
                FrigidEdit.RecordChanges(rendererProperty);
                bindings.Insert(bindingIndex, new MaterialPropertyBinding<T>(rendererProperty));
            }

            public static void RemoveAt(RendererAnimatorProperty rendererProperty, ref List<MaterialPropertyBinding<T>> bindings, int bindingIndex)
            {
                FrigidEdit.RecordChanges(rendererProperty);
                bindings.RemoveAt(bindingIndex);
            }

            public string GetPropertyId()
            {
                return this.propertyId;
            }

            public void SetPropertyId(RendererAnimatorProperty rendererProperty, string propertyId)
            {
                if (this.propertyId != propertyId)
                {
                    FrigidEdit.RecordChanges(rendererProperty);
                    this.propertyId = propertyId;
                }
            }

            public short GetCadence()
            {
                return this.cadence;
            }

            public void SetCadence(RendererAnimatorProperty rendererProperty, short cadence)
            {
                Debug.Assert(cadence >= 0 && cadence <= 2, "Cadence out of range.");

                if (this.cadence != cadence)
                {
                    FrigidEdit.RecordChanges(rendererProperty);
                    this.cadence = cadence;
                    this.animationValues.Clear();
                    this.frameValues.Clear();
                    this.orientationValues.Clear();
                    switch (this.cadence)
                    {
                        case 0:
                            for (int animationIndex = 0; animationIndex < rendererProperty.Body.GetAnimationCount(); animationIndex++)
                            {
                                this.animationValues.Add(new T());
                            }
                            break;
                        case 1:
                            for (int animationIndex = 0; animationIndex < rendererProperty.Body.GetAnimationCount(); animationIndex++)
                            {
                                this.frameValues.Add(new Nested1DList<T>());
                                for (int frameIndex = 0; frameIndex < rendererProperty.Body.GetFrameCount(animationIndex); frameIndex++)
                                {
                                    this.frameValues[animationIndex].Add(new T());
                                }
                            }
                            break;
                        case 2:
                            for (int animationIndex = 0; animationIndex < rendererProperty.Body.GetAnimationCount(); animationIndex++)
                            {
                                this.orientationValues.Add(new Nested2DList<T>());
                                for (int frameIndex = 0; frameIndex < rendererProperty.Body.GetFrameCount(animationIndex); frameIndex++)
                                {
                                    this.orientationValues[animationIndex].Add(new Nested1DList<T>());
                                    for (int orientationIndex = 0; orientationIndex < rendererProperty.Body.GetOrientationCount(animationIndex); orientationIndex++)
                                    {
                                        this.orientationValues[animationIndex][frameIndex].Add(new T());
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            public T GetValue(int animationIndex)
            {
                if (this.cadence != 0) return new T();
                return this.animationValues[animationIndex];
            }

            public T GetValue(int animationIndex, int frameIndex)
            {
                if (this.cadence != 1) return new T();
                return this.frameValues[animationIndex][frameIndex];
            }

            public T GetValue(int animationIndex, int frameIndex, int orientationIndex)
            {
                if (this.cadence != 2) return new T();
                return this.orientationValues[animationIndex][frameIndex][orientationIndex];
            }

            public void SetValue(RendererAnimatorProperty rendererProperty, int animationIndex, T value)
            {
                if (this.cadence != 0) return;
                if (!this.animationValues[animationIndex].Equals(value))
                {
                    FrigidEdit.RecordChanges(rendererProperty);
                    this.animationValues[animationIndex] = value;
                }
            }

            public void SetValue(RendererAnimatorProperty rendererProperty, int animationIndex, int frameIndex, T value)
            {
                if (this.cadence != 1) return;
                if (!this.frameValues[animationIndex][frameIndex].Equals(value))
                {
                    FrigidEdit.RecordChanges(rendererProperty);
                    this.frameValues[animationIndex][frameIndex] = value;
                }
            }

            public void SetValue(RendererAnimatorProperty rendererProperty, int animationIndex, int frameIndex, int orientationIndex, T value)
            {
                if (this.cadence != 2) return;
                if (!this.orientationValues[animationIndex][frameIndex][orientationIndex].Equals(value))
                {
                    FrigidEdit.RecordChanges(rendererProperty);
                    this.orientationValues[animationIndex][frameIndex][orientationIndex] = value;
                }
            }

            public void AnimationAddedAt(RendererAnimatorProperty rendererProperty, int animationIndex)
            {
                FrigidEdit.RecordChanges(rendererProperty);
                switch (this.cadence)
                {
                    case 0:
                        this.animationValues.Insert(animationIndex, new T());
                        break;
                    case 1:
                        this.frameValues.Insert(animationIndex, new Nested1DList<T>());
                        for (int frameIndex = 0; frameIndex < rendererProperty.Body.GetFrameCount(animationIndex); frameIndex++)
                        {
                            this.frameValues[animationIndex].Add(new T());
                        }
                        break;
                    case 2:
                        this.orientationValues.Insert(animationIndex, new Nested2DList<T>());
                        for (int frameIndex = 0; frameIndex < rendererProperty.Body.GetFrameCount(animationIndex); frameIndex++)
                        {
                            this.orientationValues[animationIndex].Add(new Nested1DList<T>());
                            for (int orientationIndex = 0; orientationIndex < rendererProperty.Body.GetOrientationCount(animationIndex); orientationIndex++)
                            {
                                this.orientationValues[animationIndex][frameIndex].Add(new T());
                            }
                        }
                        break;
                }
            }

            public void AnimationRemovedAt(RendererAnimatorProperty rendererProperty, int animationIndex)
            {
                FrigidEdit.RecordChanges(rendererProperty);
                switch (this.cadence)
                {
                    case 0:
                        this.animationValues.RemoveAt(animationIndex);
                        break;
                    case 1:
                        this.frameValues.RemoveAt(animationIndex);
                        break;
                    case 2:
                        this.orientationValues.RemoveAt(animationIndex);
                        break;
                }
            }

            public void FrameAddedAt(RendererAnimatorProperty rendererProperty, int animationIndex, int frameIndex)
            {
                FrigidEdit.RecordChanges(rendererProperty);
                switch (this.cadence)
                {
                    case 1:
                        this.frameValues[animationIndex].Insert(frameIndex, new T());
                        break;
                    case 2:
                        this.orientationValues[animationIndex].Insert(frameIndex, new Nested1DList<T>());
                        for (int orientationIndex = 0; orientationIndex < rendererProperty.Body.GetOrientationCount(animationIndex); orientationIndex++)
                        {
                            this.orientationValues[animationIndex][frameIndex].Add(new T());
                        }
                        break;
                }
            }

            public void FrameRemovedAt(RendererAnimatorProperty rendererProperty, int animationIndex, int frameIndex)
            {
                FrigidEdit.RecordChanges(rendererProperty);
                switch (this.cadence)
                {
                    case 1:
                        this.frameValues[animationIndex].RemoveAt(frameIndex);
                        break;
                    case 2:
                        this.orientationValues[animationIndex].RemoveAt(frameIndex);
                        break;
                }
            }

            public void OrientationAddedAt(RendererAnimatorProperty rendererProperty, int animationIndex, int frameIndex, int orientationIndex)
            {
                FrigidEdit.RecordChanges(rendererProperty);
                switch (this.cadence)
                {
                    case 2:
                        this.orientationValues[animationIndex][frameIndex].Insert(orientationIndex, new T());
                        break;
                }
            }

            public void OrientationRemovedAt(RendererAnimatorProperty rendererProperty, int animationIndex, int frameIndex, int orientationIndex)
            {
                FrigidEdit.RecordChanges(rendererProperty);
                switch (this.cadence)
                {
                    case 2:
                        this.orientationValues[animationIndex][frameIndex].RemoveAt(orientationIndex);
                        break;
                }
            }
        }
    }
}
