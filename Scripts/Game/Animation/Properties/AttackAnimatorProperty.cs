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
        private Nested2DList<AttackBehaviour> attackBehaviours;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<ForceCompleteBehaviour> forceCompleteBehaviours;

        private Action toForceCompleteOnAnimationEnd;

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

        public bool IsIgnoringDamage
        {
            get
            {
                return this.attack.IsIgnoringDamage;
            }
            set
            {
                this.attack.IsIgnoringDamage = value;
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

        public AttackBehaviour GetAttackBehaviour(int animationIndex, int frameIndex)
        {
            return this.attackBehaviours[animationIndex][frameIndex];
        }

        public void SetAttackBehaviour(int animationIndex, int frameIndex, AttackBehaviour attackBehaviour)
        {
            if (this.attackBehaviours[animationIndex][frameIndex] != attackBehaviour)
            {
                FrigidEdit.RecordChanges(this);
                this.attackBehaviours[animationIndex][frameIndex] = attackBehaviour;
            }
        }

        public ForceCompleteBehaviour GetForceCompleteBehaviour(int animationIndex, int frameIndex)
        {
            return this.forceCompleteBehaviours[animationIndex][frameIndex];
        }

        public void SetForceCompleteBehaviour(int animationIndex, int frameIndex, ForceCompleteBehaviour forceCompleteBehaviour)
        {
            if (this.forceCompleteBehaviours[animationIndex][frameIndex] != forceCompleteBehaviour)
            {
                FrigidEdit.RecordChanges(this);
                this.forceCompleteBehaviours[animationIndex][frameIndex] = forceCompleteBehaviour;
            }
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.attack = FrigidEdit.AddComponent<SprayProjectileAttack>(this.gameObject);
            this.attackBehaviours = new Nested2DList<AttackBehaviour>();
            this.forceCompleteBehaviours = new Nested2DList<ForceCompleteBehaviour>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.attackBehaviours.Add(new Nested1DList<AttackBehaviour>());
                this.forceCompleteBehaviours.Add(new Nested1DList<ForceCompleteBehaviour>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.attackBehaviours[animationIndex].Add(AttackBehaviour.NoAttack);
                    this.forceCompleteBehaviours[animationIndex].Add(ForceCompleteBehaviour.NoForceComplete);
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.attackBehaviours.Insert(animationIndex, new Nested1DList<AttackBehaviour>());
            this.forceCompleteBehaviours.Insert(animationIndex, new Nested1DList<ForceCompleteBehaviour>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.attackBehaviours[animationIndex].Add(AttackBehaviour.NoAttack);
                this.forceCompleteBehaviours[animationIndex].Add(ForceCompleteBehaviour.NoForceComplete);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.attackBehaviours.RemoveAt(animationIndex);
            this.forceCompleteBehaviours.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.attackBehaviours[animationIndex].Insert(frameIndex, AttackBehaviour.NoAttack);
            this.forceCompleteBehaviours[animationIndex].Insert(frameIndex, ForceCompleteBehaviour.NoForceComplete);
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.attackBehaviours[animationIndex].RemoveAt(frameIndex);
            this.forceCompleteBehaviours[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            AttackAnimatorProperty otherAttackProperty = otherProperty as AttackAnimatorProperty;
            if (otherAttackProperty) 
            {
                otherAttackProperty.SetAttackBehaviour(toAnimationIndex, toFrameIndex, this.GetAttackBehaviour(fromAnimationIndex, fromFrameIndex));
                otherAttackProperty.SetForceCompleteBehaviour(toAnimationIndex, toFrameIndex, this.GetForceCompleteBehaviour(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void AnimationEnter()
        {
            this.toForceCompleteOnAnimationEnd = null;
            base.AnimationEnter();
        }

        public override void AnimationExit()
        {
            this.toForceCompleteOnAnimationEnd?.Invoke();
            base.AnimationExit();
        }

        public override void FrameEnter()
        {
            if (!this.Body.Previewing)
            {
                this.transform.localPosition = this.GetLocalPosition(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);

                switch (this.GetAttackBehaviour(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
                {
                    case AttackBehaviour.NoAttack:
                        goto skipAttack;
                    case AttackBehaviour.AttackEveryCycle:
                        break;
                    case AttackBehaviour.AttackOnFirstCycle:
                        if (this.Body.CycleIndex == 0)
                        {
                            break;
                        }
                        goto skipAttack;
                }

                switch (this.GetForceCompleteBehaviour(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
                {
                    case ForceCompleteBehaviour.ForceCompleteOnAnimationEnd:
                        this.attack.Perform(this.Body.ElapsedDuration, ref this.toForceCompleteOnAnimationEnd, null);
                        break;
                    case ForceCompleteBehaviour.NoForceComplete:
                        this.attack.Perform(this.Body.ElapsedDuration, null);
                        break;
                }

            skipAttack:;
            }
            base.FrameEnter();
        }

        public enum AttackBehaviour
        {
            NoAttack,
            AttackEveryCycle,
            AttackOnFirstCycle
        }

        public enum ForceCompleteBehaviour
        {
            NoForceComplete,
            ForceCompleteOnAnimationEnd
        }
    }
}
