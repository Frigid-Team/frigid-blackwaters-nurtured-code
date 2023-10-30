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
        private float playVolume;
        [SerializeField]
        [HideInInspector]
        private float warmingDuration;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<PlayBehaviour> playBehaviours;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<bool> waitForEndOfClips;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<AudioClipSerializedReference> audioClips;

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
                    this.audioSource.volume = value ? 0.0f : this.playVolume;
                    this.audioSource.playOnAwake = value;
                }
            }
        }

        public float PlayVolume
        {
            get
            {
                return this.playVolume;
            }
            set
            {
                if (this.playVolume != value)
                {

                    FrigidEdit.RecordChanges(this);
                    FrigidEdit.RecordChanges(this.audioSource);
                    this.playVolume = value;
                    this.audioSource.volume = this.audioSource.loop ? 0.0f : value;
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

        public PlayBehaviour GetPlayBehaviour(int animationIndex, int frameIndex)
        {
            return this.playBehaviours[animationIndex][frameIndex];
        }

        public void SetPlayBehaviour(int animationIndex, int frameIndex, PlayBehaviour playBehaviour)
        {
            if (this.playBehaviours[animationIndex][frameIndex] != playBehaviour)
            {
                FrigidEdit.RecordChanges(this);
                this.playBehaviours[animationIndex][frameIndex] = playBehaviour;
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

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.audioSource = FrigidEdit.AddComponent<AudioSource>(this.gameObject);
            this.audioSource.playOnAwake = false;
            this.audioSource.volume = 1.0f;
            this.playVolume = 1.0f;
            this.playBehaviours = new Nested2DList<PlayBehaviour>();
            this.waitForEndOfClips = new Nested2DList<bool>();
            this.audioClips = new Nested2DList<AudioClipSerializedReference>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.playBehaviours.Add(new Nested1DList<PlayBehaviour>());
                this.waitForEndOfClips.Add(new Nested1DList<bool>());
                this.audioClips.Add(new Nested1DList<AudioClipSerializedReference>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.playBehaviours[animationIndex].Add(PlayBehaviour.NoPlay);
                    this.waitForEndOfClips[animationIndex].Add(false);
                    this.audioClips[animationIndex].Add(new AudioClipSerializedReference());
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playBehaviours.Insert(animationIndex, new Nested1DList<PlayBehaviour>());
            this.waitForEndOfClips.Insert(animationIndex, new Nested1DList<bool>());
            this.audioClips.Insert(animationIndex, new Nested1DList<AudioClipSerializedReference>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.playBehaviours[animationIndex].Add(PlayBehaviour.NoPlay);
                this.waitForEndOfClips[animationIndex].Add(false);
                this.audioClips[animationIndex].Add(new AudioClipSerializedReference());
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playBehaviours.RemoveAt(animationIndex);
            this.waitForEndOfClips.RemoveAt(animationIndex);
            this.audioClips.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playBehaviours[animationIndex].Insert(frameIndex, PlayBehaviour.NoPlay);
            this.waitForEndOfClips[animationIndex].Insert(frameIndex, false);
            this.audioClips[animationIndex].Insert(frameIndex, new AudioClipSerializedReference());
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playBehaviours[animationIndex].RemoveAt(frameIndex);
            this.waitForEndOfClips[animationIndex].RemoveAt(frameIndex);
            this.audioClips[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            AudioAnimatorProperty otherAudioProperty = otherProperty as AudioAnimatorProperty;
            if (otherAudioProperty)
            {
                otherAudioProperty.SetPlayBehaviour(toAnimationIndex, toFrameIndex, this.GetPlayBehaviour(fromAnimationIndex, fromFrameIndex));
                otherAudioProperty.SetWaitForEndOfClip(toAnimationIndex, toFrameIndex, this.GetWaitForEndOfClip(fromAnimationIndex, fromFrameIndex));
                otherAudioProperty.SetAudioClipByReference(toAnimationIndex, toFrameIndex, new AudioClipSerializedReference(this.GetAudioClipByReference(fromAnimationIndex, fromFrameIndex)));
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
                        Tween.Value(this.WarmingDuration * (1 - this.audioSource.volume / this.PlayVolume), onUpdate: (float progress01) => { this.audioSource.volume = startingVolume + progress01 * (this.PlayVolume - startingVolume); }),
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
                        Tween.Value(this.WarmingDuration * this.audioSource.volume / this.PlayVolume, onUpdate: (float progress01) => { this.audioSource.volume = (1 - progress01) * startingVolume; }),
                        this.gameObject
                        );
                }
            }
            base.AnimationExit();
        }

        public override void FrameEnter()
        {
            if (!this.Body.Previewing && !this.Loop)
            {
                switch (this.GetPlayBehaviour(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
                {
                    case PlayBehaviour.NoPlay:
                        goto skipPlay;
                    case PlayBehaviour.PlayEveryCycle:
                        break;
                    case PlayBehaviour.PlayOnFirstCycle:
                        if (this.Body.CycleIndex == 0)
                        {
                            break;
                        }
                        goto skipPlay;
                }

                AudioClip chosenAudioClip = this.GetAudioClipByReference(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex).MutableValue;
                this.PlayOneShot(chosenAudioClip);
                if (this.GetWaitForEndOfClip(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
                {
                    this.clipEndDuration = Mathf.Max(this.clipEndDuration, this.Body.ElapsedDuration + chosenAudioClip.length);
                }

            skipPlay:;
            }
            base.FrameEnter();
        }

        public override float GetDuration()
        {
            return Mathf.Max(this.clipEndDuration, base.GetDuration());
        }

        public enum PlayBehaviour
        {
            NoPlay,
            PlayEveryCycle,
            PlayOnFirstCycle
        }
    }
}
