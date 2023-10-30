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
                    FrigidEdit.RecordChanges(this);
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
                    FrigidEdit.RecordChanges(this);
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
            FrigidEdit.RecordChanges(this);
            FrigidEdit.RemoveComponent(this.hitModifier);
            this.hitModifier = (HitModifier)FrigidEdit.AddComponent(this.gameObject, hitModifierType);
        }

        public int GetNumberHurtBoxProperties()
        {
            return this.hurtBoxProperties.Count;
        }

        public HurtBoxAnimatorProperty GetHurtBoxProperty(int propertyIndex)
        {
            return this.hurtBoxProperties[propertyIndex];
        }

        public void SetHurtBoxProperty(int propertyIndex, HurtBoxAnimatorProperty hurtBoxProperty)
        {
            if (this.hurtBoxProperties[propertyIndex] != hurtBoxProperty)
            {
                FrigidEdit.RecordChanges(this);
                this.hurtBoxProperties[propertyIndex] = hurtBoxProperty;
            }
        }

        public void AddHurtBoxProperty(int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.hurtBoxProperties.Insert(propertyIndex, null);
        }

        public void RemoveHurtBoxProperty(int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.hurtBoxProperties.RemoveAt(propertyIndex);
        }

        public bool GetAddedThisFrame(int animationIndex, int frameIndex)
        {
            return this.addedThisFrames[animationIndex][frameIndex];
        }

        public void SetAddedThisFrame(int animationIndex, int frameIndex, bool addedThisFrame)
        {
            if (this.addedThisFrames[animationIndex][frameIndex] != addedThisFrame)
            {
                FrigidEdit.RecordChanges(this);
                this.addedThisFrames[animationIndex][frameIndex] = addedThisFrame;
            }
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.hitModifier = FrigidEdit.AddComponent<ContinuousHitModifier>(this.gameObject);
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
            FrigidEdit.RecordChanges(this);
            this.addedThisFrames.Insert(animationIndex, new Nested1DList<bool>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.addedThisFrames[animationIndex].Add(false);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.addedThisFrames.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.addedThisFrames[animationIndex].Insert(frameIndex, false);
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.addedThisFrames[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            HitModifierAnimatorProperty otherHitModifierProperty = otherProperty as HitModifierAnimatorProperty;
            if (otherHitModifierProperty)
            {
                otherHitModifierProperty.SetAddedThisFrame(toAnimationIndex, toFrameIndex, this.GetAddedThisFrame(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void Initialize()
        {
            if (!this.PlayAudioWhenModified) return;
            for (int hurtBoxIndex = 0; hurtBoxIndex < this.GetNumberHurtBoxProperties(); hurtBoxIndex++)
            {
                this.GetHurtBoxProperty(hurtBoxIndex).OnReceived += 
                    (HitInfo hitInfo) =>
                    {
                        if (!hitInfo.AppliedHitModifiers.Contains(this.hitModifier)) return;
                        foreach (AudioAnimatorProperty audioProperty in this.Body.GetReferencedProperties<AudioAnimatorProperty>())
                        {
                            if (audioProperty.Loop) continue;
                            audioProperty.PlayOneShot(this.audioClipWhenModified.MutableValue);
                        }
                    };
            }
            base.Initialize();
        }

        public override void FrameEnter()
        {
            if (!this.Body.Previewing)
            {
                if (this.GetAddedThisFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
                {
                    for (int hurtBoxIndex = 0; hurtBoxIndex < this.GetNumberHurtBoxProperties(); hurtBoxIndex++)
                    {
                        this.GetHurtBoxProperty(hurtBoxIndex).AddHitModifier(this.hitModifier);
                    }
                }
            }
            base.FrameEnter();
        }

        public override void FrameExit()
        {
            if (!this.Body.Previewing)
            {
                if (this.GetAddedThisFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
                {
                    for (int hurtBoxIndex = 0; hurtBoxIndex < this.GetNumberHurtBoxProperties(); hurtBoxIndex++)
                    {
                        this.GetHurtBoxProperty(hurtBoxIndex).RemoveHitModifier(this.hitModifier);
                    }
                }
            }
            base.FrameExit();
        }
    }
}
