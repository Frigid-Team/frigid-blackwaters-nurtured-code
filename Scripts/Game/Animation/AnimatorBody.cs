using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class AnimatorBody : FrigidMonoBehaviour
    {
        private const string NEW_ANIMATION_NAME = "New Animation";
        private const float NEW_FRAME_RATE = 12f;

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

        private bool paused;

        private int playedAnimationIndex;
        private float elapsedDuration;
        private Vector2 direction;

        private int currAnimationIndex;
        private int currFrameIndex;
        private int currOrientationIndex;

        private Action onComplete;

        public Vector2 Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                this.direction = value;
                if (this.ProceedAnimating)
                {
                    RefreshOrientation(this.currAnimationIndex, this.currFrameIndex, false);
                }
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

        public AnimatorProperty RootProperty
        {
            get
            {
                return this.rootProperty;
            }
        }

        public int PropertyCount
        {
            get
            {
                return 1 + this.rootProperty.ChildPropertyCount;
            }
        }

        public int PropertyDepth
        {
            get
            {
                return this.rootProperty.ChildHeight;
            }
        }

        public bool Paused
        {
            get
            {
                return this.paused;
            }
            set
            {
                if (this.paused != value)
                {
                    this.paused = value;
                    if (this.paused)
                    {
                        this.rootProperty.Paused();
                    }
                    else
                    {
                        this.rootProperty.UnPaused();
                    }
                }
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
            return body;
        }

        public void PlayByName(string animationName, Action onComplete = null)
        {
            int animationIndex = this.animationNames.IndexOf(animationName);
            if (animationIndex >= 0)
            {
                Play(animationIndex, onComplete);
            }
            else
            {
                Debug.LogWarning("AnimationName " + animationName + " does not exist in AnimatorBody " + this.name + ".");
            }
        }

        public void Play(int animationIndex, Action onComplete = null)
        {
            this.playedAnimationIndex = animationIndex;
            this.onComplete = onComplete;
            this.elapsedDuration = 0;
            if (this.ProceedAnimating)
            {
                RefreshAnimation(true);
            }
        }

        public void PlayEmpty()
        {
            this.playedAnimationIndex = -1;
            this.onComplete = null;
            this.elapsedDuration = 0;
            if (this.ProceedAnimating)
            {
                RefreshAnimation(true);
            }
        }

        public List<AnimatorProperty> GetProperties()
        {
            List<AnimatorProperty> properties = new List<AnimatorProperty>();
            void Visit(AnimatorProperty currProperty)
            {
                properties.Add(currProperty);
                foreach (AnimatorProperty childProperty in currProperty.ChildProperties)
                {
                    Visit(childProperty);
                }
            }
            Visit(this.rootProperty);
            return properties;
        }

        public List<P> GetProperties<P>() where P : AnimatorProperty
        {
            List<P> properties = new List<P>();
            foreach (AnimatorProperty baseProperty in GetProperties())
            {
                P property;
                if (property = baseProperty as P)
                {
                    properties.Add(property);
                }
            }
            return properties;
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

            int propertyCount = this.PropertyCount;
            List<AnimatorProperty> properties = GetProperties();
            for (int propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
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
            int propertyCount = this.PropertyCount;
            List<AnimatorProperty> properties = GetProperties();
            for (int propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
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
            int propertyCount = this.PropertyCount;
            List<AnimatorProperty> properties = GetProperties();
            for (int propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
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
            List<AnimatorProperty> properties = GetProperties();
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
            List<AnimatorProperty> properties = GetProperties();
            properties[fromPropertyIndex].CopyPasteToAnotherFrame(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
            for (int orientationIndex = 0; orientationIndex < Mathf.Min(GetOrientationCount(fromAnimationIndex), GetOrientationCount(toAnimationIndex)); orientationIndex++)
            {
                properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, orientationIndex, orientationIndex);
            }
        }

        public void CopyPasteOrientationAcrossAllFrames(int fromPropertyIndex, int toPropertyIndex, int fromAnimationIndex, int toAnimationIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            List<AnimatorProperty> properties = GetProperties();
            for (int frameIndex = 0; frameIndex < Mathf.Min(GetFrameCount(fromAnimationIndex), GetFrameCount(toAnimationIndex)); frameIndex++)
            {
                properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex, fromOrientationIndex, toOrientationIndex);
            }
        }

        public void CopyPasteOrientation(int fromPropertyIndex, int toPropertyIndex, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            List<AnimatorProperty> properties = GetProperties();
            properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

#if UNITY_EDITOR
        public void SetAssetPreviewsInProperties()
        {
            FrigidEditMode.RecordPotentialChangesToFullHierarchy(this.gameObject);
            foreach (AnimatorProperty property in GetProperties())
            {
                if (GetAnimationCount() > 0)
                {
                    property.AnimationEnter(0, 0);
                    if (GetFrameCount(0) > 0)
                    {
                        property.SetFrameEnter(0, 0, 0, 0);
                        if (GetOrientationCount(0) > 0)
                        {
                            property.OrientFrameEnter(0, 0, 0, 0);
                        }
                    }
                }
            }
        }
#endif

        protected override void Awake()
        {
            base.Awake();

            this.playedAnimationIndex = -1;
            this.elapsedDuration = 0;

            this.currAnimationIndex = -1;
            this.currFrameIndex = -1;
            this.currOrientationIndex = -1;

            if (this.animationCount == 0)
            {
                Debug.LogError("There are no animations in AnimatorBody " + this.name + ".");
                return;
            }
            this.direction = Vector2.zero;
            this.rootProperty.Initialize();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshAnimation(false);
            RefreshFrame(this.currAnimationIndex, false);
            RefreshOrientation(this.currAnimationIndex, this.currFrameIndex, false);
        }

        protected override void Update()
        {
            base.Update();

            if (!this.ProceedAnimating) return;

            this.elapsedDuration += Time.deltaTime;
            RefreshFrame(this.currAnimationIndex, false);
            
            if (this.currAnimationIndex != -1 &&
                !GetLooping(this.currAnimationIndex) &&
                this.onComplete != null &&
                this.elapsedDuration >= GetFrameCount(this.currAnimationIndex) * 1 / GetFrameRate(this.currAnimationIndex) &&
                this.AllPropertiesCompletedAtEndOfAnimation)
            {
                this.onComplete.Invoke();
                this.onComplete = null;
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject()
        {
            return true;
        }
#endif

        private bool ProceedAnimating
        {
            get
            {
                return this.gameObject.activeInHierarchy && !this.paused;
            }
        }

        private bool AllPropertiesCompletedAtEndOfAnimation
        {
            get
            {
                bool allPropertiesCompleted = false;
                foreach (AnimatorProperty property in GetProperties())
                {
                    allPropertiesCompleted &= property.IsCompletedAtEndOfAnimation(this.currAnimationIndex, this.elapsedDuration);
                }
                return allPropertiesCompleted;
            }
        }

        private void RefreshAnimation(bool force)
        {
            if (force || this.currAnimationIndex != this.playedAnimationIndex)
            {
                int previousAnimationIndex = this.currAnimationIndex;
                if (previousAnimationIndex != -1)
                {
                    this.rootProperty.AnimationExit(previousAnimationIndex);
                }
                this.currAnimationIndex = this.playedAnimationIndex;
                if (this.currAnimationIndex != -1)
                {
                    this.rootProperty.PreAnimationSetup(this.currAnimationIndex, true);
                    this.rootProperty.AnimationEnter(this.currAnimationIndex, this.elapsedDuration);
                }
                RefreshFrame(previousAnimationIndex, true);
            }
        }

        private void RefreshFrame(int originalAnimationIndex, bool force)
        {
            int newFrameIndex = -1;
            if (this.currAnimationIndex != -1)
            {
                int framesElapsed = Mathf.FloorToInt(this.elapsedDuration * GetFrameRate(this.currAnimationIndex));
                newFrameIndex = GetLooping(this.currAnimationIndex) ? (framesElapsed % GetFrameCount(this.currAnimationIndex)) : Mathf.Min(framesElapsed, GetFrameCount(this.currAnimationIndex) - 1);
            }

            if (force || newFrameIndex != this.currFrameIndex)
            {
                int previousFrameIndex = this.currFrameIndex;
                if (originalAnimationIndex != -1 && previousFrameIndex != -1)
                {
                    this.rootProperty.SetFrameExit(originalAnimationIndex, previousFrameIndex);
                }
                this.currFrameIndex = newFrameIndex;
                if (this.currAnimationIndex != -1 && this.currFrameIndex != -1)
                {
                    int loopsElapsed = Mathf.FloorToInt(this.elapsedDuration / (GetFrameCount(this.currAnimationIndex) * 1 / GetFrameRate(this.currAnimationIndex)));
                    this.rootProperty.SetFrameEnter(this.currAnimationIndex, this.currFrameIndex, this.elapsedDuration, loopsElapsed);
                }
                RefreshOrientation(originalAnimationIndex, previousFrameIndex, true);
            }
        }

        private void RefreshOrientation(int originalAnimationIndex, int originalFrameIndex, bool force)
        {
            int newOrientationIndex = -1;
            if (this.currAnimationIndex != -1 && GetOrientationCount(this.currAnimationIndex) > 0)
            {
                newOrientationIndex = 0;
                for (int orientationIndex = 1; orientationIndex < GetOrientationCount(this.currAnimationIndex); orientationIndex++)
                {
                    float a1 = Vector2.Angle(this.direction, GetOrientationDirection(this.currAnimationIndex, newOrientationIndex));
                    float a2 = Vector2.Angle(this.direction, GetOrientationDirection(this.currAnimationIndex, orientationIndex));
                    if (a2 < a1)
                    {
                        newOrientationIndex = orientationIndex;
                    }
                }
            }

            if (force || newOrientationIndex != this.currOrientationIndex)
            {
                int previousOrientationIndex = this.currOrientationIndex;
                if (originalAnimationIndex != -1 && originalFrameIndex != -1 && previousOrientationIndex != -1)
                {
                    this.rootProperty.OrientFrameExit(originalAnimationIndex, originalFrameIndex, previousOrientationIndex);
                }
                this.currOrientationIndex = newOrientationIndex;
                if (this.currAnimationIndex != -1 && this.currFrameIndex != -1 && this.currOrientationIndex != -1)
                {
                    this.rootProperty.OrientFrameEnter(this.currAnimationIndex, this.currFrameIndex, this.currOrientationIndex, this.elapsedDuration);
                }
            }
        }
    }
}
