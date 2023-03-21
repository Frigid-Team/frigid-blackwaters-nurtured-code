using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class AnimatorBody : FrigidMonoBehaviourWithUpdate
    {
        private const string NEW_ANIMATION_NAME = "New Animation";
        private const float NEW_FRAME_RATE = 12f;

        [SerializeField]
        private bool rotateToDirection;
        [SerializeField]
        [HideInInspector]
        private int animationCount;
        [SerializeField]
        [HideInInspector]
        private List<string> animationNames;
        [SerializeField]
        [HideInInspector]
        private List<bool> loopings;
        [SerializeField]
        [HideInInspector]
        private List<float> frameRates;
        [SerializeField]
        [HideInInspector]
        private List<int> frameCounts;
        [SerializeField]
        [HideInInspector]
        private List<int> orientationCounts;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<Vector2> orientationDirections;
        [SerializeField]
        [ReadOnly]
        private AnimatorProperty rootProperty;
        [SerializeField]
        [ReadOnly]
        private List<SubAnimatorBodyAnimatorProperty> subBodyProperties;
        [SerializeField]
        [ReadOnly]
        private Nested2DList<SubAnimatorBodyAnimatorProperty> subBodyPropertiesPerAnimation;

        private float timeScale;
        private Action onTimeScaleChanged;

        private bool playAnimationDirty;
        private int playedAnimationIndex;
        private float selfElapsedDuration;
        private Vector2 direction;

        private int currAnimationIndex;
        private Action<int, int> onAnimationUpdated;
        private int currFrameIndex;
        private Action<int, int> onFrameUpdated;
        private int currOrientationIndex;
        private Action<int, int> onOrientationUpdated;

        private bool completedSelf = false;
        private Action onComplete;

        public bool RotateToDirection
        {
            get
            {
                return this.rotateToDirection;
            }
            set
            {
                if (value != this.rotateToDirection)
                {
                    FrigidEditMode.RecordPotentialChanges(this);
                    this.rotateToDirection = value;
                }
            }
        }

        public Vector2 Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                this.direction = value.normalized;
                if (this.gameObject.activeInHierarchy)
                {
                    RefreshOrientation();
                }
                foreach (SubAnimatorBodyAnimatorProperty subBodyProperty in GetSubBodyProperties())
                {
                    subBodyProperty.SubBody.direction = value;
                }
                UpdateLocalRotation();
            }
        }

        public bool Active
        {
            get
            {
                return this.gameObject.activeSelf;
            }
            set
            {
                this.gameObject.SetActive(value);
            }
        }

        public float TimeScale
        {
            get
            {
                return this.timeScale;
            }
            set
            {
                if (this.timeScale != value)
                {
                    this.timeScale = value;
                    this.onTimeScaleChanged?.Invoke();
                    foreach (SubAnimatorBodyAnimatorProperty subBodyProperty in GetSubBodyProperties())
                    {
                        subBodyProperty.SubBody.TimeScale = value;
                    }
                }
            }
        }

        public Action OnTimeScaleChanged
        {
            get
            {
                return this.onTimeScaleChanged;
            }
            set
            {
                this.onTimeScaleChanged = value;
            }
        }

        public Bounds TotalAreaOccupied
        {
            get
            {
                Bounds? totalAreaOccupied = this.RootProperty.GetAreaOccupied();
                foreach (SubAnimatorBodyAnimatorProperty subBodyProperty in GetCurrentSubBodyProperties())
                {
                    Bounds subBodyAreaOccupied = subBodyProperty.SubBody.TotalAreaOccupied;
                    if (!totalAreaOccupied.HasValue)
                    {
                        totalAreaOccupied = subBodyAreaOccupied;
                    }
                    else
                    {
                        totalAreaOccupied.Value.Encapsulate(subBodyAreaOccupied);
                    }
                }
                return totalAreaOccupied.HasValue ? totalAreaOccupied.Value : new Bounds(this.transform.position, Vector3.zero);
            }
        }

        public float ElapsedDuration
        {
            get
            {
                float elapsedDuration = this.SelfElapsedDuration;
                foreach (SubAnimatorBodyAnimatorProperty subBodyProperty in GetCurrentSubBodyProperties())
                {
                    elapsedDuration = Mathf.Max(subBodyProperty.SubBody.ElapsedDuration, elapsedDuration);
                }
                return elapsedDuration;
            }
        }

        public float RemainingDuration
        {
            get
            {
                float remainingDuration = this.SelfRemainingDuration;
                foreach (SubAnimatorBodyAnimatorProperty subBodyProperty in GetCurrentSubBodyProperties())
                {
                    remainingDuration = Mathf.Max(subBodyProperty.SubBody.RemainingDuration, remainingDuration);
                }
                return remainingDuration;
            }
        }

        public float TotalDuration
        {
            get
            {
                float totalDuration = this.SelfTotalDuration;
                foreach (SubAnimatorBodyAnimatorProperty subBodyProperty in GetCurrentSubBodyProperties())
                {
                    totalDuration = Mathf.Max(subBodyProperty.SubBody.SelfTotalDuration, totalDuration);
                }
                return totalDuration;
            }
        }

        public int CurrentCycleIndex
        {
            get
            {
                if (this.CurrAnimationIndex == -1) return 0;
                return Mathf.FloorToInt(this.SelfElapsedDuration / (GetFrameCount(this.CurrAnimationIndex) / GetFrameRate(this.CurrAnimationIndex)));
            }
        }

        public int CurrAnimationIndex
        {
            get
            {
                return this.currAnimationIndex;
            }
        }

        public Action<int, int> OnAnimationUpdated
        {
            get
            {
                return this.onAnimationUpdated;
            }
            set
            {
                this.onAnimationUpdated = value;
            }
        }

        public int CurrFrameIndex
        {
            get
            {
                return this.currFrameIndex;
            }
        }

        public Action<int, int> OnFrameUpdated
        {
            get
            {
                return this.onFrameUpdated;
            }
            set
            {
                this.onFrameUpdated = value;
            }
        }

        public int CurrOrientationIndex
        {
            get
            {
                return this.currOrientationIndex;
            }
        }

        public Action<int, int> OnOrientationUpdated
        {
            get
            {
                return this.onOrientationUpdated;
            }
            set
            {
                this.onOrientationUpdated = value;
            }
        }

        public AnimatorProperty RootProperty
        {
            get
            {
                return this.rootProperty;
            }
        }

        public List<AnimatorProperty> Properties
        {
            get
            {
                List<AnimatorProperty> properties = new List<AnimatorProperty>();
                void Visit(AnimatorProperty property)
                {
                    properties.Add(property);
                    foreach (AnimatorProperty childProperty in property.ChildProperties) Visit(childProperty);
                }
                Visit(this.RootProperty);
                return properties;
            }
        }

        public int PropertyCount
        {
            get
            {
                return 1 + this.RootProperty.ChildPropertyCount;
            }
        }

        public int PropertyDepth
        {
            get
            {
                return this.RootProperty.ChildHeight;
            }
        }

        public static AnimatorBody SetupOn(GameObject gameObject)
        {
            AnimatorBody body = FrigidEditMode.AddComponent<AnimatorBody>(gameObject);
            if (body == null) return null;
            body.animationCount = 0;
            body.animationNames = new List<string>();
            body.frameRates = new List<float>();
            body.loopings = new List<bool>();
            body.frameCounts = new List<int>();
            body.orientationCounts = new List<int>();
            body.orientationDirections = new Nested2DList<Vector2>();
            body.rootProperty = AnimatorProperty.SetupOn<RootAnimatorProperty>(gameObject, body);
            body.subBodyProperties = new List<SubAnimatorBodyAnimatorProperty>();
            body.subBodyPropertiesPerAnimation = new Nested2DList<SubAnimatorBodyAnimatorProperty>();
            return body;
        }

        public bool Play(string animationName, Action onComplete = null)
        {
            int animationIndex = this.animationNames.IndexOf(animationName);
            return Play(animationIndex, onComplete);
        }

        public bool Play(int animationIndex, Action onComplete = null)
        {
            if (animationIndex != -1)
            {
                this.playAnimationDirty = true;
                this.playedAnimationIndex = animationIndex;
                this.completedSelf = false;
                this.onComplete = onComplete;
                this.selfElapsedDuration = 0;
                if (this.gameObject.activeInHierarchy)
                {
                    RefreshAnimation();
                }
                this.playAnimationDirty = false;
                return true;
            }
            return false;
        }

        public void Stop()
        {
            this.playedAnimationIndex = -1;
            this.onComplete = null;
            this.selfElapsedDuration = 0;
            if (this.gameObject.activeInHierarchy)
            {
                RefreshAnimation();
            }
        }

        public bool TryFindProperty<P>(string propertyName, out P property) where P : AnimatorProperty
        {
            foreach (P searchProperty in GetProperties<P>())
            {
                if (searchProperty.PropertyName == propertyName)
                {
                    property = searchProperty;
                    return true;
                }
            }
            property = null;
            return false;
        }

        public bool TryFindCurrentProperty<P>(string propertyName, out P currentProperty) where P : AnimatorProperty
        {
            return TryFindPropertyIn<P>(this.CurrAnimationIndex, propertyName, out currentProperty);
        }

        public bool TryFindPropertyIn<P>(string animationName, string propertyName, out P currentProperty) where P : AnimatorProperty
        {
            return TryFindPropertyIn<P>(this.animationNames.IndexOf(animationName), propertyName, out currentProperty);
        }

        public bool TryFindPropertyIn<P>(int animationIndex, string propertyName, out P currentProperty) where P : AnimatorProperty
        {
            foreach (P searchProperty in GetPropertiesIn<P>(animationIndex))
            {
                if (searchProperty.PropertyName == propertyName)
                {
                    currentProperty = searchProperty;
                    return true;
                }
            }
            currentProperty = null;
            return false;
        }

        public List<P> GetProperties<P>() where P : AnimatorProperty
        {
            List<P> properties = new List<P>();
            void Visit(AnimatorProperty baseProperty)
            {
                P property = baseProperty as P;
                if (property) properties.Add(property);
                foreach (AnimatorProperty childProperty in baseProperty.ChildProperties) Visit(childProperty);
            }
            Visit(this.rootProperty);
            foreach (SubAnimatorBodyAnimatorProperty subBodyProperty in this.subBodyProperties)
            {
                properties.AddRange(subBodyProperty.SubBody.GetProperties<P>());
            }
            return properties;
        }

        public List<P> GetCurrentProperties<P>() where P : AnimatorProperty
        {
            return GetPropertiesIn<P>(this.CurrAnimationIndex);
        }

        public List<P> GetPropertiesIn<P>(string animationName) where P : AnimatorProperty
        {
            return GetPropertiesIn<P>(this.animationNames.IndexOf(animationName));
        }
 
        public List<P> GetPropertiesIn<P>(int animationIndex) where P : AnimatorProperty
        {
            List<P> properties = new List<P>();

            if (animationIndex == -1) return properties;

            void Visit(AnimatorProperty baseProperty, bool enabled)
            {
                if (enabled)
                {
                    P property = baseProperty as P;
                    if (property) properties.Add(property);
                    foreach (AnimatorProperty childProperty in baseProperty.ChildProperties) Visit(childProperty, childProperty.GetBinded(animationIndex));
                }
            }
            Visit(this.rootProperty, true);
            foreach (SubAnimatorBodyAnimatorProperty subBodyProperty in this.subBodyPropertiesPerAnimation[animationIndex])
            {
                properties.AddRange(subBodyProperty.SubBody.GetPropertiesIn<P>(subBodyProperty.GetSubAnimationIndex(animationIndex)));
            }
            return properties;
        }

        public void RegisterPropertyCreation(AnimatorProperty property)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            SubAnimatorBodyAnimatorProperty subBodyProperty = property as SubAnimatorBodyAnimatorProperty;
            if (subBodyProperty)
            {
                this.subBodyProperties.Add(subBodyProperty);
                for (int animationIndex = 0; animationIndex < GetAnimationCount(); animationIndex++)
                {
                    void Visit(AnimatorProperty property, bool enabled)
                    {
                        if (enabled)
                        {
                            if (property == subBodyProperty)
                            {
                                this.subBodyPropertiesPerAnimation[animationIndex].Add(subBodyProperty);
                                return;
                            }
                            foreach (AnimatorProperty childProperty in property.ChildProperties) Visit(childProperty, childProperty.GetBinded(animationIndex));
                        }
                    }
                    Visit(this.rootProperty, true);
                }
            }
        }

        public void RegisterPropertyDestruction(AnimatorProperty property)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            SubAnimatorBodyAnimatorProperty subBodyProperty = property as SubAnimatorBodyAnimatorProperty;
            if (subBodyProperty)
            {
                this.subBodyProperties.Remove(subBodyProperty);
                for (int animationIndex = 0; animationIndex < GetAnimationCount(); animationIndex++)
                {
                    void Visit(AnimatorProperty property, bool enabled)
                    {
                        if (enabled)
                        {
                            if (property == subBodyProperty)
                            {
                                this.subBodyPropertiesPerAnimation[animationIndex].Remove(subBodyProperty);
                                return;
                            }
                            foreach (AnimatorProperty childProperty in property.ChildProperties) Visit(childProperty, childProperty.GetBinded(animationIndex));
                        }
                    }
                    Visit(this.rootProperty, true);
                }
            }
        }

        public void PropertyBindChanged(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            void Visit(AnimatorProperty property, bool enabled)
            {
                SubAnimatorBodyAnimatorProperty subBodyProperty = property as SubAnimatorBodyAnimatorProperty;
                if (subBodyProperty)
                {
                    if (enabled)
                    {
                        if (!this.subBodyPropertiesPerAnimation[animationIndex].Contains(subBodyProperty))
                        {
                            this.subBodyPropertiesPerAnimation[animationIndex].Add(subBodyProperty);
                        }
                    }
                    else
                    {
                        if (this.subBodyPropertiesPerAnimation[animationIndex].Contains(subBodyProperty))
                        {
                            this.subBodyPropertiesPerAnimation[animationIndex].Remove(subBodyProperty);
                        }
                    }
                }
                foreach (AnimatorProperty childProperty in property.ChildProperties) Visit(childProperty, enabled && childProperty.GetBinded(animationIndex));
            }
            Visit(this.rootProperty, true);
        }

        public int GetDepthOf(AnimatorProperty property)
        {
            int foundDepth = -1;
            void Visit(AnimatorProperty currProperty, int depth)
            {
                if (property == currProperty)
                {
                    foundDepth = depth;
                    return;
                }
                foreach (AnimatorProperty childProperty in currProperty.ChildProperties)
                {
                    Visit(childProperty, depth + 1);
                }
            }
            Visit(this.rootProperty, 0);
            return foundDepth;
        }

        public void AddAnimationAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.animationCount++;
            this.animationNames.Insert(animationIndex, NEW_ANIMATION_NAME);
            this.frameRates.Insert(animationIndex, NEW_FRAME_RATE);
            this.loopings.Insert(animationIndex, false);
            this.frameCounts.Insert(animationIndex, 0);
            this.orientationCounts.Insert(animationIndex, 0);
            this.orientationDirections.Insert(animationIndex, new Nested1DList<Vector2>());
            this.rootProperty.AnimationAddedAt(animationIndex);
            this.subBodyPropertiesPerAnimation.Insert(animationIndex, new Nested1DList<SubAnimatorBodyAnimatorProperty>());
        }

        public void RemoveAnimationAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.animationCount--;
            this.animationNames.RemoveAt(animationIndex);
            this.frameRates.RemoveAt(animationIndex);
            this.loopings.RemoveAt(animationIndex);
            this.frameCounts.RemoveAt(animationIndex);
            this.orientationCounts.RemoveAt(animationIndex);
            this.orientationDirections.RemoveAt(animationIndex);
            this.rootProperty.AnimationRemovedAt(animationIndex);
            this.subBodyPropertiesPerAnimation.RemoveAt(animationIndex);
        }

        public int GetAnimationCount()
        {
            return this.animationCount;
        }

        public string GetAnimationName(int animationIndex)
        {
            return this.animationNames[animationIndex];
        }

        public void SetAnimationName(int animationIndex, string animationName)
        {
            if (this.animationNames[animationIndex] != animationName)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.animationNames[animationIndex] = animationName;
            }
        }

        public bool GetLooping(int animationIndex)
        {
            return this.loopings[animationIndex];
        }

        public void SetLooping(int animationIndex, bool looping)
        {
            if (this.loopings[animationIndex] != looping)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.loopings[animationIndex] = looping;
            }
        }

        public float GetFrameRate(int animationIndex)
        {
            return this.frameRates[animationIndex];
        }

        public void SetFrameRate(int animationIndex, float frameRate)
        {
            if (this.frameRates[animationIndex] != frameRate)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.frameRates[animationIndex] = frameRate;
            }
        }

        public int GetFrameCount(int animationIndex)
        {
            return this.frameCounts[animationIndex];
        }

        public void SetFrameCount(int animationIndex, int frameCount)
        {
            frameCount = Mathf.Max(0, frameCount);
            for (int frameIndex = this.frameCounts[animationIndex]; frameIndex < frameCount; frameIndex++)
            {
                AddFrameAt(animationIndex, frameIndex);
            }
            for (int frameIndex = this.frameCounts[animationIndex] - 1; frameIndex > frameCount - 1; frameIndex--)
            {
                RemoveFrameAt(animationIndex, frameIndex);
            }
        }

        public void AddFrameAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.frameCounts[animationIndex]++;
            this.rootProperty.FrameAddedAt(animationIndex, frameIndex);
        }

        public void RemoveFrameAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.frameCounts[animationIndex]--;
            this.rootProperty.FrameRemovedAt(animationIndex, frameIndex);
        }

        public int GetOrientationCount(int animationIndex)
        {
            return this.orientationCounts[animationIndex];
        }

        public void SetOrientationCount(int animationIndex, int orientationCount)
        {
            orientationCount = Mathf.Max(0, orientationCount);
            for (int orientationIndex = this.orientationCounts[animationIndex]; orientationIndex < orientationCount; orientationIndex++)
            {
                AddOrientationAt(animationIndex, orientationIndex);
            }
            for (int orientationIndex = this.orientationCounts[animationIndex] - 1; orientationIndex > orientationCount - 1; orientationIndex--)
            {
                RemoveOrientationAt(animationIndex, orientationIndex);
            }
        }

        public void AddOrientationAt(int animationIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.orientationCounts[animationIndex]++;
            this.orientationDirections[animationIndex].Insert(orientationIndex, Vector2.zero);
            for (int frameIndex = 0; frameIndex < GetFrameCount(animationIndex); frameIndex++)
            {
                this.rootProperty.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
            }
        }

        public void RemoveOrientationAt(int animationIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.orientationCounts[animationIndex]--;
            this.orientationDirections[animationIndex].RemoveAt(orientationIndex);
            for (int frameIndex = 0; frameIndex < GetFrameCount(animationIndex); frameIndex++)
            {
                this.rootProperty.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
            }
        }

        public Vector2 GetOrientationDirection(int animationIndex, int orientationIndex)
        {
            return this.orientationDirections[animationIndex][orientationIndex];
        }

        public void SetOrientationDirection(int animationIndex, int orientationIndex, Vector2 direction)
        {
            if (this.orientationDirections[animationIndex][orientationIndex] != direction)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.orientationDirections[animationIndex][orientationIndex] = direction;
            }
        }

        public void CopyPasteAnimation(int fromAnimationIndex, int toAnimationIndex)
        {
            SetAnimationName(toAnimationIndex, GetAnimationName(fromAnimationIndex));
            SetOrientationCount(toAnimationIndex, GetOrientationCount(fromAnimationIndex));
            for (int orientationIndex = 0; orientationIndex < GetOrientationCount(toAnimationIndex); orientationIndex++)
            {
                SetOrientationDirection(toAnimationIndex, orientationIndex, GetOrientationDirection(fromAnimationIndex, orientationIndex));
            }
            SetFrameCount(toAnimationIndex, GetFrameCount(fromAnimationIndex));
            SetLooping(toAnimationIndex, GetLooping(fromAnimationIndex));
            SetFrameRate(toAnimationIndex, GetFrameRate(fromAnimationIndex));

            List<AnimatorProperty> properties = this.Properties;
            for (int propertyIndex = 0; propertyIndex < properties.Count; propertyIndex++)
            {
                properties[propertyIndex].CopyPasteToAnotherAnimation(properties[propertyIndex], fromAnimationIndex, toAnimationIndex);
                for (int frameIndex = 0; frameIndex < Mathf.Min(GetFrameCount(fromAnimationIndex), GetFrameCount(toAnimationIndex)); frameIndex++)
                {
                    properties[propertyIndex].CopyPasteToAnotherFrame(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex);
                    for (int orientationIndex = 0; orientationIndex < Mathf.Min(GetOrientationCount(fromAnimationIndex), GetOrientationCount(toAnimationIndex)); orientationIndex++)
                    {
                        properties[propertyIndex].CopyPasteToAnotherOrientation(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex, orientationIndex, orientationIndex);
                    }
                }
            }
        }

        public void CopyPasteAllFramesAndTheirOrientationsAcrossAllProperties(int fromAnimationIndex, int toAnimationIndex)
        {
            List<AnimatorProperty> properties = this.Properties;
            for (int propertyIndex = 0; propertyIndex < properties.Count; propertyIndex++)
            {
                properties[propertyIndex].CopyPasteToAnotherAnimation(properties[propertyIndex], fromAnimationIndex, toAnimationIndex);
                for (int frameIndex = 0; frameIndex < Mathf.Min(GetFrameCount(fromAnimationIndex), GetFrameCount(toAnimationIndex)); frameIndex++)
                {
                    properties[propertyIndex].CopyPasteToAnotherFrame(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex);
                    for (int orientationIndex = 0; orientationIndex < Mathf.Min(GetOrientationCount(fromAnimationIndex), GetOrientationCount(toAnimationIndex)); orientationIndex++)
                    {
                        properties[propertyIndex].CopyPasteToAnotherOrientation(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex, orientationIndex, orientationIndex);
                    }
                }
            }
        }

        public void CopyPasteFrameAndItsOrientationsAcrossAllProperties(int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            List<AnimatorProperty> properties = this.Properties;
            for (int propertyIndex = 0; propertyIndex < properties.Count; propertyIndex++)
            {
                properties[propertyIndex].CopyPasteToAnotherFrame(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
                for (int orientationIndex = 0; orientationIndex < Mathf.Min(GetOrientationCount(fromAnimationIndex), GetOrientationCount(toAnimationIndex)); orientationIndex++)
                {
                    properties[propertyIndex].CopyPasteToAnotherOrientation(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, orientationIndex, orientationIndex);
                }
            }
        }

        public void CopyPasteAllFramesAndTheirOrientations(int fromPropertyIndex, int toPropertyIndex, int fromAnimationIndex, int toAnimationIndex) 
        {
            List<AnimatorProperty> properties = this.Properties;
            for (int frameIndex = 0; frameIndex < Mathf.Min(GetFrameCount(fromAnimationIndex), GetFrameCount(toAnimationIndex)); frameIndex++)
            {
                properties[fromPropertyIndex].CopyPasteToAnotherFrame(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex);
                for (int orientationIndex = 0; orientationIndex < Mathf.Min(GetOrientationCount(fromAnimationIndex), GetOrientationCount(toAnimationIndex)); orientationIndex++)
                {
                    properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex, orientationIndex, orientationIndex);
                }
            }
        }

        public void CopyPasteFrameAndItsOrientations(int fromPropertyIndex, int toPropertyIndex, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            List<AnimatorProperty> properties = this.Properties;
            properties[fromPropertyIndex].CopyPasteToAnotherFrame(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
            for (int orientationIndex = 0; orientationIndex < Mathf.Min(GetOrientationCount(fromAnimationIndex), GetOrientationCount(toAnimationIndex)); orientationIndex++)
            {
                properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, orientationIndex, orientationIndex);
            }
        }

        public void CopyPasteOrientationAcrossAllFrames(int fromPropertyIndex, int toPropertyIndex, int fromAnimationIndex, int toAnimationIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            List<AnimatorProperty> properties = this.Properties;
            for (int frameIndex = 0; frameIndex < Mathf.Min(GetFrameCount(fromAnimationIndex), GetFrameCount(toAnimationIndex)); frameIndex++)
            {
                properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex, fromOrientationIndex, toOrientationIndex);
            }
        }

        public void CopyPasteOrientation(int fromPropertyIndex, int toPropertyIndex, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            List<AnimatorProperty> properties = this.Properties;
            properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

#if UNITY_EDITOR
        public void SetAssetPreviewsInProperties()
        {
            FrigidEditMode.RecordPotentialChangesToFullHierarchy(this.gameObject);
            this.selfElapsedDuration = 0;
            this.currAnimationIndex = 0;
            this.currFrameIndex = 0;
            this.currOrientationIndex = 0;
            foreach (AnimatorProperty property in this.Properties)
            {
                if (GetAnimationCount() > 0)
                {
                    property.AnimationEnter();
                    if (GetFrameCount(0) > 0)
                    {
                        property.FrameEnter();
                        if (GetOrientationCount(0) > 0)
                        {
                            property.OrientationEnter();
                        }
                    }
                }
            }
        }
#endif

        protected override void Awake()
        {
            base.Awake();

            this.timeScale = 1f;

            this.playedAnimationIndex = -1;
            this.selfElapsedDuration = 0;

            this.currAnimationIndex = -1;
            this.currFrameIndex = -1;
            this.currOrientationIndex = -1;

            this.direction = Vector2.zero;
            this.RootProperty.Initialize();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshAnimation();
            RefreshFrame();
            RefreshOrientation();
        }

        protected override void Update()
        {
            base.Update();

            this.selfElapsedDuration += Time.deltaTime * this.TimeScale;
            RefreshFrame();
            if (!this.completedSelf)
            {
                if (this.CurrAnimationIndex != -1 &&
                    !GetLooping(this.CurrAnimationIndex) &&
                    this.SelfRemainingDuration <= 0 &&
                    this.RemainingDuration <= 0 &&
                    this.RootProperty.IsCompletedAtEndOfAnimation())
                {
                    bool subBodiesCompleted = true;
                    foreach (SubAnimatorBodyAnimatorProperty subBodyProperty in GetSubBodyPropertiesIn(this.CurrAnimationIndex)) subBodiesCompleted &= subBodyProperty.SubBody.completedSelf;

                    if (subBodiesCompleted)
                    {
                        this.completedSelf = true;
                        this.onComplete?.Invoke();
                    }
                }
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject()
        {
            return true;
        }
#endif

        private float SelfElapsedDuration
        {
            get
            {
                return this.selfElapsedDuration;
            }
        }

        private float SelfRemainingDuration
        {
            get
            {
                return this.SelfTotalDuration - SelfElapsedDuration;
            }
        }

        private float SelfTotalDuration
        {
            get
            {
                if (this.CurrAnimationIndex == -1 || GetLooping(this.CurrAnimationIndex)) return float.MaxValue;
                return GetFrameCount(this.CurrAnimationIndex) / GetFrameRate(this.CurrAnimationIndex);
            }
        }

        private List<SubAnimatorBodyAnimatorProperty> GetSubBodyProperties()
        {
            return this.subBodyProperties;
        }

        private List<SubAnimatorBodyAnimatorProperty> GetCurrentSubBodyProperties()
        {
            return GetSubBodyPropertiesIn(this.CurrAnimationIndex);
        }

        private List<SubAnimatorBodyAnimatorProperty> GetSubBodyPropertiesIn(int animationIndex)
        {
            if (animationIndex == -1) return new List<SubAnimatorBodyAnimatorProperty>();
            return this.subBodyPropertiesPerAnimation[animationIndex].Items;
        }

        private void UpdateLocalRotation()
        {
            if (this.RotateToDirection)
            {
                float orientationAngle = 0;
                if (this.CurrAnimationIndex != -1 && this.CurrOrientationIndex != -1)
                {
                    orientationAngle = GetOrientationDirection(this.CurrAnimationIndex, this.CurrOrientationIndex).ComponentAngle0To2PI();
                }
                this.transform.localRotation = Quaternion.Euler(0, 0, (this.Direction.ComponentAngle0To2PI() - orientationAngle) * Mathf.Rad2Deg);
            }
        }

        private void RefreshAnimation()
        {
            UpdateAnimationIndex(HandleAnimationExit, HandleAnimationEnter);
        }

        private void UpdateAnimationIndex(Action onUpdateBefore, Action onUpdateAfter)
        {
            if (this.playAnimationDirty || this.CurrAnimationIndex != this.playedAnimationIndex)
            {
                onUpdateBefore?.Invoke();
                int prevAnimationIndex = this.CurrAnimationIndex;
                this.currAnimationIndex = this.playedAnimationIndex;
                this.onAnimationUpdated?.Invoke(prevAnimationIndex, this.CurrAnimationIndex);
                if (this.CurrAnimationIndex != -1) this.RootProperty.SetActive(true);
                UpdateLocalRotation();
                UpdateFrameIndex(null, null);
                UpdateOrientationIndex(null, null);
                onUpdateAfter?.Invoke();
            }
        }

        private void HandleAnimationEnter()
        {
            if (this.CurrAnimationIndex != -1)
            {
                this.RootProperty.AnimationEnter();
                HandleFrameEnter();
            }
        }

        private void HandleAnimationExit()
        {
            if (this.CurrAnimationIndex != -1)
            {
                this.RootProperty.AnimationExit();
                HandleFrameExit();
            }
        }

        private void RefreshFrame()
        {
            UpdateFrameIndex(HandleFrameExit, HandleFrameEnter);
        }

        private void UpdateFrameIndex(Action onUpdateBefore, Action onUpdateAfter)
        {
            int newFrameIndex = -1;
            if (this.CurrAnimationIndex != -1 && GetFrameCount(this.CurrAnimationIndex) > 0)
            {
                int framesElapsed = Mathf.FloorToInt(this.SelfElapsedDuration * GetFrameRate(this.CurrAnimationIndex));
                newFrameIndex = GetLooping(this.CurrAnimationIndex) ? (framesElapsed % GetFrameCount(this.CurrAnimationIndex)) : Mathf.Min(framesElapsed, GetFrameCount(this.CurrAnimationIndex) - 1);
            }

            if (newFrameIndex != this.CurrFrameIndex)
            {
                onUpdateBefore?.Invoke();
                int prevFrameIndex = this.CurrFrameIndex;
                this.currFrameIndex = newFrameIndex;
                this.onFrameUpdated?.Invoke(prevFrameIndex, this.CurrFrameIndex);
                UpdateOrientationIndex(null, null);
                onUpdateAfter?.Invoke();
            }
        }

        private void HandleFrameEnter()
        {
            if (this.CurrAnimationIndex != -1 && this.CurrFrameIndex != -1)
            {
                this.RootProperty.FrameEnter();
                HandleOrientationEnter();
            }
        }

        private void HandleFrameExit()
        {
            if (this.CurrAnimationIndex != -1 && this.CurrFrameIndex != -1)
            {
                this.RootProperty.FrameExit();
                HandleOrientationExit();
            }
        }

        private void RefreshOrientation()
        {
            UpdateOrientationIndex(HandleOrientationExit, HandleOrientationEnter);
        }

        private void UpdateOrientationIndex(Action onUpdateBefore, Action onUpdateAfter)
        {
            int newOrientationIndex = -1;
            if (this.CurrAnimationIndex != -1 && GetOrientationCount(this.CurrAnimationIndex) > 0)
            {
                newOrientationIndex = 0;
                for (int orientationIndex = 1; orientationIndex < GetOrientationCount(this.CurrAnimationIndex); orientationIndex++)
                {
                    float a1 = Vector2.Angle(this.direction, GetOrientationDirection(this.CurrAnimationIndex, newOrientationIndex));
                    float a2 = Vector2.Angle(this.direction, GetOrientationDirection(this.CurrAnimationIndex, orientationIndex));
                    if (a2 < a1 + float.Epsilon)
                    {
                        newOrientationIndex = orientationIndex;
                    }
                }
            }

            if (newOrientationIndex != this.CurrOrientationIndex)
            {
                onUpdateBefore?.Invoke();
                int prevOrientationIndex = this.CurrOrientationIndex;
                this.currOrientationIndex = newOrientationIndex;
                this.onOrientationUpdated?.Invoke(prevOrientationIndex, this.CurrOrientationIndex);
                UpdateLocalRotation();
                onUpdateAfter?.Invoke();
            }
        }

        private void HandleOrientationEnter()
        {
            if (this.CurrAnimationIndex != -1 && this.CurrFrameIndex != -1 && this.CurrOrientationIndex != -1)
            {
                this.RootProperty.OrientationEnter();
            }
        }

        private void HandleOrientationExit()
        {
            if (this.CurrAnimationIndex != -1 && this.CurrFrameIndex != -1 && this.CurrOrientationIndex != -1)
            {
                this.RootProperty.OrientationExit();
            }
        }
    }
}
