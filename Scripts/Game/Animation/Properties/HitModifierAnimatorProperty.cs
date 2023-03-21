using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class HitModifierAnimatorProperty : AnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private HitModifier hitModifier;
        [SerializeField]
        [ReadOnly]
        private List<HurtBoxAnimatorProperty> hurtBoxProperties;
        [SerializeField]
        [HideInInspector]
        private bool playAudioWhenModified;
        [SerializeField]
        [HideInInspector]
        private AudioClipSerializedReference audioClipWhenModified;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<bool> addedThisFrames;

        public bool PlayAudioWhenModified
        {
            get
            {
                return this.playAudioWhenModified;
            }
            set
            {
                if (this.playAudioWhenModified != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this);
                    this.playAudioWhenModified = value;
                }
            }
        }

        public AudioClipSerializedReference AudioClipWhenModifiedByReference
        {
            get
            {
                return this.audioClipWhenModified;
            }
            set
            {
                if (this.audioClipWhenModified != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this);
                    this.audioClipWhenModified = value;
                }
            }
        }

        public Type GetHitModifierType()
        {
            return this.hitModifier.GetType();
        }

        public void SetHitModifierType(Type hitModifierType)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            FrigidEditMode.RemoveComponent(this.hitModifier);
            this.hitModifier = (HitModifier)FrigidEditMode.AddComponent(this.gameObject, hitModifierType);
        }

        public int GetNumberHurtBoxProperties()
        {
            return this.hurtBoxProperties.Count;
        }

        public HurtBoxAnimatorProperty GetHurtBoxProperty(int index)
        {
            return this.hurtBoxProperties[index];
        }

        public void SetHurtBoxProperty(int index, HurtBoxAnimatorProperty hurtBoxProperty)
        {
            if (this.hurtBoxProperties[index] != hurtBoxProperty)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.hurtBoxProperties[index] = hurtBoxProperty;
            }
        }

        public void AddHurtBoxProperty(int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.hurtBoxProperties.Insert(index, null);
        }

        public void RemoveHurtBoxProperty(int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.hurtBoxProperties.RemoveAt(index);
        }

        public bool GetAddedThisFrame(int animationIndex, int frameIndex)
        {
            return this.addedThisFrames[animationIndex][frameIndex];
        }

        public void SetAddedThisFrame(int animationIndex, int frameIndex, bool addedThisFrame)
        {
            if (this.addedThisFrames[animationIndex][frameIndex] != addedThisFrame)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.addedThisFrames[animationIndex][frameIndex] = addedThisFrame;
            }
        }

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.hitModifier = FrigidEditMode.AddComponent<ContinuousHitModifier>(this.gameObject);
            this.hurtBoxProperties = new List<HurtBoxAnimatorProperty>();
            this.addedThisFrames = new Nested2DList<bool>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.addedThisFrames.Add(new Nested1DList<bool>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.addedThisFrames[animationIndex].Add(false);
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.addedThisFrames.Insert(animationIndex, new Nested1DList<bool>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.addedThisFrames[animationIndex].Add(false);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.addedThisFrames.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.addedThisFrames[animationIndex].Insert(frameIndex, false);
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.addedThisFrames[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            HitModifierAnimatorProperty otherHitModifierProperty = otherProperty as HitModifierAnimatorProperty;
            if (otherHitModifierProperty)
            {
                otherHitModifierProperty.SetAddedThisFrame(toAnimationIndex, toFrameIndex, GetAddedThisFrame(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void Initialize()
        {
            base.Initialize();
            if (!this.PlayAudioWhenModified) return;
            for (int hurtBoxIndex = 0; hurtBoxIndex < GetNumberHurtBoxProperties(); hurtBoxIndex++)
            {
                GetHurtBoxProperty(hurtBoxIndex).OnReceived += 
                    (HitInfo hitInfo) =>
                    {
                        if (!hitInfo.TryGetHitModifier(out HitModifier hitModifier) || hitModifier != this.hitModifier) return;
                        foreach (AudioAnimatorProperty audioProperty in this.Body.GetCurrentProperties<AudioAnimatorProperty>())
                        {
                            if (audioProperty.Loop) continue;
                            audioProperty.PlayOneShot(this.audioClipWhenModified.MutableValue);
                        }
                    };
            }
        }

        public override void FrameEnter()
        {
            base.FrameEnter();
            if (GetAddedThisFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
            {
                for (int hurtBoxIndex = 0; hurtBoxIndex < GetNumberHurtBoxProperties(); hurtBoxIndex++)
                {
                    GetHurtBoxProperty(hurtBoxIndex).AddHitModifier(this.hitModifier);
                }
            }
        }

        public override void FrameExit()
        {
            base.FrameExit();
            if (GetAddedThisFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
            {
                for (int hurtBoxIndex = 0; hurtBoxIndex < GetNumberHurtBoxProperties(); hurtBoxIndex++)
                {
                    GetHurtBoxProperty(hurtBoxIndex).RemoveHitModifier(this.hitModifier);
                }
            }
        }
    }
}
