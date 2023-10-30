using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class SubAnimatorBodyAnimatorProperty : AnimatorProperty
    {
        private string NEW_SUB_BODY_NAME = "Sub_Body";

        [SerializeField]
        private AnimatorBody subBody;
        [SerializeField]
        [HideInInspector]
        private List<int> subAnimationIndexes;
        [SerializeField]
        [HideInInspector]
        private List<bool> ignoreSubAnimationLoopsAndDurations;

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
                FrigidEdit.RecordChanges(this);
                this.subAnimationIndexes[animationIndex] = subAnimationIndex;
            }
        }

        public bool GetIgnoreSubAnimationLoopAndDuration(int animationIndex)
        {
            return this.ignoreSubAnimationLoopsAndDurations[animationIndex];
        }

        public void SetIgnoreSubAnimationLoopAndDuration(int animationIndex, bool ignoreSubAnimationCompletionAndDuration)
        {
            if (this.ignoreSubAnimationLoopsAndDurations[animationIndex] != ignoreSubAnimationCompletionAndDuration)
            {
                FrigidEdit.RecordChanges(this);
                this.ignoreSubAnimationLoopsAndDurations[animationIndex] = ignoreSubAnimationCompletionAndDuration;
            }
        }

        public override List<P> GetReferencedProperties<P>()
        {
            List<P> referencedProperties = new List<P>(base.GetReferencedProperties<P>());
            referencedProperties.AddRange(this.SubBody.GetReferencedProperties<P>());
            return referencedProperties;
        }

        public override List<P> GetReferencedPropertiesIn<P>(int animationIndex)
        {
            List<P> referencedProperties = new List<P>(base.GetReferencedPropertiesIn<P>(animationIndex));
            referencedProperties.AddRange(this.SubBody.GetReferencedPropertiesIn<P>(this.GetSubAnimationIndex(animationIndex)));
            return referencedProperties;
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            GameObject subBodyGameObject = FrigidEdit.CreateGameObject(this.NEW_SUB_BODY_NAME, this.transform);
            this.subBody = AnimatorBody.CreateBody(subBodyGameObject);
            this.subAnimationIndexes = new List<int>();
            this.ignoreSubAnimationLoopsAndDurations = new List<bool>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.subAnimationIndexes.Add(0);
                this.ignoreSubAnimationLoopsAndDurations.Add(false);
            }
            base.Created();
        }

        public override void Destroyed()
        {
            FrigidEdit.RecordChanges(this);
            FrigidEdit.DestroyGameObject(this.subBody.gameObject);
            this.subBody = null;
            base.Destroyed();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.subAnimationIndexes.Insert(animationIndex, 0);
            this.ignoreSubAnimationLoopsAndDurations.Insert(animationIndex, false);
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.subAnimationIndexes.RemoveAt(animationIndex);
            this.ignoreSubAnimationLoopsAndDurations.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            SubAnimatorBodyAnimatorProperty otherSubBodyProperty = otherProperty as SubAnimatorBodyAnimatorProperty;
            if (otherSubBodyProperty)
            {
                this.SetSubAnimationIndex(toAnimationIndex, otherSubBodyProperty.GetSubAnimationIndex(fromAnimationIndex));
                this.SetIgnoreSubAnimationLoopAndDuration(toAnimationIndex, otherSubBodyProperty.GetIgnoreSubAnimationLoopAndDuration(fromAnimationIndex));
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Body.OnTimeScaleChanged += () => { this.SubBody.TimeScale = this.Body.TimeScale; };
            this.SubBody.TimeScale = this.Body.TimeScale;
            this.Body.OnDirectionChanged += () => { this.SubBody.Direction = this.Body.Direction; };
            this.SubBody.Direction = this.Body.Direction;
        }

        public override void Enable(bool enabled)
        {
            if (!enabled)
            {
                this.subBody.Stop();
            }
            base.Enable(enabled);
        }

        public override void AnimationEnter()
        {
            if (this.Body.Previewing)
            {
                this.subBody.Preview(this.GetSubAnimationIndex(this.Body.CurrAnimationIndex), this.Body.ElapsedDuration, this.Body.Direction);
            }
            else
            {
                if (!this.subBody.Play(this.GetSubAnimationIndex(this.Body.CurrAnimationIndex)))
                {
                    Debug.LogError("SubAnimatorBodyAnimatorProperty " + this.name + " tried to play an animation that doesn't exist.");
                }
            }
            base.AnimationEnter();
        }

        public override Bounds? GetVisibleArea()
        {
            Bounds? baseAreaOccupied = base.GetVisibleArea();
            if (baseAreaOccupied.HasValue)
            {
                baseAreaOccupied.Value.Encapsulate(this.subBody.VisibleArea);
                return baseAreaOccupied;
            }
            return this.subBody.VisibleArea;
        }

        public override float GetDuration()
        {
            return this.GetIgnoreSubAnimationLoopAndDuration(this.Body.CurrAnimationIndex) ? base.GetDuration() : Mathf.Max(this.subBody.TotalDuration, base.GetDuration());
        }
    }
}
