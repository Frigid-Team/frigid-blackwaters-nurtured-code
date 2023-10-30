using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class AnimatorProperty : FrigidMonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private AnimatorBody body;
        [SerializeField]
        [ReadOnly]
        private List<AnimatorProperty> childProperties;
        [SerializeField]
        [HideInInspector]
        private List<bool> binds;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Vector2> localPositions;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> localRotations;

        private Action<bool> onEnabledChanged;

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
                    FrigidEdit.RecordChanges(this.gameObject);
                    this.gameObject.name = value;
                }
            }
        }

        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
        }

        public Action<bool> OnEnabledChanged
        {
            get
            {
                return this.onEnabledChanged;
            }
            set
            {
                this.onEnabledChanged = value;
            }
        }

        public virtual List<P> GetReferencedProperties<P>() where P : AnimatorProperty
        {
            List<P> referencedProperties = new List<P>();
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                P referencedProperty = childProperty as P;
                if (referencedProperty) referencedProperties.Add(referencedProperty);
                referencedProperties.AddRange(childProperty.GetReferencedProperties<P>());
            }
            return referencedProperties;
        }

        public virtual List<P> GetReferencedPropertiesIn<P>(int animationIndex) where P : AnimatorProperty
        {
            if (animationIndex == -1) return new List<P>();
            List<P> referencedProperties = new List<P>();
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                if (childProperty.GetBinded(animationIndex))
                {
                    P property = childProperty as P;
                    if (property) referencedProperties.Add(property);
                    referencedProperties.AddRange(childProperty.GetReferencedPropertiesIn<P>(animationIndex));
                }
            }
            return referencedProperties;
        }

        public int GetDescendentPropertyDepth()
        {
            int propertyDepth = 0;
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                propertyDepth = Mathf.Max(childProperty.GetDescendentPropertyDepth() + 1, propertyDepth);
            }
            return propertyDepth;
        }

        public int GetDescendentPropertyDepthOf(AnimatorProperty property)
        {
            if (this.childProperties.Contains(property))
            {
                return 0;
            }
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                if (childProperty == property)
                {
                    return 0;
                }
                
                int childDepth = childProperty.GetDescendentPropertyDepthOf(property);
                if (childDepth != -1)
                {
                    return childDepth + 1;
                }
            }
            return -1;
        }

        public int GetNumberDescendentProperties()
        {
            int numberDescendentProperties = 0;
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                numberDescendentProperties++;
                numberDescendentProperties += childProperty.GetNumberDescendentProperties();
            }
            return numberDescendentProperties;
        }

        public List<AnimatorProperty> GetDescendentProperties()
        {
            List<AnimatorProperty> descendentProperties = new List<AnimatorProperty>();
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                descendentProperties.Add(childProperty);
                descendentProperties.AddRange(childProperty.GetDescendentProperties());
            }
            return descendentProperties;
        }

        public int GetNumberChildProperties()
        {
            return this.childProperties.Count;
        }

        public AnimatorProperty GetChildPropertyAt(int childIndex)
        {
            return this.childProperties[childIndex];
        }

        public static AnimatorProperty CreateProperty(AnimatorBody body, Transform parent, Type propertyType)
        {
            GameObject propertyObject = FrigidEdit.CreateGameObject("New Property", parent);
            AnimatorProperty newProperty = (AnimatorProperty)FrigidEdit.AddComponent(propertyObject, propertyType);
            FrigidEdit.RecordChanges(newProperty);
            newProperty.body = body;
            newProperty.Created();
            return newProperty;
        }

        public static void DestroyProperty(AnimatorProperty property)
        {
            FrigidEdit.RecordChanges(property);
            property.Destroyed();
            FrigidEdit.DestroyGameObject(property.gameObject);
        }

        public void AddChildPropertyAt(int childIndex, Type propertyType)
        {
            AnimatorProperty childProperty = CreateProperty(this.Body, this.transform, propertyType);
            FrigidEdit.RecordChanges(this);
            this.childProperties.Insert(childIndex, childProperty);
            childProperty.transform.SetSiblingIndex(this.childProperties.IndexOf(childProperty));
        }

        public void RemoveChildPropertyAt(int childIndex)
        {
            AnimatorProperty childProperty = this.childProperties[childIndex];
            FrigidEdit.RecordChanges(this);
            this.childProperties.RemoveAt(childIndex);
            DestroyProperty(childProperty);
        }

        public bool GetBinded(int animationIndex)
        {
            return this.binds[animationIndex];
        }

        public void SetBinded(int animationIndex, bool binded)
        {
            if (this.binds[animationIndex] != binded)
            {
                FrigidEdit.RecordChanges(this);
                this.binds[animationIndex] = binded;
            }
        }

        public Vector2 GetLocalPosition(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.localPositions[animationIndex][frameIndex][orientationIndex];
        }

        public void SetLocalPosition(int animationIndex, int frameIndex, int orientationIndex, Vector2 localPosition)
        {
            if (this.localPositions[animationIndex][frameIndex][orientationIndex] != localPosition)
            {
                FrigidEdit.RecordChanges(this);
                this.localPositions[animationIndex][frameIndex][orientationIndex] = localPosition;
            }
        }

        public float GetLocalRotation(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.localRotations[animationIndex][frameIndex][orientationIndex];
        }

        public void SetLocalRotation(int animationIndex, int frameIndex, int orientationIndex, float localRotation)
        {
            if (this.localRotations[animationIndex][frameIndex][orientationIndex] != localRotation)
            {
                FrigidEdit.RecordChanges(this);
                this.localRotations[animationIndex][frameIndex][orientationIndex] = localRotation;
            }
        }

        public virtual void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.childProperties = new List<AnimatorProperty>();
            this.binds = new List<bool>();
            this.localPositions = new Nested3DList<Vector2>();
            this.localRotations = new Nested3DList<float>();
            for (int animationIndex = 0; animationIndex < this.body.GetAnimationCount(); animationIndex++)
            {
                this.binds.Add(false);
                this.localPositions.Add(new Nested2DList<Vector2>());
                this.localRotations.Add(new Nested2DList<float>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.localPositions[animationIndex].Add(new Nested1DList<Vector2>());
                    this.localRotations[animationIndex].Add(new Nested1DList<float>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.localPositions[animationIndex][frameIndex].Add(Vector2.zero);
                        this.localRotations[animationIndex][frameIndex].Add(0);
                    }
                }
            }
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                childProperty.Created();
            }
        }

        public virtual void Destroyed()
        {
            FrigidEdit.RecordChanges(this);
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                childProperty.Destroyed();
            }
        }

        public virtual void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.binds.Insert(animationIndex, false);
            this.localPositions.Insert(animationIndex, new Nested2DList<Vector2>());
            this.localRotations.Insert(animationIndex, new Nested2DList<float>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.localPositions[animationIndex].Add(new Nested1DList<Vector2>());
                this.localRotations[animationIndex].Add(new Nested1DList<float>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.localPositions[animationIndex][frameIndex].Add(Vector2.zero);
                    this.localRotations[animationIndex][frameIndex].Add(0);
                }
            }
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                childProperty.AnimationAddedAt(animationIndex);
            }
        }

        public virtual void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.binds.RemoveAt(animationIndex);
            this.localPositions.RemoveAt(animationIndex);
            this.localRotations.RemoveAt(animationIndex);
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                childProperty.AnimationRemovedAt(animationIndex);
            }
        }

        public virtual void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.localPositions[animationIndex].Insert(frameIndex, new Nested1DList<Vector2>());
            this.localRotations[animationIndex].Insert(frameIndex, new Nested1DList<float>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.localPositions[animationIndex][frameIndex].Add(Vector2.zero);
                this.localRotations[animationIndex][frameIndex].Add(0);
            }
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                childProperty.FrameAddedAt(animationIndex, frameIndex);
            }
        }

        public virtual void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.localPositions[animationIndex].RemoveAt(frameIndex);
            this.localRotations[animationIndex].RemoveAt(frameIndex);
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                childProperty.FrameRemovedAt(animationIndex, frameIndex);
            }
        }

        public virtual void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.localPositions[animationIndex][frameIndex].Insert(orientationIndex, Vector2.zero);
            this.localRotations[animationIndex][frameIndex].Insert(orientationIndex, 0);
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                childProperty.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
            }
        }

        public virtual void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.localPositions[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.localRotations[animationIndex][frameIndex].RemoveAt(orientationIndex);
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                childProperty.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
            }
        }

        public virtual void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex) 
        {
            otherProperty.SetBinded(toAnimationIndex, this.GetBinded(fromAnimationIndex));
        }

        public virtual void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex) { }

        public virtual void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex) 
        {
            otherProperty.SetLocalPosition(toAnimationIndex, toFrameIndex, toOrientationIndex, this.GetLocalPosition(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
            otherProperty.SetLocalRotation(toAnimationIndex, toFrameIndex, toOrientationIndex, this.GetLocalRotation(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
        }

        public virtual void Initialize() 
        {
            this.enabled = false;
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                childProperty.Initialize();
            }
        }

        public virtual void Enable(bool enabled)
        {
            if (this.enabled != enabled)
            {
                this.enabled = enabled;
                this.onEnabledChanged?.Invoke(enabled);
            }
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                childProperty.Enable(enabled && childProperty.GetBinded(this.Body.CurrAnimationIndex));
            }
        }

        public virtual void AnimationEnter()
        {
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.AnimationEnter();
                }
            }
        }

        public virtual void AnimationExit()
        {
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.AnimationExit();
                }
            }
        }

        public virtual void FrameEnter()
        {
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.FrameEnter();
                }
            }
        }

        public virtual void FrameExit()
        {
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.FrameExit();
                }
            }
        }

        public virtual void OrientationEnter()
        {
            this.transform.localPosition = this.GetLocalPosition(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
            this.transform.localRotation = Quaternion.Euler(0, 0, this.GetLocalRotation(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex));
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.OrientationEnter();
                }
            }
        }

        public virtual void OrientationExit()
        {
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                if (childProperty.GetBinded(this.Body.CurrAnimationIndex))
                {
                    childProperty.OrientationExit();
                }
            }
        }

        public virtual Bounds? GetVisibleArea()
        {
            Bounds? areaOccupied = null;
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                if (!childProperty.GetBinded(this.Body.CurrAnimationIndex)) continue;
                Bounds? childAreaOccupied = childProperty.GetVisibleArea();
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

        public virtual float GetDuration()
        {
            float duration = 0;
            foreach (AnimatorProperty childProperty in this.childProperties)
            {
                if (!childProperty.GetBinded(this.Body.CurrAnimationIndex)) continue;
                duration = Mathf.Max(childProperty.GetDuration(), duration);
            }
            return duration;
        }

        protected AnimatorBody Body
        {
            get
            {
                return this.body;
            }
        }
    }
}
