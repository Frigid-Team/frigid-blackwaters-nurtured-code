using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class SortingPointAnimatorProperty : SortingOrderedAnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private SortingGroup sortingGroup;
        [SerializeField]
        [HideInInspector]
        private List<bool> shownInAnimations;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Vector2> localOffsets;

        private Action<bool> onShownChanged;

        public bool Shown
        {
            get
            {
                return this.Body.CurrAnimationIndex != -1 && GetBinded(this.Body.CurrAnimationIndex) && GetShownInAnimation(this.Body.CurrAnimationIndex);
            }
        }

        public Action<bool> OnShownChanged
        {
            get
            {
                return this.onShownChanged;
            }
            set
            {
                this.onShownChanged = value;
            }
        }

        public override int CurrentSortingOrder
        {
            get
            {
                return this.sortingGroup.sortingOrder;
            }
        }

        public bool GetShownInAnimation(int animationIndex)
        {
            return this.shownInAnimations[animationIndex];
        }

        public void SetShownInAnimation(int animationIndex, bool shownInAnimation)
        {
            if (this.shownInAnimations[animationIndex] != shownInAnimation)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.shownInAnimations[animationIndex] = shownInAnimation;
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
            this.sortingGroup = FrigidEditMode.AddComponent<SortingGroup>(this.gameObject);
            this.sortingGroup.sortingLayerName = FrigidSortingLayer.World.ToString();
            this.shownInAnimations = new List<bool>();
            this.localOffsets = new Nested3DList<Vector2>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.shownInAnimations.Add(false);
                this.localOffsets.Add(new Nested2DList<Vector2>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
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
            this.shownInAnimations.Insert(animationIndex, false);
            this.localOffsets.Insert(animationIndex, new Nested2DList<Vector2>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
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
            this.shownInAnimations.RemoveAt(animationIndex);
            this.localOffsets.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
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

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            SortingPointAnimatorProperty otherSortingPointProperty = otherProperty as SortingPointAnimatorProperty;
            if (otherSortingPointProperty)
            {
                otherSortingPointProperty.SetShownInAnimation(toAnimationIndex, GetShownInAnimation(fromAnimationIndex));
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            SortingPointAnimatorProperty otherSortingPointProperty = otherProperty as SortingPointAnimatorProperty;
            if (otherSortingPointProperty)
            {
                otherSortingPointProperty.SetLocalOffset(toAnimationIndex, toFrameIndex, toOrientationIndex, GetLocalOffset(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Body.OnAnimationUpdated +=
                (int prevAnimationIndex, int currAnimationIndex) =>
                {
                    bool prevShown = prevAnimationIndex != -1 && GetBinded(prevAnimationIndex) && GetShownInAnimation(prevAnimationIndex);
                    bool currShown = currAnimationIndex != -1 && GetBinded(currAnimationIndex) && GetShownInAnimation(currAnimationIndex);
                    if (prevShown != currShown) this.onShownChanged?.Invoke(currShown);
                };
        }

        public override void OrientationEnter()
        {
            if (GetShownInAnimation(this.Body.CurrAnimationIndex))
            {
                this.sortingGroup.sortingOrder = GetSortingOrder(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                this.transform.localPosition = GetLocalOffset(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
            }
            base.OrientationEnter();
        }
    }
}
