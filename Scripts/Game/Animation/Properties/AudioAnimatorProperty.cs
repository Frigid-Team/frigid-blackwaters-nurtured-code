using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class AudioAnimatorProperty : AnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private AudioSource audioSource;
        [SerializeField]
        [HideInInspector]
        private float warmingDuration;
        [SerializeField]
        [HideInInspector]
        private float maxVolume;
        [SerializeField]
        [HideInInspector]
        private List<bool> isPlayedInAnimations;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<bool> playThisFrames;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<AudioClipSerializedReference> audioClips;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<bool> onlyPlayOnFirstLoops;

        private FrigidCoroutine warmingRoutine;
        private float endDuration;

        public bool Loop
        {
            get
            {
                return this.audioSource.loop;
            }
            set
            {
                if (this.audioSource.loop != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this.audioSource);
                    this.audioSource.loop = value;
                }
            }
        }


        public float WarmingDuration
        {
            get
            {
                return this.warmingDuration;
            }
            set
            {
                if (this.warmingDuration != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this);
                    this.warmingDuration = value;
                }
            }
        }

        public float MaxVolume
        {
            get
            {
                return this.maxVolume;
            }
            set
            {
                if (this.maxVolume != value)
                {

                    FrigidEditMode.RecordPotentialChanges(this);
                    this.maxVolume = value;
                }
            }
        }

        public AudioClip AudioClip
        {
            get
            {
                return this.audioSource.clip;
            }
            set
            {
                if (this.audioSource.clip != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this.audioSource);
                    this.audioSource.clip = value;
                }
            }
        }

        public override List<AnimatorProperty> ChildProperties
        {
            get
            {
                return new List<AnimatorProperty>();
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
            return this.playThisFrames[animationIndex][frameIndex];
        }

        public void SetPlayThisFrame(int animationIndex, int frameIndex, bool playThisFrame)
        {
            if (this.playThisFrames[animationIndex][frameIndex] != playThisFrame)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.playThisFrames[animationIndex][frameIndex] = playThisFrame;
            }
        }

        public AudioClipSerializedReference GetAudioClipByReference(int animationIndex, int frameIndex)
        {
            return this.audioClips[animationIndex][frameIndex];
        }

        public void SetAudioClipByReference(int animationIndex, int frameIndex, AudioClipSerializedReference audioClip)
        {
            if (this.audioClips[animationIndex][frameIndex] != audioClip)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.audioClips[animationIndex][frameIndex] = audioClip;
            }
        }

        public bool GetOnlyPlayOnFirstLoop(int animationIndex, int frameIndex)
        {
            return this.onlyPlayOnFirstLoops[animationIndex][frameIndex];
        }

        public void SetOnlyPlayOnFirstLoop(int animationIndex, int frameIndex, bool onlyPlayOnFirstLoop)
        {
            if (this.onlyPlayOnFirstLoops[animationIndex][frameIndex] != onlyPlayOnFirstLoop)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.onlyPlayOnFirstLoops[animationIndex][frameIndex] = onlyPlayOnFirstLoop;
            }
        }

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.audioSource = FrigidEditMode.AddComponent<AudioSource>(this.gameObject);
            FrigidEditMode.RecordPotentialChanges(this.audioSource);
            this.audioSource.playOnAwake = false;
            this.isPlayedInAnimations = new List<bool>();
            this.playThisFrames = new Nested2DList<bool>();
            this.audioClips = new Nested2DList<AudioClipSerializedReference>();
            this.onlyPlayOnFirstLoops = new Nested2DList<bool>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.isPlayedInAnimations.Add(false);
                this.playThisFrames.Add(new Nested1DList<bool>());
                this.audioClips.Add(new Nested1DList<AudioClipSerializedReference>());
                this.onlyPlayOnFirstLoops.Add(new Nested1DList<bool>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.playThisFrames[animationIndex].Add(false);
                    this.audioClips[animationIndex].Add(new AudioClipSerializedReference());
                    this.onlyPlayOnFirstLoops[animationIndex].Add(false);
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.isPlayedInAnimations.Insert(animationIndex, false);
            this.playThisFrames.Insert(animationIndex, new Nested1DList<bool>());
            this.audioClips.Insert(animationIndex, new Nested1DList<AudioClipSerializedReference>());
            this.onlyPlayOnFirstLoops.Insert(animationIndex, new Nested1DList<bool>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.playThisFrames[animationIndex].Add(false);
                this.audioClips[animationIndex].Add(new AudioClipSerializedReference());
                this.onlyPlayOnFirstLoops[animationIndex].Add(false);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.isPlayedInAnimations.RemoveAt(animationIndex);
            this.playThisFrames.RemoveAt(animationIndex);
            this.audioClips.RemoveAt(animationIndex);
            this.onlyPlayOnFirstLoops.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.playThisFrames[animationIndex].Insert(frameIndex, false);
            this.audioClips[animationIndex].Insert(frameIndex, new AudioClipSerializedReference());
            this.onlyPlayOnFirstLoops[animationIndex].Insert(frameIndex, false);
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.playThisFrames[animationIndex].RemoveAt(frameIndex);
            this.audioClips[animationIndex].RemoveAt(frameIndex);
            this.onlyPlayOnFirstLoops[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            AudioAnimatorProperty otherAudioProperty = otherProperty as AudioAnimatorProperty;
            if (otherAudioProperty)
            {
                otherAudioProperty.SetPlayThisFrame(toAnimationIndex, toFrameIndex, GetPlayThisFrame(fromAnimationIndex, fromFrameIndex));
                otherAudioProperty.SetAudioClipByReference(toAnimationIndex, toFrameIndex, new AudioClipSerializedReference(GetAudioClipByReference(fromAnimationIndex, fromFrameIndex)));
                otherAudioProperty.SetOnlyPlayOnFirstLoop(toAnimationIndex, toFrameIndex, GetOnlyPlayOnFirstLoop(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void AnimationEnter(int animationIndex, float elapsedDuration)
        {
            if (this.Loop)
            {
                FrigidCoroutine.Kill(this.warmingRoutine);
                float startingVolume = this.audioSource.volume;
                if (GetIsPlayedInAnimation(animationIndex)) 
                {
                    this.warmingRoutine = FrigidCoroutine.Run(
                        TweenCoroutine.Value(this.WarmingDuration * (1 - this.audioSource.volume / this.MaxVolume), onUpdate: (float progress01) => { this.audioSource.volume = startingVolume + progress01 * (this.MaxVolume - startingVolume); }),
                        this.gameObject
                        );
                }
                else
                {
                    this.warmingRoutine = FrigidCoroutine.Run(
                        TweenCoroutine.Value(this.WarmingDuration * this.audioSource.volume / this.MaxVolume, onUpdate: (float progress01) => { this.audioSource.volume = (1 - progress01) * startingVolume; }),
                        this.gameObject
                        );
                }
            }
            this.endDuration = 0;
            base.AnimationEnter(animationIndex, elapsedDuration);
        }

        public override void SetFrameEnter(int animationIndex, int frameIndex, float elapsedDuration, int loopsElapsed)
        {
            if (!this.Loop && GetPlayThisFrame(animationIndex, frameIndex) && (loopsElapsed == 0 || !GetOnlyPlayOnFirstLoop(animationIndex, frameIndex)))
            {
                AudioClip chosenAudioClip = GetAudioClipByReference(animationIndex, frameIndex).MutableValue;
                this.audioSource.PlayOneShot(chosenAudioClip);
                this.endDuration = Mathf.Max(this.endDuration, elapsedDuration + chosenAudioClip.length);
            }
            base.SetFrameEnter(animationIndex, frameIndex, elapsedDuration, loopsElapsed);
        }

        protected override bool CanCompleteAtEndOfAnimation(int animationIndex, float elapsedDuration)
        {
            return elapsedDuration >= this.endDuration;
        }
    }
}
