using System;
using UnityEngine;

using FrigidBlackwaters.Utility;

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

        public Type GetAttackType()
        {
            return this.attack.GetType();
        }

        public void SetAttackType(Type attackType)
        {
            FrigidEdit.RecordChanges(this);
            FrigidEdit.RemoveComponent(this.attack);
            this.attack = (Attack)FrigidEdit.AddComponent(this.gameObject, attackType);
        }

        public bool GetAttackThisFrame(int animationIndex, int frameIndex)
        {
            return this.attackThisFrames[animationIndex][frameIndex];
        }

        public void SetAttackThisFrame(int animationIndex, int frameIndex, bool attackThisFrame)
        {
            if (this.attackThisFrames[animationIndex][frameIndex] != attackThisFrame)
            {
                FrigidEdit.RecordChanges(this);
                this.attackThisFrames[animationIndex][frameIndex] = attackThisFrame;
            }
        }
        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.attack = FrigidEdit.AddComponent<SprayProjectileAttack>(this.gameObject);
            this.attackThisFrames = new Nested2DList<bool>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.attackThisFrames.Add(new Nested1DList<bool>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.attackThisFrames[animationIndex].Add(false);
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.attackThisFrames.Insert(animationIndex, new Nested1DList<bool>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.attackThisFrames[animationIndex].Add(false);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.attackThisFrames.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.attackThisFrames[animationIndex].Insert(frameIndex, false);
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.attackThisFrames[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            AttackAnimatorProperty otherAttackProperty = otherProperty as AttackAnimatorProperty;
            if (otherAttackProperty) 
            {
                otherAttackProperty.SetAttackThisFrame(toAnimationIndex, toFrameIndex, this.GetAttackThisFrame(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.forceStop = false;
        }

        public override void FrameEnter()
        {
            if (!this.Body.Previewing)
            {
                this.transform.localPosition = this.GetLocalPosition(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                if (this.GetAttackThisFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex) && !this.forceStop)
                {
                    this.attack.Perform(this.Body.ElapsedDuration);
                }
                this.transform.localPosition = Vector2.zero;
            }
            base.FrameEnter();
        }
    }
}
