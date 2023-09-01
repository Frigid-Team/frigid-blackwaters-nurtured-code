using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class HitBoxAnimatorProperty : DamageDealerBoxAnimatorProperty<HitBox, HurtBox, HitInfo>
    {
        [SerializeField]
        [HideInInspector]
        private Nested2DList<int> damageBonuses;

        public int DamageBonus
        {
            get
            {
                return this.DamageDealerBox.DamageBonus;
            }
            set
            {
                this.DamageDealerBox.DamageBonus = value;
            }
        }

        public int GetDamageBonus(int animationIndex, int frameIndex)
        {
            return this.damageBonuses[animationIndex][frameIndex];
        }

        public void SetDamageBonus(int animationIndex, int frameIndex, int damageBonus)
        {
            if (this.damageBonuses[animationIndex][frameIndex] != damageBonus)
            {
                FrigidEdit.RecordChanges(this);
                this.damageBonuses[animationIndex][frameIndex] = damageBonus;
            }
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.damageBonuses = new Nested2DList<int>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.damageBonuses.Add(new Nested1DList<int>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.damageBonuses[animationIndex].Add(0);
                }
            }
            this.gameObject.layer = (int)FrigidLayer.HitBoxes;
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.damageBonuses.Insert(animationIndex, new Nested1DList<int>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.damageBonuses[animationIndex].Add(0);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.damageBonuses.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.damageBonuses[animationIndex].Insert(frameIndex, 0);
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.damageBonuses[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            HitBoxAnimatorProperty otherHitBoxProperty = otherProperty as HitBoxAnimatorProperty;
            if (otherHitBoxProperty)
            {
                otherHitBoxProperty.SetDamageBonus(toAnimationIndex, toFrameIndex, this.GetDamageBonus(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void FrameEnter()
        {
            if (!this.Body.Previewing)
            {
                this.DamageDealerBox.DamageBonus += this.GetDamageBonus(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex);
            }
            base.FrameEnter();
        }

        public override void FrameExit()
        {
            if (!this.Body.Previewing)
            {
                this.DamageDealerBox.DamageBonus -= this.GetDamageBonus(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex);
            }
            base.FrameExit();
        }
    }
}
