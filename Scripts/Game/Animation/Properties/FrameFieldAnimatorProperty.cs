using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class FrameFieldAnimatorProperty<P> : ParameterAnimatorProperty<P> where P : AnimatorProperty
    {
        [SerializeField]
        [HideInInspector]
        private List<Nested2DList<bool>> areValuesControlled;

        public bool GetIsValueControlled(int propertyIndex, int animationIndex, int frameIndex)
        {
            return this.areValuesControlled[propertyIndex][animationIndex][frameIndex];
        }

        public void SetIsValueControlled(int propertyIndex, int animationIndex, int frameIndex, bool isValueControlled)
        {
            if (this.areValuesControlled[propertyIndex][animationIndex][frameIndex] != isValueControlled)
            {
                FrigidEdit.RecordChanges(this);
                this.areValuesControlled[propertyIndex][animationIndex][frameIndex] = isValueControlled;
            }
        }

        public override void AddParameteredPropertyAt(int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.areValuesControlled.Insert(propertyIndex, new Nested2DList<bool>());
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.areValuesControlled[propertyIndex].Add(new Nested1DList<bool>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.areValuesControlled[propertyIndex][animationIndex].Add(false);
                }
            }
            base.AddParameteredPropertyAt(propertyIndex);
        }

        public override void RemoveParameteredPropertyAt(int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.areValuesControlled.RemoveAt(propertyIndex);
            base.RemoveParameteredPropertyAt(propertyIndex);
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.areValuesControlled = new List<Nested2DList<bool>>();
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                this.areValuesControlled[propertyIndex].Insert(animationIndex, new Nested1DList<bool>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.areValuesControlled[propertyIndex][animationIndex].Add(false);
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                this.areValuesControlled[propertyIndex].RemoveAt(animationIndex);
            }
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                this.areValuesControlled[propertyIndex][animationIndex].Insert(frameIndex, false);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                this.areValuesControlled[propertyIndex][animationIndex].RemoveAt(frameIndex);
            }
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            if (otherProperty == this)
            {
                for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
                {
                    this.SetIsValueControlled(propertyIndex, toAnimationIndex, toFrameIndex, this.GetIsValueControlled(propertyIndex, fromAnimationIndex, fromFrameIndex));
                }
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void Initialize()
        {
            this.SetValuesInProperties();
            base.Initialize();
        }

        protected abstract void SetValue(int propertyIndex, int animationIndex, int frameIndex);

        protected void SetValuesInProperties()
        {
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                P parameteredProperty = this.GetParameteredProperty(propertyIndex);
                if (parameteredProperty == null) continue;

                for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
                {
                    if (!this.GetBinded(animationIndex)) continue;

                    for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                    {
                        if (!this.GetIsValueControlled(propertyIndex, animationIndex, frameIndex)) continue;

                        this.SetValue(propertyIndex, animationIndex, frameIndex);
                    }
                }
            }
        }
    }
}
