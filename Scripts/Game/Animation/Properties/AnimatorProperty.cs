using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class AnimatorProperty : FrigidMonoBehaviour
    {
        private const string NEW_PROPERTY_NAME = "New Property";

        [SerializeField]
        [ReadOnly]
        private AnimatorBody body;
        [SerializeField]
        [HideInInspector]
        private List<bool> binds;

        public int ChildHeight
        {
            get
            {
                int childHeight = 0;
                foreach (AnimatorProperty childProperty in this.ChildProperties)
                {
                    childHeight = Mathf.Max(childProperty.ChildHeight + 1, childHeight);
                }
                return childHeight;
            }
        }

        public int ChildPropertyCount
        {
            get
            {
                int childPropertyCount = 0;
                foreach (AnimatorProperty childProperty in this.ChildProperties)
                {
                    childPropertyCount++;
                    childPropertyCount += childProperty.ChildPropertyCount;
                }
                return childPropertyCount;
            }
        }

        public abstract List<AnimatorProperty> ChildProperties { get; }

        public string PropertyName
        {
            get
            {
                return this.gameObject.name;
            }
            set
            {
                if (this.gameObject.name != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this);
                    this.gameObject.name = value;
                }
            }
        }

        public static P SetupOn<P>(GameObject gameObject, AnimatorBody body) where P : AnimatorProperty
        {
            P newProperty = FrigidEditMode.AddComponent<P>(gameObject);
            newProperty.body = body;
            newProperty.binds = new List<bool>();
            newProperty.Created();
            return newProperty;
        }

        public bool GetBinded(int animationIndex)
        {
            return this.binds[animationIndex];
        }

        public void SetBinded(int animationIndex, bool binded)
        {
            if (this.binds[animationIndex] != binded)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.binds[animationIndex] = binded;
            }
        }

        public virtual void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.binds = new List<bool>();
            for (int animationIndex = 0; animationIndex < this.body.GetAnimationCount(); animationIndex++)
            {
                this.binds.Add(false);
            }
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.Created();
            }
        }

        public virtual void Destroyed()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.Destroyed();
            }
        }

        public virtual void AnimationAddedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.binds.Insert(animationIndex, false);
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.AnimationAddedAt(animationIndex);
            }
        }

        public virtual void AnimationRemovedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.binds.RemoveAt(animationIndex);
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.AnimationRemovedAt(animationIndex);
            }
        }

        public virtual void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.FrameAddedAt(animationIndex, frameIndex);
            }
        }

        public virtual void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.FrameRemovedAt(animationIndex, frameIndex);
            }
        }

        public virtual void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
            }
        }

        public virtual void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
            }
        }

        public virtual void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex) 
        {
            otherProperty.binds[toAnimationIndex] = this.binds[fromAnimationIndex];
        }

        public virtual void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex) { }

        public virtual void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex) { }

        public virtual void Initialize() 
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.Initialize();
            }
        }

        public void PreAnimationSetup(int animationIndex, bool propertyEnabled)
        {
            this.gameObject.SetActive(propertyEnabled);
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.PreAnimationSetup(animationIndex, propertyEnabled && childProperty.GetBinded(animationIndex));
            }
        }

        public virtual void Paused()
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.Paused();
            }
        }

        public virtual void UnPaused() 
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.UnPaused();
            }
        }

        public virtual void AnimationEnter(int animationIndex, float elapsedDuration)
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(animationIndex))
                {
                    childProperty.AnimationEnter(animationIndex, elapsedDuration);
                }
            }
        }

        public virtual void AnimationExit(int animationIndex)
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(animationIndex))
                {
                    childProperty.AnimationExit(animationIndex);
                }
            }
        }

        public virtual void SetFrameEnter(int animationIndex, int frameIndex, float elapsedDuration, int loopsElapsed)
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(animationIndex))
                {
                    childProperty.SetFrameEnter(animationIndex, frameIndex, elapsedDuration, loopsElapsed);
                }
            }
        }

        public virtual void SetFrameExit(int animationIndex, int frameIndex)
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(animationIndex))
                {
                    childProperty.SetFrameExit(animationIndex, frameIndex);
                }
            }
        }

        public virtual void OrientFrameEnter(int animationIndex, int frameIndex, int orientationIndex, float elapsedDuration)
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(animationIndex))
                {
                    childProperty.OrientFrameEnter(animationIndex, frameIndex, orientationIndex, elapsedDuration);
                }
            }
        }

        public virtual void OrientFrameExit(int animationIndex, int frameIndex, int orientationIndex)
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(animationIndex))
                {
                    childProperty.OrientFrameExit(animationIndex, frameIndex, orientationIndex);
                }
            }
        }

        public bool IsCompletedAtEndOfAnimation(int animationIndex, float elapsedDuration)
        {
            bool canComplete = true;
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                canComplete &= childProperty.IsCompletedAtEndOfAnimation(animationIndex, elapsedDuration);
            }
            canComplete &= CanCompleteAtEndOfAnimation(animationIndex, elapsedDuration);
            return canComplete;
        }

        protected AnimatorBody Body
        {
            get
            {
                return this.body;
            }
        }

        protected virtual bool CanCompleteAtEndOfAnimation(int animationIndex, float elapsedDuration)
        {
            return true;
        }
        
        protected P CreateSubProperty<P>() where P : AnimatorProperty
        {
            GameObject propertyObject = FrigidEditMode.CreateGameObject(this.transform);
            P newProperty = FrigidEditMode.AddComponent<P>(propertyObject);
            propertyObject.name = NEW_PROPERTY_NAME;
            newProperty.body = this.body;
            newProperty.Created();
            return newProperty;
        }

        protected void DestroySubProperty(AnimatorProperty property)
        {
            property.Destroyed();
            FrigidEditMode.DestroyGameObject(property.gameObject);
        }
    }
}
