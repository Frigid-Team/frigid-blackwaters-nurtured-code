using FrigidBlackwaters.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class AttackAnimatorProperty : AnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private Attack attack;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<bool> attackThisFrames;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Vector2> localOffsets;

        private bool forceStop;

        public DamageAlignment DamageAlignment
        {
            get
            {
                return this.attack.DamageAlignment;
            }
            set
            {
                this.attack.DamageAlignment = value;
            }
        }

        public int DamageBonus
        {
            get
            {
                return this.attack.DamageBonus;
            }
            set
            {
                this.attack.DamageBonus = value;
            }
        }

        public Action<HitInfo> OnHitDealt
        {
            get
            {
                return this.attack.OnHitDealt;
            }
            set
            {
                this.attack.OnHitDealt = value;
            }
        }

        public Action<BreakInfo> OnBreakDealt
        {
            get
            {
                return this.attack.OnBreakDealt;
            }
            set
            {
                this.attack.OnBreakDealt = value;
            }
        }

        public Action<ThreatInfo> OnThreatDealt
        {
            get
            {
                return this.attack.OnThreatDealt;
            }
            set
            {
                this.attack.OnThreatDealt = value;
            }
        }

        public bool ForceStop
        {
            get
            {
                return this.forceStop;
            }
            set
            {
                this.forceStop = value;
            }
        }

        public override List<AnimatorProperty> ChildProperties 
        { 
            get
            {
                return new List<AnimatorProperty>();
            }
        }

        public Type GetAttackType()
        {
            return this.attack.GetType();
        }

        public void SetAttackType(Type attackType)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.attack = (Attack)FrigidEditMode.AddComponent(this.gameObject, attackType);
        }

        public bool GetAttackThisFrame(int animationIndex, int frameIndex)
        {
            return this.attackThisFrames[animationIndex][frameIndex];
        }

        public void SetAttackThisFrame(int animationIndex, int frameIndex, bool attackThisFrame)
        {
            if (this.attackThisFrames[animationIndex][frameIndex] != attackThisFrame)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.attackThisFrames[animationIndex][frameIndex] = attackThisFrame;
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

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.attack = FrigidEditMode.AddComponent<SprayProjectileAttack>(this.gameObject);
            this.attackThisFrames = new Nested2DList<bool>();
            this.localOffsets = new Nested3DList<Vector2>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.attackThisFrames.Add(new Nested1DList<bool>());
                this.localOffsets.Add(new Nested2DList<Vector2>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.attackThisFrames[animationIndex].Add(false);
                    this.localOffsets[animationIndex].Add(new Nested1DList<Vector2>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                    }
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.attackThisFrames.Insert(animationIndex, new Nested1DList<bool>());
            this.localOffsets.Insert(animationIndex, new Nested2DList<Vector2>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.attackThisFrames[animationIndex].Add(false);
                this.localOffsets[animationIndex].Add(new Nested1DList<Vector2>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.attackThisFrames.RemoveAt(animationIndex);
            this.localOffsets.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.attackThisFrames[animationIndex].Insert(frameIndex, false);
            this.localOffsets[animationIndex].Insert(frameIndex, new Nested1DList<Vector2>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.attackThisFrames[animationIndex].RemoveAt(frameIndex);
            this.localOffsets[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets[animationIndex][frameIndex].Insert(orientationIndex, Vector2.zero);
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets[animationIndex][frameIndex].RemoveAt(orientationIndex);
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            AttackAnimatorProperty otherAttackProperty = otherProperty as AttackAnimatorProperty;
            if (otherAttackProperty) 
            {
                otherAttackProperty.SetAttackThisFrame(toAnimationIndex, toFrameIndex, GetAttackThisFrame(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            AttackAnimatorProperty otherAttackProperty = otherProperty as AttackAnimatorProperty;
            if (otherAttackProperty)
            {
                otherAttackProperty.SetLocalOffset(toAnimationIndex, toFrameIndex, toOrientationIndex, GetLocalOffset(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.forceStop = false;
        }

        public override void SetFrameEnter(int animationIndex, int frameIndex, float elapsedDuration, int loopsElapsed)
        {
            if (GetAttackThisFrame(animationIndex, frameIndex) && !this.forceStop)
            {
                this.attack.Perform(elapsedDuration);
            }
            base.SetFrameEnter(animationIndex, frameIndex, elapsedDuration, loopsElapsed);
        }

        public override void OrientFrameEnter(int animationIndex, int frameIndex, int orientationIndex, float elapsedDuration)
        {
            this.transform.localPosition = GetLocalOffset(animationIndex, frameIndex, orientationIndex);
            base.OrientFrameEnter(animationIndex, frameIndex, orientationIndex, elapsedDuration);
        }
    }
}
