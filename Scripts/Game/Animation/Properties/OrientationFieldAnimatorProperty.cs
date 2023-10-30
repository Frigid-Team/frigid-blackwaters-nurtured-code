using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class OrientationFieldAnimatorProperty<P> : ParameterAnimatorProperty<P> where P : AnimatorProperty
    {
        [SerializeField]
        [HideInInspector]
        private List<Nested3DList<bool>> areValuesControlled;
        
        public bool GetIsValueControlled(int propertyIndex, int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.areValuesControlled[propertyIndex][animationIndex][frameIndex][orientationIndex];
        }

        public void SetIsValueControlled(int propertyIndex, int animationIndex, int frameIndex, int orientationIndex, bool isValueControlled)
        {
            if (this.areValuesControlled[propertyIndex][animationIndex][frameIndex][orientationIndex] != isValueControlled)
            {
                FrigidEdit.RecordChanges(this);
                this.areValuesControlled[propertyIndex][animationIndex][frameIndex][orientationIndex] = isValueControlled;
            }
        }

        public override void AddParameteredPropertyAt(int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.areValuesControlled.Insert(propertyIndex, new Nested3DList<bool>());
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.areValuesControlled[propertyIndex].Add(new Nested2DList<bool>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.areValuesControlled[propertyIndex][animationIndex].Add(new Nested1DList<bool>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.areValuesControlled[propertyIndex][animationIndex][frameIndex].Add(false);
                    }
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
            this.areValuesControlled = new List<Nested3DList<bool>>();
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                this.areValuesControlled[propertyIndex].Insert(animationIndex, new Nested2DList<bool>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.areValuesControlled[propertyIndex][animationIndex].Add(new Nested1DList<bool>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.areValuesControlled[propertyIndex][animationIndex][frameIndex].Add(false);
                    }
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
                this.areValuesControlled[propertyIndex][animationIndex].Insert(frameIndex, new Nested1DList<bool>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.areValuesControlled[propertyIndex][animationIndex][frameIndex].Add(false);
                }
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

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                this.areValuesControlled[propertyIndex][animationIndex][frameIndex].Insert(orientationIndex, false);
            }
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                this.areValuesControlled[propertyIndex][animationIndex][frameIndex].RemoveAt(orientationIndex);
            }
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            if (otherProperty == this)
            {
                for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
                {
                    this.SetIsValueControlled(propertyIndex, toAnimationIndex, toFrameIndex, toOrientationIndex, this.GetIsValueControlled(propertyIndex, fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
                }
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void Initialize()
        {
            this.SetValuesInProperties();
            base.Initialize();
        }

        protected abstract void SetValue(int propertyIndex, int animationIndex, int frameIndex, int orientationIndex);

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
                        for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                        {
                            if (!this.GetIsValueControlled(propertyIndex, animationIndex, frameIndex, orientationIndex)) continue;

                            this.SetValue(propertyIndex, animationIndex, frameIndex, orientationIndex);
                        }
                    }
                }
            }
        }
    }
}
