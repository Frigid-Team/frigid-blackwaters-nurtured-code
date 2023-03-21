using System;
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
        [ReadOnly]
        private List<AnimatorProperty> childProperties;
        [SerializeField]
        [HideInInspector]
        private List<bool> binds;

        public int ChildHeight
        {
            get
            {
                int childHeight = 0;
                foreach (AnimatorProperty childProperty in this.childProperties)
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
                foreach (AnimatorProperty childProperty in this.childProperties)
                {
                    childPropertyCount++;
                    childPropertyCount += childProperty.ChildPropertyCount;
                }
                return childPropertyCount;
            }
        }

        public List<AnimatorProperty> ChildProperties 
        {
            get
            {
                return this.childProperties;
            }
        }

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
                    FrigidEditMode.RecordPotentialChanges(this.gameObject);
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

        public int GetNumberChildProperties()
        {
            return this.childProperties.Count;
        }

        public AnimatorProperty GetChildPropertyAt(int index)
        {
            return this.childProperties[index];
        }

        public void AddChildPropertyAt(int index, Type propertyType)
        {
            GameObject propertyObject = FrigidEditMode.CreateGameObject(this.transform);
            AnimatorProperty childProperty = (AnimatorProperty)FrigidEditMode.AddComponent(propertyObject, propertyType);
            propertyObject.name = NEW_PROPERTY_NAME;
            childProperty.body = this.Body;
            childProperty.Created();
            FrigidEditMode.RecordPotentialChanges(this);
            this.childProperties.Insert(index, childProperty);
            this.Body.RegisterPropertyCreation(childProperty);
            childProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(childProperty));
        }

        public void RemoveChildPropertyAt(int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            AnimatorProperty childProperty = this.childProperties[index];
            this.Body.RegisterPropertyDestruction(childProperty);
            this.childProperties.RemoveAt(index);
            childProperty.Destroyed();
            FrigidEditMode.DestroyGameObject(childProperty.gameObject);
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
                this.Body.PropertyBindChanged(animationIndex);
            }
        }

        public virtual void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.childProperties = new List<AnimatorProperty>();
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

        public void SetActive(bool propertyEnabled)
        {
            if (this.gameObject.activeSelf != propertyEnabled)
            {
                this.gameObject.SetActive(propertyEnabled);
            }
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                childProperty.SetActive(propertyEnabled && childProperty.GetBinded(this.Body.CurrAnimationIndex));
            }
        }

        public virtual void AnimationEnter()
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.AnimationEnter();
                }
            }
        }

        public virtual void AnimationExit()
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.AnimationExit();
                }
            }
        }

        public virtual void FrameEnter()
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.FrameEnter();
                }
            }
        }

        public virtual void FrameExit()
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.FrameExit();
                }
            }
        }

        public virtual void OrientationEnter()
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.OrientationEnter();
                }
            }
        }

        public virtual void OrientationExit()
        {
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.OrientationExit();
                }
            }
        }

        public bool IsCompletedAtEndOfAnimation()
        {
            bool canComplete = true;
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                canComplete &= !childProperty.GetBinded(this.Body.CurrAnimationIndex) || childProperty.IsCompletedAtEndOfAnimation();
            }
            canComplete &= CanCompleteAtEndOfAnimation();
            return canComplete;
        }

        public Bounds? GetAreaOccupied()
        {
            Bounds? areaOccupied = CalculateAreaOccupied();
            foreach (AnimatorProperty childProperty in this.ChildProperties)
            {
                Bounds? childAreaOccupied = childProperty.GetAreaOccupied();
                if (childAreaOccupied.HasValue) 
                {
                    if (!areaOccupied.HasValue)
                    {
                        areaOccupied = childAreaOccupied;
                    }
                    else
                    {
                        areaOccupied.Value.Encapsulate(childAreaOccupied.Value);
                    }
                }
            }
            return areaOccupied;
        }

        protected AnimatorBody Body
        {
            get
            {
                return this.body;
            }
        }

        protected virtual bool CanCompleteAtEndOfAnimation()
        {
            return true;
        }

        protected virtual Bounds? CalculateAreaOccupied()
        {
            return null;
        }
    }
}
