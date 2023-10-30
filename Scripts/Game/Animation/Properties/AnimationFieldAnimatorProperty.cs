#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class AnimationFieldAnimatorProperty<P> : ParameterAnimatorProperty<P> where P : AnimatorProperty
    {
        [SerializeField]
        [HideInInspector]
        private List<Nested1DList<bool>> areValuesControlled;

        public bool GetIsValueControlled(int propertyIndex, int animationIndex)
        {
            return this.areValuesControlled[propertyIndex][animationIndex];
        }

        public void SetIsValueControlled(int propertyIndex, int animationIndex, bool isValueControlled)
        {
            if (this.areValuesControlled[propertyIndex][animationIndex] != isValueControlled)
            {
                FrigidEdit.RecordChanges(this);
                this.areValuesControlled[propertyIndex][animationIndex] = isValueControlled;
            }
        }

        public override void AddParameteredPropertyAt(int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.areValuesControlled.Insert(propertyIndex, new Nested1DList<bool>());
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.areValuesControlled[propertyIndex].Add(false);
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
            this.areValuesControlled = new List<Nested1DList<bool>>();
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                this.areValuesControlled[propertyIndex].Insert(animationIndex, false);
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

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            if (otherProperty == this)
            {
                for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
                {
                    this.SetIsValueControlled(propertyIndex, toAnimationIndex, this.GetIsValueControlled(propertyIndex, fromAnimationIndex));
                }
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void Initialize()
        {
            this.SetValuesInProperties();
            base.Initialize();
        }

        protected abstract void SetValue(int propertyIndex, int animationIndex);

        protected void SetValuesInProperties()
        {
            for (int propertyIndex = 0; propertyIndex < this.GetNumberParameteredProperties(); propertyIndex++)
            {
                P parameteredProperty = this.GetParameteredProperty(propertyIndex);
                if (parameteredProperty == null) continue;

                for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
                {
                    if (!this.GetBinded(animationIndex)) continue;

                    if (!this.GetIsValueControlled(propertyIndex, animationIndex)) continue;

                    this.SetValue(propertyIndex, animationIndex);
                }
            }
        }
    }
}
#endif
