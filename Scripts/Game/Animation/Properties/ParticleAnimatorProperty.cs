using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class ParticleAnimatorProperty : SortingOrderedAnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private ParticleSystem particleSystem;
        [SerializeField]
        [ReadOnly]
        private ParticleSystemRenderer particleSystemRenderer;
        [SerializeField]
        [HideInInspector]
        private List<bool> isPlayedInAnimations;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<bool> playedThisFrames;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<bool> onlyPlayOnFirstCycles;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Vector2> localOffsets;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> localRotationsDeg;

        public override int CurrentSortingOrder
        {
            get
            {
                return this.particleSystemRenderer.sortingOrder;
            }
        }

        public bool Loop
        {
            get
            {
                return this.particleSystem.main.loop;
            }
            set
            {
                if (this.particleSystem.main.loop != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this.particleSystem);
                    ParticleSystem.MainModule main = this.particleSystem.main;
                    main.loop = value;
                }
            }
        }

        public bool GetIsPlayedInAnimation(int animationIndex)
        {
            return this.isPlayedInAnimations[animationIndex];
        }

        public void SetIsPlayedInAnimation(int animationIndex, bool isPlayedInAnimation)
        {
            if (this.isPlayedInAnimations[animationIndex] != isPlayedInAnimation)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.isPlayedInAnimations[animationIndex] = isPlayedInAnimation;
            }
        }

        public bool GetPlayThisFrame(int animationIndex, int frameIndex)
        {
            return this.playedThisFrames[animationIndex][frameIndex];
        }

        public void SetPlayThisFrame(int animationIndex, int frameIndex, bool playThisFrame)
        {
            if (this.playedThisFrames[animationIndex][frameIndex] != playThisFrame)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.playedThisFrames[animationIndex][frameIndex] = playThisFrame;
            }
        }

        public bool GetOnlyPlayOnFirstCycle(int animationIndex, int frameIndex)
        {
            return this.onlyPlayOnFirstCycles[animationIndex][frameIndex];
        }

        public void SetOnlyPlayOnFirstCycle(int animationIndex, int frameIndex, bool onlyPlayOnFirstCycle)
        {
            if (this.onlyPlayOnFirstCycles[animationIndex][frameIndex] != onlyPlayOnFirstCycle)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.onlyPlayOnFirstCycles[animationIndex][frameIndex] = onlyPlayOnFirstCycle;
            }
        }

        public Vector2 GetLocalOffset(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.localOffsets[animationIndex][frameIndex][orientationIndex];
        }

        public void SetLocalOffset(int animationIndex, int frameIndex, int orientationIndex, Vector2 localOffset)
        {
            if (this.localOffsets[animationIndex][frameIndex][orientationIndex] != localOffset)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.localOffsets[animationIndex][frameIndex][orientationIndex] = localOffset;
            }
        }

        public float GetLocalRotationDeg(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.localRotationsDeg[animationIndex][frameIndex][orientationIndex];
        }

        public void SetLocalRotationDeg(int animationIndex, int frameIndex, int orientationIndex, float directionAngleDeg)
        {
            if (this.localRotationsDeg[animationIndex][frameIndex][orientationIndex] != directionAngleDeg)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.localRotationsDeg[animationIndex][frameIndex][orientationIndex] = directionAngleDeg;
            }
        }

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.particleSystem = FrigidEditMode.AddComponent<ParticleSystem>(this.gameObject);
            this.particleSystemRenderer = this.gameObject.GetComponent<ParticleSystemRenderer>();
            this.isPlayedInAnimations = new List<bool>();
            this.playedThisFrames = new Nested2DList<bool>();
            this.onlyPlayOnFirstCycles = new Nested2DList<bool>();
            this.localOffsets = new Nested3DList<Vector2>();
            this.localRotationsDeg = new Nested3DList<float>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.isPlayedInAnimations.Add(false);
                this.playedThisFrames.Add(new Nested1DList<bool>());
                this.onlyPlayOnFirstCycles.Add(new Nested1DList<bool>());
                this.localOffsets.Add(new Nested2DList<Vector2>());
                this.localRotationsDeg.Add(new Nested2DList<float>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.playedThisFrames[animationIndex].Add(false);
                    this.onlyPlayOnFirstCycles[animationIndex].Add(false);
                    this.localOffsets[animationIndex].Add(new Nested1DList<Vector2>());
                    this.localRotationsDeg[animationIndex].Add(new Nested1DList<float>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                        this.localRotationsDeg[animationIndex][frameIndex].Add(0);
                    }
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.isPlayedInAnimations.Insert(animationIndex, false);
            this.playedThisFrames.Insert(animationIndex, new Nested1DList<bool>());
            this.onlyPlayOnFirstCycles.Insert(animationIndex, new Nested1DList<bool>());
            this.localOffsets.Insert(animationIndex, new Nested2DList<Vector2>());
            this.localRotationsDeg.Insert(animationIndex, new Nested2DList<float>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.playedThisFrames[animationIndex].Add(false);
                this.onlyPlayOnFirstCycles[animationIndex].Add(false);
                this.localOffsets[animationIndex].Add(new Nested1DList<Vector2>());
                this.localRotationsDeg[animationIndex].Add(new Nested1DList<float>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                    this.localRotationsDeg[animationIndex][frameIndex].Add(0);
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.isPlayedInAnimations.RemoveAt(animationIndex);
            this.playedThisFrames.RemoveAt(animationIndex);
            this.onlyPlayOnFirstCycles.RemoveAt(animationIndex);
            this.localOffsets.RemoveAt(animationIndex);
            this.localRotationsDeg.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.playedThisFrames[animationIndex].Insert(frameIndex, false);
            this.onlyPlayOnFirstCycles[animationIndex].Insert(frameIndex, false);
            this.localOffsets[animationIndex].Insert(frameIndex, new Nested1DList<Vector2>());
            this.localRotationsDeg[animationIndex].Insert(frameIndex, new Nested1DList<float>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                this.localRotationsDeg[animationIndex][frameIndex].Add(0);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.playedThisFrames[animationIndex].RemoveAt(frameIndex);
            this.onlyPlayOnFirstCycles[animationIndex].RemoveAt(frameIndex);
            this.localOffsets[animationIndex].RemoveAt(frameIndex);
            this.localRotationsDeg[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets[animationIndex][frameIndex].Insert(orientationIndex, Vector2.zero);
            this.localRotationsDeg[animationIndex][frameIndex].Insert(orientationIndex, 0);
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.localRotationsDeg[animationIndex][frameIndex].RemoveAt(orientationIndex);
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            ParticleAnimatorProperty otherParticleProperty = otherProperty as ParticleAnimatorProperty;
            if (otherParticleProperty)
            {
                otherParticleProperty.SetIsPlayedInAnimation(toAnimationIndex, GetIsPlayedInAnimation(fromAnimationIndex));
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            ParticleAnimatorProperty otherParticleProperty = otherProperty as ParticleAnimatorProperty;
            if (otherParticleProperty)
            {
                otherParticleProperty.SetPlayThisFrame(toAnimationIndex, toFrameIndex, GetPlayThisFrame(fromAnimationIndex, fromFrameIndex));
                otherParticleProperty.SetOnlyPlayOnFirstCycle(toAnimationIndex, toFrameIndex, GetOnlyPlayOnFirstCycle(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            ParticleAnimatorProperty otherParticleProperty = otherProperty as ParticleAnimatorProperty;
            if (otherParticleProperty)
            {
                otherParticleProperty.SetLocalOffset(toAnimationIndex, toFrameIndex, toOrientationIndex, GetLocalOffset(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
                otherParticleProperty.SetLocalRotationDeg(toAnimationIndex, toFrameIndex, toOrientationIndex, GetLocalRotationDeg(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Body.OnTimeScaleChanged +=
                () =>
                {
                    ParticleSystem.MainModule main = this.particleSystem.main;
                    main.simulationSpeed = this.Body.TimeScale;
                };
        }

        public override void AnimationEnter()
        {
            if (this.Loop && GetIsPlayedInAnimation(this.Body.CurrAnimationIndex))
            {
                this.particleSystem.Play();
            }
            base.AnimationEnter();
        }

        public override void AnimationExit()
        {
            if (this.Loop && GetIsPlayedInAnimation(this.Body.CurrAnimationIndex))
            {
                this.particleSystem.Stop();
            }
            base.AnimationExit();
        }

        public override void FrameEnter()
        {
            if (!this.Loop && GetPlayThisFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex) && (this.Body.CurrentCycleIndex == 0 || !GetOnlyPlayOnFirstCycle(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex)))
            {
                if (this.particleSystem.isPlaying)
                {
                    this.particleSystem.Stop();
                }
                this.particleSystem.Play();
            }
            base.FrameEnter();
        }

        public override void OrientationEnter()
        {
            this.particleSystem.transform.localPosition = GetLocalOffset(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
            this.particleSystem.transform.localRotation = Quaternion.Euler(0, 0, GetLocalRotationDeg(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex) - 90);
            this.particleSystemRenderer.sortingOrder = GetSortingOrder(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
            base.OrientationEnter();
        }
    }
}
