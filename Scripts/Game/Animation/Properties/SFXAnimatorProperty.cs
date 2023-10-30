using System;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class SFXAnimatorProperty : AnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private SFX sfx;
        [SerializeField]
        [ReadOnly]
        private Nested2DList<bool> playedThisFrames;

        public Type GetSFXType()
        {
            return this.sfx.GetType();
        }

        public void SetSFXType(Type sfxType)
        {
            FrigidEdit.RecordChanges(this);
            if (this.sfx != null)
            {
                FrigidEdit.RemoveComponent(this.sfx);
            }
            this.sfx = (SFX)FrigidEdit.AddComponent(this.gameObject, sfxType);
        }

        public bool GetPlayedThisFrame(int animationIndex, int frameIndex)
        {
            return this.playedThisFrames[animationIndex][frameIndex];
        }

        public void SetPlayedThisFrame(int animationIndex, int frameIndex, bool playThisFrame)
        {
            if (this.playedThisFrames[animationIndex][frameIndex] != playThisFrame)
            {
                FrigidEdit.RecordChanges(this);
                this.playedThisFrames[animationIndex][frameIndex] = playThisFrame;
            }
        }

        public override void Created()
        {
            base.Created();
            FrigidEdit.RecordChanges(this);
            this.sfx = FrigidEdit.AddComponent<AfterImageSFX>(this.gameObject);
            this.playedThisFrames = new Nested2DList<bool>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.playedThisFrames.Add(new Nested1DList<bool>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.playedThisFrames[animationIndex].Add(false);
                }
            }
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playedThisFrames.Insert(animationIndex, new Nested1DList<bool>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.playedThisFrames[animationIndex].Add(false);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playedThisFrames.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playedThisFrames[animationIndex].Insert(frameIndex, false);
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playedThisFrames[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            SFXAnimatorProperty otherAudioProperty = otherProperty as SFXAnimatorProperty;
            if (otherAudioProperty)
            {
                otherAudioProperty.SetPlayedThisFrame(toAnimationIndex, toFrameIndex, this.GetPlayedThisFrame(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void Enable(bool enabled)
        {
            if (!enabled)
            {
                this.sfx.Stop();
            }
            base.Enable(enabled);
        }

        public override void FrameEnter()
        {
            if (!this.Body.Previewing)
            {
                if (this.GetPlayedThisFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
                {
                    this.sfx.Play(this.Body);
                }
            }
            base.FrameEnter();
        }

        public override void FrameExit()
        {
            if (!this.Body.Previewing)
            {
                int nextFrameIndex = this.Body.CurrFrameIndex + 1;
                if (this.Body.GetLoopBehaviour(this.Body.CurrAnimationIndex) != AnimatorBody.LoopBehaviour.NoLoop)
                {
                    nextFrameIndex = nextFrameIndex % this.Body.GetFrameCount(this.Body.CurrAnimationIndex);
                }
                if (nextFrameIndex >= this.Body.GetFrameCount(this.Body.CurrAnimationIndex) || !this.GetPlayedThisFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
                {
                    this.sfx.Stop();
                }
            }
            base.FrameExit();
        }
    }
}
