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

        private bool previewing;

        private float timeScale;
        private Action onTimeScaleChanged;
        private Vector2 direction;
        private Action onDirectionChanged;

        private bool playAnimationDirty;
        private int playedAnimationIndex;
        private float elapsedDuration;
  
        private int currAnimationIndex;
        private Action<int, int> onAnimationUpdated;
        private int currFrameIndex;
        private Action<int, int> onFrameUpdated;
        private int currOrientationIndex;
        private Action<int, int> onOrientationUpdated;

        private bool completed = false;
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
                    FrigidEdit.RecordChanges(this);
                    this.rotateToDirection = value;
                }
            }
        }

        public bool Previewing
        {
            get
            {
                return this.previewing;
            }
            private set
            {
                this.previewing = value;
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

        public Vector2 Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                Vector2 newDirection = value.normalized;
                if (newDirection != this.direction)
                {
                    this.direction = value.normalized;
                    if (this.gameObject.activeInHierarchy)
                    {
                        this.RefreshOrientation();
                    }
                    this.UpdateLocalRotation();
                    this.onDirectionChanged?.Invoke();
                }
            }
        }

        public Action OnDirectionChanged
        {
            get
            {
                return this.onDirectionChanged;
            }
            set
            {
                this.onDirectionChanged = value;
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

        public Bounds AreaOccupied
        {
            get
            {
                Bounds? totalAreaOccupied = this.RootProperty.GetAreaOccupied();
                return totalAreaOccupied.HasValue ? totalAreaOccupied.Value : new Bounds(this.transform.position, Vector3.zero);
            }
        }

        public bool IsLooping
        {
            get
            {
                return this.CurrAnimationIndex == -1 ? false : (this.GetLooping(this.CurrAnimationIndex) || this.RootProperty.GetLooped());
            }
        }

        public float ElapsedDuration
        {
            get
            {
                return this.elapsedDuration;
            }
            private set
            {
                this.elapsedDuration = value;
            }
        }

        public float RemainingDuration
        {
            get
            {
                return this.TotalDuration - this.ElapsedDuration;
            }
        }

        public float TotalDuration
        {
            get
            {
                if (this.CurrAnimationIndex == -1 || this.GetLooping(this.CurrAnimationIndex)) return float.MaxValue;
                return Mathf.Max(this.GetFrameCount(this.CurrAnimationIndex) / this.GetFrameRate(this.CurrAnimationIndex), this.RootProperty.GetDuration());
            }
        }

        public int CurrentCycleIndex
        {
            get
            {
                if (this.CurrAnimationIndex == -1) return 0;
                return Mathf.FloorToInt(this.ElapsedDuration / (this.GetFrameCount(this.CurrAnimationIndex) / this.GetFrameRate(this.CurrAnimationIndex)));
            }
        }

        public int CurrAnimationIndex
        {
            get
            {
                return this.currAnimationIndex;
            }
            private set
            {
                this.currAnimationIndex = value;
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
            private set
            {
                this.currFrameIndex = value;
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
            private set
            {
                this.currOrientationIndex = value;
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

        public static AnimatorBody CreateBody(GameObject gameObject)
        {
            AnimatorBody body = FrigidEdit.AddComponent<AnimatorBody>(gameObject);
            if (body == null) return null;
            body.animationCount = 0;
            body.animationNames = new List<string>();
            body.frameRates = new List<float>();
            body.loopings = new List<bool>();
            body.frameCounts = new List<int>();
            body.orientationCounts = new List<int>();
            body.orientationDirections = new Nested2DList<Vector2>();
            body.rootProperty = AnimatorProperty.CreateProperty(body, body.transform, typeof(RootAnimatorProperty));
            return body;
        }

        public static void DestroyBody(AnimatorBody body)
        {
            AnimatorProperty.DestroyProperty(body.rootProperty);
            FrigidEdit.DestroyGameObject(body.gameObject);
        }

        public bool Play(string animationName, Action onComplete = null)
        {
            int animationIndex = this.animationNames.IndexOf(animationName);
            return this.Play(animationIndex, onComplete);
        }

        public bool Play(int playedAnimationIndex, Action onComplete = null)
        {
            if (playedAnimationIndex != -1)
            {
                this.playAnimationDirty = true;
                this.playedAnimationIndex = playedAnimationIndex;
                this.completed = false;
                this.onComplete = onComplete;
                this.ElapsedDuration = 0;
                if (this.gameObject.activeInHierarchy)
                {
                    this.RefreshAnimation();
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
            this.ElapsedDuration = 0;
            if (this.gameObject.activeInHierarchy)
            {
                this.RefreshAnimation();
            }
        }

        public bool TryFindReferencedProperty<P>(string propertyName, out P property) where P : AnimatorProperty
        {
            foreach (P searchProperty in this.GetReferencedProperties<P>())
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

        public bool TryFindCurrentReferencedProperty<P>(string propertyName, out P property) where P : AnimatorProperty
        {
            return this.TryFindReferencedPropertyIn(this.CurrAnimationIndex, propertyName, out property);
        }

        public bool TryFindReferencedPropertyIn<P>(string animationName, string propertyName, out P property) where P : AnimatorProperty
        {
            return this.TryFindReferencedPropertyIn(this.animationNames.IndexOf(animationName), propertyName, out property);
        }

        public bool TryFindReferencedPropertyIn<P>(int animationIndex, string propertyName, out P property) where P : AnimatorProperty
        {
            foreach (P searchProperty in this.GetReferencedPropertiesIn<P>(animationIndex))
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

        public List<P> GetReferencedProperties<P>() where P : AnimatorProperty
        {
            List<P> referencedProperties = new List<P>();
            P rootProperty = this.RootProperty as P;
            if (rootProperty) referencedProperties.Add(rootProperty);
            referencedProperties.AddRange(this.RootProperty.GetReferencedProperties<P>());
            return referencedProperties;
        }

        public List<P> GetCurrentReferencedProperties<P>() where P : AnimatorProperty
        {
            return this.GetReferencedPropertiesIn<P>(this.CurrAnimationIndex);
        }

        public List<P> GetReferencedPropertiesIn<P>(string animationName) where P : AnimatorProperty
        {
            return this.GetReferencedPropertiesIn<P>(this.animationNames.IndexOf(animationName));
        }
 
        public List<P> GetReferencedPropertiesIn<P>(int animationIndex) where P : AnimatorProperty
        {
            List<P> referencedProperties = new List<P>();
            P rootProperty = this.RootProperty as P;
            if (rootProperty) referencedProperties.Add(rootProperty);
            referencedProperties.AddRange(this.RootProperty.GetReferencedPropertiesIn<P>(animationIndex));
            return referencedProperties;
        }

        public int GetPropertyDepth()
        {
            return this.RootProperty.GetDescendentPropertyDepth() + 1;
        }

        public int GetPropertyDepthOf(AnimatorProperty property)
        {
            if (property == this.RootProperty)
            {
                return 0;
            }
            else
            {
                int rootDepth = this.RootProperty.GetDescendentPropertyDepthOf(property);
                if (rootDepth != -1)
                {
                    return rootDepth + 1;
                }
            }
            return -1;
        }

        public int GetNumberProperties()
        {
            return this.RootProperty.GetNumberDescendentProperties() + 1;
        }

        public List<AnimatorProperty> GetProperties()
        {
            List<AnimatorProperty> properties = new List<AnimatorProperty>() { this.RootProperty };
            properties.AddRange(this.RootProperty.GetDescendentProperties());
            return properties;
        }

        public void AddAnimationAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
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
            FrigidEdit.RecordChanges(this);
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
                FrigidEdit.RecordChanges(this);
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
                FrigidEdit.RecordChanges(this);
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
                FrigidEdit.RecordChanges(this);
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
                this.AddFrameAt(animationIndex, frameIndex);
            }
            for (int frameIndex = this.frameCounts[animationIndex] - 1; frameIndex > frameCount - 1; frameIndex--)
            {
                this.RemoveFrameAt(animationIndex, frameIndex);
            }
        }

        public void AddFrameAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.frameCounts[animationIndex]++;
            this.rootProperty.FrameAddedAt(animationIndex, frameIndex);
        }

        public void RemoveFrameAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
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
                this.AddOrientationAt(animationIndex, orientationIndex);
            }
            for (int orientationIndex = this.orientationCounts[animationIndex] - 1; orientationIndex > orientationCount - 1; orientationIndex--)
            {
                this.RemoveOrientationAt(animationIndex, orientationIndex);
            }
        }

        public void AddOrientationAt(int animationIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.orientationCounts[animationIndex]++;
            this.orientationDirections[animationIndex].Insert(orientationIndex, Vector2.zero);
            for (int frameIndex = 0; frameIndex < this.GetFrameCount(animationIndex); frameIndex++)
            {
                this.rootProperty.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
            }
        }

        public void RemoveOrientationAt(int animationIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.orientationCounts[animationIndex]--;
            this.orientationDirections[animationIndex].RemoveAt(orientationIndex);
            for (int frameIndex = 0; frameIndex < this.GetFrameCount(animationIndex); frameIndex++)
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
                FrigidEdit.RecordChanges(this);
                this.orientationDirections[animationIndex][orientationIndex] = direction;
            }
        }

        public void CopyPasteAnimation(int fromAnimationIndex, int toAnimationIndex)
        {
            this.SetAnimationName(toAnimationIndex, this.GetAnimationName(fromAnimationIndex));
            this.SetOrientationCount(toAnimationIndex, this.GetOrientationCount(fromAnimationIndex));
            for (int orientationIndex = 0; orientationIndex < this.GetOrientationCount(toAnimationIndex); orientationIndex++)
            {
                this.SetOrientationDirection(toAnimationIndex, orientationIndex, this.GetOrientationDirection(fromAnimationIndex, orientationIndex));
            }
            this.SetFrameCount(toAnimationIndex, this.GetFrameCount(fromAnimationIndex));
            this.SetLooping(toAnimationIndex, this.GetLooping(fromAnimationIndex));
            this.SetFrameRate(toAnimationIndex, this.GetFrameRate(fromAnimationIndex));

            List<AnimatorProperty> properties = this.GetProperties();
            for (int propertyIndex = 0; propertyIndex < properties.Count; propertyIndex++)
            {
                properties[propertyIndex].CopyPasteToAnotherAnimation(properties[propertyIndex], fromAnimationIndex, toAnimationIndex);
                for (int frameIndex = 0; frameIndex < Mathf.Min(this.GetFrameCount(fromAnimationIndex), this.GetFrameCount(toAnimationIndex)); frameIndex++)
                {
                    properties[propertyIndex].CopyPasteToAnotherFrame(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex);
                    for (int orientationIndex = 0; orientationIndex < Mathf.Min(this.GetOrientationCount(fromAnimationIndex), this.GetOrientationCount(toAnimationIndex)); orientationIndex++)
                    {
                        properties[propertyIndex].CopyPasteToAnotherOrientation(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex, orientationIndex, orientationIndex);
                    }
                }
            }
        }

        public void CopyPasteAllFramesAndTheirOrientationsAcrossAllProperties(int fromAnimationIndex, int toAnimationIndex)
        {
            List<AnimatorProperty> properties = this.GetProperties();
            for (int propertyIndex = 0; propertyIndex < properties.Count; propertyIndex++)
            {
                properties[propertyIndex].CopyPasteToAnotherAnimation(properties[propertyIndex], fromAnimationIndex, toAnimationIndex);
                for (int frameIndex = 0; frameIndex < Mathf.Min(this.GetFrameCount(fromAnimationIndex), this.GetFrameCount(toAnimationIndex)); frameIndex++)
                {
                    properties[propertyIndex].CopyPasteToAnotherFrame(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex);
                    for (int orientationIndex = 0; orientationIndex < Mathf.Min(this.GetOrientationCount(fromAnimationIndex), this.GetOrientationCount(toAnimationIndex)); orientationIndex++)
                    {
                        properties[propertyIndex].CopyPasteToAnotherOrientation(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex, orientationIndex, orientationIndex);
                    }
                }
            }
        }

        public void CopyPasteFrameAndItsOrientationsAcrossAllProperties(int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            List<AnimatorProperty> properties = this.GetProperties();
            for (int propertyIndex = 0; propertyIndex < properties.Count; propertyIndex++)
            {
                properties[propertyIndex].CopyPasteToAnotherFrame(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
                for (int orientationIndex = 0; orientationIndex < Mathf.Min(this.GetOrientationCount(fromAnimationIndex), this.GetOrientationCount(toAnimationIndex)); orientationIndex++)
                {
                    properties[propertyIndex].CopyPasteToAnotherOrientation(properties[propertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, orientationIndex, orientationIndex);
                }
            }
        }

        public void CopyPasteAllFramesAndTheirOrientations(int fromPropertyIndex, int toPropertyIndex, int fromAnimationIndex, int toAnimationIndex) 
        {
            List<AnimatorProperty> properties = this.GetProperties();
            for (int frameIndex = 0; frameIndex < Mathf.Min(this.GetFrameCount(fromAnimationIndex), this.GetFrameCount(toAnimationIndex)); frameIndex++)
            {
                properties[fromPropertyIndex].CopyPasteToAnotherFrame(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex);
                for (int orientationIndex = 0; orientationIndex < Mathf.Min(this.GetOrientationCount(fromAnimationIndex), this.GetOrientationCount(toAnimationIndex)); orientationIndex++)
                {
                    properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex, orientationIndex, orientationIndex);
                }
            }
        }

        public void CopyPasteFrameAndItsOrientations(int fromPropertyIndex, int toPropertyIndex, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            List<AnimatorProperty> properties = this.GetProperties();
            properties[fromPropertyIndex].CopyPasteToAnotherFrame(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
            for (int orientationIndex = 0; orientationIndex < Mathf.Min(this.GetOrientationCount(fromAnimationIndex), this.GetOrientationCount(toAnimationIndex)); orientationIndex++)
            {
                properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, orientationIndex, orientationIndex);
            }
        }

        public void CopyPasteOrientationAcrossAllFrames(int fromPropertyIndex, int toPropertyIndex, int fromAnimationIndex, int toAnimationIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            List<AnimatorProperty> properties = this.GetProperties();
            for (int frameIndex = 0; frameIndex < Mathf.Min(this.GetFrameCount(fromAnimationIndex), this.GetFrameCount(toAnimationIndex)); frameIndex++)
            {
                properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, frameIndex, frameIndex, fromOrientationIndex, toOrientationIndex);
            }
        }

        public void CopyPasteOrientation(int fromPropertyIndex, int toPropertyIndex, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            List<AnimatorProperty> properties = this.GetProperties();
            properties[fromPropertyIndex].CopyPasteToAnotherOrientation(properties[toPropertyIndex], fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public void Preview(string animationName, float elapsedDuration, Vector2 direction)
        {
            this.Preview(this.animationNames.IndexOf(animationName), elapsedDuration, direction);
        }

        public void Preview(int playedAnimationIndex, float elapsedDuration, Vector2 direction)
        {
            FrigidEdit.RecordFullObjectHierarchy(this.gameObject);

            int prevAnimationIndex = this.CurrAnimationIndex;
            int prevFrameIndex = this.CurrFrameIndex;
            int prevOrientationIndex = this.CurrOrientationIndex;
            float prevElapsedDuration = this.ElapsedDuration;
            Vector2 prevDirection = this.Direction;

            this.CurrAnimationIndex = this.AnimationIndexFromPlayedAnimation(playedAnimationIndex);
            this.CurrFrameIndex = this.FrameIndexFromElapsedDuration(this.currAnimationIndex, elapsedDuration);
            this.CurrOrientationIndex = this.OrientationIndexFromDirection(this.currAnimationIndex, direction);
            this.ElapsedDuration = elapsedDuration;
            this.direction = direction;
            this.Previewing = true;

            if (this.CurrAnimationIndex != -1)
            {
                this.RootProperty.AnimationEnter();
                if (this.CurrFrameIndex != -1)
                {
                    this.RootProperty.FrameEnter();
                    if (this.CurrOrientationIndex != -1)
                    {
                        this.RootProperty.OrientationEnter();
                    }
                }
            }

            this.CurrAnimationIndex = prevAnimationIndex;
            this.CurrFrameIndex = prevFrameIndex;
            this.CurrOrientationIndex = prevOrientationIndex;
            this.ElapsedDuration = prevElapsedDuration;
            this.direction = prevDirection;
            this.Previewing = false;
        }

        protected override void Awake()
        {
            base.Awake();

            this.timeScale = 1f;

            this.playedAnimationIndex = -1;
            this.ElapsedDuration = 0;

            this.CurrAnimationIndex = -1;
            this.CurrFrameIndex = -1;
            this.CurrOrientationIndex = -1;

            this.direction = Vector2.zero;
            this.RootProperty.Initialize();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.RefreshAnimation();
            this.RefreshFrame();
            this.RefreshOrientation();
        }

        protected override void Update()
        {
            base.Update();

            this.ElapsedDuration += Time.deltaTime * this.TimeScale;
            this.RefreshFrame();
            if (!this.completed)
            {
                if (this.CurrAnimationIndex != -1 && !this.IsLooping && this.RemainingDuration <= 0)
                {
                    this.completed = true;
                    this.onComplete?.Invoke();
                }
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject()
        {
            return true;
        }
#endif

        private void UpdateLocalRotation()
        {
            if (this.RotateToDirection)
            {
                float orientationAngleDeg = 0;
                if (this.CurrAnimationIndex != -1 && this.CurrOrientationIndex != -1)
                {
                    orientationAngleDeg = this.GetOrientationDirection(this.CurrAnimationIndex, this.CurrOrientationIndex).ComponentAngle0To360();
                }
                this.transform.localRotation = Quaternion.Euler(0, 0, this.Direction.ComponentAngle0To360() - orientationAngleDeg);
            }
        }

        private void RefreshAnimation()
        {
            this.UpdateAnimationIndex(this.HandleAnimationExit, this.HandleAnimationEnter);
        }

        private int AnimationIndexFromPlayedAnimation(int playedAnimationIndex) 
        {
            if (playedAnimationIndex < 0 || playedAnimationIndex >= this.GetAnimationCount()) return -1;
            return playedAnimationIndex;
        }

        private void UpdateAnimationIndex(Action onUpdateBefore, Action onUpdateAfter)
        {
            if (this.playAnimationDirty || this.CurrAnimationIndex != this.playedAnimationIndex)
            {
                onUpdateBefore?.Invoke();
                int prevAnimationIndex = this.CurrAnimationIndex;
                this.CurrAnimationIndex = this.AnimationIndexFromPlayedAnimation(this.playedAnimationIndex);
                this.onAnimationUpdated?.Invoke(prevAnimationIndex, this.CurrAnimationIndex);
                this.RootProperty.Enable(this.CurrAnimationIndex != -1);
                this.UpdateFrameIndex(null, null);
                this.UpdateOrientationIndex(null, null);
                this.UpdateLocalRotation();
                onUpdateAfter?.Invoke();
            }
        }

        private void HandleAnimationEnter()
        {
            if (this.CurrAnimationIndex != -1)
            {
                this.RootProperty.AnimationEnter();
                this.HandleFrameEnter();
            }
        }

        private void HandleAnimationExit()
        {
            if (this.CurrAnimationIndex != -1)
            {
                this.RootProperty.AnimationExit();
                this.HandleFrameExit();
            }
        }

        private void RefreshFrame()
        {
            this.UpdateFrameIndex(this.HandleFrameExit, this.HandleFrameEnter);
        }

        private int FrameIndexFromElapsedDuration(int animationIndex, float elapsedDuration)
        {
            int frameIndex = -1;
            if (animationIndex != -1 && this.GetFrameCount(animationIndex) > 0)
            {
                int framesElapsed = Mathf.FloorToInt(elapsedDuration * this.GetFrameRate(animationIndex));
                frameIndex = this.GetLooping(animationIndex) ? (framesElapsed % this.GetFrameCount(animationIndex)) : Mathf.Min(framesElapsed, this.GetFrameCount(animationIndex) - 1);
            }
            return frameIndex;
        }

        private void UpdateFrameIndex(Action onUpdateBefore, Action onUpdateAfter)
        {
            int newFrameIndex = this.FrameIndexFromElapsedDuration(this.CurrAnimationIndex, this.ElapsedDuration);
            if (newFrameIndex != this.CurrFrameIndex)
            {
                onUpdateBefore?.Invoke();
                int prevFrameIndex = this.CurrFrameIndex;
                this.CurrFrameIndex = newFrameIndex;
                this.onFrameUpdated?.Invoke(prevFrameIndex, this.CurrFrameIndex);
                this.UpdateOrientationIndex(null, null);
                this.UpdateLocalRotation();
                onUpdateAfter?.Invoke();
            }
        }

        private void HandleFrameEnter()
        {
            if (this.CurrAnimationIndex != -1 && this.CurrFrameIndex != -1)
            {
                this.RootProperty.FrameEnter();
                this.HandleOrientationEnter();
            }
        }

        private void HandleFrameExit()
        {
            if (this.CurrAnimationIndex != -1 && this.CurrFrameIndex != -1)
            {
                this.RootProperty.FrameExit();
                this.HandleOrientationExit();
            }
        }

        private void RefreshOrientation()
        {
            this.UpdateOrientationIndex(this.HandleOrientationExit, this.HandleOrientationEnter);
        }

        private int OrientationIndexFromDirection(int animationIndex, Vector2 direction)
        {
            int finalOrientationIndex = -1;
            if (animationIndex != -1 && this.GetOrientationCount(animationIndex) > 0)
            {
                if (direction == Vector2.zero) return 0;

                finalOrientationIndex = 0;
                for (int orientationIndex = 1; orientationIndex < this.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    float a1 = Vector2.Angle(direction, this.GetOrientationDirection(animationIndex, finalOrientationIndex));
                    float a2 = Vector2.Angle(direction, this.GetOrientationDirection(animationIndex, orientationIndex));
                    if (a2 < a1 + float.Epsilon)
                    {
                        finalOrientationIndex = orientationIndex;
                    }
                }
            }
            return finalOrientationIndex;
        }

        private void UpdateOrientationIndex(Action onUpdateBefore, Action onUpdateAfter)
        {
            int newOrientationIndex = this.OrientationIndexFromDirection(this.CurrAnimationIndex, this.Direction);
            if (newOrientationIndex != this.CurrOrientationIndex)
            {
                onUpdateBefore?.Invoke();
                int prevOrientationIndex = this.CurrOrientationIndex;
                this.CurrOrientationIndex = newOrientationIndex;
                this.onOrientationUpdated?.Invoke(prevOrientationIndex, this.CurrOrientationIndex);
                this.UpdateLocalRotation();
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
