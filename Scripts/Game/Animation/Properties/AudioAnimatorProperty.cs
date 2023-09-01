using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class AudioAnimatorProperty : AnimatorProperty
    {
        private static SceneVariable<Dictionary<AudioClip, float>> lastTimesPlayedPerOneShotClip;

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
        private Nested2DList<bool> playThisFrames;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<bool> waitForEndOfClips;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<AudioClipSerializedReference> audioClips;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<bool> onlyPlayOnFirstCycles;

        private FrigidCoroutine warmingRoutine;
        private float clipEndDuration;

        static AudioAnimatorProperty()
        {
            lastTimesPlayedPerOneShotClip = new SceneVariable<Dictionary<AudioClip, float>>(() => new Dictionary<AudioClip, float>());
        }

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
                    FrigidEdit.RecordChanges(this.audioSource);
                    this.audioSource.loop = value;
                    this.audioSource.volume = value ? 0.0f : 1.0f;
                    this.audioSource.playOnAwake = value;
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
                    FrigidEdit.RecordChanges(this);
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

                    FrigidEdit.RecordChanges(this);
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
                    FrigidEdit.RecordChanges(this.audioSource);
                    this.audioSource.clip = value;
                }
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
                FrigidEdit.RecordChanges(this);
                this.playThisFrames[animationIndex][frameIndex] = playThisFrame;
            }
        }

        public bool GetWaitForEndOfClip(int animationIndex, int frameIndex)
        {
            return this.waitForEndOfClips[animationIndex][frameIndex];
        }

        public void SetWaitForEndOfClip(int animationIndex, int frameIndex, bool waitForEndOfClip)
        {
            if (this.waitForEndOfClips[animationIndex][frameIndex] != waitForEndOfClip)
            {
                FrigidEdit.RecordChanges(this);
                this.waitForEndOfClips[animationIndex][frameIndex] = waitForEndOfClip;
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
                FrigidEdit.RecordChanges(this);
                this.audioClips[animationIndex][frameIndex] = audioClip;
            }
        }

        public bool GetOnlyPlayOnFirstCycle(int animationIndex, int frameIndex)
        {
            return this.onlyPlayOnFirstCycles[animationIndex][frameIndex];
        }

        public void SetOnlyPlayOnFirstCycle(int animationIndex, int frameIndex, bool onlyPlayOnFirstLoop)
        {
            if (this.onlyPlayOnFirstCycles[animationIndex][frameIndex] != onlyPlayOnFirstLoop)
            {
                FrigidEdit.RecordChanges(this);
                this.onlyPlayOnFirstCycles[animationIndex][frameIndex] = onlyPlayOnFirstLoop;
            }
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.audioSource = FrigidEdit.AddComponent<AudioSource>(this.gameObject);
            this.audioSource.playOnAwake = false;
            this.audioSource.volume = 1.0f;
            this.playThisFrames = new Nested2DList<bool>();
            this.waitForEndOfClips = new Nested2DList<bool>();
            this.audioClips = new Nested2DList<AudioClipSerializedReference>();
            this.onlyPlayOnFirstCycles = new Nested2DList<bool>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.playThisFrames.Add(new Nested1DList<bool>());
                this.waitForEndOfClips.Add(new Nested1DList<bool>());
                this.audioClips.Add(new Nested1DList<AudioClipSerializedReference>());
                this.onlyPlayOnFirstCycles.Add(new Nested1DList<bool>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.playThisFrames[animationIndex].Add(false);
                    this.waitForEndOfClips[animationIndex].Add(false);
                    this.audioClips[animationIndex].Add(new AudioClipSerializedReference());
                    this.onlyPlayOnFirstCycles[animationIndex].Add(false);
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playThisFrames.Insert(animationIndex, new Nested1DList<bool>());
            this.waitForEndOfClips.Insert(animationIndex, new Nested1DList<bool>());
            this.audioClips.Insert(animationIndex, new Nested1DList<AudioClipSerializedReference>());
            this.onlyPlayOnFirstCycles.Insert(animationIndex, new Nested1DList<bool>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.playThisFrames[animationIndex].Add(false);
                this.waitForEndOfClips[animationIndex].Add(false);
                this.audioClips[animationIndex].Add(new AudioClipSerializedReference());
                this.onlyPlayOnFirstCycles[animationIndex].Add(false);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playThisFrames.RemoveAt(animationIndex);
            this.waitForEndOfClips.RemoveAt(animationIndex);
            this.audioClips.RemoveAt(animationIndex);
            this.onlyPlayOnFirstCycles.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playThisFrames[animationIndex].Insert(frameIndex, false);
            this.waitForEndOfClips[animationIndex].Insert(frameIndex, false);
            this.audioClips[animationIndex].Insert(frameIndex, new AudioClipSerializedReference());
            this.onlyPlayOnFirstCycles[animationIndex].Insert(frameIndex, false);
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playThisFrames[animationIndex].RemoveAt(frameIndex);
            this.waitForEndOfClips[animationIndex].RemoveAt(frameIndex);
            this.audioClips[animationIndex].RemoveAt(frameIndex);
            this.onlyPlayOnFirstCycles[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            AudioAnimatorProperty otherAudioProperty = otherProperty as AudioAnimatorProperty;
            if (otherAudioProperty)
            {
                otherAudioProperty.SetPlayThisFrame(toAnimationIndex, toFrameIndex, this.GetPlayThisFrame(fromAnimationIndex, fromFrameIndex));
                otherAudioProperty.SetWaitForEndOfClip(toAnimationIndex, toFrameIndex, this.GetWaitForEndOfClip(fromAnimationIndex, fromFrameIndex));
                otherAudioProperty.SetAudioClipByReference(toAnimationIndex, toFrameIndex, new AudioClipSerializedReference(this.GetAudioClipByReference(fromAnimationIndex, fromFrameIndex)));
                otherAudioProperty.SetOnlyPlayOnFirstCycle(toAnimationIndex, toFrameIndex, this.GetOnlyPlayOnFirstCycle(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public void PlayOneShot(AudioClip audioClip)
        {
            lastTimesPlayedPerOneShotClip.Current.TryAdd(audioClip, 0);
            float volumeMultiplier = Mathf.Max(0, 1 - EasingFunctions.EaseInExpo(0, 1, Mathf.Clamp01(1 - (Time.time - lastTimesPlayedPerOneShotClip.Current[audioClip]) / audioClip.length)));
            this.audioSource.PlayOneShot(audioClip, volumeMultiplier);
            lastTimesPlayedPerOneShotClip.Current[audioClip] = Time.time;
        }

        public override void AnimationEnter()
        {
            if (!this.Body.Previewing)
            {
                if (this.Loop)
                {
                    FrigidCoroutine.Kill(this.warmingRoutine);
                    float startingVolume = this.audioSource.volume;
                    this.warmingRoutine = FrigidCoroutine.Run(
                        Tween.Value(this.WarmingDuration * (1 - this.audioSource.volume / this.MaxVolume), onUpdate: (float progress01) => { this.audioSource.volume = startingVolume + progress01 * (this.MaxVolume - startingVolume); }),
                        this.gameObject
                        );
                }
                this.clipEndDuration = 0;
            }
            base.AnimationEnter();
        }

        public override void AnimationExit()
        {
            if (!this.Body.Previewing)
            {
                if (this.Loop)
                {
                    FrigidCoroutine.Kill(this.warmingRoutine);
                    float startingVolume = this.audioSource.volume;
                    this.warmingRoutine = FrigidCoroutine.Run(
                        Tween.Value(this.WarmingDuration * this.audioSource.volume / this.MaxVolume, onUpdate: (float progress01) => { this.audioSource.volume = (1 - progress01) * startingVolume; }),
                        this.gameObject
                        );
                }
            }
            base.AnimationExit();
        }

        public override void FrameEnter()
        {
            if (!this.Body.Previewing)
            {
                if (!this.Loop && this.GetPlayThisFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex) && (this.Body.CurrentCycleIndex == 0 || !this.GetOnlyPlayOnFirstCycle(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex)))
                {
                    AudioClip chosenAudioClip = this.GetAudioClipByReference(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex).MutableValue;
                    this.PlayOneShot(chosenAudioClip);
                    if (this.GetWaitForEndOfClip(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
                    {
                        this.clipEndDuration = Mathf.Max(this.clipEndDuration, this.Body.ElapsedDuration + chosenAudioClip.length);
                    }
                }
            }
            base.FrameEnter();
        }

        public override float GetDuration()
        {
            return Mathf.Max(this.clipEndDuration, base.GetDuration());
        }
    }
}
