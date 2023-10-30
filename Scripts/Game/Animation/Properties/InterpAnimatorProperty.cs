using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class InterpAnimatorProperty<P> : ParameterAnimatorProperty<P> where P : AnimatorProperty
    {
        [SerializeField]
        [HideInInspector]
        private List<Nested2DList<SerializedValueTuple<AnimationCurveSerializedReference, Span<int>>>> curvesAndRanges;

        public int GetNumberCurvesAndRanges(int propertyIndex, int animationIndex)
        {
            return this.curvesAndRanges[propertyIndex][animationIndex].Count;
        }

        public SerializedValueTuple<AnimationCurveSerializedReference, Span<int>> GetCurveAndRange(int propertyIndex, int animationIndex, int curveAndRangeIndex)
        {
            return this.curvesAndRanges[propertyIndex][animationIndex][curveAndRangeIndex];
        }

        public void SetCurveAndRange(int propertyIndex, int animationIndex, int curveAndRangeIndex, SerializedValueTuple<AnimationCurveSerializedReference, Span<int>> curveAndRange)
        {
            if (this.curvesAndRanges[propertyIndex][animationIndex][curveAndRangeIndex] != curveAndRange)
            {
                FrigidEdit.RecordChanges(this);
                this.curvesAndRanges[propertyIndex][animationIndex][curveAndRangeIndex] = curveAndRange;
            }
        }

        public void AddCurveAndRangeAt(int propertyIndex, int animationIndex, int curveAndRangeIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.curvesAndRanges[propertyIndex][animationIndex].Insert(curveAndRangeIndex, new SerializedValueTuple<AnimationCurveSerializedReference, Span<int>>(new AnimationCurveSerializedReference(), new Span<int>(0, 0)));
        }

        public void RemoveCurveAndRangeAt(int propertyIndex, int animationIndex, int curveAndRangedIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.curvesAndRanges[propertyIndex][animationIndex].RemoveAt(curveAndRangedIndex);
        }

        public override void AddParameteredPropertyAt(int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.curvesAndRanges.Insert(propertyIndex, new Nested2DList<SerializedValueTuple<AnimationCurveSerializedReference, Span<int>>>());
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.curvesAndRanges[propertyIndex].Add(new Nested1DList<SerializedValueTuple<AnimationCurveSerializedReference, Span<int>>>());
            }
            base.AddParameteredPropertyAt(propertyIndex);
        }

        public override void RemoveParameteredPropertyAt(int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.curvesAndRanges.RemoveAt(propertyIndex);
            base.RemoveParameteredPropertyAt(propertyIndex);
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.curvesAndRanges = new List<Nested2DList<SerializedValueTuple<AnimationCurveSerializedReference, Span<int>>>>();
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                this.curvesAndRanges[propertyIndex].Insert(animationIndex, new Nested1DList<SerializedValueTuple<AnimationCurveSerializedReference, Span<int>>>());
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                this.curvesAndRanges[propertyIndex].RemoveAt(animationIndex);
            }
            base.AnimationRemovedAt(animationIndex);
        }

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            if (otherProperty == this)
            {
                for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
                {
                    for (int curveAndRangeIndex = this.GetNumberCurvesAndRanges(propertyIndex, toAnimationIndex); 
                        curveAndRangeIndex < this.GetNumberCurvesAndRanges(propertyIndex, fromAnimationIndex); 
                        curveAndRangeIndex++)
                    {
                        this.AddCurveAndRangeAt(propertyIndex, toAnimationIndex, curveAndRangeIndex);
                    }
                    for (int curveAndRangeIndex = this.GetNumberCurvesAndRanges(propertyIndex, fromAnimationIndex) - 1; 
                        curveAndRangeIndex >= this.GetNumberCurvesAndRanges(propertyIndex, toAnimationIndex); 
                        curveAndRangeIndex--)
                    {
                        this.RemoveCurveAndRangeAt(propertyIndex, toAnimationIndex, curveAndRangeIndex);
                    }
                    for (int curveAndRangeIndex = 0; 
                        curveAndRangeIndex < this.GetNumberCurvesAndRanges(propertyIndex, fromAnimationIndex); 
                        curveAndRangeIndex++)
                    {
                        this.SetCurveAndRange(propertyIndex, toAnimationIndex, curveAndRangeIndex, this.GetCurveAndRange(propertyIndex, fromAnimationIndex, curveAndRangeIndex));
                    }
                }
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void Initialize()
        {
            this.InterpolateValuesInProperties();
            base.Initialize();
        }

        protected abstract void InterpolateValue(int propertyIndex, int animationIndex, int frameIndex, int orientationIndex, float progress01);

        protected void InterpolateValuesInProperties()
        {
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
                {
                    if (!this.GetBinded(animationIndex)) continue;

                    for (int curveAndRangeIndex = 0; curveAndRangeIndex < this.GetNumberCurvesAndRanges(propertyIndex, animationIndex); curveAndRangeIndex++)
                    {
                        SerializedValueTuple<AnimationCurveSerializedReference, Span<int>> curveAndRange = this.GetCurveAndRange(propertyIndex, animationIndex, curveAndRangeIndex);
                        AnimationCurve curve = curveAndRange.Item1.MutableValue;
                        Span<int> range = curveAndRange.Item2;

                        int startFrameIndex = Mathf.Max(0, range.Min);
                        int endFrameIndex = Mathf.Min(this.Body.GetFrameCount(animationIndex) - 1, range.Max);
                        for (int frameIndex = startFrameIndex; frameIndex <= endFrameIndex; frameIndex++)
                        {
                            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                            {
                                this.InterpolateValue(propertyIndex, animationIndex, frameIndex, orientationIndex, Mathf.Clamp01(curve.Evaluate((float)frameIndex / (endFrameIndex - startFrameIndex))));
                            }
                        }
                    }
                }
            }
        }
    }
}
