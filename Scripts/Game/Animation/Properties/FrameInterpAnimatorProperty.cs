using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class FrameInterpAnimatorProperty : AnimatorProperty
    {
        [SerializeField]
        [HideInInspector]
        private Nested2DList<SerializedValueTuple<AnimatorProperty, AnimationCurveSerializedReference>> interpolatedProperties;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<SerializedValueTuple<int, int>> frameRanges; 

        public int GetNumberInterpolatedProperties(int animationIndex)
        {
            return this.interpolatedProperties[animationIndex].Count;
        }

        public SerializedValueTuple<AnimatorProperty, AnimationCurveSerializedReference> GetInterpolatedProperty(int animationIndex, int propertyIndex)
        {
            return this.interpolatedProperties[animationIndex][propertyIndex];
        }

        public void SetInterpolatedProperty(int animationIndex, int propertyIndex, SerializedValueTuple<AnimatorProperty, AnimationCurveSerializedReference> interpolatedProperty)
        {
            if (interpolatedProperty != this.interpolatedProperties[animationIndex][propertyIndex])
            {
                FrigidEdit.RecordChanges(this);
                this.interpolatedProperties[animationIndex][propertyIndex] = interpolatedProperty;
            }
        }

        public void AddInterpolatedPropertyAt(int animationIndex, int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.interpolatedProperties[animationIndex].Insert(propertyIndex, new SerializedValueTuple<AnimatorProperty, AnimationCurveSerializedReference>(null, new AnimationCurveSerializedReference()));
        }

        public void RemoveInterpolatedPropertyAt(int animationIndex, int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.interpolatedProperties[animationIndex].RemoveAt(propertyIndex);
        }

        public int GetNumberFrameRanges(int animationIndex)
        {
            return this.frameRanges[animationIndex].Count;
        } 

        public SerializedValueTuple<int, int> GetFrameRange(int animationIndex, int rangeIndex)
        {
            return this.frameRanges[animationIndex][rangeIndex];
        }

        public void SetFrameRange(int animationIndex, int rangeIndex, SerializedValueTuple<int, int> frameRange)
        {
            if (frameRange != this.frameRanges[animationIndex][rangeIndex])
            {
                FrigidEdit.RecordChanges(this);
                this.frameRanges[animationIndex][rangeIndex] = frameRange;
            }
        }

        public void AddFrameRangeAt(int animationIndex, int rangeIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.frameRanges[animationIndex].Insert(rangeIndex, new SerializedValueTuple<int, int>(0, 0));
        }

        public void RemoveFrameRangeAt(int animationIndex, int rangeIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.frameRanges[animationIndex].RemoveAt(rangeIndex);
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.interpolatedProperties = new Nested2DList<SerializedValueTuple<AnimatorProperty, AnimationCurveSerializedReference>>();
            this.frameRanges = new Nested2DList<SerializedValueTuple<int, int>>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.interpolatedProperties.Add(new Nested1DList<SerializedValueTuple<AnimatorProperty, AnimationCurveSerializedReference>>());
                this.frameRanges.Add(new Nested1DList<SerializedValueTuple<int, int>>());
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.interpolatedProperties.Insert(animationIndex, new Nested1DList<SerializedValueTuple<AnimatorProperty, AnimationCurveSerializedReference>>());
            this.frameRanges.Insert(animationIndex, new Nested1DList<SerializedValueTuple<int, int>>());
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.interpolatedProperties.RemoveAt(animationIndex);
            this.frameRanges.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            FrameInterpAnimatorProperty otherFrameInterpolatedProperty = otherProperty as FrameInterpAnimatorProperty;
            if (otherFrameInterpolatedProperty)
            {
                for (int rangeIndex = otherFrameInterpolatedProperty.GetNumberFrameRanges(toAnimationIndex); rangeIndex < this.GetNumberFrameRanges(fromAnimationIndex); rangeIndex++)
                {
                    otherFrameInterpolatedProperty.AddFrameRangeAt(toAnimationIndex, rangeIndex);
                }
                for (int rangeIndex = this.GetNumberFrameRanges(fromAnimationIndex) - 1; rangeIndex >= otherFrameInterpolatedProperty.GetNumberFrameRanges(toAnimationIndex); rangeIndex--)
                {
                    otherFrameInterpolatedProperty.RemoveFrameRangeAt(toAnimationIndex, rangeIndex);
                }
                for (int rangeIndex = 0; rangeIndex < this.GetNumberFrameRanges(fromAnimationIndex); rangeIndex++)
                {
                    otherFrameInterpolatedProperty.SetFrameRange(toAnimationIndex, rangeIndex, this.GetFrameRange(fromAnimationIndex, rangeIndex));
                }
                for (int propertyIndex = otherFrameInterpolatedProperty.GetNumberInterpolatedProperties(toAnimationIndex); propertyIndex < this.GetNumberInterpolatedProperties(fromAnimationIndex); propertyIndex++)
                {
                    otherFrameInterpolatedProperty.AddInterpolatedPropertyAt(toAnimationIndex, propertyIndex);
                }
                for (int propertyIndex = this.GetNumberInterpolatedProperties(fromAnimationIndex) - 1; propertyIndex >= otherFrameInterpolatedProperty.GetNumberInterpolatedProperties(toAnimationIndex); propertyIndex--)
                {
                    otherFrameInterpolatedProperty.RemoveInterpolatedPropertyAt(toAnimationIndex, propertyIndex);
                }
                for (int propertyIndex = 0; propertyIndex < this.GetNumberInterpolatedProperties(fromAnimationIndex); propertyIndex++)
                {
                    otherFrameInterpolatedProperty.SetInterpolatedProperty(toAnimationIndex, propertyIndex, this.GetInterpolatedProperty(fromAnimationIndex, propertyIndex));
                }
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void Initialize()
        {
            this.InterpolateProperties();
            base.Initialize();
        }

        protected abstract void InterpolateValue(AnimatorProperty interpolatedProperty, int animationIndex, int frameIndex, int orientationIndex, float progress01);

        protected void InterpolateProperties()
        {
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                if (!this.GetBinded(animationIndex)) continue;

                for (int propertyIndex = 0; propertyIndex < this.GetNumberInterpolatedProperties(animationIndex); propertyIndex++)
                {
                    SerializedValueTuple<AnimatorProperty, AnimationCurveSerializedReference> interpolatedProperty = this.GetInterpolatedProperty(animationIndex, propertyIndex);
                    AnimatorProperty property = interpolatedProperty.Item1;
                    AnimationCurve curve = interpolatedProperty.Item2.MutableValue;
                    for (int rangeIndex = 0; rangeIndex < this.GetNumberFrameRanges(animationIndex); rangeIndex++)
                    {
                        SerializedValueTuple<int, int> frameRange = this.GetFrameRange(animationIndex, rangeIndex);
                        int startIndex = frameRange.Item1;
                        int endIndex = frameRange.Item2;
                        for (int frameIndex = Mathf.Max(0, startIndex); frameIndex <= Mathf.Min(this.Body.GetFrameCount(animationIndex) - 1, endIndex); frameIndex++)
                        {
                            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                            {
                                this.InterpolateValue(property, animationIndex, frameIndex, orientationIndex, Mathf.Clamp01(curve.Evaluate((float)frameIndex / (endIndex - startIndex))));
                            }
                        }
                    }
                }
            }
        }
    }
}
