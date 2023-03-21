using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class SubAnimatorBodyAnimatorProperty : AnimatorProperty
    {
        [SerializeField]
        private AnimatorBody subBody;
        [SerializeField]
        [HideInInspector]
        private List<int> subAnimationIndexes;

        private bool subAnimationCompleted;

        public AnimatorBody SubBody
        {
            get
            {
                return this.subBody;
            }
        }

        public int GetSubAnimationIndex(int animationIndex)
        {
            return this.subAnimationIndexes[animationIndex];
        }

        public void SetSubAnimationIndex(int animationIndex, int subAnimationIndex)
        {
            if (this.subAnimationIndexes[animationIndex] != subAnimationIndex)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.subAnimationIndexes[animationIndex] = subAnimationIndex;
            }
        }

        public override void Created()
        {
            GameObject newAnimatorGameObject = FrigidEditMode.CreateGameObject(this.transform);
            this.subBody = AnimatorBody.SetupOn(newAnimatorGameObject);
            FrigidEditMode.RecordPotentialChanges(this);
            this.subAnimationIndexes = new List<int>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.subAnimationIndexes.Add(0);
            }
            base.Created();
        }

        public override void Destroyed()
        {
            FrigidEditMode.DestroyGameObject(this.subBody.gameObject);
            base.Destroyed();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.subAnimationIndexes.Insert(animationIndex, 0);
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.subAnimationIndexes.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void AnimationEnter()
        {
            this.subAnimationCompleted = false;
            this.subBody.Play(GetSubAnimationIndex(this.Body.CurrAnimationIndex), () => this.subAnimationCompleted = true);
            base.AnimationEnter();
        }

        protected override bool CanCompleteAtEndOfAnimation()
        {
            return this.subAnimationCompleted;
        }
    }
}
